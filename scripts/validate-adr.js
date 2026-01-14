#!/usr/bin/env node
/**
 * ADR Frontmatter Validation Script
 *
 * Validates that implementation-type ADRs have implementation_issue field.
 *
 * Usage: node scripts/validate-adr.js [directory]
 * Default directory: docs/adr/
 */

import { readFileSync, readdirSync } from 'node:fs';
import { join, basename } from 'node:path';
import matter from 'gray-matter';

const ISSUE_PATTERN = /^#\d+$|^https:\/\/github\.com\/.+\/issues\/\d+$/;

/**
 * Validate a single ADR file
 * @param {string} filePath - Path to ADR file
 * @returns {{file: string, errors: string[], warnings: string[]}}
 */
export function validateAdr(filePath) {
  const fileName = basename(filePath);
  const errors = [];
  const warnings = [];

  // Skip README
  if (fileName === 'README.md' || fileName === 'exclusions.md') {
    return { file: fileName, errors, warnings };
  }

  let content;
  try {
    content = readFileSync(filePath, 'utf-8');
  } catch {
    errors.push(`Could not read file`);
    return { file: fileName, errors, warnings };
  }

  let frontmatter;
  try {
    const parsed = matter(content);
    frontmatter = parsed.data;
  } catch {
    errors.push(`Invalid frontmatter YAML`);
    return { file: fileName, errors, warnings };
  }

  // Check type field
  const adrType = frontmatter.type || 'process';
  if (adrType !== 'process' && adrType !== 'implementation') {
    errors.push(`Invalid type '${adrType}' - must be 'process' or 'implementation'`);
    return { file: fileName, errors, warnings };
  }

  // If implementation type, require implementation_issue
  if (adrType === 'implementation') {
    const issue = frontmatter.implementation_issue;
    if (!issue) {
      errors.push(`type is 'implementation' but implementation_issue is missing`);
    } else if (typeof issue !== 'string') {
      errors.push(`implementation_issue must be a string (got ${typeof issue})`);
    } else if (!ISSUE_PATTERN.test(issue)) {
      errors.push(
        `implementation_issue '${issue}' has invalid format - use '#123' or full GitHub URL`
      );
    }
  }

  return { file: fileName, errors, warnings };
}

/**
 * Validate all ADR files in a directory
 * @param {string} directory - Path to ADR directory
 * @returns {{results: Array, hasErrors: boolean}}
 */
export function validateDirectory(directory) {
  const files = readdirSync(directory).filter((f) => f.endsWith('.md'));
  const results = files.map((f) => validateAdr(join(directory, f)));
  const hasErrors = results.some((r) => r.errors.length > 0);
  return { results, hasErrors };
}

/**
 * Format results for console output
 * @param {Array} results - Validation results
 * @returns {string}
 */
export function formatResults(results) {
  const lines = [];

  for (const result of results) {
    if (result.errors.length === 0 && result.warnings.length === 0) {
      continue;
    }

    for (const error of result.errors) {
      lines.push(`❌ ${result.file}: ${error}`);
    }
    for (const warning of result.warnings) {
      lines.push(`⚠️  ${result.file}: ${warning}`);
    }
  }

  return lines.join('\n');
}

// CLI entry point
if (process.argv[1].endsWith('validate-adr.js')) {
  const directory = process.argv[2] || 'docs/adr';
  const { results, hasErrors } = validateDirectory(directory);
  const output = formatResults(results);

  if (output) {
    console.log(output);
  }

  if (hasErrors) {
    console.log('\n❌ ADR validation failed');
    process.exit(1);
  } else {
    console.log('✅ All ADR files valid');
    process.exit(0);
  }
}

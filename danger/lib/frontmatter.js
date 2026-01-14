/**
 * YAML frontmatter parsing utilities
 *
 * Parses frontmatter from markdown files:
 * ---
 * status: Draft
 * version: v1
 * issue: '#93'
 * ---
 *
 * @see docs/plans/2026-01-14-dangerjs-validation-design.md
 */

// Frontmatter regex: matches content between --- markers at start of file
const FRONTMATTER_PATTERN = /^---\r?\n([\s\S]*?)\r?\n---/;

/**
 * Parse YAML frontmatter from markdown content
 * @param {string} content - Markdown file content
 * @returns {Object|null} Parsed frontmatter or null if not found
 */
export function parseFrontmatter(content) {
  const match = content.match(FRONTMATTER_PATTERN);
  if (!match) {
    return null;
  }

  const yamlContent = match[1];
  const result = {};

  // Simple YAML parser for key: value pairs
  const lines = yamlContent.split(/\r?\n/);
  for (const line of lines) {
    const trimmed = line.trim();
    if (!trimmed || trimmed.startsWith('#')) {
      continue;
    }

    const colonIndex = trimmed.indexOf(':');
    if (colonIndex === -1) {
      continue;
    }

    const key = trimmed.substring(0, colonIndex).trim();
    let value = trimmed.substring(colonIndex + 1).trim();

    // Remove quotes if present
    if (
      (value.startsWith("'") && value.endsWith("'")) ||
      (value.startsWith('"') && value.endsWith('"'))
    ) {
      value = value.slice(1, -1);
    }

    result[key] = value;
  }

  return result;
}

/**
 * Extract issue number from frontmatter
 * @param {Object} frontmatter - Parsed frontmatter object
 * @returns {string|null} Issue number (e.g., '93') or null
 */
export function getIssueNumber(frontmatter) {
  if (!frontmatter || !frontmatter.issue) {
    return null;
  }

  // Handle formats: '#93', '93', 93
  const issue = String(frontmatter.issue);
  return issue.replace(/^#/, '');
}

/**
 * Get plan status from frontmatter
 * @param {Object} frontmatter - Parsed frontmatter object
 * @returns {string|null} Status value or null
 */
export function getPlanStatus(frontmatter) {
  if (!frontmatter) {
    return null;
  }
  return frontmatter.status || null;
}

/**
 * Auto-merge and squash commit validation rules
 *
 * Validates:
 * - Auto-merge is enabled on PR
 * - PR title follows conventional commits format
 * - PR body contains issue reference (Refs: #XX or Closes #XX)
 *
 * @see docs/plans/2026-01-14-dangerjs-validation-design.md
 */

import { danger, fail } from 'danger';
import { CONVENTIONAL_COMMIT_PATTERN, ISSUE_REFERENCE_PATTERN } from '../constants.js';

/**
 * Validate auto-merge is enabled
 */
export async function validateAutoMerge() {
  const autoMerge = danger.github.pr.auto_merge;

  if (!autoMerge) {
    fail(
      'PR must have auto-merge enabled. Enable via PR settings → "Enable auto-merge".\n\nThis ensures PRs are merged automatically once all checks pass.'
    );
  }
}

/**
 * Validate PR title follows conventional commits format
 * This will become the squash commit message header
 */
export async function validateCommitMessage() {
  const prTitle = danger.github.pr.title || '';

  // Check conventional commit format
  if (!CONVENTIONAL_COMMIT_PATTERN.test(prTitle)) {
    fail(
      `PR title must follow conventional commits format.\n\n` +
        `Current: "${prTitle}"\n\n` +
        `Expected: type(scope): description\n\n` +
        `Valid types: feat, fix, docs, style, refactor, perf, test, build, ci, chore, revert\n\n` +
        `Examples:\n` +
        `- feat(auth): add login functionality\n` +
        `- fix(api): resolve null pointer exception\n` +
        `- docs: update README`
    );
    return; // Don't validate further if format is wrong
  }

  // Check header length (max 100 characters)
  if (prTitle.length > 100) {
    fail(
      `PR title exceeds 100 characters (${prTitle.length} chars).\n\n` +
        `Conventional commits recommend keeping the header under 100 characters for readability.`
    );
  }

  // Check subject starts with lowercase
  const match = prTitle.match(CONVENTIONAL_COMMIT_PATTERN);
  if (match) {
    const colonIndex = prTitle.indexOf(':');
    if (colonIndex !== -1) {
      const subject = prTitle.substring(colonIndex + 1).trim();
      if (subject.length > 0 && subject[0] === subject[0].toUpperCase() && subject[0] !== subject[0].toLowerCase()) {
        fail(
          `PR title subject should start with lowercase.\n\n` +
            `Current: "${prTitle}"\n\n` +
            `The subject after the colon should start with a lowercase letter.`
        );
      }
    }
  }
}

/**
 * Validate PR body contains issue reference
 * This will be included in the squash commit message body
 */
export async function validateIssueReference() {
  const prBody = danger.github.pr.body || '';

  if (!ISSUE_REFERENCE_PATTERN.test(prBody)) {
    fail(
      `PR body must contain an issue reference.\n\n` +
        `Add one of the following to your PR description:\n` +
        `- Refs: #123\n` +
        `- Closes #123\n` +
        `- Fixes #123\n\n` +
        `This ensures traceability between commits and issues.`
    );
  }
}

/**
 * Run all auto-merge validations
 */
export async function validateAutoMergeSettings() {
  await validateAutoMerge();
  await validateCommitMessage();
  await validateIssueReference();
}

/**
 * DangerJS validation constants
 *
 * Shared patterns, paths, and configuration
 *
 * @see docs/plans/2026-01-14-dangerjs-validation-design.md
 */

// Plan file paths
export const PLANS_DIR = 'docs/plans';
export const PLANS_ARCHIVE_DIR = 'docs/plans/archive';
export const PLAN_FILE_PATTERN = /^docs\/plans\/.*\.md$/;

// Plan statuses
export const PLAN_STATUS = {
  DRAFT: 'Draft',
  APPROVED: /^Approved/,
  IMPLEMENTED: 'Implemented',
};

// PR size thresholds
export const PR_SIZE_WARN_THRESHOLD = 500;

// Minimum PR description length
export const MIN_DESCRIPTION_LENGTH = 50;

// Conventional commit pattern
export const CONVENTIONAL_COMMIT_PATTERN =
  /^(feat|fix|docs|style|refactor|perf|test|build|ci|chore|revert)(\(.+\))?:\s+.+/;

// Issue reference patterns in commit body
export const ISSUE_REFERENCE_PATTERN = /(Refs|Closes|Fixes):\s*#\d+/i;

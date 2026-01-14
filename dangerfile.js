/**
 * DangerJS PR Validation Rules
 *
 * Phase 1: Basic checklist validation
 * - Validates checked items in PR body have evidence links
 * - Warns on unchecked items in Test Plan section
 *
 * @see docs/adr/0031-pr-validation-automation.md
 */

import { danger, fail, warn, message } from 'danger';

// Regex patterns for validation
const CHECKBOX_CHECKED = /^[\s]*-\s*\[x\]\s*(.+)$/gim;
const CHECKBOX_UNCHECKED = /^[\s]*-\s*\[\s\]\s*(.+)$/gim;
const EVIDENCE_LINK =
  /\[.+?\]\(.+?\)|https?:\/\/[^\s)]+|#\d+|[a-f0-9]{7,40}/i;
const TEST_PLAN_SECTION = /##\s*Test\s*[Pp]lan([\s\S]*?)(?=##|$)/;
const ACCEPTANCE_CRITERIA_SECTION =
  /##\s*Acceptance\s*[Cc]riteria([\s\S]*?)(?=##|$)/;

/**
 * Parse checkbox items from markdown text
 */
function parseCheckboxes(text, checked) {
  const pattern = checked ? CHECKBOX_CHECKED : CHECKBOX_UNCHECKED;
  const matches = [];
  let match;

  // Reset regex state
  pattern.lastIndex = 0;

  while ((match = pattern.exec(text)) !== null) {
    matches.push({
      text: match[1].trim(),
      hasEvidenceLink: EVIDENCE_LINK.test(match[1]),
    });
  }

  return matches;
}

/**
 * Extract section content from PR body
 */
function extractSection(body, sectionPattern) {
  const match = body.match(sectionPattern);
  return match ? match[1] : null;
}

/**
 * Validate PR checklist items
 */
async function validateChecklist() {
  const prBody = danger.github.pr.body || '';

  // Check for Test Plan section
  const testPlanContent = extractSection(prBody, TEST_PLAN_SECTION);

  if (testPlanContent) {
    // Find unchecked items in Test Plan
    const uncheckedItems = parseCheckboxes(testPlanContent, false);
    if (uncheckedItems.length > 0) {
      const itemList = uncheckedItems.map((item) => `- ${item.text}`).join('\n');
      fail(
        `Test Plan has unchecked items. Complete all items before merging:\n\n${itemList}`
      );
    }

    // Find checked items without evidence links
    const checkedItems = parseCheckboxes(testPlanContent, true);
    const itemsWithoutEvidence = checkedItems.filter(
      (item) => !item.hasEvidenceLink
    );

    if (itemsWithoutEvidence.length > 0) {
      const itemList = itemsWithoutEvidence
        .map((item) => `- ${item.text}`)
        .join('\n');
      warn(
        `Test Plan items are checked but lack evidence links:\n\n${itemList}\n\nAdd links to CI runs, commits, or PR comments as evidence.`
      );
    }
  }

  // Check for Acceptance Criteria section (if present)
  const acceptanceCriteriaContent = extractSection(
    prBody,
    ACCEPTANCE_CRITERIA_SECTION
  );

  if (acceptanceCriteriaContent) {
    const uncheckedCriteria = parseCheckboxes(acceptanceCriteriaContent, false);
    if (uncheckedCriteria.length > 0) {
      const itemList = uncheckedCriteria
        .map((item) => `- ${item.text}`)
        .join('\n');
      fail(
        `Acceptance Criteria has unchecked items:\n\n${itemList}\n\nComplete all criteria or document why they're out of scope.`
      );
    }
  }
}

/**
 * Validate PR has a description
 */
async function validateDescription() {
  const prBody = danger.github.pr.body || '';

  if (prBody.trim().length < 50) {
    warn(
      'PR description is very short. Please provide more context about the changes.'
    );
  }

  if (!prBody.includes('## Summary') && !prBody.includes('## Description')) {
    warn('PR is missing a Summary or Description section.');
  }
}

/**
 * Validate PR size
 */
async function validateSize() {
  const additions = danger.github.pr.additions || 0;
  const deletions = danger.github.pr.deletions || 0;
  const totalChanges = additions + deletions;

  if (totalChanges > 500) {
    warn(
      `This PR has ${totalChanges} lines changed. Consider breaking into smaller PRs for easier review.`
    );
  }
}

/**
 * Provide helpful messages
 */
async function provideMessages() {
  const prBody = danger.github.pr.body || '';

  // Check if this is a first-time contributor
  const author = danger.github.pr.user.login;
  const authorAssociation = danger.github.pr.author_association;

  if (
    authorAssociation === 'FIRST_TIME_CONTRIBUTOR' ||
    authorAssociation === 'FIRST_TIMER'
  ) {
    message(`Welcome @${author}! Thanks for your first contribution.`);
  }

  // Remind about squash merge
  if (!prBody.toLowerCase().includes('breaking')) {
    message(
      'Remember: This PR will be squash-merged. Ensure the PR title follows conventional commits format.'
    );
  }
}

// Run all validations
async function run() {
  await validateChecklist();
  await validateDescription();
  await validateSize();
  await provideMessages();
}

run();

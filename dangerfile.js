/**
 * DangerJS PR Validation Rules
 *
 * Phase 1: Strict checklist validation
 * - Validates checked items in PR body have markdown link evidence
 * - Fails on unchecked items in Test Plan section
 * - Fails on missing evidence links
 * - Fails on missing PR description
 *
 * @see docs/adr/0031-pr-validation-automation.md
 * @see docs/plans/2026-01-14-dangerjs-validation-design.md
 */

import { danger, fail, warn, message } from 'danger';

// Regex patterns for validation
const CHECKBOX_CHECKED = /^[\s]*-\s*\[x\]\s*(.+)$/gim;
const CHECKBOX_UNCHECKED = /^[\s]*-\s*\[\s\]\s*(.+)$/gim;

// Strict evidence pattern: markdown links only [text](url)
const EVIDENCE_LINK = /\[[^\]]+\]\([^)]+\)/;

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
 * Validate checklist items in a section
 * @param {string} sectionName - Name for error messages
 * @param {string} content - Section content to validate
 */
function validateSectionChecklist(sectionName, content) {
  if (!content) return;

  // Find unchecked items
  const uncheckedItems = parseCheckboxes(content, false);
  if (uncheckedItems.length > 0) {
    const itemList = uncheckedItems.map((item) => `- ${item.text}`).join('\n');
    fail(
      `${sectionName} has unchecked items. Complete all items before merging:\n\n${itemList}`
    );
  }

  // Find checked items without evidence links (markdown format required)
  const checkedItems = parseCheckboxes(content, true);
  const itemsWithoutEvidence = checkedItems.filter(
    (item) => !item.hasEvidenceLink
  );

  if (itemsWithoutEvidence.length > 0) {
    const itemList = itemsWithoutEvidence
      .map((item) => `- ${item.text}`)
      .join('\n');
    fail(
      `${sectionName} items are checked but lack evidence links:\n\n${itemList}\n\nAdd markdown links [description](url) as evidence.`
    );
  }
}

/**
 * Validate PR checklist items
 */
async function validateChecklist() {
  const prBody = danger.github.pr.body || '';

  // Check for Test Plan section
  const testPlanContent = extractSection(prBody, TEST_PLAN_SECTION);
  validateSectionChecklist('Test Plan', testPlanContent);

  // Check for Acceptance Criteria section (if present)
  const acceptanceCriteriaContent = extractSection(
    prBody,
    ACCEPTANCE_CRITERIA_SECTION
  );
  validateSectionChecklist('Acceptance Criteria', acceptanceCriteriaContent);
}

/**
 * Validate PR has a description
 */
async function validateDescription() {
  const prBody = danger.github.pr.body || '';

  if (prBody.trim().length < 50) {
    fail(
      'PR description is too short. Please provide context about the changes (minimum 50 characters).'
    );
  }

  if (!prBody.includes('## Summary') && !prBody.includes('## Description')) {
    fail('PR must have a Summary or Description section.');
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

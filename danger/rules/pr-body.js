/**
 * PR body validation rules
 *
 * Validates:
 * - Test Plan section: all items checked with evidence
 * - Acceptance Criteria section: all items checked with evidence
 * - Summary/Description section present
 * - Minimum description length
 *
 * @see docs/plans/2026-01-14-dangerjs-validation-design.md
 */

import { parseCheckboxes } from '../lib/checklist.js';
import {
  extractTestPlan,
  extractAcceptanceCriteria,
  hasSummaryOrDescription,
} from '../lib/sections.js';
import { MIN_DESCRIPTION_LENGTH, PR_SIZE_WARN_THRESHOLD } from '../constants.js';

/**
 * Create PR body validator
 * @param {Object} dangerContext - The danger context
 */
export function createPRBodyValidator(dangerContext) {
  const { danger, fail, warn } = dangerContext;

  /**
   * Validate checklist items in a section
   * @param {string} sectionName - Name for error messages
   * @param {string} content - Section content to validate
   */
  function validateSectionChecklist(sectionName, content) {
    if (!content) return;

    // Find unchecked items
    const uncheckedItems = parseCheckboxes(content, 'unchecked');
    if (uncheckedItems.length > 0) {
      const itemList = uncheckedItems.map((item) => `- ${item.text}`).join('\n');
      fail(`${sectionName} has unchecked items. Complete all items before merging:\n\n${itemList}`);
    }

    // Find checked items without evidence links (markdown format required)
    const checkedItems = parseCheckboxes(content, 'checked');
    const itemsWithoutEvidence = checkedItems.filter((item) => !item.hasEvidenceLink);

    if (itemsWithoutEvidence.length > 0) {
      const itemList = itemsWithoutEvidence.map((item) => `- ${item.text}`).join('\n');
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
    const testPlanContent = extractTestPlan(prBody);
    validateSectionChecklist('Test Plan', testPlanContent);

    // Check for Acceptance Criteria section (if present)
    const acceptanceCriteriaContent = extractAcceptanceCriteria(prBody);
    validateSectionChecklist('Acceptance Criteria', acceptanceCriteriaContent);
  }

  /**
   * Validate PR has a description
   */
  async function validateDescription() {
    const prBody = danger.github.pr.body || '';

    if (prBody.trim().length < MIN_DESCRIPTION_LENGTH) {
      fail(
        `PR description is too short. Please provide context about the changes (minimum ${MIN_DESCRIPTION_LENGTH} characters).`
      );
    }

    if (!hasSummaryOrDescription(prBody)) {
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

    if (totalChanges > PR_SIZE_WARN_THRESHOLD) {
      warn(
        `This PR has ${totalChanges} lines changed. Consider breaking into smaller PRs for easier review.`
      );
    }
  }

  /**
   * Run all PR body validations
   */
  return async function validatePRBody() {
    await validateChecklist();
    await validateDescription();
    await validateSize();
  };
}

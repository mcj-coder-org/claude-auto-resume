/**
 * Plan file validation rules
 *
 * Validates:
 * - Plan file detection (modified files + frontmatter lookup)
 * - Success Criteria: all items checked with evidence
 * - Approval section: all items checked with evidence links
 * - Plan status for final PRs
 * - Archive location for completed plans
 *
 * @see docs/plans/2026-01-14-dangerjs-validation-design.md
 */

import { parseFrontmatter, getPlanStatus } from '../lib/frontmatter.js';
import { parseCheckboxes } from '../lib/checklist.js';
import { extractSuccessCriteria, extractApproval } from '../lib/sections.js';
import { PLANS_DIR, PLANS_ARCHIVE_DIR, PLAN_FILE_PATTERN, PLAN_STATUS } from '../constants.js';

/**
 * Create plan files validator
 * @param {Object} dangerContext - The danger context
 */
export function createPlanFilesValidator(dangerContext) {
  const { danger, fail, warn, markdown } = dangerContext;

  /**
   * Check if a path is a plan file
   * @param {string} path - File path
   * @returns {boolean}
   */
  function isPlanFile(path) {
    return PLAN_FILE_PATTERN.test(path);
  }

  /**
   * Check if plan is in archive directory
   * @param {string} path - File path
   * @returns {boolean}
   */
  function isArchived(path) {
    return path.startsWith(PLANS_ARCHIVE_DIR);
  }

  /**
   * Get linked issue number from PR
   * @returns {string|null} Issue number or null
   */
  function getLinkedIssueNumber() {
    const prBody = danger.github.pr.body || '';

    // Look for "Closes #XX" or "Refs: #XX" patterns
    const closesMatch = prBody.match(/Closes\s+#(\d+)/i);
    if (closesMatch) {
      return closesMatch[1];
    }

    const refsMatch = prBody.match(/Refs:\s*#(\d+)/i);
    if (refsMatch) {
      return refsMatch[1];
    }

    return null;
  }

  /**
   * Check if PR is closing the parent issue (final PR)
   * @param {string} prBody - PR body text
   * @returns {boolean}
   */
  function isClosingIssue(prBody) {
    return /Closes\s+#\d+/i.test(prBody);
  }

  /**
   * Find plan files from modified files
   * @returns {string[]} Array of plan file paths
   */
  function findModifiedPlanFiles() {
    const modifiedFiles = danger.git.modified_files || [];
    const createdFiles = danger.git.created_files || [];
    const allFiles = [...modifiedFiles, ...createdFiles];

    return allFiles.filter(isPlanFile);
  }

  /**
   * Validate Success Criteria section
   * @param {string} content - Plan file content
   * @param {string} planPath - Path to plan file for error messages
   */
  function validateSuccessCriteria(content, planPath) {
    const successCriteria = extractSuccessCriteria(content);
    if (!successCriteria) {
      return; // No success criteria section
    }

    // Find unchecked items
    const uncheckedItems = parseCheckboxes(successCriteria, 'unchecked');
    if (uncheckedItems.length > 0) {
      const itemList = uncheckedItems.map((item) => `- ${item.text}`).join('\n');
      fail(`Plan file \`${planPath}\` has unchecked Success Criteria items:\n\n${itemList}`);
    }

    // Find checked items without evidence
    const checkedItems = parseCheckboxes(successCriteria, 'checked');
    const itemsWithoutEvidence = checkedItems.filter((item) => !item.hasEvidenceLink);

    if (itemsWithoutEvidence.length > 0) {
      const itemList = itemsWithoutEvidence.map((item) => `- ${item.text}`).join('\n');
      fail(
        `Plan file \`${planPath}\` Success Criteria items lack evidence links:\n\n${itemList}\n\nAdd markdown links [description](url) as evidence.`
      );
    }
  }

  /**
   * Validate Approval section
   * @param {string} content - Plan file content
   * @param {string} planPath - Path to plan file for error messages
   */
  function validateApproval(content, planPath) {
    const approval = extractApproval(content);
    if (!approval) {
      return; // No approval section
    }

    // Find unchecked approval items
    const uncheckedItems = parseCheckboxes(approval, 'unchecked');
    if (uncheckedItems.length > 0) {
      const itemList = uncheckedItems.map((item) => `- ${item.text}`).join('\n');
      fail(`Plan file \`${planPath}\` has unchecked Approval items:\n\n${itemList}`);
    }

    // Find checked approval items without links
    const checkedItems = parseCheckboxes(approval, 'checked');
    const itemsWithoutLinks = checkedItems.filter((item) => !item.hasEvidenceLink);

    if (itemsWithoutLinks.length > 0) {
      const itemList = itemsWithoutLinks.map((item) => `- ${item.text}`).join('\n');
      fail(
        `Plan file \`${planPath}\` Approval items lack evidence links:\n\n${itemList}\n\nAdd markdown links [Approval](url) to approval comments.`
      );
    }
  }

  /**
   * Validate plan status for final PRs
   * @param {string} content - Plan file content
   * @param {string} planPath - Path to plan file
   * @param {boolean} isFinalPR - Whether this PR closes the parent issue
   */
  function validatePlanStatus(content, planPath, isFinalPR) {
    const frontmatter = parseFrontmatter(content);
    const status = getPlanStatus(frontmatter);

    if (isFinalPR) {
      if (status !== PLAN_STATUS.IMPLEMENTED) {
        fail(
          `Plan file \`${planPath}\` status must be "Implemented" when closing the parent issue. Current status: "${status || 'not set'}"`
        );
      }

      if (!isArchived(planPath)) {
        fail(
          `Plan file \`${planPath}\` must be moved to \`${PLANS_ARCHIVE_DIR}/\` when closing the parent issue.`
        );
      }
    }
  }

  /**
   * Check if all Success Criteria are complete
   * @param {string} content - Plan file content
   * @returns {boolean}
   */
  function isSuccessCriteriaComplete(content) {
    const successCriteria = extractSuccessCriteria(content);
    if (!successCriteria) {
      return true; // No criteria = complete
    }

    const uncheckedItems = parseCheckboxes(successCriteria, 'unchecked');
    return uncheckedItems.length === 0;
  }

  /**
   * Validate a single plan file
   * @param {string} planPath - Path to plan file
   * @param {string} content - Plan file content
   */
  async function validatePlanFile(planPath, content) {
    const prBody = danger.github.pr.body || '';
    const isFinalPR = isClosingIssue(prBody) && isSuccessCriteriaComplete(content);

    // Skip validation for unmodified archived plans
    const modifiedFiles = danger.git.modified_files || [];
    if (isArchived(planPath) && !modifiedFiles.includes(planPath)) {
      return;
    }

    validateSuccessCriteria(content, planPath);
    validateApproval(content, planPath);
    validatePlanStatus(content, planPath, isFinalPR);
  }

  /**
   * Run all plan file validations
   */
  return async function validatePlanFiles() {
    const modifiedPlanFiles = findModifiedPlanFiles();
    const linkedIssue = getLinkedIssueNumber();

    // Validate each modified plan file
    for (const planPath of modifiedPlanFiles) {
      try {
        const content = await danger.github.utils.fileContents(planPath);
        await validatePlanFile(planPath, content);
      } catch {
        warn(`Could not read plan file: ${planPath}`);
      }
    }

    // Warn if no plan file found
    if (modifiedPlanFiles.length === 0 && linkedIssue) {
      warn(
        `No plan file found for this PR. If this work has a plan, ensure it is updated.\n\nExpected location: \`${PLANS_DIR}/YYYY-MM-DD-<topic>-design.md\` with frontmatter \`issue: '#${linkedIssue}'\``
      );

      // Add comment to PR
      markdown(
        `### No Plan File Detected\n\nThis PR does not modify any plan files. If this work is tracked by a plan, please update the relevant plan file in \`${PLANS_DIR}/\`.`
      );
    }
  };
}

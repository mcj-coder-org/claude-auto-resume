/**
 * Plan file amendment validation rules
 *
 * Validates:
 * - Struck-through items have approval links
 * - Version history updated when amendments detected
 *
 * @see docs/plans/2026-01-14-dangerjs-validation-design.md
 */

import { parseCheckboxes } from '../lib/checklist.js';
import { extractSuccessCriteria, extractSection } from '../lib/sections.js';
import { PLAN_FILE_PATTERN } from '../constants.js';

/**
 * Create amendments validator
 * @param {Object} dangerContext - The danger context
 */
export function createAmendmentsValidator(dangerContext) {
  const { danger, fail } = dangerContext;

  /**
   * Check if a path is a plan file
   * @param {string} path - File path
   * @returns {boolean}
   */
  function isPlanFile(path) {
    return PLAN_FILE_PATTERN.test(path);
  }

  /**
   * Extract Version History section from plan content
   * @param {string} content - Plan file content
   * @returns {string|null} Version History content or null
   */
  function extractVersionHistory(content) {
    return extractSection(content, 'Version\\s*History');
  }

  /**
   * Check if version history has been updated (has multiple rows)
   * @param {string} versionHistory - Version History section content
   * @returns {boolean}
   */
  function hasVersionHistoryUpdate(versionHistory) {
    if (!versionHistory) {
      return false;
    }

    // Count table rows (lines starting with |)
    const tableRows = versionHistory
      .split(/\r?\n/)
      .filter((line) => line.trim().startsWith('|') && !line.includes('---'));

    // Header + separator = 2 rows, any data rows after = amendment tracking
    // v1 row expected, so 1+ data row means at least initial version
    return tableRows.length >= 1;
  }

  /**
   * Validate struck-through items have approval links
   * @param {string} content - Plan file content
   * @param {string} planPath - Path to plan file
   */
  function validateStruckThroughItems(content, planPath) {
    const successCriteria = extractSuccessCriteria(content);
    if (!successCriteria) {
      return;
    }

    const struckItems = parseCheckboxes(successCriteria, 'struck');

    for (const item of struckItems) {
      if (!item.hasEvidenceLink) {
        fail(
          `Plan file \`${planPath}\` has descoped item without approval link:\n\n- ~~${item.text}~~\n\nDescoped items must include [Approval](url) link to the approval comment.`
        );
      }
    }
  }

  /**
   * Detect if plan file has amendments by checking git diff
   * @param {string} planPath - Path to plan file
   * @returns {Promise<{hasNewItems: boolean, hasRemovals: boolean}>}
   */
  async function detectAmendments(planPath) {
    const result = { hasNewItems: false, hasRemovals: false };

    try {
      // Get the diff for this specific file
      const diffData = await danger.git.diffForFile(planPath);
      if (!diffData) {
        return result;
      }

      const diff = diffData.diff || '';

      // Check for new checklist items (lines starting with + and containing checkbox)
      const addedCheckboxes = diff.match(/^\+[^+].*-\s*\[.\]/gm);
      if (addedCheckboxes && addedCheckboxes.length > 0) {
        result.hasNewItems = true;
      }

      // Check for struck-through items (lines containing ~~)
      const struckItems = diff.match(/~~.+~~/gm);
      if (struckItems && struckItems.length > 0) {
        result.hasRemovals = true;
      }
    } catch {
      // If we can't get the diff, assume no amendments
    }

    return result;
  }

  /**
   * Validate amendments in a plan file
   * @param {string} planPath - Path to plan file
   * @param {string} content - Plan file content
   */
  async function validateAmendments(planPath, content) {
    // Validate struck-through items have approval links
    validateStruckThroughItems(content, planPath);

    // Check if there are amendments
    const amendments = await detectAmendments(planPath);

    // If there are amendments, version history should be updated
    if (amendments.hasNewItems || amendments.hasRemovals) {
      const versionHistory = extractVersionHistory(content);
      if (!hasVersionHistoryUpdate(versionHistory)) {
        fail(
          `Plan file \`${planPath}\` appears to have amendments but Version History section is missing or not updated.\n\nAdd a new row to the Version History table documenting the change.`
        );
      }
    }
  }

  /**
   * Run amendment validation for all modified plan files
   */
  return async function validateAllAmendments() {
    const modifiedFiles = danger.git.modified_files || [];
    const planFiles = modifiedFiles.filter(isPlanFile);

    for (const planPath of planFiles) {
      try {
        const content = await danger.github.utils.fileContents(planPath);
        await validateAmendments(planPath, content);
      } catch {
        // Errors reading file are handled in plan-file.js
      }
    }
  };
}

/**
 * Markdown section extraction utilities
 *
 * Extracts content from specific markdown sections:
 * ## Section Name
 * content here...
 *
 * @see docs/plans/2026-01-14-dangerjs-validation-design.md
 */

/**
 * Extract a section's content from markdown text
 * @param {string} text - Markdown text
 * @param {string} sectionName - Section header to find (without ##)
 * @returns {string|null} Section content or null if not found
 */
export function extractSection(text, sectionName) {
  // Escape special regex characters in section name
  const escapedName = sectionName.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');

  // Match section header with flexible spacing and case
  const pattern = new RegExp(`##\\s*${escapedName}([\\s\\S]*?)(?=##|$)`, 'im');

  const match = text.match(pattern);
  return match ? match[1].trim() : null;
}

/**
 * Extract Test Plan section
 * @param {string} text - Markdown text
 * @returns {string|null} Test Plan content or null
 */
export function extractTestPlan(text) {
  return extractSection(text, 'Test\\s*Plan');
}

/**
 * Extract Acceptance Criteria section
 * @param {string} text - Markdown text
 * @returns {string|null} Acceptance Criteria content or null
 */
export function extractAcceptanceCriteria(text) {
  return extractSection(text, 'Acceptance\\s*Criteria');
}

/**
 * Extract Success Criteria section (plan files)
 * @param {string} text - Markdown text
 * @returns {string|null} Success Criteria content or null
 */
export function extractSuccessCriteria(text) {
  return extractSection(text, 'Success\\s*Criteria');
}

/**
 * Extract Approval section (plan files)
 * @param {string} text - Markdown text
 * @returns {string|null} Approval content or null
 */
export function extractApproval(text) {
  return extractSection(text, 'Approval');
}

/**
 * Check if text has a Summary or Description section
 * @param {string} text - Markdown text
 * @returns {boolean} True if section found
 */
export function hasSummaryOrDescription(text) {
  return text.includes('## Summary') || text.includes('## Description');
}

/**
 * Evidence link detection utilities
 *
 * Strict pattern: markdown links only [text](url)
 *
 * @see docs/plans/2026-01-14-dangerjs-validation-design.md
 */

// Strict evidence pattern: markdown links only [text](url)
export const EVIDENCE_LINK_PATTERN = /\[[^\]]+\]\([^)]+\)/;

/**
 * Check if text contains an evidence link (markdown format)
 * @param {string} text - Text to check
 * @returns {boolean} True if markdown link found
 */
export function hasEvidenceLink(text) {
  return EVIDENCE_LINK_PATTERN.test(text);
}

/**
 * Extract all evidence links from text
 * @param {string} text - Text to extract from
 * @returns {string[]} Array of markdown links
 */
export function extractEvidenceLinks(text) {
  const matches = text.match(/\[[^\]]+\]\([^)]+\)/g);
  return matches || [];
}

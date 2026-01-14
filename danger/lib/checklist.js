/**
 * Checkbox parsing utilities
 *
 * Parses markdown checkboxes:
 * - [x] Checked item
 * - [ ] Unchecked item
 * - ~~Struck through~~ item
 *
 * @see docs/plans/2026-01-14-dangerjs-validation-design.md
 */

import { hasEvidenceLink } from './evidence.js';

// Regex patterns for checkbox types
const CHECKBOX_CHECKED = /^[\s]*-\s*\[x\]\s*(.+)$/gim;
const CHECKBOX_UNCHECKED = /^[\s]*-\s*\[\s\]\s*(.+)$/gim;
const STRUCK_THROUGH = /^[\s]*-\s*(?:\[.\]\s*)?~~(.+?)~~(.*)$/gim;

/**
 * Parse checkbox items from markdown text
 * @param {string} text - Markdown text to parse
 * @param {'checked' | 'unchecked' | 'struck'} type - Type of checkboxes to find
 * @returns {Array<{text: string, hasEvidenceLink: boolean, fullLine: string}>}
 */
export function parseCheckboxes(text, type) {
  let pattern;
  switch (type) {
    case 'checked':
      pattern = CHECKBOX_CHECKED;
      break;
    case 'unchecked':
      pattern = CHECKBOX_UNCHECKED;
      break;
    case 'struck':
      pattern = STRUCK_THROUGH;
      break;
    default:
      throw new Error(`Unknown checkbox type: ${type}`);
  }

  const matches = [];
  let match;

  // Reset regex state
  pattern.lastIndex = 0;

  while ((match = pattern.exec(text)) !== null) {
    const fullLine = match[0];
    const itemText = type === 'struck' ? match[1] + (match[2] || '') : match[1];

    matches.push({
      text: itemText.trim(),
      hasEvidenceLink: hasEvidenceLink(fullLine),
      fullLine: fullLine.trim(),
    });
  }

  return matches;
}

/**
 * Get all checkbox items from text
 * @param {string} text - Markdown text to parse
 * @returns {{checked: Array, unchecked: Array, struck: Array}}
 */
export function getAllCheckboxes(text) {
  return {
    checked: parseCheckboxes(text, 'checked'),
    unchecked: parseCheckboxes(text, 'unchecked'),
    struck: parseCheckboxes(text, 'struck'),
  };
}

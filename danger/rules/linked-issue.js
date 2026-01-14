/**
 * Linked issue validation rules
 *
 * When PR closes an issue, validates:
 * - All sub-issues are closed
 * - All task list items in issue body are checked
 * - All checked items have evidence links
 *
 * @see docs/plans/2026-01-14-dangerjs-validation-design.md
 */

import { parseCheckboxes } from '../lib/checklist.js';

/**
 * Create linked issue validator
 * @param {Object} dangerContext - The danger context
 */
export function createLinkedIssueValidator(dangerContext) {
  const { danger, fail, warn } = dangerContext;

  /**
   * Extract issue number from "Closes #XX" pattern in PR body
   * @returns {string|null} Issue number or null
   */
  function getClosingIssueNumber() {
    const prBody = danger.github.pr.body || '';
    const match = prBody.match(/Closes\s+#(\d+)/i);
    return match ? match[1] : null;
  }

  /**
   * Extract sub-issue references from issue body
   * Looks for patterns like:
   * - | [#123](url) | in tables
   * - #123 inline references
   * - [#123](url) markdown links
   *
   * @param {string} issueBody - Issue body text
   * @returns {string[]} Array of issue numbers
   */
  function extractSubIssues(issueBody) {
    const subIssues = new Set();

    // Match markdown links with issue numbers [#123](url)
    const linkMatches = issueBody.matchAll(/\[#(\d+)\]\([^)]+\)/g);
    for (const match of linkMatches) {
      subIssues.add(match[1]);
    }

    // Match inline issue references #123 (but not in URLs)
    // Be careful not to match issues in markdown links we already captured
    const inlineMatches = issueBody.matchAll(/(?<!\[)#(\d+)(?!\])/g);
    for (const match of inlineMatches) {
      subIssues.add(match[1]);
    }

    return Array.from(subIssues);
  }

  /**
   * Fetch issue from GitHub API
   * @param {string} issueNumber - Issue number
   * @returns {Promise<Object|null>} Issue object or null
   */
  async function fetchIssue(issueNumber) {
    try {
      const owner = danger.github.pr.base.repo.owner.login;
      const repo = danger.github.pr.base.repo.name;
      const response = await danger.github.api.issues.get({
        owner,
        repo,
        issue_number: parseInt(issueNumber, 10),
      });
      return response.data;
    } catch {
      return null;
    }
  }

  /**
   * Validate all sub-issues are closed
   * @param {string[]} subIssueNumbers - Array of issue numbers
   * @param {string} parentIssueNumber - Parent issue number for error messages
   */
  async function validateSubIssuesClosed(subIssueNumbers, parentIssueNumber) {
    const openSubIssues = [];

    for (const issueNumber of subIssueNumbers) {
      const issue = await fetchIssue(issueNumber);
      if (issue && issue.state !== 'closed') {
        openSubIssues.push({
          number: issueNumber,
          title: issue.title,
        });
      }
    }

    if (openSubIssues.length > 0) {
      const issueList = openSubIssues
        .map((issue) => `- #${issue.number}: ${issue.title}`)
        .join('\n');

      fail(
        `Cannot close issue #${parentIssueNumber} - the following sub-issues are still open:\n\n${issueList}\n\nClose all sub-issues before merging this PR.`
      );
    }
  }

  /**
   * Validate task list items in issue body
   * @param {string} issueBody - Issue body text
   * @param {string} issueNumber - Issue number for error messages
   */
  function validateIssueTaskList(issueBody, issueNumber) {
    // Find unchecked items
    const uncheckedItems = parseCheckboxes(issueBody, 'unchecked');
    if (uncheckedItems.length > 0) {
      const itemList = uncheckedItems.map((item) => `- ${item.text}`).join('\n');
      fail(
        `Issue #${issueNumber} has unchecked task list items:\n\n${itemList}\n\nComplete all tasks before closing the issue.`
      );
    }

    // Find checked items without evidence
    const checkedItems = parseCheckboxes(issueBody, 'checked');
    const itemsWithoutEvidence = checkedItems.filter((item) => !item.hasEvidenceLink);

    if (itemsWithoutEvidence.length > 0) {
      const itemList = itemsWithoutEvidence.map((item) => `- ${item.text}`).join('\n');
      fail(
        `Issue #${issueNumber} task list items lack evidence links:\n\n${itemList}\n\nAdd markdown links [description](url) as evidence for each completed task.`
      );
    }
  }

  /**
   * Run linked issue validation
   */
  return async function validateLinkedIssue() {
    const closingIssueNumber = getClosingIssueNumber();

    if (!closingIssueNumber) {
      // PR doesn't close any issue, nothing to validate
      return;
    }

    // Fetch the issue being closed
    const issue = await fetchIssue(closingIssueNumber);

    if (!issue) {
      warn(`Could not fetch issue #${closingIssueNumber}. Linked issue validation skipped.`);
      return;
    }

    const issueBody = issue.body || '';

    // Extract and validate sub-issues
    const subIssues = extractSubIssues(issueBody);
    if (subIssues.length > 0) {
      // Filter out the issue itself if referenced
      const otherSubIssues = subIssues.filter((n) => n !== closingIssueNumber);
      if (otherSubIssues.length > 0) {
        await validateSubIssuesClosed(otherSubIssues, closingIssueNumber);
      }
    }

    // Validate task list in issue body
    validateIssueTaskList(issueBody, closingIssueNumber);
  };
}

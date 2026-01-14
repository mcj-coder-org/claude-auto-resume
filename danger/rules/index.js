/**
 * DangerJS rules orchestrator
 *
 * Coordinates all validation rules.
 * Receives danger context from main dangerfile to avoid ESM import issues.
 *
 * @see docs/plans/2026-01-14-dangerjs-validation-design.md
 */

import { createPRBodyValidator } from './pr-body.js';
import { createPlanFilesValidator } from './plan-file.js';
import { createAmendmentsValidator } from './amendment.js';
import { createAutoMergeValidator } from './auto-merge.js';
import { createLinkedIssueValidator } from './linked-issue.js';

/**
 * Create the main validation runner
 * @param {Object} dangerContext - The danger context object
 * @param {Object} dangerContext.danger - The danger object
 * @param {Function} dangerContext.fail - The fail function
 * @param {Function} dangerContext.warn - The warn function
 * @param {Function} dangerContext.message - The message function
 * @param {Function} dangerContext.markdown - The markdown function
 */
export function createValidationRunner(dangerContext) {
  const { danger, message } = dangerContext;

  const validatePRBody = createPRBodyValidator(dangerContext);
  const validatePlanFiles = createPlanFilesValidator(dangerContext);
  const validateAllAmendments = createAmendmentsValidator(dangerContext);
  const validateAutoMergeSettings = createAutoMergeValidator(dangerContext);
  const validateLinkedIssue = createLinkedIssueValidator(dangerContext);

  /**
   * Provide helpful messages
   */
  async function provideMessages() {
    const prBody = danger.github.pr.body || '';

    // Check if this is a first-time contributor
    const author = danger.github.pr.user.login;
    const authorAssociation = danger.github.pr.author_association;

    if (authorAssociation === 'FIRST_TIME_CONTRIBUTOR' || authorAssociation === 'FIRST_TIMER') {
      message(`Welcome @${author}! Thanks for your first contribution.`);
    }

    // Remind about squash merge
    if (!prBody.toLowerCase().includes('breaking')) {
      message(
        'Remember: This PR will be squash-merged. Ensure the PR title follows conventional commits format.'
      );
    }
  }

  /**
   * Run all validation rules
   */
  return async function runAllValidations() {
    // Phase 1: PR body validations
    await validatePRBody();

    // Phase 2: Plan file validations
    await validatePlanFiles();

    // Phase 3: Amendment validations
    await validateAllAmendments();

    // Phase 4: Auto-merge validations
    await validateAutoMergeSettings();

    // Phase 5: Linked issue validations
    await validateLinkedIssue();

    // Helpful messages
    await provideMessages();
  };
}

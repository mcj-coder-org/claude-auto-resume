/**
 * DangerJS rules orchestrator
 *
 * Coordinates all validation rules
 *
 * @see docs/plans/2026-01-14-dangerjs-validation-design.md
 */

import { danger, message } from 'danger';
import { validatePRBody } from './pr-body.js';
import { validatePlanFiles } from './plan-file.js';
import { validateAllAmendments } from './amendment.js';
import { validateAutoMergeSettings } from './auto-merge.js';
import { validateLinkedIssue } from './linked-issue.js';

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
export async function runAllValidations() {
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
}

/**
 * DangerJS PR Validation Rules
 *
 * Validates:
 * - PR body (Test Plan, Acceptance Criteria, description)
 * - Plan files (Success Criteria, approvals, status)
 * - Amendments (struck-through items, version history)
 * - Auto-merge settings (enabled, conventional commits)
 * - Linked issues (sub-issues closed, task lists complete)
 *
 * @see docs/adr/0031-pr-validation-automation.md
 * @see docs/plans/2026-01-14-dangerjs-validation-design.md
 */

import { danger, fail, warn, message, markdown } from 'danger';
import { createValidationRunner } from './danger/rules/index.js';

// Create the validation runner with danger context
// This pattern avoids ESM import issues with the danger package in nested modules
const runAllValidations = createValidationRunner({
  danger,
  fail,
  warn,
  message,
  markdown,
});

// Run all validations
runAllValidations();

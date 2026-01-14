/**
 * DangerJS PR Validation Rules
 *
 * Validates:
 * - PR body (Test Plan, Acceptance Criteria, description)
 * - Plan files (Success Criteria, approvals, status)
 *
 * @see docs/adr/0031-pr-validation-automation.md
 * @see docs/plans/2026-01-14-dangerjs-validation-design.md
 */

import { runAllValidations } from './danger/rules/index.js';

// Run all validations
runAllValidations();

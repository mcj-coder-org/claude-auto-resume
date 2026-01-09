---
title: Security Policy
summary: Vulnerability reporting policy and security measures for the project
audience: [user, security-researcher, contributor]
topics: [security, vulnerabilities, reporting, disclosure]
prerequisites: []
related: [docs/adr/0005-security-scanning.md]
last_validated: 2026-01-09
---

# Security Policy

## Supported Versions

| Version | Supported          |
| ------- | ------------------ |
| 1.x.x   | :white_check_mark: |
| < 1.0   | :x:                |

## Reporting a Vulnerability

We take security vulnerabilities seriously. If you discover a security issue, please report it responsibly.

### How to Report

1. **Do NOT** create a public GitHub issue for security vulnerabilities
2. Use [GitHub Security Advisories](https://github.com/mcj-coder-org/claude-auto-resume/security/advisories/new) to report privately
3. Include as much detail as possible:
   - Description of the vulnerability
   - Steps to reproduce
   - Potential impact
   - Suggested fix (if any)

### What to Expect

- **Acknowledgement:** Within 48 hours of your report
- **Initial Assessment:** Within 7 days
- **Resolution Timeline:** Depends on severity
  - Critical: 7 days
  - High: 14 days
  - Medium: 30 days
  - Low: 90 days

### Disclosure Policy

- We follow [coordinated disclosure](https://en.wikipedia.org/wiki/Coordinated_vulnerability_disclosure)
- We will credit reporters in release notes (unless anonymity is requested)
- We request 90 days before public disclosure to allow time for a fix

## Security Measures

This project implements multiple layers of security:

- **Pre-commit:** Secret scanning with secretlint
- **Push protection:** GitHub Secret Scanning
- **SAST:** GitHub CodeQL analysis
- **Dependencies:** Dependabot alerts and updates
- **CI:** Vulnerability scanning in pull requests

## Scope

The following are in scope for security reports:

- The `claude-auto-resume` CLI tool
- Build and release pipelines
- Documentation website

The following are out of scope:

- Third-party dependencies (report to maintainers directly)
- Social engineering attacks
- Denial of service attacks

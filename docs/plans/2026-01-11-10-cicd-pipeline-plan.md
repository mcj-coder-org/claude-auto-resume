# Phase 5a: CI/CD Pipeline Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Implement GitHub Actions CI/CD workflows that enforce code quality, build across platforms, run tests, and validate PRs.

**Architecture:** Four workflow files covering CI (lint/build/test), PR title validation, nightly extended testing, and Dependabot auto-merge. All workflows use NuGet and npm caching for efficiency.

**Tech Stack:** GitHub Actions, .NET 10, Node.js 22, wagoid/commitlint-github-action, codecov/codecov-action

---

## Task 1: Create CI Workflow

**Files:**

- Create: `.github/workflows/ci.yml`

**Step 1: Write the CI workflow file**

Create `.github/workflows/ci.yml` with three jobs: lint, build, test.

```yaml
name: CI

on:
  push:
    branches: [main, 'feature/**']
  pull_request:
    branches: [main]

concurrency:
  group: ${{ github.workflow }}-${{ github.ref }}
  cancel-in-progress: true

jobs:
  lint:
    name: Lint
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v4
        with:
          fetch-depth: 0

      - name: Setup Node.js
        uses: actions/setup-node@v4
        with:
          node-version: '22'
          cache: 'npm'

      - name: Install npm dependencies
        run: npm ci

      - name: Validate commit messages
        uses: wagoid/commitlint-github-action@v6

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Check .NET formatting
        run: dotnet format --verify-no-changes

      - name: Check Prettier formatting
        run: npm run lint:format

      - name: Check Markdown
        run: npm run lint:markdown

      - name: Check spelling
        run: npm run lint:spelling

      - name: Check secrets
        run: npm run lint:secrets

  build:
    name: Build (${{ matrix.os }})
    needs: lint
    strategy:
      fail-fast: false
      matrix:
        os: [ubuntu-latest, windows-latest, macos-latest]
    runs-on: ${{ matrix.os }}
    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Cache NuGet packages
        uses: actions/cache@v4
        with:
          path: ~/.nuget/packages
          key: ${{ runner.os }}-nuget-${{ hashFiles('**/packages.lock.json', '**/*.csproj', 'Directory.Packages.props') }}
          restore-keys: |
            ${{ runner.os }}-nuget-

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore -c Release

      - name: Upload build artifacts
        uses: actions/upload-artifact@v4
        if: matrix.os == 'ubuntu-latest'
        with:
          name: build-artifacts
          path: |
            **/bin/Release/**
            !**/obj/**

  test:
    name: Test (${{ matrix.os }})
    needs: build
    strategy:
      fail-fast: false
      matrix:
        os: [ubuntu-latest, windows-latest, macos-latest]
    runs-on: ${{ matrix.os }}
    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Cache NuGet packages
        uses: actions/cache@v4
        with:
          path: ~/.nuget/packages
          key: ${{ runner.os }}-nuget-${{ hashFiles('**/packages.lock.json', '**/*.csproj', 'Directory.Packages.props') }}
          restore-keys: |
            ${{ runner.os }}-nuget-

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore -c Release

      - name: Run tests
        run: dotnet test --no-build -c Release --collect:"XPlat Code Coverage" --results-directory ./coverage

      - name: Upload coverage to Codecov
        uses: codecov/codecov-action@v4
        if: matrix.os == 'ubuntu-latest'
        with:
          directory: ./coverage
          fail_ci_if_error: false
          token: ${{ secrets.CODECOV_TOKEN }}
```

**Step 2: Verify the workflow syntax**

Run: `cd .github/workflows && cat ci.yml | head -50`
Expected: File contents displayed without syntax errors

**Step 3: Commit**

```bash
git add .github/workflows/ci.yml
git commit -m "ci: add ci workflow with lint, build, and test jobs

Implements cross-platform build matrix (Ubuntu, Windows, macOS)
with NuGet caching, code coverage, and quality gates per ADR-0021.

refs #10

Co-Authored-By: Claude Opus 4.5 <noreply@anthropic.com>"
```

---

## Task 2: Create PR Title Validation Workflow

**Files:**

- Create: `.github/workflows/pr-title.yml`

**Step 1: Write the PR title validation workflow**

Create `.github/workflows/pr-title.yml`:

```yaml
name: PR Title

on:
  pull_request:
    types: [opened, edited, synchronize, reopened]

jobs:
  validate:
    name: Validate PR Title
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Setup Node.js
        uses: actions/setup-node@v4
        with:
          node-version: '22'
          cache: 'npm'

      - name: Install dependencies
        run: npm ci

      - name: Validate PR title
        uses: amannn/action-semantic-pull-request@v5
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
        with:
          # Require conventional commit format
          types: |
            feat
            fix
            docs
            style
            refactor
            perf
            test
            build
            ci
            chore
            revert
            deps
          # Require scope
          requireScope: false
          # Disallow WIP
          disallowScopes: |
            wip
          # Subject pattern (lowercase, no period)
          subjectPattern: ^[a-z].+[^.]$
          subjectPatternError: |
            PR title must:
            - Start with lowercase letter
            - Not end with a period
            Example: "feat: add new feature"

      - name: Check for issue reference
        run: |
          PR_BODY="${{ github.event.pull_request.body }}"
          if ! echo "$PR_BODY" | grep -qiE "(refs?|closes?|fixes?|resolves?)\s*#[0-9]+"; then
            echo "::warning::PR body should reference an issue (e.g., 'refs #123', 'closes #456')"
          fi
```

**Step 2: Verify the workflow syntax**

Run: `cd .github/workflows && cat pr-title.yml | head -30`
Expected: File contents displayed without syntax errors

**Step 3: Commit**

```bash
git add .github/workflows/pr-title.yml
git commit -m "ci: add pr title validation workflow

Validates PR titles against conventional commit format using
amannn/action-semantic-pull-request. Warns if no issue reference.

refs #10

Co-Authored-By: Claude Opus 4.5 <noreply@anthropic.com>"
```

---

## Task 3: Create Nightly Workflow

**Files:**

- Create: `.github/workflows/nightly.yml`

**Step 1: Write the nightly workflow**

Create `.github/workflows/nightly.yml`:

```yaml
name: Nightly

on:
  schedule:
    # Run at 02:00 UTC every day
    - cron: '0 2 * * *'
  workflow_dispatch:
    inputs:
      run_mutation:
        description: 'Run mutation testing'
        required: false
        default: 'true'
        type: boolean
      run_benchmarks:
        description: 'Run benchmarks'
        required: false
        default: 'true'
        type: boolean

jobs:
  e2e-tests:
    name: E2E Tests
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Cache NuGet packages
        uses: actions/cache@v4
        with:
          path: ~/.nuget/packages
          key: ${{ runner.os }}-nuget-${{ hashFiles('**/packages.lock.json', '**/*.csproj', 'Directory.Packages.props') }}
          restore-keys: |
            ${{ runner.os }}-nuget-

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore -c Release

      - name: Run system tests
        run: dotnet test tests/McjCoderOrg.ClaudeAutoResume.SystemTests --no-build -c Release --logger "trx;LogFileName=system-tests.trx"

      - name: Upload test results
        uses: actions/upload-artifact@v4
        if: always()
        with:
          name: e2e-test-results
          path: '**/TestResults/*.trx'

  mutation-testing:
    name: Mutation Testing
    runs-on: ubuntu-latest
    if: ${{ github.event_name == 'schedule' || inputs.run_mutation == 'true' }}
    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Install Stryker
        run: dotnet tool install -g dotnet-stryker

      - name: Run mutation testing
        run: |
          cd src/McjCoderOrg.ClaudeAutoResume
          dotnet stryker --reporter "html" --reporter "json" --output ../mutation-report || true
        continue-on-error: true

      - name: Upload mutation report
        uses: actions/upload-artifact@v4
        if: always()
        with:
          name: mutation-report
          path: src/mutation-report/**

  benchmarks:
    name: Benchmarks
    runs-on: ubuntu-latest
    if: ${{ github.event_name == 'schedule' || inputs.run_benchmarks == 'true' }}
    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Cache NuGet packages
        uses: actions/cache@v4
        with:
          path: ~/.nuget/packages
          key: ${{ runner.os }}-nuget-${{ hashFiles('**/packages.lock.json', '**/*.csproj', 'Directory.Packages.props') }}
          restore-keys: |
            ${{ runner.os }}-nuget-

      - name: Restore dependencies
        run: dotnet restore

      - name: Run benchmarks
        run: |
          if [ -d "tests/McjCoderOrg.ClaudeAutoResume.Benchmarks" ]; then
            dotnet run -c Release --project tests/McjCoderOrg.ClaudeAutoResume.Benchmarks -- --filter "*" --exporters json
          else
            echo "No benchmark project found, skipping"
          fi
        continue-on-error: true

      - name: Upload benchmark results
        uses: actions/upload-artifact@v4
        if: always()
        with:
          name: benchmark-results
          path: BenchmarkDotNet.Artifacts/**
          if-no-files-found: ignore
```

**Step 2: Verify the workflow syntax**

Run: `cd .github/workflows && cat nightly.yml | head -50`
Expected: File contents displayed without syntax errors

**Step 3: Commit**

```bash
git add .github/workflows/nightly.yml
git commit -m "ci: add nightly workflow for e2e, mutation, and benchmarks

Scheduled at 02:00 UTC daily with manual trigger option.
Runs system tests, Stryker mutation testing, and BenchmarkDotNet.

refs #10

Co-Authored-By: Claude Opus 4.5 <noreply@anthropic.com>"
```

---

## Task 4: Create Dependabot Auto-merge Workflow

**Files:**

- Create: `.github/workflows/dependabot-automerge.yml`

**Step 1: Write the Dependabot auto-merge workflow**

Create `.github/workflows/dependabot-automerge.yml`:

```yaml
name: Dependabot Auto-merge

on:
  pull_request:
    types: [opened, synchronize, reopened]

permissions:
  contents: write
  pull-requests: write

jobs:
  automerge:
    name: Auto-merge Dependabot PRs
    runs-on: ubuntu-latest
    if: github.actor == 'dependabot[bot]'
    steps:
      - name: Fetch Dependabot metadata
        id: metadata
        uses: dependabot/fetch-metadata@v2
        with:
          github-token: ${{ secrets.GITHUB_TOKEN }}

      - name: Auto-approve patch updates
        if: steps.metadata.outputs.update-type == 'version-update:semver-patch'
        run: gh pr review --approve "$PR_URL"
        env:
          PR_URL: ${{ github.event.pull_request.html_url }}
          GH_TOKEN: ${{ secrets.GITHUB_TOKEN }}

      - name: Enable auto-merge for patch updates
        if: steps.metadata.outputs.update-type == 'version-update:semver-patch'
        run: gh pr merge --auto --squash "$PR_URL"
        env:
          PR_URL: ${{ github.event.pull_request.html_url }}
          GH_TOKEN: ${{ secrets.GITHUB_TOKEN }}

      - name: Comment on minor/major updates
        if: steps.metadata.outputs.update-type != 'version-update:semver-patch'
        run: |
          gh pr comment "$PR_URL" --body "This is a **${{ steps.metadata.outputs.update-type }}** update and requires manual review."
        env:
          PR_URL: ${{ github.event.pull_request.html_url }}
          GH_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

**Step 2: Verify the workflow syntax**

Run: `cd .github/workflows && cat dependabot-automerge.yml | head -30`
Expected: File contents displayed without syntax errors

**Step 3: Commit**

```bash
git add .github/workflows/dependabot-automerge.yml
git commit -m "ci: add dependabot auto-merge workflow

Auto-approves and merges patch updates from Dependabot.
Minor and major updates require manual review with comment notification.

refs #10

Co-Authored-By: Claude Opus 4.5 <noreply@anthropic.com>"
```

---

## Task 5: Verify All Workflows and Push

**Step 1: Run linting on all workflow files**

Run: `npm run lint:format && npm run lint:spelling`
Expected: All checks pass

**Step 2: Run dotnet build to ensure no regressions**

Run: `dotnet build`
Expected: Build succeeds

**Step 3: Run tests**

Run: `dotnet test`
Expected: All tests pass

**Step 4: List all workflows**

Run: `ls -la .github/workflows/`
Expected: Shows ci.yml, pr-title.yml, nightly.yml, dependabot-automerge.yml, codeql.yml

**Step 5: Push branch**

Run: `git push -u origin feature/10-cicd-pipeline`
Expected: Branch pushed successfully

---

## Task 6: Create Pull Request

**Step 1: Create PR**

```bash
gh pr create --title "ci: implement ci/cd pipeline workflows" --body "$(cat <<'EOF'
## Summary

- Add CI workflow with lint, build (cross-platform), and test jobs
- Add PR title validation workflow enforcing conventional commits
- Add nightly workflow for E2E tests, mutation testing, and benchmarks
- Add Dependabot auto-merge workflow for patch updates
- Configure NuGet and npm caching across all workflows

## Test plan

- [ ] CI workflow runs on push to feature branch
- [ ] All lint checks pass
- [ ] Cross-platform build succeeds
- [ ] Tests pass on all platforms
- [ ] Coverage uploads to Codecov
- [ ] PR title validation warns on invalid titles
- [ ] Nightly workflow can be triggered manually

refs #10

---

🤖 Generated with [Claude Code](https://claude.com/claude-code)
EOF
)"
```

Expected: PR created with link displayed

**Step 2: Verify PR was created**

Run: `gh pr view`
Expected: Shows PR details

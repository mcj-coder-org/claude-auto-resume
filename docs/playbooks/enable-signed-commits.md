# Playbook: Enable Signed Commits

## Overview

This playbook guides you through setting up GPG-signed commits for your repository.
Signed commits provide cryptographic verification that commits were made by the
claimed author.

## Prerequisites

- Git installed (2.0+)
- GPG installed (`gpg --version` to check)
- GitHub/GitLab/Azure DevOps account with write access

## Step 1: Check Existing GPG Keys

```bash
gpg --list-secret-keys --keyid-format=long
```

If you see output like `sec rsa4096/XXXXXXXXXXXXXXXX`, you already have a key.
Skip to Step 3.

## Step 2: Generate a New GPG Key

```bash
gpg --full-generate-key
```

When prompted:

1. **Key type:** Select `RSA and RSA` (default)
2. **Key size:** Enter `4096` for maximum security
3. **Expiration:** Choose based on your security policy (0 = never expires)
4. **Real name:** Enter your full name (must match your Git config)
5. **Email:** Enter your email (must match your GitHub verified email)
6. **Passphrase:** Create a strong passphrase

## Step 3: Get Your GPG Key ID

```bash
gpg --list-secret-keys --keyid-format=long
```

Output example:

```text
sec   rsa4096/3AA5C34371567BD2 2024-01-01 [SC]
      ABCDEF1234567890ABCDEF1234567890ABCDEF12
uid                 [ultimate] Your Name <your.email@example.com>
ssb   rsa4096/42B317FD4BA89E7A 2024-01-01 [E]
```

Your key ID is `3AA5C34371567BD2` (the part after `rsa4096/`).

## Step 4: Export Your Public Key

```bash
gpg --armor --export YOUR_KEY_ID
```

Copy the entire output including:

```text
-----BEGIN PGP PUBLIC KEY BLOCK-----
...
-----END PGP PUBLIC KEY BLOCK-----
```

## Step 5: Add Key to Your Platform

### GitHub

1. Go to **Settings** > **SSH and GPG keys** > **New GPG key**
2. Paste your public key
3. Click **Add GPG key**

### GitLab

1. Go to **Preferences** > **GPG Keys**
2. Paste your public key
3. Click **Add key**

### Azure DevOps

Azure DevOps does not natively support GPG signature verification.
Signed commits will show as unverified but are still cryptographically signed.

## Step 6: Configure Git

```bash
# Set your signing key
git config --global user.signingkey YOUR_KEY_ID

# Enable signing for all commits
git config --global commit.gpgsign true

# Enable signing for all tags
git config --global tag.gpgsign true
```

## Step 7: Configure GPG Agent (Recommended)

To avoid entering your passphrase for every commit:

### macOS/Linux

Add to `~/.gnupg/gpg-agent.conf`:

```text
default-cache-ttl 3600
max-cache-ttl 86400
```

Then reload:

```bash
gpgconf --kill gpg-agent
```

### Windows

GPG4Win includes a GUI agent. Ensure it's running in the system tray.

## Step 8: Test Your Setup

```bash
# Create a test commit
echo "test" >> test-signing.txt
git add test-signing.txt
git commit -m "test: verify GPG signing works"

# Verify the signature
git log --show-signature -1
```

You should see `Good signature from "Your Name <your.email@example.com>"`.

## Step 9: Verify on Platform

Push your test commit and check the commit on GitHub/GitLab.
You should see a "Verified" badge next to the commit.

## Step 10: Enable Branch Protection

**Only after verifying Steps 8-9 succeed:**

### GitHub

```bash
gh api repos/OWNER/REPO/branches/main/protection/required_signatures -X POST
```

### GitLab

```bash
glab api projects/PROJECT_ID/protected_branches/main -X PATCH \
  -f code_owner_approval_required=true
```

## Troubleshooting

### "gpg failed to sign the data"

```bash
# Ensure GPG_TTY is set
export GPG_TTY=$(tty)

# Add to your shell profile (~/.bashrc, ~/.zshrc)
echo 'export GPG_TTY=$(tty)' >> ~/.bashrc
```

### "secret key not available"

Your Git email doesn't match your GPG key email:

```bash
git config --global user.email "your.gpg.email@example.com"
```

### Commits show "Unverified" on GitHub

1. Ensure your email is verified on GitHub
2. Ensure the GPG key email matches your GitHub verified email
3. Re-upload your public key if recently regenerated

## Pre-commit Hook Verification

Add this to `.husky/pre-commit` to block commits if signing isn't configured:

```bash
# Require GPG commit signing to be configured
if [ "$(git config --get commit.gpgsign)" != "true" ]; then
  echo ""
  echo "❌ ERROR: GPG commit signing is not enabled"
  echo "   All commits must be cryptographically signed."
  echo ""
  echo "   To enable signed commits:"
  echo "   1. Follow the setup guide: docs/playbooks/enable-signed-commits.md"
  echo "   2. Configure git: git config --global commit.gpgsign true"
  echo "   3. Ensure your GPG key is added to GitHub"
  echo ""
  echo "ℹ️  To bypass (not recommended): git commit --no-verify"
  exit 1
fi
```

### Email/Key Mismatch Validation

A common cause of "Unverified" commits on GitHub is when the Git `user.email` doesn't
match the email associated with the GPG signing key. Add this check to `.husky/pre-commit`
to catch mismatches before committing:

```bash
# Validate signing key email matches Git user.email
# This prevents "Unverified" commits on GitHub due to email/key mismatch
SIGNING_KEY=$(git config --get user.signingkey)
GIT_EMAIL=$(git config --get user.email)

if [ -n "$SIGNING_KEY" ] && [ -n "$GIT_EMAIL" ]; then
  # Get the email associated with the signing key
  KEY_EMAIL=$(gpg --list-keys --with-colons "$SIGNING_KEY" 2>/dev/null \
    | grep '^uid:' | head -1 | cut -d: -f10 \
    | sed -n 's/.*<\([^>]*\)>.*/\1/p')

  if [ -n "$KEY_EMAIL" ] && [ "$KEY_EMAIL" != "$GIT_EMAIL" ]; then
    echo ""
    echo "❌ ERROR: Git email does not match GPG signing key email"
    echo ""
    echo "   Git user.email:     $GIT_EMAIL"
    echo "   Signing key email:  $KEY_EMAIL"
    echo ""
    echo "   This would cause 'Unverified' commits on GitHub."
    echo ""
    echo "   To fix, either:"
    echo "   1. Update Git email: git config user.email '$KEY_EMAIL'"
    echo "   2. Use a different signing key that matches your email"
    echo "   3. Add $GIT_EMAIL as a UID to your GPG key"
    echo ""
    echo "   See: docs/playbooks/enable-signed-commits.md"
    echo ""
    echo "ℹ️  To bypass (not recommended): git commit --no-verify"
    exit 1
  fi
fi
```

**Why this matters:** GitHub verifies commits by checking that:

1. The GPG signature is valid
2. The signing key's email matches a verified email on your GitHub account
3. The commit author email matches the signing key's email

If these don't align, commits show "Unverified" even though they're cryptographically
signed correctly.

## Pre-push Hook Verification

While the pre-commit hook ensures GPG signing is **configured**, it cannot detect commits
that were created before signing was enabled, or commits created using `--no-verify`.

Create `.husky/pre-push` to detect commits with **invalid or missing signatures** before
pushing. This prevents branch protection from rejecting your push:

```bash
# Check for unsigned or invalid commits before pushing
# Valid signatures: G (Good), U (Good but untrusted)
# Invalid: N (None), B (Bad), E (Error), R (Revoked), X/Y (Expired)

# Detect the default branch dynamically
DEFAULT_BRANCH=$(git symbolic-ref refs/remotes/origin/HEAD 2>/dev/null | sed 's@^refs/remotes/origin/@@')
if [ -z "$DEFAULT_BRANCH" ]; then
  DEFAULT_BRANCH="main"
fi

while read local_ref local_sha remote_ref remote_sha; do
  # Skip branch deletions and tag pushes
  if [ "$local_sha" = "0000000000000000000000000000000000000000" ]; then
    continue
  fi
  if echo "$local_ref" | grep -q '^refs/tags/'; then
    continue
  fi

  if [ "$remote_sha" = "0000000000000000000000000000000000000000" ]; then
    # New branch - check commits since divergence from default branch
    merge_base=$(git merge-base "origin/$DEFAULT_BRANCH" "$local_sha" 2>/dev/null)
    if [ -n "$merge_base" ]; then
      range="$merge_base..$local_sha"
    else
      range="$local_sha~50..$local_sha"
    fi
  else
    range="$remote_sha..$local_sha"
  fi

  # Check for any non-valid signatures
  invalid=$(git log --format="%H %G?" "$range" 2>/dev/null | grep -vE " [GU]$")
  if [ -n "$invalid" ]; then
    echo "❌ ERROR: Found commits with invalid or missing signatures"
    echo "Run: git rebase origin/$DEFAULT_BRANCH --exec 'git commit --amend --no-edit -S'"
    exit 1
  fi
done
```

## Fixing Unsigned Commits

If you have existing unsigned commits that need to be signed:

### Rebase and Re-sign All Commits

```bash
# Fetch latest main
git fetch origin main

# Rebase and sign each commit
git rebase origin/main --exec "git commit --amend --no-edit -S"

# Force push (with lease for safety)
git push --force-with-lease
```

### Sign Only the Last Commit

```bash
git commit --amend --no-edit -S
git push --force-with-lease
```

### Verify Commits Are Signed

```bash
# Check signature status
git log --format="%h %G? %s" origin/main..HEAD
```

Signature status codes:

- `G` - Good signature
- `U` - Good signature, but untrusted key
- `N` - No signature (unsigned)
- `B` - Bad signature (tampered)
- `E` - Cannot check signature (missing key)
- `R` - Good signature from revoked key
- `X` - Expired signature
- `Y` - Good signature from expired key

## See Also

- [GitHub: Signing commits](https://docs.github.com/en/authentication/managing-commit-signature-verification/signing-commits)
- [GitLab: Signing commits with GPG](https://docs.gitlab.com/ee/user/project/repository/signed_commits/gpg.html)
- [GPG documentation](https://gnupg.org/documentation/)

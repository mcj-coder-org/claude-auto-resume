#!/usr/bin/env bash
# Development environment setup script for Unix (Linux/macOS)
#
# Sets up the development environment for McjCoderOrg.ClaudeAutoResume.
# Checks prerequisites, restores dependencies, configures git hooks, and verifies the build.
#
# Usage: ./scripts/setup.sh

set -e

# Colours
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[0;33m'
CYAN='\033[0;36m'
GRAY='\033[0;90m'
NC='\033[0m' # No Colour

write_status() {
    echo -e "${GREEN}[OK]${NC} $1"
}

write_checking() {
    echo -en "${YELLOW}[..]${NC} $1"
}

write_error() {
    echo -e "${RED}[FAIL]${NC} $1"
}

write_skip() {
    echo -e "${GRAY}[SKIP]${NC} $1"
}

command_exists() {
    command -v "$1" >/dev/null 2>&1
}

echo ""
echo -e "${CYAN}McjCoderOrg.ClaudeAutoResume - Development Setup${NC}"
echo -e "${CYAN}=================================================${NC}"
echo ""

# Check prerequisites
echo -e "Checking prerequisites..."
echo ""

# Check .NET SDK
write_checking ".NET SDK 10.0+: "
if command_exists dotnet; then
    DOTNET_VERSION=$(dotnet --version 2>/dev/null)
    if [[ "$DOTNET_VERSION" =~ ^10\. ]]; then
        echo ""
        write_status ".NET SDK $DOTNET_VERSION"
    else
        echo ""
        write_error ".NET SDK 10.0+ required, found $DOTNET_VERSION"
        echo -e "  ${GRAY}Install from: https://dotnet.microsoft.com/download/dotnet/10.0${NC}"
        exit 1
    fi
else
    echo ""
    write_error ".NET SDK not found"
    echo -e "  ${GRAY}Install from: https://dotnet.microsoft.com/download/dotnet/10.0${NC}"
    exit 1
fi

# Check Node.js
write_checking "Node.js 22+: "
if command_exists node; then
    NODE_VERSION=$(node --version 2>/dev/null)
    NODE_MAJOR=$(echo "$NODE_VERSION" | sed 's/v\([0-9]*\).*/\1/')
    if [ "$NODE_MAJOR" -ge 22 ]; then
        echo ""
        write_status "Node.js $NODE_VERSION"
    else
        echo ""
        write_error "Node.js 22+ required, found $NODE_VERSION"
        echo -e "  ${GRAY}Install from: https://nodejs.org/${NC}"
        exit 1
    fi
else
    echo ""
    write_error "Node.js not found"
    echo -e "  ${GRAY}Install from: https://nodejs.org/${NC}"
    exit 1
fi

# Check Git
write_checking "Git: "
if command_exists git; then
    GIT_VERSION=$(git --version 2>/dev/null)
    echo ""
    write_status "$GIT_VERSION"
else
    echo ""
    write_error "Git not found"
    echo -e "  ${GRAY}Install from: https://git-scm.com/${NC}"
    exit 1
fi

# Check GitHub CLI (optional)
write_checking "GitHub CLI: "
if command_exists gh; then
    GH_VERSION=$(gh --version 2>/dev/null | head -1)
    echo ""
    write_status "$GH_VERSION"
else
    echo ""
    write_skip "GitHub CLI not found (optional)"
    echo -e "  ${GRAY}Install from: https://cli.github.com/${NC}"
fi

echo ""
echo "Installing dependencies..."
echo ""

# Install npm dependencies
write_checking "npm install: "
if npm install --silent 2>/dev/null; then
    echo ""
    write_status "npm packages installed"
else
    echo ""
    write_error "npm install failed"
    exit 1
fi

# Restore .NET dependencies
write_checking "dotnet restore: "
if dotnet restore --verbosity quiet 2>/dev/null; then
    echo ""
    write_status ".NET packages restored"
else
    echo ""
    write_error "dotnet restore failed"
    exit 1
fi

# Restore .NET tools
write_checking "dotnet tool restore: "
if dotnet tool restore --verbosity quiet 2>/dev/null; then
    echo ""
    write_status ".NET tools restored"
else
    echo ""
    write_error "dotnet tool restore failed"
    exit 1
fi

echo ""
echo "Configuring git hooks..."
echo ""

# Configure git hooks via Husky
write_checking "Husky hooks: "
if npx husky install 2>/dev/null; then
    echo ""
    write_status "Git hooks configured"
else
    echo ""
    write_skip "Husky install skipped (may already be configured)"
fi

echo ""
echo "Verifying build..."
echo ""

# Build the solution
write_checking "dotnet build: "
if dotnet build --verbosity quiet 2>/dev/null; then
    echo ""
    write_status "Build successful"
else
    echo ""
    write_error "Build failed"
    exit 1
fi

# Run tests
write_checking "dotnet test: "
if dotnet test --verbosity quiet --no-build 2>/dev/null; then
    echo ""
    write_status "All tests passed"
else
    echo ""
    write_error "Tests failed"
    exit 1
fi

echo ""
echo -e "${CYAN}=================================================${NC}"
echo -e "${GREEN}Setup complete!${NC}"
echo ""
echo "Next steps:"
echo -e "  ${GRAY}1. Open the project in VS Code or Visual Studio${NC}"
echo -e "  ${GRAY}2. Read CONTRIBUTING.md for contribution guidelines${NC}"
echo -e "  ${GRAY}3. Check docs/ for project documentation${NC}"
echo ""

# ADR-0001: License

## Status

Proposed

## Date

2026-01-09

## Context

We need to choose an open source license for the project.

### Requirements

- Permissive for broad adoption
- Compatible with .NET ecosystem
- Clear attribution requirements

### Options Considered

#### Option 1: MIT (Selected)

Simple permissive license.

**Pros:**

- Maximum permissiveness
- Industry standard
- Compatible with all other licenses
- Simple to understand

**Cons:**

- No patent grant
- No warranty disclaimer emphasis

#### Option 2: Apache 2.0

Permissive with patent grant.

**Pros:**

- Patent protection
- Contribution terms

**Cons:**

- More complex
- NOTICE file requirement

#### Option 3: GPL-3.0

Copyleft license.

**Pros:**

- Protects source availability

**Cons:**

- Incompatible with proprietary use
- Limits adoption

## Decision

**MIT License** for maximum permissiveness and adoption.

```text
MIT License

Copyright (c) 2026 McjCoderOrg

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

## Consequences

### Positive

- Broad adoption possible
- Simple compliance
- Industry standard

### Negative

- No patent protection
- No contribution agreement

## References

- [MIT License](https://opensource.org/licenses/MIT)
- [Choose a License](https://choosealicense.com/)

---
applyTo: "**"
---

# Project general coding standards

## Code Complexity

- AuthorWithLowCyclomaticComplexity: Ensure that the authored code maintains a very low cyclomatic complexity.
- AuthorWithLowCognitiveComplexity: Ensure that the authored code maintains a very low cognitive complexity.
- AuthorWithLowDefectDensity: Ensure that the authored code maintains a low defect density.
- AuthorWithLowCodeDuplication: Ensure that the authored code has low code duplication.
- NeverHardcodeSecrets: Avoid harcoding API keys, tokens or sensitive data.
- ValidateURLs: Validate and sanitize all URLs before rendering.
- UseSafeURLLoadingPractices: Check that URLs being loaded are from trusted sources and apply Content Security Policies (CSP).
- UseHTTPS: Ensure all API calls are made over HTTPS to prevent man-in-the-middle attacks.
- AuditThirdPartyLibraries: Regularly check and update third-party libraries to prevent security vulnerabilities in the latest version.

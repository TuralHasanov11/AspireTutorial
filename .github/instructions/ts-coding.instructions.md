---
applyTo: "**/*.ts,"
---

# Project coding standards for TypeScript and Vue

Apply the [general coding guidelines](./general-coding.instructions.md) to all code.

## General Guidelines

- Use TypeScript for all new code
- Follow functional programming principles where possible
- Use interfaces for data structures and type definitions
- Prefer immutable data (const, readonly)
- Use optional chaining (?.) and nullish coalescing (??) operators
- SanitizeUserInputs: Ensure that all user inputs are sanitized before rendering. Use libraries like `DOMPurify`.
- UseSecureStorage: Do not store sensitive data in localStorage, sessionStorage, IndexDB or similar.

## Naming Conventions

- Use PascalCase for component names, interfaces, and type aliases
- Use camelCase for variables, functions, and methods
- Prefix private class members with underscore (\_)
- Use ALL_CAPS for constants

## Error Handling

- Use try/catch blocks for async operations
- Always log errors with contextual information

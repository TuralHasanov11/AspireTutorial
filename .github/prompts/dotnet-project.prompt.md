---
mode: "agent"
model: GPT-4o
tools: ["githubRepo", "codebase", "notion"]
description: "Scan the codebase for necessary .NET features to be implemented"
---

Your goal is to identify and document the necessary .NET features that need to be implemented in the codebase.

Ask for the specific features or areas of the codebase to focus on if not provided.

Requirements for the project:

- Ensures that the DI container validates the service configuration when the application is built
- Enable Configuration Binding Generator
- (Server: Kestrel) is removed from the HTTP responses
- Accelerate Builds In Visual Studio
- Configure Host Options
- Response Caching
- Distributed Caching
- Output Caching
- Rate Limiting
- Response Compression
- Culture Middleware
- CORS
- Request Decompression
- HTTP Logging
- Health checks
- Metrics
- Event Counters
- Global Exception Handling
- Resiliency
- Circuit Breaker
- Outbox Messages
- Health Checks
- CQRS
- Request Timeouts
- Authorization Permission Policies
- Database Context Pooling / Compiled Queries
- Multi Factor Authentication
- Prevent Primitive Obsession
- Load Shedding
- Suppress Startup Lifetime messages
- GDPR and Redaction
- Feature Management
- Localization and Internationalization
- Idempotency

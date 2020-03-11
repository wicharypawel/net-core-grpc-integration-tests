# Net Core gRPC Integration Tests

## This repository

This repository shows how to test gRPC code. Testing types are:

- unit testing - mock dependencies and test gRPC services in isolation
- integration testing - test how service implementation work with dependencies

While integration testing we will use in memory server to isolate physical machine. Following implementation use MVC in-memory test server. This approach is not yet official.

## Getting started

1. Download repository 
2. Download .Net SDK (in the moment of writing 3.1.101)
3. Open in VS 2019 or newer
4. Start debugging project
5. Start testing in VS

## Cleanup

- not apply

## Sources

- https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests (this code is reusing MVC in-memory test server)
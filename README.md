# The Unified Document Viewer

A small ASP.NET Core application that consolidates sale and service documents into a single unified document DTO.

**Requirements:** .NET 9 SDK

**Quick Links:**
- Source: [Chap10.csproj](Chap10.csproj#L1)
- Tests: Tests/Chap10.Tests

**Build**

Restore and build the solution from the repository root:

```powershell
dotnet restore
dotnet build -c Release
```

**Run**

Run the application (development):

```powershell
dotnet run --project Chap10.csproj
```

The app binds to the configured URLs in `appsettings.json` when run locally.

**Tests**

A unit test project using xUnit and Moq has been added at `Tests/Chap10.Tests`.

Run the tests from the repository root:

```powershell
dotnet test Tests/Chap10.Tests -c Release
```

What the tests cover:
- `UnifiedDocumentService` behavior when both APIs return data.
- Handling of `HttpRequestException` and `TaskCanceledException` from API clients.
- Scenarios where one or both API clients return `null` or throw — service still returns a consolidated DTO.

**Where to look**
- Service implementation: [Services/UnifiedDocumentService/UnifiedDocumentService.cs](Services/UnifiedDocumentService/UnifiedDocumentService.cs#L1)
- API client interfaces: [Services/Shared/ISaleApiClient.cs](Services/Shared/ISaleApiClient.cs#L1), [Services/Shared/IServiceApiClient.cs](Services/Shared/IServiceApiClient.cs#L1)
- Tests: [Tests/Chap10.Tests/UnifiedDocumentServiceTests.cs](Tests/Chap10.Tests/UnifiedDocumentServiceTests.cs#L1)

**Extending tests**

Add more unit tests in `Tests/Chap10.Tests` to cover logging, edge cases, and residual behaviors. Consider adding integration tests that wire lightweight HTTP test servers to exercise API client implementations.

**AI Collaboration Narrative**

This repository was updated with an AI-assisted workflow to add a robust unit test suite and improve developer documentation. Summary of the collaboration process and quality steps taken:

- High-level strategy: guide the AI to focus on testable units (the `UnifiedDocumentService`) and create small, deterministic tests that exercise success and error paths. Keep tests isolated using mocks to avoid external dependencies.
- Verification & refinement process: iteratively inspected service code, identified public interfaces (`ISaleApiClient`, `IServiceApiClient`) and DTOs, then asked the AI to author tests that mock those interfaces. Each test was designed to assert clear outcomes (presence/absence of DTOs and VIN preservation).
- Ensuring code quality: used existing coding patterns in the repository (constructor injection, DTO shapes) and preferred minimal, readable tests. Chose xUnit + Moq for familiarity and stable tooling. The produced changes were limited in scope and placed in `Tests/Chap10.Tests` to avoid touching production code.
- How you can validate locally: run `dotnet restore`, `dotnet build`, and `dotnet test` as shown above to verify tests and build success.

If you'd like, I can also:
- Add a CI workflow to run `dotnet test` on GitHub Actions.
- Expand tests to cover other services/controllers.
- Run the tests here and report results (if you allow running commands).

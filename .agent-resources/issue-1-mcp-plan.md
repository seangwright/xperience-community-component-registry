# Issue #1 plan: Expose registry data over MCP

## Goal

Expose the same component registry data (definitions + usage detail) over a self-hosted MCP server, while keeping existing Admin UI behavior unchanged.

See [GitHub issue for this repository](https://github.com/seangwright/xperience-community-component-registry/issues/1).

## Current state (repo)

- Registry definition stores + `IComponentUsageService` live in `src/XperienceCommunity.ComponentRegistry`.
- DTOs + mapping from registry definitions to client models are currently duplicated in Admin pages:
  - `PageBuilderComponentViewerPage`
  - `EmailBuilderComponentViewerPage`
  - `FormBuilderComponentViewerPage`
- No MCP SDK wiring exists yet (`AddMcpServer` not present).
- Public setup docs currently only mention `builder.Services.AddComponentRegistry();`.

## Proposed architecture

1. **Move read-model mapping into core project**
   - Add a new read service in `XperienceCommunity.ComponentRegistry` that returns normalized DTOs for:
     - Page Builder widgets/sections/templates
     - Email Builder widgets/sections/templates
     - Form Builder components/sections
   - Service encapsulates localization mapping (`IAdminBuildersLocalizationService`) and store access.
   - Admin pages consume this service instead of mapping inline.

2. **Expose registry + usage via MCP tools**
   - Add MCP SDK packages to `XperienceCommunity.ComponentRegistry`:
     - `ModelContextProtocol`
     - `ModelContextProtocol.AspNetCore` (HTTP transport hosting)
   - Add MCP tool types in core project for:
     - listing component definitions per builder/type
     - getting usage detail for specific component identifiers (delegating to `IComponentUsageService`)
   - Keep tool contracts stable and text-friendly for agent consumption.

3. **Configurable MCP registration**
   - Add options model, e.g. `ComponentRegistryMcpOptions`:
     - `Enabled` (bool)
     - `EndpointPath` (string, default `/mcp`)
     - optional auth/allowlist toggles
   - Add extension method, e.g. `AddComponentRegistryMcp(IConfiguration)` or options overload.
   - In host app (`Program.cs`), register MCP conditionally via settings:
     - if enabled: `builder.Services.AddMcpServer()` with HTTP transport and tool registration.

4. **HTTP transport and client compatibility**
   - Server is hosted over HTTP (SSE/streamable HTTP).

## Phased implementation plan

### Phase 0 — package + API baseline

- Add MCP package references centrally (`Directory.Packages.props`) and to core project `.csproj`.
- Verify target framework compatibility with selected MCP package version.
- Add feature flag options class + configuration binding.

### Phase 1 — extract mapping service (no behavior change)

- Introduce core DTO/read-model types (shared by Admin + MCP).
- Add `IComponentRegistryReadService` (name TBD) in core project with methods like:
  - `GetPageBuilderRegistryAsync()`
  - `GetEmailBuilderRegistryAsync()`
  - `GetFormBuilderRegistryAsync()`
- Refactor `PageBuilderComponentViewerPage` first to use service.
- Refactor Email/Form pages in same pattern (recommended to avoid split architecture).
- Keep existing page client property shapes unchanged to avoid frontend impact.

### Phase 2 — MCP server tools

- Add MCP tool class(es) in core project, attributed for discovery by SDK.
- Implement tools for:
  - list all component definitions by builder/type
  - get single-component usage detail by identifier + type
  - optional batch usage endpoint (maps to `GetBatchUsageAsync`)
- Ensure validation + clear error responses for unknown identifiers/types.

### Phase 3 — host integration + config

- Update sample host (`examples/DancingGoat/Program.cs`) with conditional MCP registration.
- Add `appsettings.json` section for MCP config and endpoint path.
- Ensure endpoint mapping occurs in request pipeline when enabled.

### Phase 4 — docs + verification

- Update `README.md` and `docs/Usage-Guide.md`:
  - how to enable/disable MCP
  - endpoint + security guidance
- Add tests:
  - unit tests for read-model mapping service
  - unit tests for MCP tool handlers (mock stores + usage service)
  - smoke/integration test for MCP endpoint enabled/disabled behavior (if feasible)

## Potential issues / risks

1. **Localization dependency in core project**
   - Mapping currently depends on `IAdminBuildersLocalizationService` (Admin package).
     - Keep localization in Admin adapter, MCP returns raw names/descriptions.

1. **Security / data exposure**
   - Usage tools expose page/form/email usage data broadly.
   - Mitigation: MCP default `Enabled = false`; document auth/network restrictions; optionally require host auth policy.
   - Recommend for `IsDevelopment() == true` environments only.

1. **Versioning risk (MCP SDK preview)**
   - C# SDK is preview and can introduce breaking changes.
   - Mitigation: pin tested version in central package versions + document upgrade guidance.

1. **Performance risk in usage endpoints**
   - Existing usage queries are DB-heavy for large datasets.
   - Mitigation: pagination/limits in MCP tool args, optional timeout/cancellation, avoid large batch defaults.
     - Add cancellation token support to db queries

1. **Backward compatibility risk for Admin client**
   - DTO move can break Admin template contracts if field names change.
     - Will bump to v2.0.0 for MCP support

## Open decisions to resolve early

1. Should MCP live in the existing host process only, or support separate hosted process later?
   - Live in host process
2. Should localized labels be returned by MCP, or raw registration metadata only?
   - Localized labels
3. Minimal auth requirement for MCP endpoint in docs/sample (none, API key, existing app auth)?
   - Focus on "dev" environment scenario first
4. Do we include Email/Form tools in initial PR, or deliver Page Builder first and follow with others?
   - All registry components

## Recommended incremental delivery (PR slicing)

1. **PR1**: Extract core read-model + refactor Admin pages (no MCP yet).
2. **PR2**: Add MCP tools + config flag + sample host integration (Page Builder first).
3. **PR3**: Extend MCP coverage to Email/Form + docs.

## Acceptance criteria

- Admin registry pages function unchanged after mapping extraction.
- MCP endpoint can be toggled by app settings (`Enabled` false by default).
- MCP exposes:
  - component definitions
  - usage detail via existing `IComponentUsageService`
- Setup/docs include secure-by-default guidance.

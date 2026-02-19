# Usage Guide

## Setup

Add the "Admin" package to your ASP.NET Core application using the .NET CLI. This includes the custom admin UI application and all required services.

```powershell
dotnet add package XperienceCommunity.ComponentRegistry.Admin
```

If you wish to separately install just the registry services, you can use the following:

```powershell
dotnet add package XperienceCommunity.ComponentRegistry
```

## Quick Start

Register the library's services in your ASP.NET Core application:

```csharp
// Program.cs

// ...

builder.Services.AddComponentRegistry();
builder.Services.AddComponentRegistryMcp(builder.Configuration);
```

Run the application and navigate to the "Component Registry" application in the Xperience administration under the "Development" category.

You can control access to Component Registry application and each of the 3 registry pages through Xperience's role and permission management.

### MCP endpoint (optional)

MCP support is disabled by default.

Add configuration:

```json
{
  "XperienceCommunity": {
    "ComponentRegistry": {
      "Mcp": {
        "Enabled": true,
        "EndpointPath": "/mcp/component-registry"
      }
    }
  }
}
```

Map endpoint in request pipeline:

```csharp
app.MapComponentRegistryMcp(app.Configuration);
```

Configure your project's MCP servers using your AI development tool of choice.

Example: VS Code and GitHub Copilot `.vscode/mcp.json`

```json
{
  "servers": {
    "xperience-component-registry": {
      "type": "http",
      "url": "http://localhost:53856/mcp/component-registry"
    }
  }
}
```

> [!WARNING]
> The MCP server exposes component definitions and usage details (including page and content details) without any authentication. The MCP server feature is **intended for development-environments only**.
>
> Use [environment identification extensions](https://docs.kentico.com/documentation/developers-and-admins/configuration/saas-configuration#environment-identification-extension-methods) or [environment specific settings](https://docs.kentico.com/guides/development/deployment/deploy-to-private-cloud#separate-the-app-settings) to disable the MCP server for non-local deployments.

### Permissions

- `View`: Required to see the application tile on the administration dashboard
- `View Page Builder components`: Enables viewing the Page Builder component registry page.
- `View Page Builder component usages`: Gives expanded permissions on the registry page, enabling viewing the usages of individual Page Builder components across all website channels. This permission does not evaluate any [page permission management](https://docs.kentico.com/x/permissions_pagelevel_xp) and could expose content to administration users they normally would not have access to.
- `View Form Builder components`: Enables viewing the Form Builder component registry page.
- `View Form Builder component usages`: Gives expanded permissions on the registry page, enabling viewing the usages of individual Form Builder components across all forms. This permission does not evaluate other roles defined through [role management](https://docs.kentico.com/x/7IVwCg) and could expose a list of forms to administration users they normally would not have access to.
- `View Page Builder components`: Enables viewing the Email Builder component registry page.
- `View Page Builder component usages`: Gives expanded permissions on the registry page, enabling viewing the usages of individual Email Builder components across all email channels. This permission does not evaluate other roles defined through [role management](https://docs.kentico.com/x/7IVwCg) and could expose content to administration users they normally would not have access to.

## MCP Tools Reference

When MCP support is enabled (via `appsettings.json`), the following tools are exposed through an HTTP endpoint to enable AI agents and other clients to discover and interact with component registrations.

### component_registry_list_definitions

Lists registered component definitions for a specific builder and component type.

**Parameters:**

- `builder` (string, required): Builder type - `page`, `email`, or `form`
- `componentType` (string, optional): Component type filter - `widget`, `section`, `template`, `page-template`, `component`, or `all` (default: `all`)

**Returns:** `ComponentDefinitionListResponse`

```json
{
  "Builder": "page",
  "ComponentType": "widget",
  "Items": [
    {
      "Builder": "page",
      "ComponentType": "widget",
      "Identifier": "MyCompany.HeroWidget",
      "Name": "Hero Banner",
      "Description": "Full-width hero banner with image and text",
      "IconClass": "icon-picture",
      "MarkedTypeName": "MyCompany.Components.HeroWidgetViewComponent",
      "PropertiesTypeName": "MyCompany.Components.HeroWidgetProperties",
      "ContentTypeNames": ["MyCompany.Article", "MyCompany.LandingPage"]
    }
  ]
}
```

**Example use case:** Discover all widgets available for Page Builder, or list all email templates registered in the system.

### component_registry_get_usage

Gets detailed usage information for a specific component identifier across all pages, emails, or forms.

**Parameters:**

- `builder` (string, required): Builder type - `page`, `email`, or `form`
- `componentType` (string, required): Component type - `widget`, `template`, `page-template`, `component`, or `section`
- `componentIdentifier` (string, required): The unique identifier of the component

**Returns:** Varies by builder type:

- **Page Builder**: `ComponentUsageDetailDto` - Contains `TotalPagesUsing`, `TotalVariants`, `LastModified`, and list of `Pages` with their `Variants`
- **Email Builder**: `EmailConfigurationUsageDetailDto` - Contains `TotalEmailConfigurationsUsing`, `TotalVariants`, and list of `EmailConfigurations` with their `Variants`
- **Form Builder**: `FormComponentUsageDetailDto` - Contains `TotalFormClassesUsing`, `TotalFormBuilderFormsUsing`, and lists of `FormClasses` and `FormBuilderForms`

Each variant includes:

- `LanguageName`: Display name of the language (e.g., "English (United States)")
- `IsPublished`: Whether the variant is published
- `LastModified`: Last modification date
- `ConfigurationJson`: The component's JSON configuration

**Example use case:** Determine which pages use a specific widget and in which languages, including whether changes are published or in draft.

### component_registry_get_page_batch_usage

Gets page builder usage details for multiple widget or page template identifiers in a single request (batch operation).

**Parameters:**

- `componentIdentifiers` (array of strings, required): List of component identifiers to query
- `componentType` (string, required): Component type - `widget` or `page-template`

**Returns:** `List<ComponentUsageDetailDto>` - Array of usage details, one per identifier

**Example use case:** Retrieve usage statistics for multiple widgets at once to build a dashboard or report showing which components are most frequently used.

### component_registry_get_web_page_url

Gets the absolute URL for a web page by its ID and language, enabling agents to visit and validate component rendering.

**Parameters:**

- `webPageItemId` (int, required): The web page item ID from usage data (e.g., from `PageUsageDto.WebPageItemId`)
- `languageName` (string, required): Language name (e.g., "en-US")
- `isPublished` (bool, optional): Whether to retrieve published URL (`true`, default) or shareable preview URL for unpublished changes (`false`)

**Returns:** `WebPageUrlResponse`

```json
{
  "WebPageItemId": 123,
  "LanguageName": "en-US",
  "IsPublished": false,
  "UrlType": "ShareablePreview",
  "Url": "https://example.com/preview/abc123def456",
  "Success": true,
  "ErrorMessage": null
}
```

**URL Types:**

- `Published`: Standard live page URL for published content
- `ShareablePreview`: Temporary preview link for unpublished draft changes

**Example use case:** After discovering a widget is used on specific pages, retrieve the URL to visit and visually inspect how the widget renders. For unpublished changes, use `isPublished=false` with the `IsPublished` value from `PageVariantDto` to get a shareable preview link.

### Complete Workflow Example

1. **Discover components**: Use `component_registry_list_definitions` to find all page widgets
2. **Find usage**: Use `component_registry_get_usage` to see which pages use a specific widget
3. **Get URLs**: Use `component_registry_get_web_page_url` with `WebPageItemId` and `LanguageName` from the usage data
4. **Visit & validate**: Navigate to the returned URLs to inspect component rendering in published or preview mode

This enables AI agents to autonomously discover, analyze, and validate component implementations across your Xperience application.

## Custom use

You can inject `IComponentDefinitionStore<TDefinition>` into your own code to access all the component registrations where `TDefinition` is one of the following types:

- `XperienceCommunity.ComponentRegistry.PageBuilderWidgetDefinition`
- `XperienceCommunity.ComponentRegistry.PageBuilderSectionDefinition`
- `XperienceCommunity.ComponentRegistry.PageBuilderPageTemplateDefinition`
- `XperienceCommunity.ComponentRegistry.EmailBuilderWidgetDefinition`
- `XperienceCommunity.ComponentRegistry.EmailBuilderSectionDefinition`
- `XperienceCommunity.ComponentRegistry.EmailBuilderTemplateDefinition`
- `XperienceCommunity.ComponentRegistry.FormBuilderComponentDefinition`
- `XperienceCommunity.ComponentRegistry.FormBuilderSectionDefinition`

Each type has its own Store service.

By default the registry uses assembly scanning through Xperience's `[assembly: AssemblyDiscoverable]` marker attribute for fast identification. The registry will also automatically include the "host" ASP.NET Core assembly's components even if this assembly does not have the attribute.

You can also supply your own list of assemblies to scan for components using the `IServiceCollection` overload:

```csharp
IEnumerable<Assembly> assemblies = [...];

builder.Services.AddComponentRegistry(assemblies);
```

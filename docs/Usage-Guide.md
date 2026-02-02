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
```

Run the application and navigate to the "Component Registry" application in the Xperience administration under the "Development" category.

You can control access to Component Registry application and each of the 3 registry pages through Xperience's role and permission management.

### Permissions

- `View`: Required to see the application tile on the administration dashboard
- `View Page Builder components`: Enables viewing the Page Builder component registry page.
- `View Page Builder component usages`: Gives expanded permissions on the registry page, enabling viewing the usages of individual Page Builder components across all website channels. This permission does not evaluate any [page permission management](https://docs.kentico.com/x/permissions_pagelevel_xp) and could expose content to administration users they normally would not have access to.
- `View Form Builder components`: Enables viewing the Form Builder component registry page.
- `View Form Builder component usages`: Gives expanded permissions on the registry page, enabling viewing the usages of individual Form Builder components across all forms. This permission does not evaluate other roles defined through [role management](https://docs.kentico.com/x/7IVwCg) and could expose a list of forms to administration users they normally would not have access to.
- `View Page Builder components`: Enables viewing the Email Builder component registry page.
- `View Page Builder component usages`: Gives expanded permissions on the registry page, enabling viewing the usages of individual Email Builder components across all email channels. This permission does not evaluate other roles defined through [role management](https://docs.kentico.com/x/7IVwCg) and could expose content to administration users they normally would not have access to.

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

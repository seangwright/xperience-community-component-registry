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

You can also supply your own list of assemblies to scan for components using the `IServiceCollection` overload:

```csharp
IEnumerable<Assembly> assemblies = [...];

builder.Services.AddComponentRegistry(assemblies);
```

By default the assembly scanning uses Xperience's `[assembly: AssemblyDiscoverable]` marker attribute for fast identification.

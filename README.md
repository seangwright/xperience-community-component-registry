# Xperience Community: Component Registry

[![CI: Build and Test](https://github.com/seangwright/xperience-community-component-registry/actions/workflows/ci.yml/badge.svg)](https://github.com/seangwright/xperience-community-component-registry/actions/workflows/ci.yml)

[![Release: Publish to NuGet](https://github.com/seangwright/xperience-community-component-registry/actions/workflows/publish.yml/badge.svg)](https://github.com/seangwright/xperience-community-component-registry/actions/workflows/publish.yml)

[![NuGet Package](https://img.shields.io/nuget/v/XperienceCommunity.ComponentRegistry.svg)](https://www.nuget.org/packages/XperienceCommunity.ComponentRegistry)

[![NuGet Package](https://img.shields.io/nuget/v/XperienceCommunity.ComponentRegistry.Admin.svg)](https://www.nuget.org/packages/XperienceCommunity.ComponentRegistry.Admin)

## Description

This project enables administrators to view all registered custom components in an Xperience by Kentico application, like Page Builder widgets, and explore which channels and web pages use those components all through a friendly user interface in the Xperience administration.

<div style="display: flex; gap: 1rem; flex-wrap: wrap">
  <a href="https://raw.githubusercontent.com/seangwright/xperience-community-component-registry/main/images/component-registry-admin-page-builder.jpg">
    <img src="https://raw.githubusercontent.com/seangwright/xperience-community-component-registry/main/images/component-registry-admin-page-builder.jpg"
    width="600" alt="Component registry for Page Builder in Xperience administration">
  </a>

  <a href="https://raw.githubusercontent.com/seangwright/xperience-community-component-registry/main/images/component-registry-admin-email-builder.jpg">
    <img src="https://raw.githubusercontent.com/seangwright/xperience-community-component-registry/main/images/component-registry-admin-email-builder.jpg"
    width="600" alt="Component registry for Email Builder in Xperience administration">
  </a>

  <a href="https://raw.githubusercontent.com/seangwright/xperience-community-component-registry/main/images/component-registry-admin-form-builder.jpg">
    <img src="https://raw.githubusercontent.com/seangwright/xperience-community-component-registry/main/images/component-registry-admin-form-builder.jpg"
    width="600" alt="Component registry for Form Builder in Xperience administration">
  </a>

  </div>

Interested in how and why this library was created? Read the blog post [Dream and Experiment: Building a Component Registry Dashboard with AI](https://community.kentico.com/blog/dream-and-experiment-building-a-component-registry-dashboard-with-ai).

## Requirements

### Library Version Matrix

| Xperience Version | Library Version |
| ----------------- | --------------- |
| >= 31.1.1         | >= 1.0.0        |

### Dependencies

- [ASP.NET Core 10.0](https://dotnet.microsoft.com/en-us/download)
- [Xperience by Kentico](https://docs.kentico.com)

## Package Installation

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

## Full Instructions

View the [Usage Guide](./docs/Usage-Guide.md) for more detailed instructions on permission management and custom scenarios.

## Contributing

To see the guidelines for Contributing to Kentico open source software, please see [Kentico's `CONTRIBUTING.md`](https://github.com/Kentico/.github/blob/main/CONTRIBUTING.md) for more information and follow the [Kentico's `CODE_OF_CONDUCT`](https://github.com/Kentico/.github/blob/main/CODE_OF_CONDUCT.md).

Instructions and technical details for contributing to **this** project can be found in [Contributing Setup](./docs/Contributing-Setup.md).

## License

Distributed under the MIT License. See [`LICENSE.md`](./LICENSE.md) for more information.

## Support

This project is provided freely to the Kentico community and has no guaranteed support policy. If updates to this repository are not made on timelines that meet your needs, you are welcome to fork it and customize your version to add features and resolve bugs 😄.

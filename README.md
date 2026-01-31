# XperienceCommunity.ComponentRegistry

[![CI: Build and Test](https://github.com/Kentico/repo-template/actions/workflows/ci.yml/badge.svg)](https://github.com/Kentico/repo-template/actions/workflows/ci.yml)

## Description

This project enables administrators to view all registered custom components in an Xperience by Kentico application, like Page Builder widgets, and explore which channels and web pages use those components all through a friendly user interface in the Xperience administration.

## Requirements

### Library Version Matrix

---This matrix explains which versions of the library are compatible with different versions of Xperience by Kentico / Kentico Xperience 13---

| Xperience Version | Library Version |
| ----------------- | --------------- |
| >= 31.1.0         | 1.0.0           |

### Dependencies

---These are all the dependencies required to use (not build) the library---

- [ASP.NET Core 10.0](https://dotnet.microsoft.com/en-us/download)
- [Xperience by Kentico](https://docs.kentico.com)

### Other requirements

---A list of other requirements and prerequisites needed to use the library. If there are none, don't include this section in the readme.---

## Package Installation

---This details the steps required to add the library to a solution. This could include multiple packages (NuGet and/or npm)---

Add the package to your application using the .NET CLI

```powershell
dotnet add package XperienceCommunity.ComponentRegistry.Admin
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

View the [Usage Guide](./docs/Usage-Guide.md) for more detailed instructions.

## Contributing

To see the guidelines for Contributing to Kentico open source software, please see [Kentico's `CONTRIBUTING.md`](https://github.com/Kentico/.github/blob/main/CONTRIBUTING.md) for more information and follow the [Kentico's `CODE_OF_CONDUCT`](https://github.com/Kentico/.github/blob/main/CODE_OF_CONDUCT.md).

Instructions and technical details for contributing to **this** project can be found in [Contributing Setup](./docs/Contributing-Setup.md).

## License

Distributed under the MIT License. See [`LICENSE.md`](./LICENSE.md) for more information.

## Support

This project is provided freely to the Kentico community and has no guaranteed support policy. If updates to this repository are not made on timelines that meet your needs, you are welcome to fork it and customize your version to add features and resolve bugs 😄.

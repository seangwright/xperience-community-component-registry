using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace XperienceCommunity.ComponentRegistry;

/// <summary>
/// Registration and endpoint mapping extensions for component registry MCP support.
/// </summary>
public static class ComponentRegistryMcpServiceCollectionExtensions
{
    /// <summary>
    /// Adds component registry MCP services based on app configuration.
    /// </summary>
    public static IServiceCollection AddComponentRegistryMcp(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var optionsSection = configuration.GetSection(ComponentRegistryMcpOptions.SectionName);
        services.Configure<ComponentRegistryMcpOptions>(optionsSection);

        var options = optionsSection.Get<ComponentRegistryMcpOptions>() ?? new ComponentRegistryMcpOptions();
        if (!options.Enabled)
        {
            return services;
        }

        services.AddMcpServer()
            .WithHttpTransport()
            .WithToolsFromAssembly(typeof(ComponentRegistryMcpTools).Assembly);

        return services;
    }

    /// <summary>
    /// Maps component registry MCP endpoints when enabled.
    /// </summary>
    public static WebApplication MapComponentRegistryMcp(
        this WebApplication app,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(configuration);

        var options = configuration.GetSection(ComponentRegistryMcpOptions.SectionName)
            .Get<ComponentRegistryMcpOptions>() ?? new ComponentRegistryMcpOptions();

        if (!options.Enabled)
        {
            return app;
        }

        app.MapMcp(pattern: options.EndpointPath);

        return app;
    }
}

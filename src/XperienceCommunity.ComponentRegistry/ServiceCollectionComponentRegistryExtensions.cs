using Microsoft.Extensions.DependencyInjection;

using RegisterWidgetAttribute = Kentico.PageBuilder.Web.Mvc.RegisterWidgetAttribute;
using RegisterSectionAttribute = Kentico.PageBuilder.Web.Mvc.RegisterSectionAttribute;
using RegisterPageTemplateAttribute = Kentico.PageBuilder.Web.Mvc.PageTemplates.RegisterPageTemplateAttribute;

using System.Reflection;

namespace XperienceCommunity.ComponentRegistry;

public static class ServiceCollectionComponentRegistryExtensions
{
    public static IServiceCollection AddComponentRegistry(this IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => a.GetCustomAttributes(typeof(CMS.AssemblyDiscoverableAttribute), inherit: false).Length > 0)
            .ToList();

        return AddComponentRegistry(services, assemblies);
    }

    public static IServiceCollection AddComponentRegistry(this IServiceCollection services, IEnumerable<Assembly> assembliesToScan)
    {
        var widgetStore = new ComponentDefinitionStore<PageBuilderWidgetDefinition>();
        var widgetAttrs = assembliesToScan
            .SelectMany(a => a.GetCustomAttributes(typeof(RegisterWidgetAttribute), inherit: false))
            .Cast<RegisterWidgetAttribute>();

        foreach (var attr in widgetAttrs)
        {
            var definition = new PageBuilderWidgetDefinition(
                identifier: attr.Identifier,
                name: attr.Name,
                markedType: attr.MarkedType,
                description: attr.Description,
                iconClass: attr.IconClass,
                allowCache: attr.AllowCache);
            widgetStore.Add(definition);
        }
        var sectionStore = new ComponentDefinitionStore<PageBuilderSectionDefinition>();
        var sectionAttrs = assembliesToScan
            .SelectMany(a => a.GetCustomAttributes(typeof(RegisterSectionAttribute), inherit: false))
            .Cast<RegisterSectionAttribute>();

        foreach (var attr in sectionAttrs)
        {
            var definition = new PageBuilderSectionDefinition(
                identifier: attr.Identifier,
                name: attr.Name,
                markedType: attr.MarkedType,
                description: attr.Description,
                iconClass: attr.IconClass);
            sectionStore.Add(definition);
        }
        var templateStore = new ComponentDefinitionStore<PageBuilderPageTemplateDefinition>();
        var templateAttrs = assembliesToScan
            .SelectMany(a => a.GetCustomAttributes(typeof(RegisterPageTemplateAttribute), inherit: false))
            .Cast<RegisterPageTemplateAttribute>();

        foreach (var attr in templateAttrs)
        {
            var definition = new PageBuilderPageTemplateDefinition(
                identifier: attr.Identifier,
                name: attr.Name,
                markedType: attr.MarkedType,
                description: attr.Description,
                iconClass: attr.IconClass,
                contentTypeNames: attr.ContentTypeNames);
            templateStore.Add(definition);
        }
        _ = services
            .AddSingleton<IComponentDefinitionStore<PageBuilderWidgetDefinition>>(widgetStore)
            .AddSingleton<IComponentDefinitionStore<PageBuilderSectionDefinition>>(sectionStore)
            .AddSingleton<IComponentDefinitionStore<PageBuilderPageTemplateDefinition>>(templateStore)
            .AddScoped<IComponentUsageService, ComponentUsageService>();

        return services;
    }
}

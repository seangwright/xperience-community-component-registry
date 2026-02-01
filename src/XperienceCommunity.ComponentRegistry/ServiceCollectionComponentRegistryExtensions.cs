using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using RegisterEmailSectionAttribute = Kentico.EmailBuilder.Web.Mvc.RegisterEmailSectionAttribute;
using RegisterEmailTemplateAttribute = Kentico.EmailBuilder.Web.Mvc.RegisterEmailTemplateAttribute;
using RegisterEmailWidgetAttribute = Kentico.EmailBuilder.Web.Mvc.RegisterEmailWidgetAttribute;
using RegisterFormComponentAttribute = Kentico.Forms.Web.Mvc.RegisterFormComponentAttribute;
using RegisterFormSectionAttribute = Kentico.Forms.Web.Mvc.RegisterFormSectionAttribute;
using RegisterPageTemplateAttribute = Kentico.PageBuilder.Web.Mvc.PageTemplates.RegisterPageTemplateAttribute;
using RegisterSectionAttribute = Kentico.PageBuilder.Web.Mvc.RegisterSectionAttribute;
using RegisterWidgetAttribute = Kentico.PageBuilder.Web.Mvc.RegisterWidgetAttribute;

namespace XperienceCommunity.ComponentRegistry;

public static class ServiceCollectionComponentRegistryExtensions
{
    /// <summary>
    /// Registers component registry services by scanning assemblies marked with <see cref="CMS.AssemblyDiscoverableAttribute"/> and the entry assembly
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddComponentRegistry(this IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => a.GetCustomAttributes(typeof(CMS.AssemblyDiscoverableAttribute), inherit: false).Length > 0)
            .Concat(Assembly.GetEntryAssembly() is { } entry ? [entry] : [])
            .ToList();

        return AddComponentRegistry(services, assemblies);
    }

    /// <summary>
    /// Registers component registry services by scanning specified assemblies for Page Builder, Email Builder, and Form Builder components
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="assembliesToScan">Assemblies to scan for component registrations</param>
    /// <returns>Service collection for chaining</returns>
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
        var emailWidgetStore = new ComponentDefinitionStore<EmailBuilderWidgetDefinition>();
        var emailWidgetAttrs = assembliesToScan
            .SelectMany(a => a.GetCustomAttributes(typeof(RegisterEmailWidgetAttribute), inherit: false))
            .Cast<RegisterEmailWidgetAttribute>();

        foreach (var attr in emailWidgetAttrs)
        {
            var definition = new EmailBuilderWidgetDefinition(
                identifier: attr.Identifier,
                name: attr.Name,
                markedType: attr.MarkedType,
                description: attr.Description,
                iconClass: attr.IconClass,
                propertiesType: attr.PropertiesType);
            emailWidgetStore.Add(definition);
        }
        var emailSectionStore = new ComponentDefinitionStore<EmailBuilderSectionDefinition>();
        var emailSectionAttrs = assembliesToScan
            .SelectMany(a => a.GetCustomAttributes(typeof(RegisterEmailSectionAttribute), inherit: false))
            .Cast<RegisterEmailSectionAttribute>();

        foreach (var attr in emailSectionAttrs)
        {
            var definition = new EmailBuilderSectionDefinition(
                identifier: attr.Identifier,
                name: attr.Name,
                markedType: attr.MarkedType,
                description: attr.Description,
                iconClass: attr.IconClass);
            emailSectionStore.Add(definition);
        }
        var emailTemplateStore = new ComponentDefinitionStore<EmailBuilderTemplateDefinition>();
        var emailTemplateAttrs = assembliesToScan
            .SelectMany(a => a.GetCustomAttributes(typeof(RegisterEmailTemplateAttribute), inherit: false))
            .Cast<RegisterEmailTemplateAttribute>();

        foreach (var attr in emailTemplateAttrs)
        {
            var definition = new EmailBuilderTemplateDefinition(
                identifier: attr.Identifier,
                name: attr.Name,
                markedType: attr.MarkedType,
                description: attr.Description,
                iconClass: attr.IconClass,
                contentTypeNames: attr.ContentTypeNames);
            emailTemplateStore.Add(definition);
        }
        var formComponentStore = new ComponentDefinitionStore<FormBuilderComponentDefinition>();
        var formComponentAttrs = assembliesToScan
            .SelectMany(a => a.GetCustomAttributes(typeof(RegisterFormComponentAttribute), inherit: false))
            .Cast<RegisterFormComponentAttribute>()
            .Where(attr => attr.IsAvailableInFormBuilderEditor);

        foreach (var attr in formComponentAttrs)
        {
            var definition = new FormBuilderComponentDefinition(
                identifier: attr.Identifier,
                name: attr.Name,
                markedType: attr.MarkedType,
                description: attr.Description,
                iconClass: attr.IconClass);
            formComponentStore.Add(definition);
        }
        var formSectionStore = new ComponentDefinitionStore<FormBuilderSectionDefinition>();
        var formSectionAttrs = assembliesToScan
            .SelectMany(a => a.GetCustomAttributes(typeof(RegisterFormSectionAttribute), inherit: false))
            .Cast<RegisterFormSectionAttribute>();

        foreach (var attr in formSectionAttrs)
        {
            var definition = new FormBuilderSectionDefinition(
                identifier: attr.Identifier,
                name: attr.Name,
                markedType: attr.MarkedType,
                description: attr.Description,
                iconClass: attr.IconClass);
            formSectionStore.Add(definition);
        }
        _ = services
            .AddSingleton<IComponentDefinitionStore<PageBuilderWidgetDefinition>>(widgetStore)
            .AddSingleton<IComponentDefinitionStore<PageBuilderSectionDefinition>>(sectionStore)
            .AddSingleton<IComponentDefinitionStore<PageBuilderPageTemplateDefinition>>(templateStore)
            .AddSingleton<IComponentDefinitionStore<EmailBuilderWidgetDefinition>>(emailWidgetStore)
            .AddSingleton<IComponentDefinitionStore<EmailBuilderSectionDefinition>>(emailSectionStore)
            .AddSingleton<IComponentDefinitionStore<EmailBuilderTemplateDefinition>>(emailTemplateStore)
            .AddSingleton<IComponentDefinitionStore<FormBuilderComponentDefinition>>(formComponentStore)
            .AddSingleton<IComponentDefinitionStore<FormBuilderSectionDefinition>>(formSectionStore)
            .AddScoped<IComponentUsageService, ComponentUsageService>();

        return services;
    }
}

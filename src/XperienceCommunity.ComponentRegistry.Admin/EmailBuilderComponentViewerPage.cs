using CMS.Membership;

using Kentico.Builder.Web.Mvc;
using Kentico.Xperience.Admin.Base;

using XperienceCommunity.ComponentRegistry.Admin;

[assembly: UIPage(
    uiPageType: typeof(EmailBuilderComponentViewerPage),
    parentType: typeof(ComponentRegistryApplicationPage),
    slug: "email-builder",
    name: "Email Builder",
    templateName: "@xperience-community-component-registry/web-admin/EmailBuilderComponentViewer",
    order: 2,
    Icon = Icons.CustomElement)]

namespace XperienceCommunity.ComponentRegistry.Admin;

/// <summary>
/// Page for displaying all registered email builder component definitions.
/// </summary>
[UIPermission(SystemPermissions.VIEW)]
public class EmailBuilderComponentViewerPage(
    IComponentDefinitionStore<EmailBuilderWidgetDefinition> emailWidgetStore,
    IComponentDefinitionStore<EmailBuilderSectionDefinition> emailSectionStore,
    IComponentDefinitionStore<EmailBuilderTemplateDefinition> emailTemplateStore,
    IComponentUsageService componentUsageService,
    IAdminBuildersLocalizationService localizer) : Page<EmailBuilderComponentViewerPageClientProperties>
{
    public override Task<EmailBuilderComponentViewerPageClientProperties> ConfigureTemplateProperties(
        EmailBuilderComponentViewerPageClientProperties properties)
    {
        var widgets = emailWidgetStore.GetAll()
            .Select(w => new EmailComponentDto(
                w.Identifier,
                localizer.LocalizeString(w.Name),
                localizer.LocalizeString(w.Description),
                w.IconClass,
                w.MarkedType?.FullName,
                w.PropertiesType?.FullName))
            .ToList();

        var sections = emailSectionStore.GetAll()
            .Select(s => new EmailComponentDto(
                s.Identifier,
                localizer.LocalizeString(s.Name),
                localizer.LocalizeString(s.Description),
                s.IconClass,
                s.MarkedType?.FullName,
                null))
            .ToList();

        var emailTemplates = emailTemplateStore.GetAll()
            .Select(et => new EmailTemplateDto(
                et.Identifier,
                localizer.LocalizeString(et.Name),
                localizer.LocalizeString(et.Description),
                et.IconClass,
                et.MarkedType?.FullName,
                et.ContentTypeNames))
            .ToList();

        properties.Widgets = widgets;
        properties.Sections = sections;
        properties.EmailTemplates = emailTemplates;

        return Task.FromResult(properties);
    }

    /// <summary>
    /// Retrieves detailed usage information for an email builder widget component.
    /// </summary>
    [PageCommand(CommandName = "GetEmailBuilderWidgetUsage", Permission = SystemPermissions.VIEW)]
    public async Task<ICommandResponse> GetEmailBuilderWidgetUsage(ComponentDetailsParams @params)
    {
        var usage = await componentUsageService.GetEmailBuilderWidgetUsageAsync(@params.ComponentIdentifier);
        return ResponseFrom(usage);
    }

    /// <summary>
    /// Retrieves detailed usage information for an email builder template component.
    /// </summary>
    [PageCommand(CommandName = "GetEmailBuilderTemplateUsage", Permission = SystemPermissions.VIEW)]
    public async Task<ICommandResponse> GetEmailBuilderTemplateUsage(ComponentDetailsParams @params)
    {
        var usage = await componentUsageService.GetEmailBuilderTemplateUsageAsync(@params.ComponentIdentifier);
        return ResponseFrom(usage);
    }
}

/// <summary>
/// Client properties for the email builder component viewer page.
/// </summary>
public class EmailBuilderComponentViewerPageClientProperties : TemplateClientProperties
{
    public IEnumerable<EmailComponentDto> Widgets { get; set; } = [];
    public IEnumerable<EmailComponentDto> Sections { get; set; } = [];
    public IEnumerable<EmailTemplateDto> EmailTemplates { get; set; } = [];
}

/// <summary>
/// Data transfer object for email builder component definitions.
/// </summary>
public record EmailComponentDto(
    string Identifier,
    string Name,
    string? Description,
    string? IconClass,
    string? MarkedTypeName,
    string? PropertiesTypeName);

/// <summary>
/// Data transfer object for email builder template definitions.
/// </summary>
public record EmailTemplateDto(
    string Identifier,
    string Name,
    string? Description,
    string? IconClass,
    string? MarkedTypeName,
    string[] ContentTypeNames);

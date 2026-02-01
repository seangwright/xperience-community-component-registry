using CMS.Membership;

using Kentico.Builder.Web.Mvc;
using Kentico.Xperience.Admin.Base;

using XperienceCommunity.ComponentRegistry.Admin;

[assembly: UIPage(
    uiPageType: typeof(PageBuilderComponentViewerPage),
    parentType: typeof(ComponentRegistryApplicationPage),
    slug: "page-builder",
    name: "Page Builder",
    templateName: "@xperience-community-component-registry/web-admin/PageBuilderComponentViewer",
    order: 1,
    Icon = Icons.CustomElement)]

namespace XperienceCommunity.ComponentRegistry.Admin;

/// <summary>
/// Page for displaying all registered component definitions.
/// </summary>
[UIPermission(SystemPermissions.VIEW)]
public class PageBuilderComponentViewerPage(
    IComponentDefinitionStore<PageBuilderWidgetDefinition> widgetStore,
    IComponentDefinitionStore<PageBuilderSectionDefinition> sectionStore,
    IComponentDefinitionStore<PageBuilderPageTemplateDefinition> pageTemplateStore,
    IComponentUsageService componentUsageService,
    IAdminBuildersLocalizationService localizer) : Page<PageBuilderComponentViewerPageClientProperties>
{
    public override Task<PageBuilderComponentViewerPageClientProperties> ConfigureTemplateProperties(
        PageBuilderComponentViewerPageClientProperties properties)
    {
        var widgets = widgetStore.GetAll()
            .Select(w => new ComponentDto(
                w.Identifier,
                localizer.LocalizeString(w.Name),
                localizer.LocalizeString(w.Description),
                w.IconClass,
                w.MarkedType?.FullName))
            .ToList();

        var sections = sectionStore.GetAll()
            .Select(s => new ComponentDto(
                s.Identifier,
                localizer.LocalizeString(s.Name),
                localizer.LocalizeString(s.Description),
                s.IconClass,
                s.MarkedType?.FullName))
            .ToList();

        var pageTemplates = pageTemplateStore.GetAll()
            .Select(pt => new PageTemplateDto(
                pt.Identifier,
                localizer.LocalizeString(pt.Name),
                localizer.LocalizeString(pt.Description),
                pt.IconClass,
                pt.MarkedType?.FullName,
                pt.ContentTypeNames))
            .ToList();

        properties.Widgets = widgets;
        properties.Sections = sections;
        properties.PageTemplates = pageTemplates;

        return Task.FromResult(properties);
    }

    /// <summary>
    /// Retrieves detailed usage information for a page builder page template component.
    /// </summary>
    [PageCommand(CommandName = "GetPageBuilderPageTemplateUsage", Permission = SystemPermissions.VIEW)]
    public async Task<ICommandResponse> GetPageBuilderPageTemplateUsage(ComponentDetailsParams @params)
    {
        var usage = await componentUsageService.GetPageBuilderPageTemplateUsageAsync(@params.ComponentIdentifier);
        return ResponseFrom(usage);
    }

    /// <summary>
    /// Retrieves detailed usage information for a page builder widget component.
    /// </summary>
    [PageCommand(CommandName = "GetPageBuilderWidgetUsage", Permission = SystemPermissions.VIEW)]
    public async Task<ICommandResponse> GetPageBuilderWidgetUsage(ComponentDetailsParams @params)
    {
        var usage = await componentUsageService.GetPageBuilderWidgetUsageAsync(@params.ComponentIdentifier);
        return ResponseFrom(usage);
    }
}

/// <summary>
/// Client properties for the component viewer page.
/// </summary>
public class PageBuilderComponentViewerPageClientProperties : TemplateClientProperties
{
    public IEnumerable<ComponentDto> Widgets { get; set; } = [];
    public IEnumerable<ComponentDto> Sections { get; set; } = [];
    public IEnumerable<PageTemplateDto> PageTemplates { get; set; } = [];
}

/// <summary>
/// Data transfer object for component definitions.
/// </summary>
public record ComponentDto(
    string Identifier,
    string Name,
    string? Description,
    string? IconClass,
    string? MarkedTypeName);

/// <summary>
/// Data transfer object for page template definitions.
/// </summary>
public record PageTemplateDto(
    string Identifier,
    string Name,
    string? Description,
    string? IconClass,
    string? MarkedTypeName,
    string[] ContentTypeNames);

/// <summary>
/// Parameters for component details page command.
/// </summary>
public record ComponentDetailsParams(string ComponentIdentifier);

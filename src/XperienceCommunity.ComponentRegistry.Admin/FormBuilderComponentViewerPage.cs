using CMS.Membership;

using Kentico.Builder.Web.Mvc;
using Kentico.Xperience.Admin.Base;

using XperienceCommunity.ComponentRegistry.Admin;

[assembly: UIPage(
    uiPageType: typeof(FormBuilderComponentViewerPage),
    parentType: typeof(ComponentRegistryApplicationPage),
    slug: "form-builder",
    name: "Form Builder",
    templateName: "@xperience-community-component-registry/web-admin/FormBuilderComponentViewer",
    order: 3,
    Icon = Icons.CustomElement)]

namespace XperienceCommunity.ComponentRegistry.Admin;

/// <summary>
/// Page for displaying all registered form builder component definitions.
/// </summary>
[UIPermission(SystemPermissions.VIEW)]
public class FormBuilderComponentViewerPage(
    IComponentDefinitionStore<FormBuilderComponentDefinition> formComponentStore,
    IComponentDefinitionStore<FormBuilderSectionDefinition> formSectionStore,
    IAdminBuildersLocalizationService localizer,
    IComponentUsageService componentUsageService) : Page<FormBuilderComponentViewerPageClientProperties>
{
    public override Task<FormBuilderComponentViewerPageClientProperties> ConfigureTemplateProperties(
        FormBuilderComponentViewerPageClientProperties properties)
    {
        var formComponents = formComponentStore.GetAll()
            .Select(c => new FormComponentDto(
                c.Identifier,
                localizer.LocalizeString(c.Name),
                localizer.LocalizeString(c.Description),
                c.IconClass,
                c.MarkedType?.FullName))
            .ToList();

        var formSections = formSectionStore.GetAll()
            .Select(s => new FormSectionDto(
                s.Identifier,
                localizer.LocalizeString(s.Name),
                localizer.LocalizeString(s.Description),
                s.IconClass,
                s.MarkedType?.FullName))
            .ToList();

        properties.FormComponents = formComponents;
        properties.FormSections = formSections;

        return Task.FromResult(properties);
    }

    [PageCommand]
    public async Task<ICommandResponse> GetFormBuilderComponentUsageAsync(
        ComponentDetailsParams @params)
    {
        if (@params?.ComponentIdentifier is null)
        {
            return ResponseFrom("Component identifier is required");
        }

        var usage = await componentUsageService.GetFormBuilderComponentUsageAsync(
            @params.ComponentIdentifier);

        return ResponseFrom(usage);
    }

    [PageCommand]
    public async Task<ICommandResponse> GetFormBuilderSectionUsageAsync(
        ComponentDetailsParams @params)
    {
        if (@params?.ComponentIdentifier is null)
        {
            return ResponseFrom("Component identifier is required");
        }

        var usage = await componentUsageService.GetFormBuilderSectionUsageAsync(
            @params.ComponentIdentifier);

        return ResponseFrom(usage);
    }
}

/// <summary>
/// Client properties for the form builder component viewer page.
/// </summary>
public class FormBuilderComponentViewerPageClientProperties : TemplateClientProperties
{
    public IEnumerable<FormComponentDto> FormComponents { get; set; } = [];
    public IEnumerable<FormSectionDto> FormSections { get; set; } = [];
}

/// <summary>
/// Data transfer object for form builder component definitions.
/// </summary>
public record FormComponentDto(
    string Identifier,
    string Name,
    string? Description,
    string? IconClass,
    string? MarkedTypeName);

/// <summary>
/// Data transfer object for form builder section definitions.
/// </summary>
public record FormSectionDto(
    string Identifier,
    string Name,
    string? Description,
    string? IconClass,
    string? MarkedTypeName);

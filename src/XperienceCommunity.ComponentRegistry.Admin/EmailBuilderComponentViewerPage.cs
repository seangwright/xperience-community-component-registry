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
[UIEvaluatePermission(ComponentRegistryPermissions.VIEW_EMAIL_BUILDER)]
public class EmailBuilderComponentViewerPage(
    IComponentRegistryReadService componentRegistryReadService,
    IComponentUsageService componentUsageService,
    IUIPermissionEvaluator permissionEvaluator) : Page<EmailBuilderComponentViewerPageClientProperties>
{
    public override async Task<EmailBuilderComponentViewerPageClientProperties> ConfigureTemplateProperties(
        EmailBuilderComponentViewerPageClientProperties properties)
    {
        var model = await componentRegistryReadService.GetEmailBuilderRegistryAsync();

        properties.Widgets = model.Widgets;
        properties.Sections = model.Sections;
        properties.EmailTemplates = model.EmailTemplates;

        // Evaluate permissions and propagate to client
        var canViewEmailBuilderUsages = await permissionEvaluator.Evaluate(
            ComponentRegistryPermissions.VIEW_EMAIL_BUILDER_USAGES);
        properties.CanViewEmailBuilderUsages = canViewEmailBuilderUsages.Succeeded;

        return properties;
    }

    /// <summary>
    /// Retrieves detailed usage information for an email builder widget component.
    /// </summary>
    [PageCommand(CommandName = "GetEmailBuilderWidgetUsage", Permission = ComponentRegistryPermissions.VIEW_EMAIL_BUILDER_USAGES)]
    public async Task<ICommandResponse> GetEmailBuilderWidgetUsage(ComponentDetailsParams @params)
    {
        var usage = await componentUsageService.GetEmailBuilderWidgetUsageAsync(@params.ComponentIdentifier);
        return ResponseFrom(usage);
    }

    /// <summary>
    /// Retrieves detailed usage information for an email builder template component.
    /// </summary>
    [PageCommand(CommandName = "GetEmailBuilderTemplateUsage", Permission = ComponentRegistryPermissions.VIEW_EMAIL_BUILDER_USAGES)]
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
    public bool CanViewEmailBuilderUsages { get; set; }
}

using System.ComponentModel;

using CMS.Websites;

using Kentico.Content.Web.Mvc.Internal;

using ModelContextProtocol.Server;

namespace XperienceCommunity.ComponentRegistry;

/// <summary>
/// MCP tools exposing component registry definitions and usage details.
/// </summary>
[McpServerToolType]
public class ComponentRegistryMcpTools(
    IComponentRegistryReadService readService,
    IComponentUsageService componentUsageService,
    IWebPageUrlRetriever webPageUrlRetriever,
    IShareablePreviewLinkGenerator shareablePreviewLinkGenerator)
{
    /// <summary>
    /// Lists registered component definitions for a builder and component type.
    /// </summary>
    [McpServerTool(Name = "component_registry_list_definitions")]
    [Description("List registered component definitions for page/email/form builders. builder: page|email|form. componentType: widget|section|template|page-template|component|all")]
    public async Task<ComponentDefinitionListResponse> ListComponentDefinitions(
        string builder,
        string componentType = "all",
        CancellationToken cancellationToken = default)
    {
        string normalizedBuilder = NormalizeBuilder(builder);
        string normalizedType = NormalizeComponentType(componentType);

        var items = normalizedBuilder switch
        {
            "page" => await ListPageBuilderDefinitions(normalizedType, cancellationToken),
            "email" => await ListEmailBuilderDefinitions(normalizedType, cancellationToken),
            "form" => await ListFormBuilderDefinitions(normalizedType, cancellationToken),
            _ => throw new ArgumentException($"Unknown builder '{builder}'. Use page|email|form.", nameof(builder))
        };

        return new ComponentDefinitionListResponse(normalizedBuilder, normalizedType, items);
    }

    /// <summary>
    /// Gets usage detail for a specific registered component identifier.
    /// </summary>
    [McpServerTool(Name = "component_registry_get_usage")]
    [Description("Get usage detail for one component by builder/type/identifier. builder: page|email|form. type: widget|template|page-template|component|section")]
    public async Task<object> GetComponentUsage(
        string builder,
        string componentType,
        string componentIdentifier,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string normalizedBuilder = NormalizeBuilder(builder);
        string normalizedType = NormalizeComponentType(componentType);

        if (string.IsNullOrWhiteSpace(componentIdentifier))
        {
            throw new ArgumentException("Component identifier is required.", nameof(componentIdentifier));
        }

        return (normalizedBuilder, normalizedType) switch
        {
            ("page", "widget") => await componentUsageService.GetPageBuilderWidgetUsageAsync(componentIdentifier),
            ("page", "page-template") => await componentUsageService.GetPageBuilderPageTemplateUsageAsync(componentIdentifier),
            ("email", "widget") => await componentUsageService.GetEmailBuilderWidgetUsageAsync(componentIdentifier),
            ("email", "template") => await componentUsageService.GetEmailBuilderTemplateUsageAsync(componentIdentifier),
            ("form", "component") => await componentUsageService.GetFormBuilderComponentUsageAsync(componentIdentifier),
            ("form", "section") => await componentUsageService.GetFormBuilderSectionUsageAsync(componentIdentifier),
            _ => throw new ArgumentException($"Unsupported usage query for builder '{builder}' and componentType '{componentType}'.")
        };
    }

    /// <summary>
    /// Gets page builder usage details for many widget/template identifiers.
    /// </summary>
    [McpServerTool(Name = "component_registry_get_page_batch_usage")]
    [Description("Get page builder batch usage. componentType: widget|page-template. identifiers: list of component IDs.")]
    public async Task<List<ComponentUsageDetailDto>> GetPageBuilderBatchUsage(
        List<string> componentIdentifiers,
        string componentType,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (componentIdentifiers is null || componentIdentifiers.Count == 0)
        {
            throw new ArgumentException("At least one component identifier is required.", nameof(componentIdentifiers));
        }

        string normalizedType = NormalizeComponentType(componentType);
        string usageType = normalizedType switch
        {
            "widget" => "Widget",
            "page-template" => "PageTemplate",
            _ => throw new ArgumentException("componentType must be widget or page-template.", nameof(componentType))
        };

        return await componentUsageService.GetBatchUsageAsync(componentIdentifiers, usageType);
    }

    /// <summary>
    /// Gets the absolute URL for a web page by its ID and language.
    /// For published pages, returns the live page URL via IWebPageUrlRetriever.
    /// For unpublished pages with draft changes, returns a shareable preview URL via IShareablePreviewLinkGenerator.
    /// Enables AI agents to visit and validate component rendering on both published and draft pages.
    /// </summary>
    [McpServerTool(Name = "component_registry_get_web_page_url")]
    [Description("Get absolute URL for a web page. If isPublished=true, returns the live URL of the published page. If isPublished=false, returns a shareable preview link for viewing unpublished changes. Use the IsPublished value from GetComponentUsage PageVariantDto to determine which URL type to request. Returns URL that agents can use to visit and validate component rendering.")]
    public async Task<WebPageUrlResponse> GetWebPageUrl(
        int webPageItemId,
        string languageName,
        bool isPublished = true,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(languageName))
        {
            throw new ArgumentException("Language name is required.", nameof(languageName));
        }

        if (webPageItemId <= 0)
        {
            throw new ArgumentException("Web page item ID must be greater than zero.", nameof(webPageItemId));
        }

        try
        {
            if (!isPublished)
            {
                // For unpublished pages with draft changes, use shareable preview URL
                try
                {
                    string shareableUrl = await shareablePreviewLinkGenerator.Generate(webPageItemId, languageName, cancellationToken);
                    if (!string.IsNullOrWhiteSpace(shareableUrl))
                    {
                        return new WebPageUrlResponse(
                            WebPageItemId: webPageItemId,
                            LanguageName: languageName,
                            IsPublished: false,
                            UrlType: "ShareablePreview",
                            Url: shareableUrl,
                            Success: true,
                            ErrorMessage: null);
                    }

                    return new WebPageUrlResponse(
                        WebPageItemId: webPageItemId,
                        LanguageName: languageName,
                        IsPublished: false,
                        UrlType: null,
                        Url: null,
                        Success: false,
                        ErrorMessage: "Shareable preview URL could not be generated for the requested page.");
                }
                catch (Exception shareableEx)
                {
                    // If shareable preview generation fails, return error
                    // This can happen if the page is not eligible for shareable preview (e.g., custom auth scheme)
                    return new WebPageUrlResponse(
                        WebPageItemId: webPageItemId,
                        LanguageName: languageName,
                        IsPublished: false,
                        UrlType: null,
                        Url: null,
                        Success: false,
                        ErrorMessage: $"Failed to generate shareable preview URL: {shareableEx.Message}");
                }
            }

            // For published pages, use standard URL retrieval
            var webPageUrl = await webPageUrlRetriever.Retrieve(
                webPageItemId,
                languageName,
                forPreview: false,
                cancellationToken);

            return new WebPageUrlResponse(
                WebPageItemId: webPageItemId,
                LanguageName: languageName,
                IsPublished: true,
                UrlType: "Published",
                Url: webPageUrl.AbsoluteUrl,
                Success: true,
                ErrorMessage: null);
        }
        catch (Exception ex)
        {
            return new WebPageUrlResponse(
                WebPageItemId: webPageItemId,
                LanguageName: languageName,
                IsPublished: isPublished,
                UrlType: null,
                Url: null,
                Success: false,
                ErrorMessage: $"Failed to retrieve URL for web page {webPageItemId}: {ex.Message}");
        }
    }

    private async Task<List<ComponentDefinitionItem>> ListPageBuilderDefinitions(string componentType, CancellationToken cancellationToken)
    {
        var model = await readService.GetPageBuilderRegistryAsync(cancellationToken);
        List<ComponentDefinitionItem> items = [];

        if (componentType is "all" or "widget")
        {
            items.AddRange(model.Widgets.Select(w => new ComponentDefinitionItem(
                Builder: "page",
                ComponentType: "widget",
                Identifier: w.Identifier,
                Name: w.Name,
                Description: w.Description,
                IconClass: w.IconClass,
                MarkedTypeName: w.MarkedTypeName,
                PropertiesTypeName: null,
                ContentTypeNames: null)));
        }

        if (componentType is "all" or "section")
        {
            items.AddRange(model.Sections.Select(s => new ComponentDefinitionItem(
                Builder: "page",
                ComponentType: "section",
                Identifier: s.Identifier,
                Name: s.Name,
                Description: s.Description,
                IconClass: s.IconClass,
                MarkedTypeName: s.MarkedTypeName,
                PropertiesTypeName: null,
                ContentTypeNames: null)));
        }

        if (componentType is "all" or "page-template")
        {
            items.AddRange(model.PageTemplates.Select(t => new ComponentDefinitionItem(
                Builder: "page",
                ComponentType: "page-template",
                Identifier: t.Identifier,
                Name: t.Name,
                Description: t.Description,
                IconClass: t.IconClass,
                MarkedTypeName: t.MarkedTypeName,
                PropertiesTypeName: null,
                ContentTypeNames: t.ContentTypeNames)));
        }

        return items;
    }

    private async Task<List<ComponentDefinitionItem>> ListEmailBuilderDefinitions(string componentType, CancellationToken cancellationToken)
    {
        var model = await readService.GetEmailBuilderRegistryAsync(cancellationToken);
        List<ComponentDefinitionItem> items = [];

        if (componentType is "all" or "widget")
        {
            items.AddRange(model.Widgets.Select(w => new ComponentDefinitionItem(
                Builder: "email",
                ComponentType: "widget",
                Identifier: w.Identifier,
                Name: w.Name,
                Description: w.Description,
                IconClass: w.IconClass,
                MarkedTypeName: w.MarkedTypeName,
                PropertiesTypeName: w.PropertiesTypeName,
                ContentTypeNames: null)));
        }

        if (componentType is "all" or "section")
        {
            items.AddRange(model.Sections.Select(s => new ComponentDefinitionItem(
                Builder: "email",
                ComponentType: "section",
                Identifier: s.Identifier,
                Name: s.Name,
                Description: s.Description,
                IconClass: s.IconClass,
                MarkedTypeName: s.MarkedTypeName,
                PropertiesTypeName: s.PropertiesTypeName,
                ContentTypeNames: null)));
        }

        if (componentType is "all" or "template")
        {
            items.AddRange(model.EmailTemplates.Select(t => new ComponentDefinitionItem(
                Builder: "email",
                ComponentType: "template",
                Identifier: t.Identifier,
                Name: t.Name,
                Description: t.Description,
                IconClass: t.IconClass,
                MarkedTypeName: t.MarkedTypeName,
                PropertiesTypeName: null,
                ContentTypeNames: t.ContentTypeNames)));
        }

        return items;
    }

    private async Task<List<ComponentDefinitionItem>> ListFormBuilderDefinitions(string componentType, CancellationToken cancellationToken)
    {
        var model = await readService.GetFormBuilderRegistryAsync(cancellationToken);
        List<ComponentDefinitionItem> items = [];

        if (componentType is "all" or "component")
        {
            items.AddRange(model.FormComponents.Select(c => new ComponentDefinitionItem(
                Builder: "form",
                ComponentType: "component",
                Identifier: c.Identifier,
                Name: c.Name,
                Description: c.Description,
                IconClass: c.IconClass,
                MarkedTypeName: c.MarkedTypeName,
                PropertiesTypeName: null,
                ContentTypeNames: null)));
        }

        if (componentType is "all" or "section")
        {
            items.AddRange(model.FormSections.Select(s => new ComponentDefinitionItem(
                Builder: "form",
                ComponentType: "section",
                Identifier: s.Identifier,
                Name: s.Name,
                Description: s.Description,
                IconClass: s.IconClass,
                MarkedTypeName: s.MarkedTypeName,
                PropertiesTypeName: null,
                ContentTypeNames: null)));
        }

        return items;
    }

    private static string NormalizeBuilder(string builder) => builder?.Trim().ToLowerInvariant() switch
    {
        "page" or "pagebuilder" or "page-builder" => "page",
        "email" or "emailbuilder" or "email-builder" => "email",
        "form" or "formbuilder" or "form-builder" => "form",
        _ => builder?.Trim().ToLowerInvariant() ?? string.Empty
    };

    private static string NormalizeComponentType(string componentType) => componentType?.Trim().ToLowerInvariant() switch
    {
        "all" => "all",
        "widget" or "widgets" => "widget",
        "section" or "sections" => "section",
        "template" or "templates" => "template",
        "pagetemplate" or "page-template" or "page_templates" => "page-template",
        "component" or "components" => "component",
        _ => componentType?.Trim().ToLowerInvariant() ?? string.Empty
    };
}

/// <summary>
/// Flattened component definition item returned by MCP list tools.
/// </summary>
public record ComponentDefinitionItem(
    string Builder,
    string ComponentType,
    string Identifier,
    string Name,
    string? Description,
    string? IconClass,
    string? MarkedTypeName,
    string? PropertiesTypeName,
    IReadOnlyList<string>? ContentTypeNames);

/// <summary>
/// Response payload for MCP definition list tool.
/// </summary>
public record ComponentDefinitionListResponse(
    string Builder,
    string ComponentType,
    IReadOnlyList<ComponentDefinitionItem> Items);

/// <summary>
/// Response payload for web page URL retrieval tool.
/// Contains absolute URL for a web page (published or shareable preview) or an error message if retrieval failed.
/// </summary>
public record WebPageUrlResponse(
    int WebPageItemId,
    string LanguageName,
    bool IsPublished,
    string? UrlType,
    string? Url,
    bool Success,
    string? ErrorMessage);

/// <summary>
/// Possible URL types returned by the web page URL tool.
/// </summary>
public static class WebPageUrlType
{
    public const string Published = "Published";
    public const string Preview = "Preview";
    public const string ShareablePreview = "ShareablePreview";
}

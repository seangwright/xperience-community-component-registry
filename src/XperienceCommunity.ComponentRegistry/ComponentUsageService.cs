using System.Data;

using CMS.ContentEngine;
using CMS.DataEngine;

using Microsoft.Extensions.Logging;

namespace XperienceCommunity.ComponentRegistry;

/// <summary>
/// Service for retrieving component usage information across the site.
/// </summary>
public interface IComponentUsageService
{
    /// <summary>
    /// Gets detailed usage information for a specific page template.
    /// </summary>
    /// <param name="templateIdentifier">The unique identifier of the page template.</param>
    /// <returns>Component usage details including all pages and language variants.</returns>
    public Task<ComponentUsageDetailDto> GetPageTemplateUsageAsync(string templateIdentifier);

    /// <summary>
    /// Gets detailed usage information for a specific widget.
    /// </summary>
    /// <param name="widgetIdentifier">The unique identifier of the widget.</param>
    /// <returns>Component usage details including all pages and language variants.</returns>
    public Task<ComponentUsageDetailDto> GetWidgetUsageAsync(string widgetIdentifier);

    /// <summary>
    /// Gets usage information for multiple components in a batch operation.
    /// </summary>
    /// <param name="identifiers">List of component identifiers to query.</param>
    /// <param name="componentType">The type of components being queried ("PageTemplate" or "Widget").</param>
    /// <returns>List of component usage details for each identifier.</returns>
    public Task<List<ComponentUsageDetailDto>> GetBatchUsageAsync(List<string> identifiers, string componentType);
}


/// <summary>
/// Service for retrieving component usage information from the database.
/// Uses CMS.DataEngine.ConnectionHelper to query CMS_WebPageItem and CMS_ContentItemCommonData.
/// </summary>
public class ComponentUsageService(ILogger<ComponentUsageService> logger) : IComponentUsageService
{
    private readonly ILogger<ComponentUsageService> logger = logger;

    /// <inheritdoc/>
    public Task<ComponentUsageDetailDto> GetPageTemplateUsageAsync(string templateIdentifier) => GetComponentUsage(
            templateIdentifier,
            "PageTemplate",
            "ContentItemCommonDataVisualBuilderTemplateConfiguration");

    /// <inheritdoc/>
    public Task<ComponentUsageDetailDto> GetWidgetUsageAsync(string widgetIdentifier) => GetComponentUsage(
            widgetIdentifier,
            "Widget",
            "ContentItemCommonDataVisualBuilderWidgets");

    /// <inheritdoc/>
    public async Task<List<ComponentUsageDetailDto>> GetBatchUsageAsync(
        List<string> identifiers,
        string componentType)
    {
        var results = new List<ComponentUsageDetailDto>();

        foreach (string identifier in identifiers)
        {
            var usage = componentType.Equals("PageTemplate", StringComparison.InvariantCultureIgnoreCase)
                ? await GetPageTemplateUsageAsync(identifier)
                : await GetWidgetUsageAsync(identifier);

            results.Add(usage);
        }

        return results;
    }

    /// <summary>
    /// Retrieves component usage by querying the database.
    /// </summary>
    private async Task<ComponentUsageDetailDto> GetComponentUsage(
        string componentIdentifier,
        string componentType,
        string configColumnName,
        CancellationToken cancellationToken = default)
    {
        var result = new ComponentUsageDetailDto
        {
            ComponentIdentifier = componentIdentifier,
            ComponentType = componentType,
            Pages = []
        };

        try
        {
            string query = @"
                SELECT
                    WPI.WebPageItemID,
                    WPI.WebPageItemName,
                    WPI.WebPageItemTreePath,
                    C.ChannelDisplayName,
                    CI.ContentItemID,
                    CL.ContentLanguageID,
                    CL.ContentLanguageName,
                    CICD.ContentItemCommonDataID,
                    CICD.ContentItemCommonDataVersionStatus,
                    CICD.ContentItemCommonDataLastPublishedWhen,
                    CICD.[{configColumnName}]
                FROM CMS_WebPageItem WPI
                INNER JOIN CMS_ContentItem CI ON WPI.WebPageItemContentItemID = CI.ContentItemID
                INNER JOIN CMS_ContentItemCommonData CICD ON CI.ContentItemID = CICD.ContentItemCommonDataContentItemID
                INNER JOIN CMS_ContentLanguage CL ON CICD.ContentItemCommonDataContentLanguageID = CL.ContentLanguageID
                INNER JOIN CMS_Channel C ON CI.ContentItemChannelID = C.ChannelID
                WHERE CICD.[{configColumnName}] LIKE @componentId ESCAPE '\'
                ORDER BY WPI.WebPageItemName, CL.ContentLanguageName";

            query = query.Replace("{configColumnName}", configColumnName);

            var dataParameters = new QueryDataParameters
            {
                { "@componentId", $"%{EscapeForLike(componentIdentifier)}%" }
            };
            var parameters = new QueryParameters(query, dataParameters, QueryTypeEnum.SQLQuery);

            using (var connection = ConnectionHelper.GetConnection())
            {
                var reader = await connection.ExecuteReaderAsync(parameters, CommandBehavior.Default, cancellationToken);
                if (reader is null)
                {
                    return result;
                }

                var pagesByItem = new Dictionary<int, PageUsageDto>();
                DateTime? lastModified = null;

                while (await reader.ReadAsync(cancellationToken))
                {
                    int webPageItemId = reader.GetInt32(reader.GetOrdinal("WebPageItemID"));
                    string pageName = reader.GetString(reader.GetOrdinal("WebPageItemName"));
                    string pagePath = reader.GetString(reader.GetOrdinal("WebPageItemTreePath"));
                    string channelDisplayName = reader.GetString(reader.GetOrdinal("ChannelDisplayName"));
                    int contentItemId = reader.GetInt32(reader.GetOrdinal("ContentItemID"));
                    string languageName = reader.GetString(reader.GetOrdinal("ContentLanguageName"));
                    int commonDataId = reader.GetInt32(reader.GetOrdinal("ContentItemCommonDataID"));
                    int versionStatusOrdinal = reader.GetOrdinal("ContentItemCommonDataVersionStatus");
                    int versionStatus = !await reader.IsDBNullAsync(versionStatusOrdinal, cancellationToken)
                        ? reader.GetInt32(versionStatusOrdinal)
                        : 0;
                    int modifiedWhenOrdinal = reader.GetOrdinal("ContentItemCommonDataLastPublishedWhen");
                    var modifiedWhen = !await reader.IsDBNullAsync(modifiedWhenOrdinal, cancellationToken)
                        ? reader.GetDateTime(modifiedWhenOrdinal)
                        : DateTime.UtcNow;
                    int configColumnOrdinal = reader.GetOrdinal(configColumnName);
                    string configJson = !await reader.IsDBNullAsync(configColumnOrdinal, cancellationToken)
                        ? reader.GetString(configColumnOrdinal)
                        : string.Empty;

                    if (lastModified == null || modifiedWhen > lastModified)
                    {
                        lastModified = modifiedWhen;
                    }

                    if (!pagesByItem.TryGetValue(webPageItemId, out var pageUsage))
                    {
                        pageUsage = new PageUsageDto
                        {
                            WebPageItemId = webPageItemId,
                            ContentItemId = contentItemId,
                            PageName = pageName,
                            PagePath = pagePath,
                            ChannelDisplayName = channelDisplayName,
                            CreatedAt = DateTime.UtcNow,
                            Variants = []
                        };
                        pagesByItem[webPageItemId] = pageUsage;
                    }

                    pageUsage.Variants.Add(new PageVariantDto
                    {
                        ContentItemCommonDataId = commonDataId,
                        LanguageName = languageName,
                        LastModified = modifiedWhen,
                        ConfigurationJson = configJson,
                        ConfigurationType = componentType,
                        IsPublished = versionStatus == (int)VersionStatus.Published
                    });
                }

                result.Pages = pagesByItem.Values.OrderBy(p => p.PageName).ToList();
                result.TotalPagesUsing = result.Pages.Count;
                result.TotalVariants = result.Pages.SelectMany(p => p.Variants).Count();
                result.LastModified = lastModified;
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving component usage for {ComponentIdentifier}", componentIdentifier);
            return result;
        }
    }

    /// <summary>
    /// Escapes special characters for SQL LIKE clause.
    /// </summary>
    private static string EscapeForLike(string value) => value
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");
}

/// <summary>
/// Represents detailed usage information for a specific component across the system.
/// </summary>
public class ComponentUsageDetailDto
{
    /// <summary>
    /// The unique identifier of the component.
    /// </summary>
    public string ComponentIdentifier { get; set; } = string.Empty;

    /// <summary>
    /// The type of component ("PageTemplate" or "Widget").
    /// </summary>
    public string ComponentType { get; set; } = string.Empty;

    /// <summary>
    /// Total number of pages using this component.
    /// </summary>
    public int TotalPagesUsing { get; set; }

    /// <summary>
    /// Total number of language variants across all pages using this component.
    /// </summary>
    public int TotalVariants { get; set; }

    /// <summary>
    /// The date when this component was last used or modified.
    /// </summary>
    public DateTime? LastModified { get; set; }

    /// <summary>
    /// List of pages using this component with their language variants.
    /// </summary>
    public List<PageUsageDto> Pages { get; set; } = [];
}

/// <summary>
/// Represents a page that uses a specific component.
/// </summary>
public class PageUsageDto
{
    /// <summary>
    /// The ID of the web page item.
    /// </summary>
    public int WebPageItemId { get; set; }

    /// <summary>
    /// The ID of the content item.
    /// </summary>
    public int ContentItemId { get; set; }

    /// <summary>
    /// The name of the page.
    /// </summary>
    public string PageName { get; set; } = string.Empty;

    /// <summary>
    /// The URL path of the page.
    /// </summary>
    public string PagePath { get; set; } = string.Empty;

    /// <summary>
    /// The display name of the channel this page belongs to.
    /// </summary>
    public string ChannelDisplayName { get; set; } = string.Empty;

    /// <summary>
    /// The date when the page was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The date when the page was last modified.
    /// </summary>
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// The language variants of this page containing component configuration.
    /// </summary>
    public List<PageVariantDto> Variants { get; set; } = [];
}

/// <summary>
/// Represents a language variant of a page's component configuration.
/// </summary>
public class PageVariantDto
{
    /// <summary>
    /// The ID of the content item common data.
    /// </summary>
    public int ContentItemCommonDataId { get; set; }

    /// <summary>
    /// The display name of the language (e.g., "English (United States)").
    /// </summary>
    public string LanguageName { get; set; } = string.Empty;

    /// <summary>
    /// The date when this variant was last modified.
    /// </summary>
    public DateTime? LastModified { get; set; }

    /// <summary>
    /// The JSON configuration for the component in this language variant.
    /// For page templates: ContentItemCommonDataVisualBuilderTemplateConfiguration
    /// For widgets: ContentItemCommonDataVisualBuilderWidgets
    /// </summary>
    public string ConfigurationJson { get; set; } = string.Empty;

    /// <summary>
    /// The type of configuration ("PageTemplate" or "Widget").
    /// </summary>
    public string ConfigurationType { get; set; } = string.Empty;

    /// <summary>
    /// Whether this language variant is published.
    /// </summary>
    public bool IsPublished { get; set; }
}

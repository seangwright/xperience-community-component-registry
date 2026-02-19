using CMS.Websites;

using Kentico.Content.Web.Mvc.Internal;

namespace XperienceCommunity.ComponentRegistry.Tests;

public class ComponentRegistryMcpToolsTests
{
    [Test]
    public async Task ListComponentDefinitions_ReturnsExpectedPageItems()
    {
        var tools = new ComponentRegistryMcpTools(
            new StubReadService(
                new PageBuilderRegistryReadModel(
                    Widgets: [new ComponentDto("w1", "Widget 1", null, null, null)],
                    Sections: [],
                    PageTemplates: [new PageTemplateDto("pt1", "Template 1", null, null, null, ["Acme.Page"])]),
                new EmailBuilderRegistryReadModel([], [], []),
                new FormBuilderRegistryReadModel([], [])),
            new StubUsageService(),
            new StubWebPageUrlRetriever(),
            new StubShareablePreviewLinkGenerator());

        var response = await tools.ListComponentDefinitions("page", "all");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Builder, Is.EqualTo("page"));
            Assert.That(response.Items, Has.Count.EqualTo(2));
            Assert.That(response.Items.Any(i => i.ComponentType == "widget" && i.Identifier == "w1"), Is.True);
            Assert.That(response.Items.Any(i => i.ComponentType == "page-template" && i.Identifier == "pt1"), Is.True);
        }
    }

    [Test]
    public async Task GetComponentUsage_RoutesToExpectedUsageMethod()
    {
        var usage = new StubUsageService();
        var tools = new ComponentRegistryMcpTools(
            new StubReadService(
                new PageBuilderRegistryReadModel([], [], []),
                new EmailBuilderRegistryReadModel([], [], []),
                new FormBuilderRegistryReadModel([], [])),
            usage,
            new StubWebPageUrlRetriever(),
            new StubShareablePreviewLinkGenerator());

        _ = await tools.GetComponentUsage("form", "section", "form.section");

        Assert.That(usage.LastCall, Is.EqualTo("form-section:form.section"));
    }

    [Test]
    public void GetPageBuilderBatchUsage_RejectsUnsupportedType()
    {
        var tools = new ComponentRegistryMcpTools(
            new StubReadService(
                new PageBuilderRegistryReadModel([], [], []),
                new EmailBuilderRegistryReadModel([], [], []),
                new FormBuilderRegistryReadModel([], [])),
            new StubUsageService(),
            new StubWebPageUrlRetriever(),
            new StubShareablePreviewLinkGenerator());

        Assert.ThrowsAsync<ArgumentException>(async () =>
            await tools.GetPageBuilderBatchUsage(["x"], "section"));
    }
}

internal sealed class StubReadService(
    PageBuilderRegistryReadModel page,
    EmailBuilderRegistryReadModel email,
    FormBuilderRegistryReadModel form) : IComponentRegistryReadService
{
    public Task<PageBuilderRegistryReadModel> GetPageBuilderRegistryAsync(CancellationToken cancellationToken = default) => Task.FromResult(page);

    public Task<EmailBuilderRegistryReadModel> GetEmailBuilderRegistryAsync(CancellationToken cancellationToken = default) => Task.FromResult(email);

    public Task<FormBuilderRegistryReadModel> GetFormBuilderRegistryAsync(CancellationToken cancellationToken = default) => Task.FromResult(form);
}

internal sealed class StubUsageService : IComponentUsageService
{
    public string? LastCall { get; private set; }

    public Task<ComponentUsageDetailDto> GetPageBuilderPageTemplateUsageAsync(string templateIdentifier)
    {
        LastCall = $"page-template:{templateIdentifier}";
        return Task.FromResult(new ComponentUsageDetailDto { ComponentIdentifier = templateIdentifier, ComponentType = "PageTemplate" });
    }

    public Task<ComponentUsageDetailDto> GetPageBuilderWidgetUsageAsync(string widgetIdentifier)
    {
        LastCall = $"page-widget:{widgetIdentifier}";
        return Task.FromResult(new ComponentUsageDetailDto { ComponentIdentifier = widgetIdentifier, ComponentType = "Widget" });
    }

    public Task<List<ComponentUsageDetailDto>> GetBatchUsageAsync(List<string> identifiers, string componentType)
    {
        LastCall = $"batch:{componentType}:{identifiers.Count}";
        return Task.FromResult(identifiers.Select(i => new ComponentUsageDetailDto { ComponentIdentifier = i, ComponentType = componentType }).ToList());
    }

    public Task<EmailConfigurationUsageDetailDto> GetEmailBuilderWidgetUsageAsync(string widgetIdentifier)
    {
        LastCall = $"email-widget:{widgetIdentifier}";
        return Task.FromResult(new EmailConfigurationUsageDetailDto { ComponentIdentifier = widgetIdentifier, ComponentType = "EmailWidget" });
    }

    public Task<EmailConfigurationUsageDetailDto> GetEmailBuilderTemplateUsageAsync(string templateIdentifier)
    {
        LastCall = $"email-template:{templateIdentifier}";
        return Task.FromResult(new EmailConfigurationUsageDetailDto { ComponentIdentifier = templateIdentifier, ComponentType = "EmailTemplate" });
    }

    public Task<FormComponentUsageDetailDto> GetFormBuilderComponentUsageAsync(string componentIdentifier)
    {
        LastCall = $"form-component:{componentIdentifier}";
        return Task.FromResult(new FormComponentUsageDetailDto { ComponentIdentifier = componentIdentifier, ComponentType = "Component" });
    }

    public Task<FormComponentUsageDetailDto> GetFormBuilderSectionUsageAsync(string sectionIdentifier)
    {
        LastCall = $"form-section:{sectionIdentifier}";
        return Task.FromResult(new FormComponentUsageDetailDto { ComponentIdentifier = sectionIdentifier, ComponentType = "Section" });
    }
}

internal sealed class StubWebPageUrlRetriever : IWebPageUrlRetriever
{
    public Task<WebPageUrl> Retrieve(IWebPageFieldsSource webPageFieldsSource, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task<WebPageUrl> Retrieve(IWebPageFieldsSource webPageFieldsSource, string languageName, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task<WebPageUrl> Retrieve(string webPageUrlPath, string webPageTreePath, int websiteChannelId, string languageName, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task<WebPageUrl> Retrieve(string webPageTreePath, string websiteChannelName, string languageName, bool forPreview = false, CancellationToken cancellationToken = default) =>
        Task.FromResult(new WebPageUrl($"/{languageName}/page", null));

    public Task<WebPageUrl> Retrieve(int webPageItemId, string languageName, bool forPreview = false, CancellationToken cancellationToken = default) =>
        Task.FromResult(new WebPageUrl($"/{languageName}/page-{webPageItemId}", null));

    public Task<WebPageUrl> Retrieve(Guid webPageItemGuid, string languageName, bool forPreview = false, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task<IDictionary<Guid, WebPageUrl>> Retrieve(IReadOnlyCollection<Guid> webPageItemGuids, string websiteChannelName, string languageName, bool forPreview = false, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();
}

internal sealed class StubShareablePreviewLinkGenerator : IShareablePreviewLinkGenerator
{
    public Task<string?> Generate(int webPageItemId, string languageName, CancellationToken cancellationToken = default) =>
        Task.FromResult<string?>($"https://preview.example.com/{languageName}/page-{webPageItemId}?preview=abc123");
}

using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.UIPages;

using XperienceCommunity.ComponentRegistry.Admin;

[assembly: UIApplication(
    identifier: ComponentRegistryApplicationPage.IDENTIFIER,
    type: typeof(ComponentRegistryApplicationPage),
    slug: "component-registry",
    name: "Component Registry",
    category: BaseApplicationCategories.DEVELOPMENT,
    icon: Icons.CustomElement,
    templateName: TemplateNames.SECTION_LAYOUT)]

namespace XperienceCommunity.ComponentRegistry.Admin;

/// <summary>
/// Application page for viewing registered components.
/// </summary>
public class ComponentRegistryApplicationPage : ApplicationPage
{
    public const string IDENTIFIER = "XperienceCommunity.ComponentRegistry.Admin.App";
}

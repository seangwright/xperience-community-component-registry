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
[UIPermission(ComponentRegistryPermissions.VIEW_PAGE_BUILDER)]
[UIPermission(ComponentRegistryPermissions.VIEW_PAGE_BUILDER_USAGES)]
[UIPermission(ComponentRegistryPermissions.VIEW_FORM_BUILDER)]
[UIPermission(ComponentRegistryPermissions.VIEW_FORM_BUILDER_USAGES)]
[UIPermission(ComponentRegistryPermissions.VIEW_EMAIL_BUILDER)]
[UIPermission(ComponentRegistryPermissions.VIEW_EMAIL_BUILDER_USAGES)]
public class ComponentRegistryApplicationPage : ApplicationPage
{
    public const string IDENTIFIER = "XperienceCommunity.ComponentRegistry.Admin.App";
}

public static class ComponentRegistryPermissions
{
    public const string VIEW_PAGE_BUILDER = "View_Page_Builder";
    public const string VIEW_PAGE_BUILDER_USAGES = "View_Page_Builder_Usages";
    public const string VIEW_FORM_BUILDER = "View_Form_Builder";
    public const string VIEW_FORM_BUILDER_USAGES = "View_Form_Builder_Usages";
    public const string VIEW_EMAIL_BUILDER = "View_Email_Builder";
    public const string VIEW_EMAIL_BUILDER_USAGES = "View_Email_Builder_Usages";
}

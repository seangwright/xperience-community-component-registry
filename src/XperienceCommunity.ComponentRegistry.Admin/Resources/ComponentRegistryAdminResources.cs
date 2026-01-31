using CMS.Localization;

using XperienceCommunity.ComponentRegistry.Admin;

[assembly: RegisterLocalizationResource(typeof(ComponentRegistryAdminResources), LocalizationTarget.Server)]
[assembly: RegisterLocalizationResource(typeof(ComponentRegistryAdminResources), LocalizationTarget.Client)]

namespace XperienceCommunity.ComponentRegistry.Admin;

#pragma warning disable S2094 // Classes should not be empty
public class ComponentRegistryAdminResources
#pragma warning restore S2094 // Classes should not be empty
{

}

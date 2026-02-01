using Kentico.Xperience.Admin.Base;

using XperienceCommunity.ComponentRegistry.Admin;

[assembly: CMS.RegisterModule(typeof(ComponentRegistryWebAdminModule))]

namespace XperienceCommunity.ComponentRegistry.Admin
{
    internal class ComponentRegistryWebAdminModule : AdminModule
    {

        public ComponentRegistryWebAdminModule() : base("XperienceCommunity.ComponentRegistry.Admin") { }

        protected override void OnInit()
        {
            base.OnInit();

            RegisterClientModule("xperience-community-component-registry", "web-admin");
        }
    }
}

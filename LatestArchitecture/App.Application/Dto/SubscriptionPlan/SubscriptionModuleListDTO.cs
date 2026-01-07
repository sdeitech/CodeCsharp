namespace App.Application.Dto.SubscriptionPlan
{
    public class SubscriptionModuleListDTO
    {
        public int ModuleId { get; set; }
        public string IsActive { get; set; }
        public string ModuleName { get; set; }
        public string ModuleKey { get; set; }
        public int DisplayOrder { get; set; }
        public List<ModulePermissionsSuperAdmin> ModulePermissions { get; set; }
        public List<ScreenPermissionsSuperAdmin> ScreenPermissions { get; set; }
        public List<ActionPermissonsSuperAdmin> ActionPermissions { get; set; }

    }

    public class SubscriptionModuleMappingList
    {
        public int SubscriptionModuleid { get; set; }
        public int Moduleid { get; set; }
        public int Planid { get; set; }
        public bool IsActive { get; set; }
    }

    public class ModulePermissionsSuperAdmin
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string ModuleKey { get; set; }
       
        public bool Permission { get; set; }
        public int DisplayOrder { get; set; }
        public string ModuleIcon { get; set; }
        public string NavigationLink { get; set; }
    }

    public class ScreenPermissionsSuperAdmin
    {
        public int ModuleId { get; set; }
       
        public int ScreenId { get; set; }
        public string ScreenName { get; set; }
        public string ScreenKey { get; set; }
        public bool Permission { get; set; }
        public int DisplayOrder { get; set; }
        public string NavigationLink { get; set; }
    }

    public class ActionPermissonsSuperAdmin
    {
        public int ModuleId { get; set; }
       
        public int ScreenId { get; set; }
        public int ActionId { get; set; }
        public string ActionName { get; set; }
        public string ActionKey { get; set; }
        public bool Permission { get; set; }

    }

}
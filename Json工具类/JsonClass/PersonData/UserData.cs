using System.Collections.Generic;


[System.Serializable]
public class UserData
{
    public UserInfo userInfo;
    public List<MenuListItem> menuList;
    public List<string> permissionList;
    public string projectInfo;
}


[System.Serializable]
public class UserInfo
{
    public string userId;
    public string userAccount;
    public string userName;
    public string headIcon;
    public int gender;
    public string landline;
    public string telePhone;
    public string organizeId;
    public string organizeName;
    public string managerId;
    public List<string> subsidiary;
    public List<string> subordinates;
    public List<string> positionIds;
    public string positionName;
    public string positionId;
    public string roleId;
    public string roleName;
    public string roleIds;
    public long loginTime;
    public string loginIPAddress;
    public string loginIPAddressName;
    public string MACAddress;
    public string loginPlatForm;
    public int prevLogin;
    public string prevLoginTime;
    public string prevLoginIPAddress;
    public string prevLoginIPAddressName;
    public bool isTenantAdmin;
    public bool isAdministrator;
    public string overdueTime;
    public string tenantId;
    public string tenantDbName;
    public string tenantDbType;
    public string portalId;
    public bool isProjectAdmin;
    public List<string> dataScope;
    public string TintLogo;
    public string DarkLogo;
}

[System.Serializable]
public class MenuListItem
{
    public string fullName;
    public string enCode;
    public string icon;
    public int type;
    public string urlAddress;
    public string linkTarget;
    public string category;
    public string propertyJson;
    public int sortCode;
    public string id;
    public string parentId;
    public bool hasChildren;
    public List<string> children;
    public int num;
    public bool isLeaf;
}



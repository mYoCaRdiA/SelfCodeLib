using System.Collections.Generic;

public class DataManager : Manager
{
    public RequestClass loginRequestClass;

    //当前所有项目信息
    public List<Datum> nowProjectItemDatas = new List<Datum>();

    //当前使用的项目信息
    public Datum nowProjectItemData;

    //当前项目建筑类型信息
    public BuildingTypeData nowBuildingTypeData;

    public UserData nowUserInfoData;

    public ModelInfoData.LoadPathItem nowModelInfoItem;

    // public ModelPropertyData nowModelPropertyData;

  

    public string nowBuildUrl="";

    public string nowUserName = "";

    public string nowProjectId = "";

    public override void Ini()
    {
        if (GameManager.dataManager == null)
        {
            GameManager.dataManager = this;
        }
       
    }

}


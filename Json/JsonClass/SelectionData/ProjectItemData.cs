//工程表的信息
using System.Collections.Generic;

public class ProjectItemData
{
    public List<Datum> list { get; set; }



}

public class Datum
{
    public string enCode { get; set; }
    public string state { get; set; }
    public string startTime { get; set; }
    public string schedule { get; set; }
    public string timeLimit { get; set; }
    public string fullName { get; set; }
    public string id { get; set; }
    public string endTime { get; set; }
    public string managerIds { get; set; }
    public List<ManagersInfo> managersInfo { get; set; }
    public string buildingVolume { get; set; }
    public string monomerNum { get; set; }
    public string picture { get; set; }
    public string designCompany { get; set; }
    public string constructionCompany { get; set; }
    public string ownerCompany { get; set; }
    public string supervisionCompany { get; set; }
    public string amount { get; set; }
    public string contractingMode { get; set; }
    public string contractingModeName { get; set; }
    public string status { get; set; }
    public string statusName { get; set; }
    public string stage { get; set; }
    public string stageName { get; set; }
    public string projectType { get; set; }
    public string projectTypeName { get; set; }
    public string ManageUser { get; set; }

    public class ManagersInfo
    {
        public string account { get; set; }
        public string headIcon { get; set; }
    }
}
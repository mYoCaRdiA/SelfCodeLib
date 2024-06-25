
using System.Collections.Generic;

public class Label
{
    public string Id;
    public string ProjectId;
    public string CreatorUserId;
    public long CreatorTime;
    public string LabelName;
    public string LabelType;
}

[System.Serializable]
public class BuildingTypeData
{
    public Dictionary<string, List<Label>> list;
}
using System.Collections.Generic;

public class TreeMapperData
{
    public List<TreeMapper> treeMappers = new List<TreeMapper>();

    public TreeMapperData(List<TreeMapper> treeMappers)
    {
        this.treeMappers = treeMappers;
    }

    public class TreeMapper
    {
        public string Path { get; set; }
        public string T_Name { get; set; }
        public string L_Name { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public int[][] ModelIds { get; set; }
    }
}

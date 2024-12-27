using System.Collections.Generic;

public class GlbConstructNodeData2
{
    public List<Category> RootItems { get; set; }

    public GlbConstructNodeData2(List<Category> RootItems)
    {
        this.RootItems = RootItems;
    }

    public class Child
    {
        public string name { get; set; }
        public string mid { get; set; }
        public string uid { get; set; }
        public List<float> matrix { get; set; }
    }

    public class MeshChild
    {
        //glb���������ƣ�ģ�壩
        public string meshId { get; set; }
        public List<Child> children { get; set; }
    }

    public class Category
    {
        //glb·��
        public string category { get; set; }
        public List<MeshChild> children { get; set; }
    }
}

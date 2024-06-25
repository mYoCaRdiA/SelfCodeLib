
using System.Collections.Generic;

public class GlbCatalogData
{
    public List<GlbClass> constructInfo = new List<GlbClass>();

    public GlbCatalogData(List<GlbClass> constructInfo)
    {
        this.constructInfo = constructInfo;
    }

    [System.Serializable]
    public class GlbClass
    {
        public string name;
        public string id;
        public string category;
        public PointSelf Point;
        public float angle_a;
        public float angle_b;
        public float angle_c;
        public string specificationNum;
        public string specification;
        public string categoryName;
        public string categoryNumber;
        public string InstanceNumber;
        public string modelPath;
    }
}



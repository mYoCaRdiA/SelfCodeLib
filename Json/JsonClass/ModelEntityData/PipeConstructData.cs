public class PipeConstructData
{
    public PipelineClass[] rectMeps;
    public PipelineClass[] circularMeps;
    public PipelineClass[] ellipseMeps;


    /// <summary>
    /// π‹µ¿¿‡
    /// </summary>
    [System.Serializable]
    public class PipelineClass
    {
        public string type;
        public float width;
        public float height;
        public float diameter;
        public float length;
        public PointSelf startPoint;
        public PointSelf endPoint;
        public string color;
        public string id;
        public string name;
        public string systemName;
        public PointSelf base_x;
    }

  
}



using System.Collections.Generic;

public class SemanticConstructData
{
    /// <summary>
    /// 模型列表
    /// </summary>
    public ModelEntitys models { get; set; }
    /// <summary>
    /// 剪切信息
    /// </summary>
    public List<string> cuts { get; set; }

    public List<MaterialEntity> materials { get; set; }



    public class ModelEntitys
    {
        public List<OutLines> columns { get; set; }
        public List<OutLines> beams { get; set; }
        public List<OutLines> floors { get; set; }
        public List<OutLines> walls { get; set; }
        public List<OutLines> hollows { get; set; }
    }

    public class OutLines
    {
        /// <summary>
        /// 模型Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 模型名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 材质Id
        /// </summary>
        public string MaterialId { get; set; }
        /// <summary>
        /// 边界轮廓
        /// </summary>
        public List<List<PointSelf>> outline { get; set; }
        public List<List<PointSelf>> cutline { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public List<PointSelf> routes { get; set; }
        /// <summary>
        /// 线形， line,arc,curve
        /// </summary>
        public string type { get; set; }
        public PointSelf base_x { get; set; }

        public double width { get; set; }
        public double height { get; set; }
        public bool instance { get; set; }
    }


    public class MaterialEntity
    {
        public string Id { get; set; }
        public string color { get; set; }
        public string materialImage { get; set; }
        public string materialSize { get; set; }
        public string materialOffset { get; set; }

    }

}



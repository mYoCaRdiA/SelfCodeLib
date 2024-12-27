using System.Collections.Generic;

public class SemanticConstructData
{
    /// <summary>
    /// ģ���б�
    /// </summary>
    public ModelEntitys models { get; set; }
    /// <summary>
    /// ������Ϣ
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
        /// ģ��Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// ģ������
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ����Id
        /// </summary>
        public string MaterialId { get; set; }
        /// <summary>
        /// �߽�����
        /// </summary>
        public List<List<PointSelf>> outline { get; set; }
        public List<List<PointSelf>> cutline { get; set; }
        /// <summary>
        /// ·��
        /// </summary>
        public List<PointSelf> routes { get; set; }
        /// <summary>
        /// ���Σ� line,arc,curve
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



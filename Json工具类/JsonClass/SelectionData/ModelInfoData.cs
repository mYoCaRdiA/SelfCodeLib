
//using System.Collections.Generic;


//public class ModelInfoData
//{
//    public List<DataItem> list { get; set; }

//    public class DataItem
//    {
//        public string Id { get; set; }
//        public string ModelName { get; set; }
//        public string LastModifyUserId { get; set; }
//        public string LastModifyTime { get; set; }
//        public string ModelSize { get; set; }
//        public string ProjectId { get; set; }
//        public ModelEntity LoadPath { get; set; }
//    }

//    public class ModelEntity
//    {
//        public long ModelId { get; set; }
//        public long VersionId { get; set; }
//        public string VersionNo { get; set; }
//        public string ModelPath { get; set; }
//        public string ModelName { get; set; }
//        public long ModelSize { get; set; }
//        public int State { get; set; }
//        public object BIMDeviation { get; set; }
//        public object GISDeviation { get; set; }
//        public string ModelType { get; set; }
//    }

//}


using System.Collections.Generic;

public class ModelInfoData
{
    public List<LoadPathItem> list { get; set; }

    public class LoadPath
    {
        public string ModelId { get; set; }
        public string VersionId { get; set; }
        public string VersionNo { get; set; }
        public string ModelPath { get; set; }
        public string ModelName { get; set; }
        public string ModelSize { get; set; }
        public int State { get; set; }
        public object BIMDeviation { get; set; }
        public object GISDeviation { get; set; }
        public object ModelType { get; set; }
    }

    public class LoadPathItem
    {
        public LoadPath LoadPath { get; set; }
        public string Catgory1 { get; set; }
        public string Catgory2 { get; set; }
        public string Catgory3 { get; set; }
        public object CreateName { get; set; }
        public object Longitude { get; set; }
        public object Latitude { get; set; }
        public object Height { get; set; }
        public object XOff { get; set; }
        public object YOff { get; set; }
        public object ZOff { get; set; }
        public double LightweightTime { get; set; }
        public double LightweightSize { get; set; }
        public object Versions { get; set; }
        public string ProjectId { get; set; }
        public string ModelName { get; set; }
        public string BuildId { get; set; }
        public string MajorId { get; set; }
        public string FloorId { get; set; }
        public string ModelSize { get; set; }
        public string VersionNO { get; set; }
        public string LastModifyUserId { get; set; }
        public string LastModifyTime { get; set; }
        public object BIMDeviation { get; set; }
        public object GISDeviation { get; set; }
        public object DeleteMark { get; set; }
        public int Status { get; set; }
        public int Visit { get; set; }
        public string Id { get; set; }
    }
}


using System.Collections.Generic;

public class GlbConstructNodeData
{
    public Dictionary<string, List<Primitive>> Primitives { get; set; }

    public GlbConstructNodeData(Dictionary<string, List<Primitive>> primitives) {

        Primitives=primitives;
    }   

    public class Primitive
    {
        public string Name { get; set; }
        public int Mesh { get; set; }
        public List<float> Matrix { get; set; }
        public Extras Extras { get; set; }
        public int Index { get; set; }
    }

    public class Extras
    {
        public int ElementID { get; set; }
        public string UniqueId { get; set; }
        public int Index { get; set; }
    }
}
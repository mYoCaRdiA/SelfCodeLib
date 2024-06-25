using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RunTimeCreateModel : MonoBehaviour
{
    public Vector3 startPoint;
    public Vector3 endPoint;
    public List<Vector3> Outlines;
    public List<List<Vector3>> Cutliness;
    public Material roadMaterial;
    //顶面与底面
    public List<Vector3> outvertices = new List<Vector3>();
    public List<int> outtriangles = new List<int>();

    //初始化场景
    public void StartInstance()
    {
        Outlines.Add(Outlines.FirstOrDefault());
        //创建拉伸模型
        List<Vector3> vertices_Button = new List<Vector3>();
        List<Vector3> vertices_Top = new List<Vector3>();
        List<List<Vector3>> vertices_Side = new List<List<Vector3>>();
        //计算多边形定点
        //外轮廓
        Outlines.ForEach(o =>
        {
            Vector3 dir_lengh = (endPoint - startPoint).normalized;
            Vector3 dirX = new Vector3(1, 0, 0);
            Vector3 dirY = new Vector3(0, 0, 1);
            if (!(dir_lengh.x == 0 & dir_lengh.z == 0))
            {
                dirY = new Vector3(0, 1, 0);
                dirX = Vector3.Cross(dirY, dir_lengh).normalized;
            }
            vertices_Button.Add(startPoint + dirX * o.x + dirY * o.y);
            vertices_Top.Add(endPoint + dirX * o.x + dirY * o.y);
        });

        for (int i = 0; i < Outlines.Count - 1; i++)
        {
            vertices_Side.Add(new List<Vector3>()
            {
                vertices_Button[i],
                vertices_Top[i],
                vertices_Top[i+1],
                vertices_Button[i+1],
            });
        }

        //洞口 
        Cutliness.ForEach(Cutlines =>
        {
            vertices_Button.Clear();
            vertices_Top.Clear();

            Cutlines.ForEach(o =>
            {
                Vector3 dir_lengh = (endPoint - startPoint).normalized;
                Vector3 dirX = new Vector3(1, 0, 0);
                Vector3 dirY = new Vector3(0, 0, 1);
                if (!(dir_lengh.x == 0 & dir_lengh.z == 0))
                {
                    dirY = new Vector3(0, 1, 0);
                    dirX = Vector3.Cross(dirY, dir_lengh).normalized;
                }
                vertices_Button.Add(startPoint + dirX * o.x + dirY * o.y);
                vertices_Top.Add(endPoint + dirX * o.x + dirY * o.y);
            });

            for (int i = 0; i < Cutlines.Count - 1; i++)
            {
                vertices_Side.Add(new List<Vector3>()
                {
                    vertices_Button[i],
                    vertices_Top[i],
                    vertices_Top[i+1],
                    vertices_Button[i+1],
                });
            }
        });


        //重新计算顶面与底面
        vertices_Button.Clear();
        vertices_Top.Clear();
        outvertices.ForEach(o =>
        {
            Vector3 dir_lengh = (endPoint - startPoint).normalized;
            Vector3 dirX = new Vector3(1, 0, 0);
            Vector3 dirY = new Vector3(0, 0, 1);
            if (!(dir_lengh.x == 0 & dir_lengh.z == 0))
            {
                dirY = new Vector3(0, 1, 0);
                dirX = Vector3.Cross(dirY, dir_lengh).normalized;
            }
            vertices_Button.Add(startPoint + dirX * o.x + dirY * o.y);
            vertices_Top.Add(endPoint + dirX * o.x + dirY * o.y);
        });








        //开始创建模型
        var creatorMeshModel = new CreatorMeshModel();
        int currentCount = creatorMeshModel._vertices.Count;
        creatorMeshModel._vertices.AddRange(vertices_Top);
        creatorMeshModel._triangles.AddRange(outtriangles.Select(o => o + currentCount));

        //将顶面倒过来
        List<int> newouttriangles = new List<int>();
        for (int i = 0; i < outtriangles.Count - 2; i = i + 3)
        {
            newouttriangles.Add(outtriangles[i + 2]);
            newouttriangles.Add(outtriangles[i + 1]);
            newouttriangles.Add(outtriangles[i]);
        }
        currentCount = creatorMeshModel._vertices.Count;
        creatorMeshModel._vertices.AddRange(vertices_Button);
        creatorMeshModel._triangles.AddRange(newouttriangles.Select(o => o + currentCount));

        vertices_Side.ForEach(x =>
        {
            creatorMeshModel.AddMesh(x);
        });
        creatorMeshModel.CreatorMeshModel_(gameObject, roadMaterial);
    }
    /// <summary>
    /// 创建模型
    /// </summary>
    public class CreatorMeshModel
    {
        public List<Vector3> _vertices = new List<Vector3>();
        public List<int> _triangles = new List<int>();
        public List<Vector2> _uvs = new List<Vector2>();

        /// <summary>
        /// 通过三角面创建
        /// </summary>
        /// <param name="vertices"></param>
        public void AddMesh(List<Vector3> vertices)
        {
            // 进行三角剖分
            List<EarClipping.Triangle> triangles = EarClipping.Triangulate(vertices);

            // 输出三角形顶点索引
            foreach (var triangle in triangles)
            {
                //Debug.Log(triangle.VertexIndex1 + ", " + triangle.VertexIndex2 + ", " + triangle.VertexIndex3);
            }
            List<int> triangless = new List<int>();
            triangles.ForEach(x =>
            {
                triangless.Add(x.VertexIndex1 + _vertices.Count);
                triangless.Add(x.VertexIndex2 + _vertices.Count);
                triangless.Add(x.VertexIndex3 + _vertices.Count);
            });
            _vertices.AddRange(vertices);
            _triangles.AddRange(triangless);
        }

        //生成网格
        public void CreatorMeshModel_(GameObject obj, Material material)
        {
            if (material != null)
            {
                var m = new Material(obj.GetComponent<MeshRenderer>().material);
                m.color = material.color;
                m.mainTexture = material.mainTexture;
                m.name = material.name;
                obj.GetComponent<MeshRenderer>().material = m;
            }
            //创建模型
            Mesh mesh = new Mesh();
            mesh.vertices = _vertices.ToArray();
            mesh.triangles = _triangles.ToArray();
            // 计算UV坐标
            mesh.RecalculateNormals();
            mesh.uv = mesh.vertices.Select(o => new Vector2(o.x * 1, o.y * 1)).ToArray();
            mesh.uv2 = mesh.vertices.Select(o => new Vector2(o.x * 1, o.y * 1)).ToArray();
            obj.GetComponent<MeshFilter>().mesh = mesh;
            obj.AddComponent<MeshCollider>();
            Destroy(obj.GetComponent<BoxCollider>());
        }

    }
    /// <summary>
    /// 耳切法创建模型
    /// </summary>
    public class EarClipping
    {
        public class Triangle
        {
            public int VertexIndex1;
            public int VertexIndex2;
            public int VertexIndex3;

            public Triangle(int vertexIndex1, int vertexIndex2, int vertexIndex3)
            {
                VertexIndex1 = vertexIndex1;
                VertexIndex2 = vertexIndex2;
                VertexIndex3 = vertexIndex3;
            }
        }

        public static List<Triangle> Triangulate(List<Vector3> vertices)
        {
            if (vertices.Count == 4)
            {
                return new List<Triangle>() { new Triangle(0, 1, 2), new Triangle(0, 2, 3) };
            }
            if (vertices.Count == 5)
            {
                return new List<Triangle>() { new Triangle(0, 1, 2), new Triangle(0, 2, 3), new Triangle(0, 3, 4) };
            }
            if (vertices.Count == 6)
            {
                return new List<Triangle>() { new Triangle(0, 1, 2), new Triangle(0, 2, 3), new Triangle(0, 3, 4), new Triangle(0, 4, 5) };
            }
            List<Triangle> triangles = new List<Triangle>();

            int vertexCount = vertices.Count;
            if (vertexCount < 3)
            {
                return triangles;
            }

            int[] vertexIndices = new int[vertexCount];
            if (Area(vertices) > 0)
            {
                for (int i = 0; i < vertexCount; i++)
                {
                    vertexIndices[i] = i;
                }
            }
            else
            {
                for (int i = 0; i < vertexCount; i++)
                {
                    vertexIndices[i] = (vertexCount - 1) - i;
                }
            }

            int vertexIndex = vertexCount;
            int remainingVertices = vertexCount;

            while (remainingVertices > 3)
            {
                int earTipIndex = FindEarTip(vertices, vertexCount, vertexIndices);

                if (earTipIndex == -1)
                {
                    Debug.Log("No more ears!");
                    break;
                }

                int previousIndex = GetPreviousIndex(vertexCount, earTipIndex);
                int nextIndex = GetNextIndex(vertexCount, earTipIndex);

                triangles.Add(new Triangle(vertexIndices[previousIndex], vertexIndices[earTipIndex], vertexIndices[nextIndex]));

                vertexCount--;
                remainingVertices--;

                vertexIndices = RemoveVertexIndex(vertexIndices, vertexCount, earTipIndex);
            }

            triangles.Add(new Triangle(vertexIndices[0], vertexIndices[1], vertexIndices[2]));
            return triangles;
        }

        private static float Area(List<Vector3> vertices)
        {
            int vertexCount = vertices.Count;
            float area = 0;

            for (int i = 0; i < vertexCount; i++)
            {
                int j = (i + 1) % vertexCount;
                area += vertices[i].x * vertices[j].y;
                area -= vertices[j].x * vertices[i].y;
            }

            area /= 2;
            return area;
        }

        private static bool IsEarTip(List<Vector3> vertices, int vertexCount, int[] vertexIndices, int earTipIndex)
        {
            int previousIndex = GetPreviousIndex(vertexCount, earTipIndex);
            int nextIndex = GetNextIndex(vertexCount, earTipIndex);

            Vector3 previousVertex = vertices[vertexIndices[previousIndex]];
            Vector3 earTipVertex = vertices[vertexIndices[earTipIndex]];
            Vector3 nextVertex = vertices[vertexIndices[nextIndex]];

            Vector3 v0 = previousVertex - earTipVertex;
            Vector3 v1 = nextVertex - earTipVertex;

            if (Vector3.Cross(v0, v1).z >= 0)
            {
                return false;
            }

            for (int i = 0; i < vertexCount; i++)
            {
                int index = vertexIndices[i];

                if (index == previousIndex || index == earTipIndex || index == nextIndex)
                {
                    continue;
                }

                if (IsPointInsideTriangle(previousVertex, earTipVertex, nextVertex, vertices[index]))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsPointInsideTriangle(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, Vector3 point)
        {
            float d1 = Sign(point, vertex1, vertex2);
            float d2 = Sign(point, vertex2, vertex3);
            float d3 = Sign(point, vertex3, vertex1);

            bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(hasNeg && hasPos);
        }

        private static float Sign(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
        }

        private static int FindEarTip(List<Vector3> vertices, int vertexCount, int[] vertexIndices)
        {
            for (int i = 0; i < vertexCount; i++)
            {
                if (IsEarTip(vertices, vertexCount, vertexIndices, i))
                {
                    return i;
                }
            }

            return -1;
        }

        private static int GetPreviousIndex(int vertexCount, int currentIndex)
        {
            int index = currentIndex - 1;
            if (index < 0)
            {
                index = vertexCount - 1;
            }

            return index;
        }

        private static int GetNextIndex(int vertexCount, int currentIndex)
        {
            int index = currentIndex + 1;
            if (index >= vertexCount)
            {
                index = 0;
            }

            return index;
        }

        private static int[] RemoveVertexIndex(int[] vertexIndices, int vertexCount, int indexToRemove)
        {
            int[] newVertexIndices = new int[vertexCount];

            for (int i = 0, j = 0; i < vertexCount + 1; i++)
            {
                if (i != indexToRemove)
                {
                    newVertexIndices[j++] = vertexIndices[i];
                }
            }

            return newVertexIndices;
        }
    }
}
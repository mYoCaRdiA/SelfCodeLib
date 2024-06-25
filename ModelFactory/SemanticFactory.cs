
using static SemanticConstructData;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Collections;

public class SemanticFactory
{
    static string meshObjPath = "Assets/Resources/ModelTemp/Semantic/MeshObj.prefab";

    /// <summary>
    /// 语义模型
    /// </summary>
    /// <param name="modelPath"></param>
    public static IEnumerator CreateUrlSemantic(string modelUrlPath, Action<float> progressUpdate = null, Transform root = null)
    {
        if (progressUpdate != null) progressUpdate.Invoke(0);
        GameObject fatherGameObj = new GameObject("语义模型");
        Transform fatherTrans = fatherGameObj.transform;
        fatherTrans.parent = root;

       // GameObject pre = GameObjectPoolTool.GetFromPoolForce(true, meshObjPath);

        string dataPath = IoTools.Model_IO.GetModelDownloadPath(modelUrlPath);
        string jsonPath = Path.Combine(dataPath, "semantics", "semantics.json");
        string jsonString = IoTools.ReadFileString(jsonPath);

        if (jsonString != "")
        {
            //加载语义模型
            SemanticConstructData semanticConstructData = JsonConvert.DeserializeObject<SemanticConstructData>(jsonString);
            for (int i = 0; i < semanticConstructData.materials.Count; i++)
            {
                MaterialEntity x = semanticConstructData.materials[i];
                //依次柱梁板墙
                var columns = semanticConstructData.models.columns.Where(o => o.MaterialId == x.Id).ToList();
                var beams = semanticConstructData.models.beams.Where(o => o.MaterialId == x.Id).ToList();
                var floors = semanticConstructData.models.floors.Where(o => o.MaterialId == x.Id).ToList();
                var walls = semanticConstructData.models.walls.Where(o => o.MaterialId == x.Id).ToList();
                // 创建一个新的材质，使用标准着色器
                //Material newMaterial = new Material(Shader.Find("Standard"));
                Material newMaterial = Resources.Load<Material>("DoubleSide");

                newMaterial.name = x.Id;

                if (!string.IsNullOrEmpty(x.materialImage))
                {
                    string picPath = Path.Combine(dataPath, x.materialImage);
                    newMaterial.mainTexture = IoTools.Picture_IO.LoadImage(picPath);
                }

                x.color = x.color.Replace(".", ",");
                string[] colorValues = x.color.Split(',');
                newMaterial.color = new Color(float.Parse(colorValues[0]) / 255, float.Parse(colorValues[1]) / 255, float.Parse(colorValues[2]) / 255);
                if (columns.Count > 0)
                {
                    columns.ForEach(x =>
                    {
                        CreatorSolid(x, newMaterial, fatherTrans);
                        
                    });
                }
                if (beams.Count > 0)
                {
                    beams.ForEach(x =>
                    {
                        CreatorSolid(x, newMaterial, fatherTrans);
                        
                    });
                }
                if (floors.Count > 0)
                {
                    floors.ForEach(x =>
                    {
                        CreatorSolid(x, newMaterial, fatherTrans, -1);
                        
                    });
                }
                if (walls.Count > 0)
                {
                    walls.ForEach(x =>
                    {
                        CreatorSolid(x, newMaterial, fatherTrans);
                        
                    });
                }
                //GameController.Instance.ChangeMaterial(newMaterial, 1);
                if (progressUpdate != null)
                {
                    progressUpdate.Invoke(((float)i) / semanticConstructData.materials.Count);
                }

            }

            if (progressUpdate != null) progressUpdate.Invoke(100);
            fatherTrans.position = new Vector3(-10.56f,-3.28f,10.83f);
            Debug.Log("语义加载完毕");
        }
        else
        {
            Debug.LogError("json为空字符串");
        }
        
        yield return null;


        //创建实体
        void CreatorSolid(OutLines lineObj, Material mat, Transform parent = null, int dir = 1)
        {
            Vector3 startPoint = new Vector3(-lineObj.routes[0].X * 0.3048f, lineObj.routes[0].Z * 0.3048f, -lineObj.routes[0].Y * 0.3048f);
            Vector3 endPoint = new Vector3(-lineObj.routes[1].X * 0.3048f, lineObj.routes[1].Z * 0.3048f, -lineObj.routes[1].Y * 0.3048f);
            List<Vector3> outs = new List<Vector3>();
            List<Vector3> outLines = new List<Vector3>();
            List<Vector3> cuts = new List<Vector3>();
            List<List<Vector3>> cutss = new List<List<Vector3>>();
            List<int> outtriangless = new List<int>();
            List<Vector3> outvertices = new List<Vector3>();
            List<Edge> edges = new List<Edge>();
            lineObj.outline[0].ForEach(x =>
            {
                outs.Add(new Vector3(-x.X * 0.3048f, dir * x.Y * 0.3048f, 0));
                outLines.Add(new Vector3(-x.X * 0.3048f, dir * x.Y * 0.3048f, 0));
            });
            double d = TriangularSurface.GetDirection(outs);// Vector3.Cross(outs[0] - outs.Last(), outs[1] - outs[0]).z;
            if (d < 0)
            {
                outs.Reverse();
            }
            for (int i = 0; i < outs.Count - 1; i++)
            {
                edges.Add(new Edge(new XY(outs[i].x, outs[i].y), new XY(outs[i + 1].x, outs[i + 1].y)));
            }
            edges.Add(new Edge(new XY(outs.Last().x, outs.Last().y), new XY(outs[0].x, outs[0].y)));

            if (lineObj.cutline != null)
            {
                lineObj.cutline.ForEach(n =>
                {
                    List<Vector3> _cuts = new List<Vector3>();
                    n.ForEach(x =>
                    {
                        _cuts.Add(new Vector3(-x.X * 0.3048f, dir * x.Y * 0.3048f, 0));
                    });
                    double d1 = TriangularSurface.GetDirection(_cuts);// Vector3.Cross(_cuts[1] - _cuts[0], _cuts[2] - _cuts[1]).z;
                    if (d1 > 0)
                    {
                        _cuts.Reverse();
                    }
                    for (int i = 0; i < _cuts.Count - 1; i++)
                    {
                        edges.Add(new Edge(new XY(_cuts[i].x, _cuts[i].y), new XY(_cuts[i + 1].x, _cuts[i + 1].y)));
                    }
                    edges.Add(new Edge(new XY(_cuts.Last().x, _cuts.Last().y), new XY(_cuts[0].x, _cuts[0].y)));
                    cuts.AddRange(_cuts);
                    cutss.Add(_cuts);
                });
                outs.AddRange(cuts);
            }
            //计算三角网格
            GameObject newPrefab = GameObjectPoolTool.GetFromPoolForce(true, meshObjPath);
            AddMesh2(newPrefab, outs.Select(o => { return new XY(o.x, o.y); }).ToList(), edges, out outvertices, out outtriangless);
            outvertices = outs;
            var cm = newPrefab.GetComponent<RunTimeCreateModel>();
            cm.startPoint = startPoint;
            cm.endPoint = endPoint;
            cm.Outlines = outLines;
            cm.Cutliness = cutss;
            cm.outtriangles = outtriangless;
            cm.outvertices = outvertices;
            cm.roadMaterial = mat;
            cm.StartInstance();
            newPrefab.name = lineObj.Name;
            newPrefab.transform.parent = parent;

            //ModelControl.allModelMaterialList.Add(new HHMaterial(newPrefab.GetComponent<MeshRenderer>().material, newPrefab, newPrefab.name, -1));
            //newPrefab.transform.parent = modelParent;
            //modelInfoDic.Add(newPrefab.transform, new ModelInfo { modelPath = modelPath });
        }
        /// <summary>
        /// 通过多边形创建
        /// </summary>
        /// <param name="vector3s"></param>
        void AddMesh2(GameObject obj, List<XY> vector3s, List<Edge> edges, out List<Vector3> vertices, out List<int> triangles)
        {
            triangles = TriangularSurface.Start(vector3s, edges);
            vertices = new List<Vector3>();
            //return;
            //var points = vector3s.Select(o => { return new Vector2(o.X, o.Y); }).ToArray();
            //PolygonCollider2D edge = prefab_old.AddComponent<PolygonCollider2D>();
            //edge.points = points;
            //Mesh mesh = edge.CreateMesh(true, true);
            //mesh.RecalculateNormals();
            //List<int> triangless = new List<int>();
            //if (mesh == null)
            //{
            //    vertices = new List<Vector3>();
            //    triangles = new List<int>();
            //}
            //else
            //{
            //    vertices = mesh.vertices.ToList();
            //    triangles = mesh.triangles.ToList();
            //}
            //Destroy(edge);

        }
    }



    public class XY
    {
        private double v;

        public float X { get; set; }
        public float Y { get; set; }
        public XY(double x, double y)
        {
            X = (float)x;
            Y = (float)y;
        }
        public XY()
        {

        }
    }
    public class Edge
    {
        private double v;

        public XY SP { get; set; }
        public XY ED { get; set; }
        public Edge(XY x, XY y)
        {
            SP = (XY)x;
            ED = (XY)y;
        }
        public Edge()
        {

        }
    }
    public class TriangleEntity
    {
        private double v;

        public XY P1 { get; set; }
        public XY P2 { get; set; }
        public XY P3 { get; set; }
        public TriangleEntity(XY x, XY y, XY z)
        {
            P1 = (XY)x;
            P2 = (XY)y;
            P3 = (XY)z;
        }
        public TriangleEntity()
        {

        }
    }
    public class Circlenum
    {
        private double v;

        public XY P1 { get; set; }
        public double R { get; set; }
        public Circlenum(XY x, double r)
        {
            P1 = (XY)x;

            R = (double)r;
        }
        public Circlenum()
        {

        }
    }

    public class TriangularSurface
    {
        /// <summary>
        /// 获取方向
        /// </summary>
        /// <returns></returns>
        public static int GetDirection(List<Vector3> vertices)
        {
            float sum = 0;
            for (int i = 0; i < vertices.Count; i++)
            {
                Vector3 current = vertices[i];
                Vector3 next = vertices[(i + 1) % vertices.Count];
                sum += (next.x - current.x) * (next.y + current.y);
            }
            if (sum > 0)
            {
                return -1;
            }
            else if (sum < 0)
            {
                return 1;
            }
            return 1;
        }
        /// <summary>
        /// 外轮廓，和洞口
        /// </summary>
        /// <param name="outLine"></param>
        /// <param name="opens"></param>
        public static List<int> Start(List<XY> points, List<Edge> edges)
        {
            List<TriangleEntity> TriangleEntitys = new List<TriangleEntity>();
            int N = edges.Count();
            int M = N;
            while (N >= 3)
            {


                if (N == 3)
                {
                    TriangleEntitys.Add(new TriangleEntity(edges[0].SP, edges[1].SP, edges[2].SP));
                    break;
                }
                else
                {
                    int k = 1;
                    List<XY> cpoints = new List<XY>();
                    //(2) k=k+1,若Lk2在有向线段L11L12之左,转入（3），否则转入(2)
                    int kk = 0;
                    for (k = 1; k < N; k++)
                    {

                        if (edges[k].ED.Y == edges[0].ED.Y && edges[k].ED.X == edges[0].ED.X) { continue; }
                        if ((edges[0].ED.X - edges[0].SP.X) * (edges[k].ED.Y - edges[0].SP.Y) - (edges[0].ED.Y - edges[0].SP.Y) * (edges[k].ED.X - edges[0].SP.X) > 0)  //大于0为左侧
                        {
                            //(3)判断当前多边形其余各边是否与线段L11LK2 或L12LK2 相交,若是,转入(2),否则转入(4);
                            Edge Lk1 = new Edge(edges[0].SP, edges[k].ED);
                            Edge Lk2 = new Edge(edges[0].ED, edges[k].ED);
                            kk++;

                            for (int i = 1; i < N; i++)
                            {
                                //if (i == k) { continue; }

                                bool aa = LineIntersect(edges[i], Lk1) || LineIntersect(edges[i], Lk2);
                                if (aa) //如存在相交，转入2,
                                {
                                    break;
                                }
                                //否则，保存节点Lk2至候选节点链表中:
                                else
                                {
                                    if (i == N - 1)
                                    {
                                        cpoints.Add(edges[k].ED);
                                    }
                                    else
                                    {
                                        continue;
                                    }

                                }
                            }

                        }
                        else
                        {

                            continue;
                        }

                    }


                    if (cpoints.Count > 0)
                    {
                        int m = 0;
                        for (int i = 0; i < cpoints.Count; i++)
                        {
                            Circlenum Cir = ThreePointCircle(edges[0].SP, edges[0].ED, cpoints[i]);
                            if (cpoints.Where(x => (x.X != cpoints[i].X || x.Y != cpoints[i].Y) && TwoPointDistance(x, Cir.P1) < Cir.R).Count() == 0)
                            {
                                m = i;
                                break;
                            }
                        }

                        TriangleEntitys.Add(new TriangleEntity(edges[0].SP, edges[0].ED, cpoints[m]));

                        //(5)
                        Edge L0m = new Edge(cpoints[m], edges[0].SP);
                        Edge L1m = new Edge(edges[0].ED, cpoints[m]);
                        if (!EdgeContains(L0m, edges) && !EdgeContains(L1m, edges)) //若两条边均不是当前多边形的边界线段
                        {
                            N = N + 1;

                            //  edges.Add(new Edge(edges[0].SP,cpoints[m]));
                            edges.Add(new Edge(cpoints[m], edges[0].ED));
                            edges[0].ED = cpoints[m];
                            continue;
                        }

                        if (EdgeContains(L0m, edges) && !EdgeContains(L1m, edges))//若L0m是当前多边形的边界线段，L1m不是
                        {
                            N = N - 1;
                            edges[0].SP = cpoints[m];
                            EdgeRemove(L0m, edges);

                            continue;
                        }
                        if (!EdgeContains(L0m, edges) && EdgeContains(L1m, edges))//若L1m是当前多边形的边界线段，L0m不是
                        {
                            N = N - 1;
                            edges[0].ED = cpoints[m];
                            EdgeRemove(L1m, edges);
                            continue;
                        }

                        if (EdgeContains(L0m, edges) && EdgeContains(L1m, edges)) //两条边都是多边形的边
                        {
                            N = N - 3;
                            edges.RemoveAt(0);
                            EdgeRemove(L0m, edges);
                            EdgeRemove(L1m, edges);
                            continue;
                        }
                    }

                }
                if (N == M) { break; }
                else { M = N; }

            }

            Console.WriteLine(TriangleEntitys);
            List<int> t_indexs = new List<int>();
            TriangleEntitys.ForEach(x =>
            {
                int index1 = points.FindIndex(o => o.X == x.P1.X && o.Y == x.P1.Y);
                int index2 = points.FindIndex(o => o.X == x.P2.X && o.Y == x.P2.Y);
                int index3 = points.FindIndex(o => o.X == x.P3.X && o.Y == x.P3.Y);
                t_indexs.Add(index1);
                t_indexs.Add(index2);
                t_indexs.Add(index3);
            });
            Console.WriteLine(string.Join(",", t_indexs));
            //（2）令k = k + 1若L在有向线段  2之左转入(3)否则转入(2);
            return t_indexs;

        }
        static bool LineIntersect(Edge e1, Edge e2)
        {
            XY aa = e1.SP;
            XY bb = e1.ED;
            XY cc = e2.SP;
            XY dd = e2.ED;

            if ((aa.X == cc.X && aa.Y == cc.Y) || (aa.X == dd.X && aa.Y == dd.Y) || (bb.X == cc.X && bb.Y == cc.Y) || (bb.X == dd.X && bb.Y == dd.Y))
            {
                return false;
            }
            else
            {
                if (LineLeft(e1.SP, e2) == !LineLeft(e1.ED, e2) && LineLeft(e2.SP, e1) == !LineLeft(e2.ED, e1))
                { return true; }
                else
                { return false; }
            }



            //return true;


            //double delta = determinant(bb.X - aa.X, cc.X - dd.X, bb.Y - aa.Y, cc.Y - dd.Y); 
            //if (delta <=0.0001 && delta >= -0.0001) // delta=0，表示两线段重合或平行
            //{
            //    return false;
            //}   
            //double namenda = determinant(cc.X - aa.X, cc.X - dd.X, cc.Y - aa.Y, cc.Y - dd.Y) / delta; 
            //if (namenda > 1 || namenda < 0)
            //{
            //    return false;
            //}
            //double miu = determinant(bb.X - aa.X, cc.X - aa.X, bb.Y - aa.Y, cc.Y - aa.Y) / delta;
            //if (miu > 1 || miu <0 )
            //{
            //    return false;
            //}



        }
        static bool LineLeft(XY p, Edge e)
        {

            if ((e.ED.X - e.SP.X) * (p.Y - e.SP.Y) - (e.ED.Y - e.SP.Y) * (p.X - e.SP.X) > 0)
            { return true; }
            else { return false; }

        }
        static double TwoPointDistance(XY p1, XY p2)
        {

            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));

        }
        static Circlenum ThreePointCircle(XY p1, XY p2, XY p3)
        {
            double x1 = p1.X;
            double y1 = p1.Y;
            double x2 = p2.X;
            double y2 = p2.Y;
            double x3 = p3.X;
            double y3 = p3.Y;
            double e = 2 * (x2 - x1);
            double f = 2 * (y2 - y1);
            double g = x2 * x2 - x1 * x1 + y2 * y2 - y1 * y1;
            double a = 2 * (x3 - x2);
            double b = 2 * (y3 - y2);
            double c = x3 * x3 - x2 * x2 + y3 * y3 - y2 * y2;
            double X = (g * b - c * f) / (e * b - a * f);
            double Y = (a * g - c * e) / (a * f - b * e);
            Circlenum xyz = new Circlenum(new XY(X, Y), Math.Sqrt((X - x1) * (X - x1) + (Y - y1) * (Y - y1)));
            return xyz;
        }
        static bool EdgeContains(Edge e, List<Edge> edge)
        {
            //List<Edge> a = edge.Where(x => x.SP.X == e.SP.X && x.SP.Y == e.SP.Y && x.ED.X == e.ED.X && x.ED.Y == e.ED.Y).ToList();
            //List<Edge> b = edge.Where(x => x.SP.X == e.SP.X && x.SP.Y == e.SP.Y && x.ED.X == e.ED.X && x.ED.Y == e.ED.Y).ToList();
            if (edge.Where(x => x.SP.X == e.SP.X && x.SP.Y == e.SP.Y && x.ED.X == e.ED.X && x.ED.Y == e.ED.Y).Count() != 0 ||
                edge.Where(x => x.SP.X == e.ED.X && x.SP.Y == e.ED.Y && x.ED.X == e.SP.X && x.ED.Y == e.SP.Y).Count() != 0)//查找边中，起点与待选点一样的边
            {
                return true;
            }
            else { return false; }
        }
        static void EdgeRemove(Edge e, List<Edge> edge)
        {
            int m = edge.FindIndex(x => x.SP.X == e.SP.X && x.SP.Y == e.SP.Y && x.ED.X == e.ED.X && x.ED.Y == e.ED.Y);
            if (m != -1)
            {
                edge.RemoveAt(m);
            }
            int n = edge.FindIndex(x => x.SP.X == e.ED.X && x.SP.Y == e.ED.Y && x.ED.X == e.SP.X && x.ED.Y == e.SP.Y);
            if (n != -1)
            {
                edge.RemoveAt(n);
            }
        }
    }

}

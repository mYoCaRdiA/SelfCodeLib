
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using UnityEditor;

namespace ZiChuanTools
{
    public static class ActiveCtrl
    {
        public static void SetTrue(object target)
        {
            if (target is GameObject gameObject)
            {
                gameObject.SetActive(true);
            }
            else if (target is Component component)
            {
                component.gameObject.SetActive(true);
            }
        }

        public static void MutipleSetTrue(params object[] targets)
        {
            foreach (var item in targets)
            {
                if (item is GameObject gameObject)
                {
                    gameObject.SetActive(true);
                }
                else if (item is Component component)
                {
                    component.gameObject.SetActive(true);
                }
            }
        }

        public static void SetFalse(object target)
        {
            if (target is GameObject gameObject)
            {
                gameObject.SetActive(false);
            }
            else if (target is Component component)
            {
                component.gameObject.SetActive(false);
            }
        }

        public static void MutipleSetFalse(params object[] targets)
        {
            foreach (var item in targets)
            {
                if (item is GameObject gameObject)
                {
                    gameObject.SetActive(false);
                }
                else if (item is Component component)
                {
                    component.gameObject.SetActive(false);
                }
            }
        }

        public static void SetOpposite(object target)
        {
            if (target is GameObject gameObject)
            {
                gameObject.SetActive(!gameObject.activeSelf);
            }
            else if (target is Component component)
            {
                component.gameObject.SetActive(!component.gameObject.activeSelf);
            }
        }

        public static void MutipleSetOpposite(params object[] targets)
        {
            foreach (var item in targets)
            {
                if (item is GameObject gameObject)
                {
                    gameObject.SetActive(!gameObject.activeSelf);
                }
                else if (item is Component component)
                {
                    component.gameObject.SetActive(!component.gameObject.activeSelf);
                }
            }
        }
    }
    public class MatrixDispose
    {
        /// <summary>
        /// threejs矩阵转unity
        /// </summary>
        /// <param name="threeJsMatrix"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static Matrix4x4 ConvertThreeJsToUnity(float[] threeJsMatrix)
        {
            if (threeJsMatrix.Length != 16)
            {
                throw new System.ArgumentException("The input matrix must have 16 elements.");
            }

            //创建一个新的Unity 4x4矩阵
            Matrix4x4 unityMatrix = new Matrix4x4();

            //将three.js的列优先矩阵转换为 Unity 的行优先矩阵
            unityMatrix.SetRow(0, new Vector4(threeJsMatrix[0], threeJsMatrix[4], threeJsMatrix[8], threeJsMatrix[12]));
            unityMatrix.SetRow(1, new Vector4(threeJsMatrix[1], threeJsMatrix[5], threeJsMatrix[9], threeJsMatrix[13]));
            unityMatrix.SetRow(2, new Vector4(threeJsMatrix[2], threeJsMatrix[6], threeJsMatrix[10], threeJsMatrix[14]));
            unityMatrix.SetRow(3, new Vector4(threeJsMatrix[3], threeJsMatrix[7], threeJsMatrix[11], threeJsMatrix[15]));

            return unityMatrix;
        }

        /// <summary>
        /// unity位置转three.js矩阵
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static List<float> UnityTransformToThreeJSMatrix(Vector3 position, Quaternion rotation)
        {
            // 创建一个4x4矩阵
            Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
            matrix.m03 = -matrix.m03;
            // 创建一个List<float>来存储矩阵数据
            List<float> matrixList = new List<float>(16);

            // 将矩阵数据填入List中，按列主序存储
            matrixList.Add(matrix.m00);
            matrixList.Add(matrix.m10);
            matrixList.Add(matrix.m20);
            matrixList.Add(matrix.m30);

            matrixList.Add(matrix.m01);
            matrixList.Add(matrix.m11);
            matrixList.Add(matrix.m21);
            matrixList.Add(matrix.m31);

            matrixList.Add(matrix.m02);
            matrixList.Add(matrix.m12);
            matrixList.Add(matrix.m22);
            matrixList.Add(matrix.m32);

            matrixList.Add(matrix.m03);
            matrixList.Add(matrix.m13);
            matrixList.Add(matrix.m23);
            matrixList.Add(matrix.m33);

            return matrixList;
        }
    }
    public class TextureDispose
    {
        /// <summary>
        /// 将Texture2D转换为Sprite
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static Sprite TextureToSprite(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// 截取图片中心
        /// </summary>
        /// <param name="source"></param>
        /// <param name="cropWidth"></param>
        /// <param name="cropHeight"></param>
        /// <returns></returns>
        public static Texture2D CropCenter(Texture2D source, float cropWidth, float cropHeight)
        {
            //计算中心点
            float centerX = source.width / 2;
            float centerY = source.height / 2;

            //计算左上角起始点
            float startX = Mathf.Max(centerX - cropWidth / 2, 0);
            float startY = Mathf.Max(centerY - cropHeight / 2, 0);

            //确保截取区域在图片范围内
            float width = Mathf.Min(cropWidth, source.width - startX);
            float height = Mathf.Min(cropHeight, source.height - startY);

            //获取像素
            UnityEngine.Color[] pixels = source.GetPixels((int)startX, (int)startY, (int)width, (int)height);

            //创建新Texture2D
            Texture2D croppedTexture = new Texture2D((int)width, (int)height);
            croppedTexture.SetPixels(pixels);
            croppedTexture.Apply();

            return croppedTexture;
        }
    }
    public static class TimeTools
    {
        /// <summary>
        /// 获取当前本地时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetLocalDateTime()
        {
            // 获取当前 Unix 时间戳（秒）
            long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // 将 Unix 时间戳转换为 DateTimeOffset
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(currentTimestamp);

            // 转换为本地时间
            DateTime localDateTime = dateTimeOffset.LocalDateTime;
            return localDateTime;
        }
        /// <summary>
        /// 获取当前世界时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetWorldDateTime()
        {
            // 获取当前 Unix 时间戳（秒）
            long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // 将 Unix 时间戳转换为 DateTimeOffset
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(currentTimestamp);

            // 转换为本地时间
            DateTime localDateTime = dateTimeOffset.DateTime;
            return localDateTime;
        }
        public static double DiffSeconds(DateTime startTime, DateTime endTime)
        {
            TimeSpan secondSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
            return secondSpan.TotalSeconds;
        }
        public static double DiffMinutes(DateTime startTime, DateTime endTime)
        {
            TimeSpan minuteSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
            return minuteSpan.TotalMinutes;
        }
        public static double DiffHours(DateTime startTime, DateTime endTime)
        {
            TimeSpan hoursSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
            return hoursSpan.TotalHours;
        }
        public static double DiffDays(DateTime startTime, DateTime endTime)
        {
            TimeSpan daysSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
            return daysSpan.TotalDays;
        }
        public static int DiffDaysInt(DateTime startTime, DateTime endTime)
        {
            TimeSpan daysSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
            return daysSpan.Days;
        }

        public static string TimeStamp2Str(string timeStamp)
        {
            if (timeStamp != null && timeStamp != "")
            {
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(1713436661996);
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
                DateTime startTime = localTime;
                DateTime dt = startTime.AddMilliseconds(double.Parse(timeStamp));
                return dt.ToString("yyyy/MM/dd HH:mm:ss");
            }
            else
            {
                Debug.LogError("时间戳为空");
                return "";
            }
        }

        /// <summary>
        /// 拿到时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {

            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
        }


        //参数格式"2024-05-04"

        public static long GetTimeStamp(string dateStr)
        {
            DateTime date;
            bool isValidDate = DateTime.TryParseExact(dateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out date);

            if (!isValidDate)
            {
                throw new ArgumentException("The provided date is not in the correct format.");
            }

            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan ts = date.ToUniversalTime() - epoch;
            return Convert.ToInt64(ts.TotalMilliseconds);
        }
    }
    public static class AudioTools
    {
        /// <summary>
        /// 将音频文件转成字符串
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        public static string AudioClipToByte(AudioClip clip)
        {
            if (clip == null)
                return string.Empty;

            float[] floatData = new float[clip.samples * clip.channels];
            clip.GetData(floatData, 0);
            byte[] outData = new byte[floatData.Length];
            Buffer.BlockCopy(floatData, 0, outData, 0, outData.Length);
            return Convert.ToBase64String(outData);
        }

        /// <summary>
        /// 将字符串转换成音频文件
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static AudioClip StringToAudio(string content)
        {
            if (content.Equals(""))
                return null;

            byte[] bytes = Convert.FromBase64String(content);
            float[] samples = new float[bytes.Length];
            Buffer.BlockCopy(bytes, 0, samples, 0, bytes.Length);
            AudioClip clip = AudioClip.Create("RecordClip", samples.Length, 1, 16000, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
    public class GameObjectPoolTool : MonoBehaviour
    {
        public static GameObject GetFromPool(bool active, string fullname)
        {
            string aimFullName = fullname;
            aimFullName = GameObjectPool.FormatPath4ResourceLoad(aimFullName);

            GameObject aimGameObj;
            aimGameObj = GameObjectPool.OutPool(aimFullName);
            if (aimGameObj == null)
            {
                return null;
            }
            else
            {
                //aimGameObj.transform.SetParent(null);
                aimGameObj.SetActive(active);
                return aimGameObj;
            }
        }

        public static async Task<GameObject> GetFromPoolForceAsync(bool active, string fullname)
        {
            string aimFullName = fullname;
            aimFullName = GameObjectPool.FormatPath4ResourceLoad(aimFullName);
            GameObject aimGameObj;
            aimGameObj = GameObjectPool.OutPool(aimFullName);

            if (aimGameObj == null)
            {
                Task getResource = GameObjectPool.PreLoadPrefabToPoolAsync(aimFullName);
                await getResource;
                aimGameObj = GameObjectPool.OutPool(aimFullName);
                aimGameObj.SetActive(active);
                return aimGameObj;
            }
            else
            {
                //aimGameObj.transform.SetParent(null);
                aimGameObj.SetActive(active);
                return aimGameObj;
            }
        }

        public static GameObject GetFromPoolForce(bool active, string fullname)
        {
            string aimFullName = fullname;
            aimFullName = GameObjectPool.FormatPath4ResourceLoad(aimFullName);
            GameObject aimGameObj;
            aimGameObj = GameObjectPool.OutPool(aimFullName);

            if (aimGameObj == null)
            {
                GameObjectPool.PreLoadPrefabToPool(aimFullName);
                aimGameObj = GameObjectPool.OutPool(aimFullName);
                aimGameObj.SetActive(active);
                return aimGameObj;
            }
            else
            {
                //aimGameObj.transform.SetParent(null);
                aimGameObj.SetActive(active);
                return aimGameObj;
            }
        }



        public static void PutInPool(GameObject gameObj)
        {
            GameObjectPool.InPool(gameObj);
        }

        public static void Release(bool releaseAll = false)
        {
            if (releaseAll)
            {
                GameObjectPool.ReleaseAll();
            }
            else
            {
                GameObjectPool.ReleaseEmptyObj();
            }
        }


        //-------------------------------Plot池

        public class GameObjectPool
        {
            //池字典
            public static Dictionary<string, List<GameObject>> itemPool = new Dictionary<string, List<GameObject>>();
            //加载过的资源
            public static Dictionary<string, GameObject> loadedPrefabs = new Dictionary<string, GameObject>();

            public static void ReleaseAll()
            {
                foreach (var item in itemPool)
                {
                    foreach (GameObject obj in item.Value)
                    {
                        if (obj != null)
                        {
                            Destroy(obj);
                        }
                    }
                }
                itemPool.Clear();
            }

            public static void ReleaseEmptyObj()
            {
                List<GameObject> allEmptyObj = new List<GameObject>();
                foreach (var item in itemPool)
                {
                    List<GameObject> tempEmptyObj = new List<GameObject>();
                    foreach (GameObject obj in item.Value)
                    {
                        if (obj == null)
                        {
                            tempEmptyObj.Add(obj);
                            allEmptyObj.Add(obj);
                        }
                    }
                    foreach (GameObject obj in tempEmptyObj)
                    {
                        item.Value.Remove(obj);
                    }
                }
            }


            public static void InPool(GameObject gameObj)
            {
                //Button but = gameObj.GetComponent<Button>();
                //if (but != null)
                //{
                //    but.onClick.RemoveAllListeners();
                //}

                if (itemPool.ContainsKey(gameObj.name) == false)
                {
                    itemPool.Add(gameObj.name, new List<GameObject>());
                }

                gameObj.SetActive(false);

                if (!itemPool[gameObj.name].Contains(gameObj))
                {
                    itemPool[gameObj.name].Add(gameObj);
                }
            }

            public static GameObject OutPool(string assetPath)
            {
                if (itemPool.ContainsKey(assetPath) == false)
                {
                    itemPool.Add(assetPath, new List<GameObject>());
                }

                if (itemPool[assetPath].Count == 0)
                {
                    return null;
                }
                else
                {
                    GameObject outGo = itemPool[assetPath][0];
                    itemPool[assetPath].Remove(outGo);
                    if (outGo != null)
                    {
                        //这里有竞争条件，可能要Lock      
                        outGo.SetActive(true);
                        return outGo;
                    }
                    else
                    {
                        return OutPool(assetPath);
                    }
                }
            }


            /// <summary>
            /// 通过路径获取预制件名字
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            private static string GetNameFromPrefabPath(string path)
            {
                string target = "";
                string[] strs = path.Split('/');
                if (strs.Length != 0)
                {
                    string temp = strs[strs.Length - 1];
                    target = FormatPath4ResourceLoad(temp);
                }
                return target;
            }


            public static string FormatPath4ResourceLoad(string oringnal)
            {
                string target = oringnal;
                if (target.Contains(".prefab"))
                {
                    target = target.Replace(".prefab", "");
                }
                if (target.Contains("Assets/Resources/"))
                {
                    target = target.Replace("Assets/Resources/", "");
                }

                return target;
            }


            /// <summary>
            /// 协程
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public static IEnumerator PreLoadPrefabToPoolIE(string path)
            {
                string assetPath = path;
                assetPath = FormatPath4ResourceLoad(assetPath);

                Task<GameObject> loadTask = LoadGameObjectAsync(path);
                while (!loadTask.IsCompleted)
                {
                    yield return null;
                }

                GameObject loadedObject = Instantiate(loadTask.Result);
                loadedObject.name = path;
                InPool(loadedObject);

                yield return loadedObject;

            }




            /// <summary>
            /// 异步
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public async static Task<GameObject> PreLoadPrefabToPoolAsync(string path)
            {
                string assetPath = path;
                assetPath = FormatPath4ResourceLoad(assetPath);
                Task<GameObject> loadTask = LoadGameObjectAsync(path);
                await loadTask;
                GameObject loadedObject = Instantiate(loadTask.Result);
                loadedObject.name = path;
                InPool(loadedObject);

                return loadedObject;
            }


            public static GameObject PreLoadPrefabToPool(string path)
            {
                string assetPath = path;
                assetPath = FormatPath4ResourceLoad(assetPath);
                GameObject gameObj = LoadGameObject(path);
                GameObject loadedObject = Instantiate(gameObj);
                loadedObject.name = path;
                InPool(loadedObject);

                return loadedObject;
            }


            static async Task<GameObject> LoadGameObjectAsync(string path)
            {
                if (loadedPrefabs.ContainsKey(path))
                {
                    return loadedPrefabs[path];
                }
                else
                {
                    ResourceRequest rr = Resources.LoadAsync<GameObject>(path);
                    while (rr.isDone == false) await Task.Delay(100);
                    GameObject gameObj = rr.asset as GameObject;
                    loadedPrefabs.Add(path, gameObj);
                    return gameObj;
                }
            }

            static GameObject LoadGameObject(string path)
            {
                if (loadedPrefabs.ContainsKey(path))
                {
                    return loadedPrefabs[path];
                }
                else
                {
                    GameObject gameObj = Resources.Load<GameObject>(path);
                    loadedPrefabs.Add(path, gameObj);
                    return gameObj;
                }
            }
        }
    }
    public static class JsonTools
    {
        /// <summary>
        /// 保存Json文件到本地
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="JsonContent"></param>
        public static void SaveJsonToLocal(string fileName, string JsonContent)
        {
            string fullPath = Application.persistentDataPath + "/" + fileName;

            int lastIndex = fullPath.LastIndexOf('/');

            DirectoryInfo dir = new DirectoryInfo(fullPath.Substring(0, lastIndex));
            if (!dir.Exists)
                dir.Create();

            if (!fileName.EndsWith(".json"))
            {
                fullPath += ".json";
            }

            FileInfo fileInfo = new FileInfo(fullPath);
            if (fileInfo.Exists)
                fileInfo.Delete();

            StreamWriter writer = fileInfo.CreateText();
            writer.Write(JsonContent);
            writer.Flush();
            writer.Dispose();
            writer.Close();
        }

        
        /// <summary>
        /// 防止文件有不明字符，故操作一遍
        /// </summary>
        /// <param name="jsonContent"></param>
        /// <returns></returns>
        public static string GetJsonStr(string json)
        {
            char startChar = default;
            char endChar = default;
            int startIndex = 0; ;

            for (int i = 0; i < json.Length; i++)
            {
                if (json[i].Equals('{') || json[i].Equals('['))
                {
                    startIndex = i;
                    break;
                }
            }

            json = json.Substring(startIndex, json.Length - 1);


            for (int i = 0; i < json.Length; i++)
            {
                if (json[i].Equals('{'))
                {
                    startChar = '{';
                    endChar = '}';
                    break;
                }
                else if (json[i].Equals('['))
                {
                    startChar = '[';
                    endChar = ']';
                    break;
                }
            }

            int lastIndex = GetLastIndex(json, startChar, endChar);

            return json.Substring(0, lastIndex + 1);

        }

        /// <summary>
        /// 获取Json结尾字符的下标
        /// </summary>
        /// <param name="s"></param>
        /// <param name="startchar"></param>
        /// <param name="endChar"></param>
        /// <returns></returns>
        public static int GetLastIndex(string s, char startchar, char endChar)
        {
            Stack<char> stack = new Stack<char>();
            int lastIndex = 0;

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == startchar)
                {
                    stack.Push(s[i]);
                }
                else
                {
                    if (s[i].Equals(endChar))
                    {
                        stack.Pop();
                        if (stack.Count == 0)
                        {
                            lastIndex = i;
                            break;
                        }
                    }
                }
            }

            return lastIndex;
        }
    }
    
    public class HashTools
    {
        public static string GenerateMD5(string txt)
        {
            using (MD5 mi = MD5.Create())
            {
                byte[] buffer = Encoding.Default.GetBytes(txt);
                //开始加密
                byte[] newBuffer = mi.ComputeHash(buffer);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < newBuffer.Length; i++)
                {
                    sb.Append(newBuffer[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
    public class ModelTools
    {
        /// <summary>
        /// 法线反转
        /// </summary>
        public void InvertsModelNormals(Transform trans)
        {

            MeshFilter filter = trans.GetComponent<MeshFilter>();
            Vector3[] normals = trans.GetComponent<MeshFilter>().mesh.normals;
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = -normals[i];
            }
            trans.GetComponent<MeshFilter>().mesh.normals = normals;

            int[] triangles = trans.GetComponent<MeshFilter>().mesh.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int t = triangles[i];
                triangles[i] = triangles[i + 2];
                triangles[i + 2] = t;
            }
            trans.GetComponent<MeshFilter>().mesh.triangles = triangles;
        }
    }
    public class ScreenShotTool
    {
        public static Texture2D texture2D;
        public static RenderTexture render;
        //上传图片的压缩比例 单位： 倍
        public static int pictureRatio = 2;
        public static Texture2D scaleTexture;
        /// <summary>
        /// 截屏
        /// </summary>
        public static IEnumerator ScreenShoot(Action<Sprite> onOver = null, bool save = true)
        {
            Rect mRect = new Rect(0, 0, Screen.width, Screen.height);

            yield return new WaitForEndOfFrame();

            Texture2D texture2D = GetSreenPicture(mRect);

            //  byte[] bytes = texture2D.EncodeToPNG();
            if (onOver != null)
            {
                onOver.Invoke(TextureDispose.TextureToSprite(texture2D));
            }
            if (save)
            {
                string fileName = string.Format("{0}{1}{2}{3}{4}{5}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                //IoTools.Picture_IO.SaveImageToAlbum(texture2D, "JiePing", fileName);
            }


        }
        public static Texture2D GetSreenPicture(Rect mRect, bool isCompress = true)
        {
            render = new RenderTexture((int)mRect.width, (int)mRect.height, 0);
            Debug.Log(render);
            Camera.main.targetTexture = render;  //设置目标
            Camera.main.Render();  //开始
            RenderTexture.active = render;  //激活渲染贴图读取信息
            texture2D = new Texture2D((int)Screen.width, (int)Screen.height, TextureFormat.RGBA32, false);
            scaleTexture = new Texture2D((int)(Screen.width / pictureRatio), (int)(Screen.height / pictureRatio), TextureFormat.RGBA32, true);
            texture2D.ReadPixels(mRect, 0, 0);  //读取截屏信息并存储为纹理数据
            texture2D.Apply();

            var temp = texture2D;
            if (isCompress)
            {
                pictureRatio = 2;
                for (int i = 0; i < scaleTexture.height; i++)//压缩图片
                {
                    for (int j = 0; j < scaleTexture.width; j++)
                    {
                        Color color = texture2D.GetPixel(j * pictureRatio, i * pictureRatio);
                        scaleTexture.SetPixel(j, i, color);

                    }
                }
                scaleTexture.Apply();
                temp = scaleTexture;
            }
            else
            {
                pictureRatio = 1;
            }

            //重置相关参数，以使用camera继续在屏幕上显示
            Camera.main.targetTexture = null;
            RenderTexture.active = null;
            GameObject.Destroy(render);
            return temp;
        }




    }
}

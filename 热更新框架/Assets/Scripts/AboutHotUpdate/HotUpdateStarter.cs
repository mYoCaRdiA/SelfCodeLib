using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HotUpdateStarter : MonoBehaviour
{
    public bool ifCheckUpdate = false;

    public string urlHead = "http://127.0.0.1:637";

    public string mainScenePath; // ������ Addressable ��ַ
    public Text loadingText;


    bool isUpdating = false;
    float nowUpdatePercent = 0;
    float targetUpdatepercent = 100;

    private string HotUpdateDataPath
    {
        get
        {
            return Path.Combine(Application.persistentDataPath, "HotUpdateData", GetHotDataFolderName());
        }
    }

    public string HotUpdateDownLoadUrlHead
    {
        get
        {
            return urlHead + "/download/" + GetHotDataFolderName() + "/";
        }
    }


    // Start is called before the first frame update
    async void Start()
    {
        Debug.Log($"catalog_{Application.version}.hash");
        // Editor�����£�HotUpdate.dll.bytes�Ѿ����Զ����أ�����Ҫ���أ��ظ����ط���������⡣
#if !UNITY_EDITOR
        if (ifCheckUpdate)
        {
            isUpdating = await CheckHotUpdate();
            if (isUpdating)
            {
                return;
            }
        }
        Assembly hotUpdateAss = Assembly.Load(File.ReadAllBytes(GetTargetDllPath()));
#else

        // Editor��������أ�ֱ�Ӳ��һ��HotUpdate����
        Assembly hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotUpdate");
#endif
        //��ת����
        Addressables.LoadSceneAsync(mainScenePath, LoadSceneMode.Single).Completed += OnSceneLoaded;
    }


    private void Update()
    {
        if (isUpdating)
        {
            loadingText.gameObject.SetActive(true);
            nowUpdatePercent = Mathf.Lerp(nowUpdatePercent, targetUpdatepercent, 0.2f);
            loadingText.text = "������:" + nowUpdatePercent.ToString("0") + "%";
        }
        else
        {
            loadingText.gameObject.SetActive(false);
        }
    }

    // ����������ɺ�Ļص�
    private void OnSceneLoaded(AsyncOperationHandle<SceneInstance> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"���� {mainScenePath} ���سɹ���");
        }
        else
        {
            Debug.LogError($"���� {mainScenePath} ����ʧ�ܣ�");
        }
    }


    string GetTargetDllPath()
    {
        string dllTargetPath = Application.persistentDataPath;
        dllTargetPath = Path.Combine(HotUpdateDataPath, "HotUpdate.dll");
        Debug.Log(dllTargetPath);
        return dllTargetPath;
        //C:/Users/Administrator/AppData/LocalLow/DefaultCompany/HotUpdate/HotUpdateData/StandaloneWindows64/HotUpdate.dll
    }



    async Task<bool> CheckHotUpdate()
    {
        //����ȸ��ļ��в����ڻ���Ϊ�գ��ֻ����ļ������������ȸ���־�ļ���˵���ȸ����жϣ������������ȸ�����
        if (IsFolderEmpty(HotUpdateDataPath) || File.Exists(Path.Combine(HotUpdateDataPath, "Update.flag")))
        {
            StartCoroutine(FullUpdateRoutine());
            return true;
        }

        //����ȥ��˵���ȸ��������������п����Ǿɵ��ȸ����� 
        bool isDllOld = false;
        bool isBundleOld = false;

        //ȷ��dll�Ƿ���Ҫ����
        string nowDllHash = ReadLocalTxt("HotUpdate.hash");
        string serverDllHash = "";
        StartCoroutine(ReadNetTxt(HotUpdateDownLoadUrlHead + "HotUpdate.hash", (hash) =>
        {
            serverDllHash = hash;
        }));

        while (serverDllHash == "")
        {
            await Task.Delay(100);
        }

        if (nowDllHash != serverDllHash)
        {
            isDllOld = true;
        }
        //ȷ��boundle�Ƿ���Ҫ����
        string nowBoundleHash = ReadLocalTxt($"catalog_{Application.version}.hash");
        string serverBoundleHash = "";
        StartCoroutine(ReadNetTxt(HotUpdateDownLoadUrlHead + $"catalog_{Application.version}.hash", (hash) => { serverBoundleHash = hash; }));

        while (serverBoundleHash == "")
        {
            await Task.Delay(100);
        }

        if (nowBoundleHash != serverBoundleHash)
        {
            isBundleOld = true;
        }
        //���dll��bundle�������µģ�����Ҫ�ȸ�
        if (isDllOld == false && isBundleOld == false)
        {
            return false;
        }
        else//���߽���ѡ���Ը���
        {
            StartCoroutine(SelectUpdateRoutine(isDllOld, isBundleOld));
            return true;
        }




        IEnumerator FullUpdateRoutine()
        {
            DeleteAllFilesAndFolders(HotUpdateDataPath);
            CreateUpdateFlagFile(HotUpdateDataPath);

            //�����ȸ�����
            yield return DownloadFile(HotUpdateDownLoadUrlHead + "HotUpdate.dll", Path.Combine(HotUpdateDataPath, "HotUpdate.dll"));
            targetUpdatepercent = 20;
            yield return DownloadFile(HotUpdateDownLoadUrlHead + "HotUpdate.hash", Path.Combine(HotUpdateDataPath, "HotUpdate.hash"));
            targetUpdatepercent = 30;
            yield return DownloadFile(HotUpdateDownLoadUrlHead + $"catalog_{Application.version}.json", Path.Combine(HotUpdateDataPath, $"catalog_{Application.version}.json"));
            targetUpdatepercent = 75;
            List<string> boundleNames = GetAllBundleFileNamesByCatalogJson();
            foreach (var item in boundleNames)
            {
                yield return DownloadFile(HotUpdateDownLoadUrlHead + item, Path.Combine(HotUpdateDataPath, item));
            }
            targetUpdatepercent = 90;
            yield return DownloadFile(HotUpdateDownLoadUrlHead + $"catalog_{Application.version}.hash", Path.Combine(HotUpdateDataPath, $"catalog_{Application.version}.hash"));
            targetUpdatepercent = 99;
            File.Delete(Path.Combine(HotUpdateDataPath, "Update.flag"));

            targetUpdatepercent = 100;
            yield return new WaitForSeconds(0.5f);
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }


        IEnumerator SelectUpdateRoutine(bool dllUpdate, bool boundleUpdate)
        {
            CreateUpdateFlagFile(HotUpdateDataPath);
            if (dllUpdate)
            {
                yield return DownloadFile(HotUpdateDownLoadUrlHead + "HotUpdate.dll", Path.Combine(HotUpdateDataPath, "HotUpdate.dll"));
                targetUpdatepercent = 20;
                yield return DownloadFile(HotUpdateDownLoadUrlHead + "HotUpdate.hash", Path.Combine(HotUpdateDataPath, "HotUpdate.hash"));
                targetUpdatepercent = 25;
            }
            if (boundleUpdate)
            {
                yield return DownloadFile(HotUpdateDownLoadUrlHead + $"catalog_{Application.version}.json", Path.Combine(HotUpdateDataPath, $"catalog_{Application.version}.json"));
                targetUpdatepercent = 30;
                List<string> boundleNames = GetAllBundleFileNamesByCatalogJson();

                //Debug.Log(folderPath);
                string[] filePath = Directory.GetFiles(HotUpdateDataPath);
                List<string> existsBoundleName = new List<string>();
                foreach (var item in filePath)
                {
                    if (item.Contains(".bundle"))
                    {
                        string fileName = Path.GetFileName(item);
                        if (boundleNames.Contains(fileName) == false)
                        {
                            File.Delete(item);
                        }
                        else
                        {
                            existsBoundleName.Add(fileName);
                        }
                    }
                }
                targetUpdatepercent = 45;

                foreach (var item in boundleNames)
                {
                    if (!existsBoundleName.Contains(item))
                    {
                        existsBoundleName.Add(item);
                        //���ز����ڵ�boundle,���ظ�����ͬhash��boundle
                        yield return DownloadFile(HotUpdateDownLoadUrlHead + item, Path.Combine(HotUpdateDataPath, item));
                    }
                }
                targetUpdatepercent = 80;

                yield return DownloadFile(HotUpdateDownLoadUrlHead + $"catalog_{Application.version}.hash", Path.Combine(HotUpdateDataPath, $"catalog_{Application.version}.hash"));
                targetUpdatepercent = 85;
                File.Delete(Path.Combine(HotUpdateDataPath, "Update.flag"));
                targetUpdatepercent = 88;
            }

            targetUpdatepercent = 100;
            yield return new WaitForSeconds(0.5f);
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }

    }


    List<string> GetAllBundleFileNamesByCatalogJson()
    {
        List<string> fileNames = new List<string>();
        string jsonStr = ReadLocalTxt($"catalog_{Application.version}.json");
        Debug.Log(jsonStr);

        AAcatalogData catalogData = JsonConvert.DeserializeObject<AAcatalogData>(jsonStr);

        foreach (string item in catalogData.m_InternalIds)
        {
            if (item.Contains(".bundle"))
            {
                string targetName = Path.GetFileName(item);
                fileNames.Add(targetName);
            }
        }
        return fileNames;
    }

    string ReadLocalTxt(string fileName)
    {
        string filePath = Path.Combine(HotUpdateDataPath, fileName);
        if (File.Exists(filePath))
        {
            return File.ReadAllText(filePath);
        }
        else
        {
            Debug.LogError("����ʧ�ܣ������ļ�������: " + filePath);
            DeleteAllFilesAndFolders(HotUpdateDataPath);
            return string.Empty;
        }
    }

    bool IsFolderEmpty(string path)
    {
        // ���Ŀ¼�Ƿ����
        if (!Directory.Exists(path))
        {
            //���û���ȸ�Ŀ¼���򴴽�
            Directory.CreateDirectory(path);
        }

        // ��ȡĿ¼�������ļ�����Ŀ¼
        string[] files = Directory.GetFiles(path);
        string[] directories = Directory.GetDirectories(path);

        // ���û���ļ���û����Ŀ¼��˵���ļ���Ϊ��
        return files.Length == 0 && directories.Length == 0;
    }

    void CreateUpdateFlagFile(string path)
    {
        // ���ָ����Ŀ¼�Ƿ����
        if (!Directory.Exists(path))
        {
            Debug.LogError("Directory does not exist: " + path);
            return;
        }

        // ���� UpdateFlag �ļ�������·��
        string flagFilePath = Path.Combine(path, "Update.flag");

        // ����ļ��Ƿ��Ѵ���
        if (File.Exists(flagFilePath))
        {
            Debug.Log("UpdateFlag file already exists.");
        }
        else
        {
            try
            {
                // ����һ�����ļ�
                File.Create(flagFilePath).Dispose();
                Debug.Log("UpdateFlag file created successfully at: " + flagFilePath);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to create UpdateFlag file: " + ex.Message);
            }
        }
    }

    void DeleteAllFilesAndFolders(string path)
    {

        //�ȼ�鵱ǰƽ̨���ȸ�Ŀ¼�Ƿ����ļ��У�û���򴴽�
        if (!Directory.Exists(path))
        {
            //���û���ȸ�Ŀ¼���򴴽�
            Directory.CreateDirectory(path);
        }

        // ɾ���ļ����µ������ļ�
        string[] files = Directory.GetFiles(path);
        foreach (string file in files)
        {
            try
            {
                File.Delete(file);
                Debug.Log($"Deleted file: {file}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to delete {file}: {ex.Message}");
                CreateUpdateFlagFile(HotUpdateDataPath);
                Destroy(this);
            }
        }

        // ɾ���ļ����µ��������ļ���
        string[] directories = Directory.GetDirectories(path);
        foreach (string directory in directories)
        {
            try
            {
                DeleteAllFilesAndFolders(directory); // �ݹ�ɾ�����ļ��к��ļ�
                Directory.Delete(directory);
                Debug.Log($"Deleted folder: {directory}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to delete {directory}: {ex.Message}");
                CreateUpdateFlagFile(HotUpdateDataPath);
                Destroy(this);
            }
        }

        // ɾ�����ļ��б���
        //try
        //{
        //    Directory.Delete(path);
        //    Debug.Log($"Deleted folder: {path}");
        //}
        //catch (System.Exception ex)
        //{
        //    Debug.LogError($"Failed to delete folder {path}: {ex.Message}");
        //    CreateUpdateFlagFile(HotUpdateDataPath);
        //    Destroy(this);
        //}

    }

    // �����ļ������浽ָ���ļ���
    IEnumerator DownloadFile(string fileUrl, string saveFilePath)
    {
        // ����һ�� UnityWebRequest �������ļ�
        UnityWebRequest request = UnityWebRequest.Get(fileUrl);

        // �������󲢵ȴ���Ӧ
        yield return request.SendWebRequest();

        // ����Ƿ��д���
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("�����ļ�ʧ��: " + fileUrl +"------------------"+ request.error);
            Destroy(this);
        }
        else
        {
            // ȷ��Ŀ���ļ��д���
            string directory = Path.GetDirectoryName(saveFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // �����ص����ݱ��浽�����ļ�
            File.WriteAllBytes(saveFilePath, request.downloadHandler.data);
            Debug.Log("�ļ��ѱ���: " + saveFilePath);
        }
    }


    // �����ı��ļ�����ȡ����
    IEnumerator ReadNetTxt(string fileUrl, Action<string> onLoadOver = null)
    {
        // ���� UnityWebRequest ����ȡ�ı��ļ�
        UnityWebRequest request = UnityWebRequest.Get(fileUrl);

        // �������󲢵ȴ���Ӧ
        yield return request.SendWebRequest();

        // ��������Ƿ�ɹ�
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("�����ļ�ʧ��: " + fileUrl + "------------------" + request.error);
            Destroy(this);
        }
        else
        {
            // ��ȡ�ļ����ݣ���Ϊ�ı��ַ�����
            string fileContents = request.downloadHandler.text;

            onLoadOver?.Invoke(request.downloadHandler.text);
            // ����ļ����ݵ�����̨
            Debug.Log("�ļ�����:\n" + fileContents);
        }
    }


    /// <summary>
    /// ��ȡ����ʱ�Ĺ���Ŀ�����ơ�
    /// </summary>
    /// <returns>���� EditorUserBuildSettings.activeBuildTarget.ToString() ���ַ������ơ�</returns>
    public static string GetHotDataFolderName()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
                return "StandaloneWindows64";
            case RuntimePlatform.WindowsEditor:
                return "StandaloneWindows64";
            case RuntimePlatform.OSXPlayer:
                return "StandaloneOSX";
            case RuntimePlatform.OSXEditor:
                return "StandaloneOSX";
            case RuntimePlatform.LinuxPlayer:
                return "StandaloneLinux64";
            case RuntimePlatform.Android:
                return "Android";
            case RuntimePlatform.IPhonePlayer:
                return "iOS";
            case RuntimePlatform.WebGLPlayer:
                return "WebGL";
            case RuntimePlatform.PS4:
                return "PS4";
            case RuntimePlatform.XboxOne:
                return "XboxOne";
            case RuntimePlatform.Switch:
                return "Switch";
            case RuntimePlatform.tvOS:
                return "tvOS";
            case RuntimePlatform.Stadia:
                return "Stadia";
            case RuntimePlatform.CloudRendering:
                return "LinuxHeadlessSimulation";
            default:
                return "UnknownTarget";
        }
    }

}

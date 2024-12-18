using HybridCLR.Editor.Commands;
using System.IO;
using System;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

public class HotUpdateTool
{
    private static string serverUrlHead = "http://127.0.0.1:637";

    [MenuItem("��Դ����/��������", priority = 100)]
    public static async void UpdateAll()
    {
        await UpdateHotUpdateAAbundle();
        UpdateHotUpdateDll();
        Debug.Log("�����ȸ���Դ�������");
    }

    [MenuItem("��Դ����/�����ȸ�����", priority = 102)]
    public static void UpdateHotUpdateDll()
    {
        CompileDllCommand.CompileDllActiveBuildTarget();
        string dllFilePath = GetOringnalDllPath();
        string targetDllFilePath = GetTargetDllPath();
        bool copyOk = CopyFile(dllFilePath, targetDllFilePath);
        GenerateFileHash(targetDllFilePath, "HotUpdate.hash");
        Debug.Log("�ȸ�dll�������");

        string GetOringnalDllPath()
        {
            string dllPath = Application.dataPath;
            dllPath = dllPath.Replace("Assets", "");
            dllPath = dllPath + "HybridCLRData/HotUpdateDlls/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/HotUpdate.dll";
            Debug.Log(dllPath);

            return dllPath;

            //D:/UnityProjects/Unity_HotUpdate_AAHybirdCLR/HybridCLRData/HotUpdateDlls/StandaloneWindows64/HotUpdateDll.dll
        }


        string GetTargetDllPath()
        {
            //C:/Users/Administrator/AppData/LocalLow/DefaultCompany/HotUpdate
            //C:\Users\Administrator\AppData\LocalLow\DefaultCompany\HotUpdate\HotUpdateData\StandaloneWindows64
            string dllTargetPath = Application.persistentDataPath;
            dllTargetPath = dllTargetPath + "/HotUpdateData/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/HotUpdate.dll";
            Debug.Log(dllTargetPath);
            return dllTargetPath;
            //C:/Users/Administrator/AppData/LocalLow/DefaultCompany/HotUpdate/HotUpdateData/StandaloneWindows64/HotUpdateDll.dll
        }
    }

    [MenuItem("��Դ����/�����ȸ���Դ", priority = 101)]
    public static async Task UpdateHotUpdateAAbundle()
    {
        ResourceEditor.SetAllAllAAPrefabName();
        AddressablesPlayerBuildResult result = null;
        AddressableAssetSettings.BuildPlayerContent(out result);

        if (result != null && !string.IsNullOrEmpty(result.Error))
        {
            Debug.LogError($"Failed to build Addressables content, content not included in Player Build. \"{result.Error}\"");
        }
        else
        {
            List<string> usefulBundleNames = await GetAllBundleFileNamesByCatalogJson(GetCatalogPath());

            string folderPath = GetNowPlatformHotUpdateFolderPath();
            //Debug.Log(folderPath);
            string[] filePath = Directory.GetFiles(folderPath);

            if (usefulBundleNames != null)
            {
                foreach (var item in filePath)
                {
                    if (item.Contains(".bundle"))
                    {
                        string fileName = Path.GetFileName(item);
                        if (!usefulBundleNames.Contains(fileName))
                        {
                            File.Delete(item);
                        }
                    }
                }
            }

            Debug.Log("AA��Դ�������");

        }



        string GetCatalogPath()
        {
            string dllTargetPath = Application.persistentDataPath;
            dllTargetPath = dllTargetPath + "/HotUpdateData/" + EditorUserBuildSettings.activeBuildTarget.ToString() + $"/catalog_{Application.version}.json";
            return dllTargetPath;
        }
    }


    [MenuItem("��Դ����/�ϴ����е�������", priority = 103)]
    public async static void UpLoadAllToServer()
    {
        string[] filePath = Directory.GetFiles(GetNowPlatformHotUpdateFolderPath());

        for (int i = 0; i < filePath.Length; i++)
        {
            if (i == 0)
            {
                await UploadFileAsync(filePath[i], true);
            }
            else
            {
                await UploadFileAsync(filePath[i]);
            }
        }

        Debug.Log($"{EditorUserBuildSettings.activeBuildTarget.ToString()}ƽ̨�����ȸ���Դ�ϴ����");

        async Task UploadFileAsync(string filePath, bool clearFolder = false)
        {
            // ����ļ��Ƿ����
            if (!System.IO.File.Exists(filePath))
            {
                Debug.LogError("�ļ�������: " + filePath);
                return;
            }

            // ��ȡ�ļ�����
            byte[] fileData = System.IO.File.ReadAllBytes(filePath);

            string clearFolderTail = clearFolder ? "&clearFolder=true" : "";
            // �����ϴ�����
            UnityWebRequest request = new UnityWebRequest(GetUpLoadUrl() + clearFolderTail, UnityWebRequest.kHttpVerbPOST);

            // ��������߽�
            string boundary = "----UnityFormBoundary";
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            // ���������
            byte[] fileNameBytes = Encoding.UTF8.GetBytes($"Content-Disposition: form-data; name=\"file\"; filename=\"{Path.GetFileName(filePath)}\"\r\n");
            byte[] fileHeaderBytes = Encoding.UTF8.GetBytes("Content-Type: application/octet-stream\r\n\r\n");

            // ���������� multipart ����
            List<byte> formData = new List<byte>();
            formData.AddRange(boundaryBytes);
            formData.AddRange(fileNameBytes);
            formData.AddRange(fileHeaderBytes);
            formData.AddRange(fileData); // ����ļ�����
            formData.AddRange(Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n")); // �����߽�

            // �����ϴ�������
            request.uploadHandler = new UploadHandlerRaw(formData.ToArray());
            request.downloadHandler = new DownloadHandlerBuffer();

            // ���� Content-Type Ϊ multipart/form-data��������ļ��ֶ�
            request.SetRequestHeader("Content-Type", "multipart/form-data; boundary=" + boundary);

            // �������󲢵ȴ���Ӧ
            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield(); // �첽�ȴ����
            }

            // �����
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("�ļ��ϴ��ɹ�: " + filePath);
            }
            else
            {
                Debug.LogError("�ļ��ϴ�ʧ��: "+ filePath +"-------------"+ request.error);
            }
        }


    }







    private static string GetUpLoadUrl()
    {
        //private static string urlHead = "http://localhost:8080/upload/testFolder?password=123456";
        string url = serverUrlHead + "/upload/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "?password=123456";
        return url;
    }

    private static string GetDownUrl()
    {
        //private static string urlHead = "http://localhost:8080/upload/testFolder?password=123456";
        string url = serverUrlHead + "/download/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "?password=123456";
        return url;
    }


    private static string GetNowPlatformHotUpdateFolderPath()
    {
        // ��ȡĿ��Ŀ¼·��
        string hotUpdateFolderPath = Application.persistentDataPath;
        hotUpdateFolderPath = hotUpdateFolderPath + "/HotUpdateData/" + EditorUserBuildSettings.activeBuildTarget.ToString();

        // ���Ŀ¼�����ڣ��򴴽���Ŀ¼
        if (!Directory.Exists(hotUpdateFolderPath))
        {
            Directory.CreateDirectory(hotUpdateFolderPath);
        }

        // �����ļ���·��
        return hotUpdateFolderPath;
    }


    public static async Task<List<string>> GetAllBundleFileNamesByCatalogJson(string filePath)
    {
        try
        {
            List<string> fileNames = new List<string>();
            // ��ȡ JSON �ļ�
            var json = await File.ReadAllTextAsync(filePath);

            // ʹ�� System.Text.Json �����л� JSON
            var catalogData = JsonConvert.DeserializeObject<AAcatalogData>(json);

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
        catch (Exception ex)
        {
            Debug.LogError($"Error reading JSON: {ex.Message}");
            return null;
        }
    }


    /// <summary>
    /// �����ļ��ĺ�����
    /// </summary>
    /// <param name="sourceFilePath">Դ�ļ���·����</param>
    /// <param name="destinationFilePath">Ŀ���ļ���·����</param>
    /// <param name="overwrite">���Ŀ���ļ��Ѵ��ڣ��Ƿ񸲸ǡ�</param>
    /// <returns>�ļ����Ƴɹ����� true�����򷵻� false��</returns>
    public static bool CopyFile(string sourceFilePath, string destinationFilePath, bool overwrite = true)
    {
        try
        {
            // ���Դ�ļ��Ƿ����
            if (!File.Exists(sourceFilePath))
            {
                Debug.LogError($"Դ�ļ������ڣ�{sourceFilePath}");
                return false;
            }

            // ��鲢����Ŀ���ļ���
            string destinationDirectory = Path.GetDirectoryName(destinationFilePath);
            if (!Directory.Exists(destinationDirectory))
            {
                Debug.Log($"Ŀ���ļ��в����ڣ����ڴ�����{destinationDirectory}");
                Directory.CreateDirectory(destinationDirectory);
            }

            // ִ���ļ�����
            File.Copy(sourceFilePath, destinationFilePath, overwrite);
            Debug.Log($"�ļ����Ƴɹ����� {sourceFilePath} �� {destinationFilePath}");
            return true;
        }
        catch (UnauthorizedAccessException ex)
        {
            Debug.LogError($"���ʱ��ܾ���{ex.Message}");
        }
        catch (IOException ex)
        {
            Debug.LogError($"�ļ���������{ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"��������{ex.Message}");
        }

        return false;
    }


    /// <summary>
    /// ��ȡ�ļ��Ĺ�ϣֵ������ HotUpdateDll.hash �ļ���
    /// </summary>
    /// <param name="filePath">Ҫ�����ϣֵ���ļ�·����</param>
    /// <returns>�����ɹ����� true�����򷵻� false��</returns>
    public static bool GenerateFileHash(string filePath, string txtName)
    {
        try
        {
            // ����ļ��Ƿ����
            if (!File.Exists(filePath))
            {
                Debug.LogError($"�ļ������ڣ�{filePath}");
                return false;
            }

            // �����ļ��Ĺ�ϣֵ
            string hash = GetFileHash(filePath);

            // ��������ļ�·��
            string hashFilePath = Path.Combine(Path.GetDirectoryName(filePath), txtName);

            // ����ϣֵд���ļ�
            File.WriteAllText(hashFilePath, hash);

            Debug.Log($"�ļ���ϣֵ���ɳɹ���{hashFilePath}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"��������{ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// �����ļ��Ĺ�ϣֵ��SHA256����
    /// </summary>
    /// <param name="filePath">�ļ�·����</param>
    /// <returns>�ļ��Ĺ�ϣֵ��</returns>
    private static string GetFileHash(string filePath)
    {
        using (var sha256 = SHA256.Create())
        {
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hashBytes = sha256.ComputeHash(stream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }



}

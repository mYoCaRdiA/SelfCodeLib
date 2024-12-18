using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;



public class Test : MonoBehaviour
{

    public string prefabAddress; // Ԥ�Ƽ��� Addressable ��ַ
    private GameObject loadedPrefab; // ���ص�Ԥ�Ƽ�ʵ��


    // Start is called before the first frame update
    void Start()
    {
        Addressables.LoadAssetAsync<GameObject>(prefabAddress).Completed += OnPrefabLoaded;
    }

    // ���������ɵĻص�
    private void OnPrefabLoaded(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"Ԥ�Ƽ� {prefabAddress} ���سɹ���");

            // ʵ����Ԥ�Ƽ�
            loadedPrefab = Instantiate(obj.Result);

            loadedPrefab.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
        }
        else
        {
            Debug.LogError($"Ԥ�Ƽ� {prefabAddress} ����ʧ�ܣ�");
        }
    }
}

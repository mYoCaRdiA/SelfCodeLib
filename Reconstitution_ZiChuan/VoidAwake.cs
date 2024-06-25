using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidAwake : MonoBehaviour
{
    // Start is called before the first frame update
    [System.Obsolete]
    void Awake()
    {
        GameObject.Find("MainCanvas").transform.FindChild("ModelSelectPage").gameObject.SetActive(true);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;


public abstract class ArEngine
{
    public static LayerMask TrackMeshLayer = LayerMask.NameToLayer("TrackMesh");
    public static LayerMask GenerateModelLayer = LayerMask.NameToLayer("GenerateModel");
    public static MyArHitInfo ScreenRaycast(Vector2 screenPostion)
    {
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(screenPostion);
        if (Physics.Raycast(ray, out hitInfo))
        {
            MyArHitInfo myHitInfo = new MyArHitInfo(hitInfo.transform, hitInfo.point);
            Debug.Log("点击了:" + hitInfo.transform.name);
            return myHitInfo;
        }
        else
        {
            return null;
        }
    }

    public static MyArHitInfo ScreenRaycastTrackMesh(Vector2 screenPostion)
    {
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(screenPostion);
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << TrackMeshLayer))
        {
            MyArHitInfo myHitInfo = new MyArHitInfo(hitInfo.transform, hitInfo.point);
            Debug.Log("点击了TrackMesh:" + hitInfo.transform.name);
            return myHitInfo;
        }
        else
        {
            return null;
        }
    }

    public static MyArHitInfo ScreenRaycastGenerateModel(Vector2 screenPostion)
    {
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(screenPostion);

        RaycastHit[] allHit = Physics.RaycastAll(ray, Mathf.Infinity, 1 << GenerateModelLayer);
        if (allHit.Length != 0)
        {
            foreach (var item in allHit)
            {
                if (item.transform.GetComponent<MeshRenderer>().sharedMaterial.color.a != 0)
                {
                    hitInfo = item;
                    MyArHitInfo myHitInfo = new MyArHitInfo(hitInfo.transform, hitInfo.point);
                    //Debug.Log(LayerMask.LayerToName(hitInfo.transform.gameObject.layer));
                   // Debug.Log("点击了GenerateModel:" + hitInfo.transform.name);
                    return myHitInfo;
                }

            }

            return null;
        }
        else
        {
            return null;
        }
    }
    public static MyArHitInfo ScreenRaycastTransparent(Vector2 screenPostion)
    {
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(screenPostion);

        RaycastHit[] allHit = Physics.RaycastAll(ray, Mathf.Infinity, 1 << GenerateModelLayer);
        if (allHit.Length != 0)
        {
            foreach (var item in allHit)
            {
                
                    hitInfo = item;
                    MyArHitInfo myHitInfo = new MyArHitInfo(hitInfo.transform, hitInfo.point);
                    //Debug.Log(LayerMask.LayerToName(hitInfo.transform.gameObject.layer));
                    // Debug.Log("点击了GenerateModel:" + hitInfo.transform.name);
                    return myHitInfo;
                

            }

            return null;
        }
        else
        {
            return null;
        }
    }
    public static MyArHitInfo ScreenRaycastAllLayer(Vector2 screenPostion)
    {
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(screenPostion);
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, -1))
        {
            MyArHitInfo myHitInfo = new MyArHitInfo(hitInfo.transform, hitInfo.point);
            Debug.Log(LayerMask.LayerToName(hitInfo.transform.gameObject.layer));

            Debug.Log("点击了任意位置:" + hitInfo.transform.name);
            return myHitInfo;
        }
        else
        {
            return null;
        }
    }

    public abstract void SwitchLight(bool isReal);
    public virtual void RunPerFrame() { }

}

/// <summary>
/// 射线点击信息
/// </summary>
public class MyArHitInfo
{
    public Transform transform;
    public Vector3 pointPostion;

    public MyArHitInfo(Transform transform, Vector3 pointPostion)
    {
        this.transform = transform;
        this.pointPostion = pointPostion;
    }
    public MyArHitInfo()
    {
    }
}


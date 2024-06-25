using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ArEngineEmu : ArEngine
{
    Light nowLight;
    public override void RunPerFrame()
    {
        base.RunPerFrame();
    }

    public ArEngineEmu(ARSession arSession, Light light)
    {
        arSession.gameObject.AddComponent<FirstPersonMove>();

        //Debug.Log("虚拟环境:初始化现实场景");
       // Task task= GameObjectPoolTool.GetFromPoolForceAsync(true, realWorldEmuModel.path);
        //Debug.Log("虚拟环境:初始化ar光源");
        nowLight = light;
    }

    public override void SwitchLight(bool isReal)
    {
        nowLight.intensity=isReal? 0 : 0.36f; 
        Debug.Log("虚拟环境:切换光源" + isReal);
    }
}
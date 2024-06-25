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

        //Debug.Log("���⻷��:��ʼ����ʵ����");
       // Task task= GameObjectPoolTool.GetFromPoolForceAsync(true, realWorldEmuModel.path);
        //Debug.Log("���⻷��:��ʼ��ar��Դ");
        nowLight = light;
    }

    public override void SwitchLight(bool isReal)
    {
        nowLight.intensity=isReal? 0 : 0.36f; 
        Debug.Log("���⻷��:�л���Դ" + isReal);
    }
}
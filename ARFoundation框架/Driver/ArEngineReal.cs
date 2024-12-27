using UnityEngine.XR.ARFoundation;
using UnityEngine;


public class ArEngineReal : ArEngine
{
    //设备是否支持ARFoundation
    public bool CouldUse
    {
        get
        {
            if (ARSession.state != ARSessionState.None && ARSession.state != ARSessionState.CheckingAvailability && ARSession.state != ARSessionState.Unsupported)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public ARSessionOrigin arSessionOrigin;
    protected ARRaycastManager arRaycastManager;
    protected ARPlaneManager arPlaneManager;
    protected AROcclusionManager arOcclusionManager;
    protected ARCameraManager arCameraManager;

    private BasicLightEstimation basicLightEstimation;


    public override void SwitchLight(bool isReal)
    {
        basicLightEstimation.SwitchLight(isReal);
    }

    public ArEngineReal(ARSession arSession, Light light)
    {
        void GetComponentFromARSession<T>(ref T target)
        {
            target = arSession.transform.GetComponent<T>();
            if (target == null)
            {
                target = arSession.transform.GetComponentInChildren<T>();
            }
        }

        GetComponentFromARSession(ref arSessionOrigin);
        GetComponentFromARSession(ref arRaycastManager);
        GetComponentFromARSession(ref arPlaneManager);
        GetComponentFromARSession(ref arOcclusionManager);
        GetComponentFromARSession(ref arCameraManager);

        basicLightEstimation = light.transform.GetComponent<BasicLightEstimation>();
    
    }
}
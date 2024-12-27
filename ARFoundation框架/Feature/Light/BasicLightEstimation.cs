using UnityEngine.XR.ARFoundation;

using UnityEngine;

/// <summary>
/// A component that can be used to access the most recently received basic light estimation information
/// for the physical environment as observed by an AR device.
/// </summary>
[RequireComponent(typeof(Light))]
public class BasicLightEstimation : MonoBehaviour
{
    //[Tooltip("The ARCameraManager which will produce frame events containing light estimation information.")]
    public ARCameraManager cameraManager;

 

    /// <summary>
    /// The estimated brightness of the physical environment, if available.
    /// </summary>
    public float? brightness { get; private set; }

    /// <summary>
    /// The estimated color temperature of the physical environment, if available.
    /// </summary>
    public float? colorTemperature { get; private set; }

    /// <summary>
    /// The estimated color correction value of the physical environment, if available.
    /// </summary>
    public Color? colorCorrection { get; private set; }

    void Awake()
    {
        m_Light = GetComponent<Light>();
        oringnalColor = m_Light.color;
        oringnalColorTemperature = m_Light.colorTemperature;
        oringnalIntensity = m_Light.intensity;
        cameraManager.frameReceived += FrameChanged;
    }



    void FrameChanged(ARCameraFrameEventArgs args)
    {
        if (!showRealLight)
        {
            m_Light.intensity = oringnalIntensity;
            m_Light.colorTemperature = oringnalColorTemperature;
            m_Light.color = oringnalColor;
            return;
        }


        if (args.lightEstimation.averageBrightness.HasValue)
        {
            brightness = args.lightEstimation.averageBrightness.Value;
            m_Light.intensity = brightness.Value;
        }
        else
        {
            brightness = null;
        }

        if (args.lightEstimation.averageColorTemperature.HasValue)
        {
            colorTemperature = args.lightEstimation.averageColorTemperature.Value;
            m_Light.colorTemperature = colorTemperature.Value;
        }
        else
        {
            colorTemperature = null;
        }

        if (args.lightEstimation.colorCorrection.HasValue)
        {
            colorCorrection = args.lightEstimation.colorCorrection.Value;
            m_Light.color = colorCorrection.Value;
        }
        else
        {
            colorCorrection = null;
        }
    }


    public void SwitchLight(bool isReal)
    {
        showRealLight = isReal;
    }

    bool showRealLight = true;
    float oringnalIntensity;
    float oringnalColorTemperature;
    Color oringnalColor;
    Light m_Light;
}
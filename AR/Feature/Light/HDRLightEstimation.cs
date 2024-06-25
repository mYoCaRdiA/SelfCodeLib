using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// A component that can be used to access the most recently received HDR light estimation information
/// for the physical environment as observed by an AR device.
/// </summary>
[RequireComponent(typeof(Light))]
public class HDRLightEstimation : MonoBehaviour
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

    /// <summary>
    /// The estimated direction of the main light of the physical environment, if available.
    /// </summary>
    public Vector3? mainLightDirection { get; private set; }

    /// <summary>
    /// The estimated color of the main light of the physical environment, if available.
    /// </summary>
    public Color? mainLightColor { get; private set; }

    /// <summary>
    /// The estimated intensity in lumens of main light of the physical environment, if available.
    /// </summary>
    public float? mainLightIntensityLumens { get; private set; }

    /// <summary>
    /// The estimated spherical harmonics coefficients of the physical environment, if available.
    /// </summary>
    public SphericalHarmonicsL2? sphericalHarmonics { get; private set; }

    void Awake()
    {
        m_Light = GetComponent<Light>();
        oringnalColor = m_Light.color;
        oringnalColorTemperature = m_Light.colorTemperature;
        oringnalIntensity = m_Light.intensity;
        oringnalRotation = m_Light.transform.rotation;
        oringnalAmbientMode = RenderSettings.ambientMode;
        oringnalSphericalHarmonicsL2 = RenderSettings.ambientProbe;
        cameraManager.frameReceived += FrameChanged;
    }


    void FrameChanged(ARCameraFrameEventArgs args)
    {
        if (!showRealLight)
        {
            m_Light.intensity = oringnalIntensity;
            m_Light.colorTemperature = oringnalColorTemperature;
            m_Light.color = oringnalColor;
            m_Light.transform.rotation = oringnalRotation;
            RenderSettings.ambientMode = oringnalAmbientMode;
            RenderSettings.ambientProbe = oringnalSphericalHarmonicsL2;
            cameraManager.frameReceived += FrameChanged;
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

        if (args.lightEstimation.mainLightDirection.HasValue)
        {
            mainLightDirection = args.lightEstimation.mainLightDirection;
            m_Light.transform.rotation = Quaternion.LookRotation(mainLightDirection.Value);
            Debug.Log("变了：" + m_Light.transform.rotation);

        }
        if (args.lightEstimation.mainLightColor.HasValue)
        {
            mainLightColor = args.lightEstimation.mainLightColor;
            m_Light.color = mainLightColor.Value;
        }
        else
        {
            mainLightColor = null;
        }

        if (args.lightEstimation.mainLightIntensityLumens.HasValue)
        {
            mainLightIntensityLumens = args.lightEstimation.mainLightIntensityLumens;
            m_Light.intensity = args.lightEstimation.averageMainLightBrightness.Value;
        }
        else
        {
            mainLightIntensityLumens = null;
        }

        if (args.lightEstimation.ambientSphericalHarmonics.HasValue)
        {
            sphericalHarmonics = args.lightEstimation.ambientSphericalHarmonics;
            RenderSettings.ambientMode = AmbientMode.Skybox;
            RenderSettings.ambientProbe = sphericalHarmonics.Value;
        }
        else
        {
            sphericalHarmonics = null;
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
    Quaternion oringnalRotation;
    AmbientMode oringnalAmbientMode;
    SphericalHarmonicsL2 oringnalSphericalHarmonicsL2;

    //     RenderSettings.ambientProbe = sphericalHarmonics.Value;

    Light m_Light;
}

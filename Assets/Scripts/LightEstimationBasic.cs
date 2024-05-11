using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;

public class LightEstimationBasic : MonoBehaviour
{
    [SerializeField] private ARCameraManager _cameraManager;
    private Light _mainLight;
     
    public float? Brightness { get; private set; }
    public float? ColorTemperature { get; private set; }
    public Color? ColorCorrection { get; private set; }
    public Vector3? MainLightDirection { get; private set; }
    public Color? MainLightColor { get; private set; }
    public float? MainLightIntensityLumens { get; private set; }
    public SphericalHarmonicsL2? SphericalHarmonics { get; private set; }

    private void Awake()
    {
        _mainLight = GetComponent<Light>();
    }

    private void OnEnable()
    {
        _cameraManager.frameReceived += FrameChanged;
    }

    private void OnDisable()
    {
        _cameraManager.frameReceived -= FrameChanged;
    }

    private void FrameChanged(ARCameraFrameEventArgs args)
    {
        if (args.lightEstimation.averageBrightness.HasValue)
        {
            Brightness = args.lightEstimation.averageBrightness.Value;
            _mainLight.intensity = Brightness.Value;
        }

        if (args.lightEstimation.averageColorTemperature.HasValue)
        {
            ColorTemperature = args.lightEstimation.averageColorTemperature.Value;
            _mainLight.colorTemperature = ColorTemperature.Value;
        }

        if (args.lightEstimation.colorCorrection.HasValue)
        {
            ColorCorrection = args.lightEstimation.colorCorrection.Value;
            _mainLight.color = ColorCorrection.Value;
        }

        if (args.lightEstimation.mainLightDirection.HasValue)
        {
            MainLightDirection = args.lightEstimation.mainLightDirection;
            _mainLight.transform.rotation = Quaternion.LookRotation(MainLightDirection.Value);
        }

        if (args.lightEstimation.mainLightColor.HasValue)
        {
            MainLightColor = args.lightEstimation.mainLightColor;

            // ARCore needs to apply energy conservation term (1 / PI) and be placed in gamma
            _mainLight.color = MainLightColor.Value / Mathf.PI;
            _mainLight.color = _mainLight.color.gamma;
        }

        if (args.lightEstimation.mainLightIntensityLumens.HasValue)
        {
            MainLightIntensityLumens = args.lightEstimation.mainLightIntensityLumens;
            _mainLight.intensity = args.lightEstimation.averageMainLightBrightness.Value;
        }

        if (args.lightEstimation.ambientSphericalHarmonics.HasValue)
        {
            SphericalHarmonics = args.lightEstimation.ambientSphericalHarmonics;
            RenderSettings.ambientMode = AmbientMode.Skybox;
            RenderSettings.ambientProbe = SphericalHarmonics.Value;
        }
    }
}

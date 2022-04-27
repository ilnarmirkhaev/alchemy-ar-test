using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;

namespace AlchemyAR.AR
{
    [RequireComponent(typeof(Light))]
    public class LightEstimation : MonoBehaviour
    {
        [SerializeField] private ARCameraManager arCameraManager;
     
        private Light _light;
    
        private void Awake()
        {
            _light = GetComponent<Light>();
        }

        private void Start()
        {
            arCameraManager.frameReceived += FrameReceived;
        }

        private void FrameReceived(ARCameraFrameEventArgs args)
        {
            var lightEstimation = args.lightEstimation;

            if (lightEstimation.averageBrightness.HasValue)
                _light.intensity = lightEstimation.averageBrightness.Value;

            if (lightEstimation.averageColorTemperature.HasValue)
                _light.colorTemperature = lightEstimation.averageColorTemperature.Value;

            if (lightEstimation.colorCorrection.HasValue)
                _light.color = lightEstimation.colorCorrection.Value;

            if (lightEstimation.mainLightDirection.HasValue)
                _light.transform.rotation = Quaternion.LookRotation(lightEstimation.mainLightDirection.Value);

            if (lightEstimation.mainLightColor.HasValue)
                _light.color = lightEstimation.mainLightColor.Value;

            if (lightEstimation.averageMainLightBrightness.HasValue)
                _light.intensity = lightEstimation.averageMainLightBrightness.Value;

            if (lightEstimation.ambientSphericalHarmonics.HasValue)
            {
                RenderSettings.ambientMode = AmbientMode.Skybox;
                RenderSettings.ambientProbe = lightEstimation.ambientSphericalHarmonics.Value;
            }
        }
    }
}

#if HDPipeline
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace ProceduralWorlds.HDRPTOD
{
    [ExecuteAlways]
    public class HDRPTimeOfDayAdditionalProbe : MonoBehaviour
    {
        public HDRPTimeOfDayReflectionProbeProfile Profile
        {
            get { return m_profile; }
            set
            {
                if (m_profile != value)
                {
                    m_profile = value;
                }
            }
        }
        [SerializeField] private HDRPTimeOfDayReflectionProbeProfile m_profile;

        [SerializeField] private HDAdditionalReflectionData m_globalHDProbeData;
        public ReflectionProbeTODData m_currentData;
        public ReflectionProbe m_globalProbe;
        public float m_globalMultiplier = 1f;
        public bool m_allowInRayTracing = false;

        public void OnEnable()
        {
            if (m_globalProbe == null)
            {
                m_globalProbe = GetComponent<ReflectionProbe>();
                if (m_globalProbe == null)
                {
                    m_globalProbe = gameObject.AddComponent<ReflectionProbe>();
                    m_globalHDProbeData = GetComponent<HDAdditionalReflectionData>();
                    if (m_globalHDProbeData == null)
                    {
                        m_globalHDProbeData = gameObject.AddComponent<HDAdditionalReflectionData>();
                    }
                }
            }

            if (m_globalProbe != null)
            {
                m_globalHDProbeData = GetComponent<HDAdditionalReflectionData>();
                m_globalProbe.enabled = enabled;
                UpdateProbeSystem(HDRPTimeOfDayAPI.RayTracingSSGIActive());
                if (HDRPTimeOfDay.Instance != null)
                {
                    HDRPTimeOfDay.Instance.AddAdditionalProbe(this);
                }
            }
        }
        private void Start()
        {
            OnDisable();
            OnEnable();
        }
        public void OnDisable()
        {
            if (m_globalProbe != null)
            {
                m_globalProbe.enabled = false;
                if (HDRPTimeOfDay.Instance != null)
                {
                    HDRPTimeOfDay.Instance.RemoveAdditionalProbe(this);
                }
            }
        }
        /// <summary>
        /// Refreshes the probe system
        /// </summary>
        public void Refresh(bool rayTracing)
        {
            UpdateProbeSystem(rayTracing);
        }
        /// <summary>
        /// Disables and removes the probes from the system
        /// </summary>
        public void DisableProbe()
        {
            if (m_globalProbe != null)
            {
                m_globalProbe.enabled = false;
                m_globalProbe.enabled = false;
            }
        }
        /// <summary>
        /// Enables the probe and adds it to the system
        /// </summary>
        public void EnableProbe()
        {
            if (m_globalProbe != null)
            {
                m_globalProbe.enabled = true;
                m_globalProbe.enabled = true;
                m_globalHDProbeData = GetComponent<HDAdditionalReflectionData>();
                m_globalProbe.enabled = enabled;
                UpdateProbeSystem(HDRPTimeOfDayAPI.RayTracingSSGIActive());
            }
        }
        /// <summary>
        /// Sets the box size
        /// </summary>
        /// <param name="size"></param>
        public void SetProbeSize(Vector3 size)
        {
            if (m_globalHDProbeData != null)
            {
                m_globalHDProbeData.settings.influence.boxSize = size;
            }
        }
        /// <summary>
        /// Sets the sphere size
        /// </summary>
        /// <param name="size"></param>
        public void SetProbeSize(float size)
        {
            if (m_globalHDProbeData != null)
            {
                m_globalHDProbeData.settings.influence.sphereRadius = size;
            }
        }

        /// <summary>
        /// Applies new probe data to the global probe
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool SetNewData(ReflectionProbeTODData data, bool rayTracing)
        {
            if (data.Validate(HDRPTimeOfDay.Instance, Profile.m_probeTimeMode))
            {
                m_currentData = data;
                if (Profile.m_probeTimeMode == ProbeTimeMode.TimeTransition)
                {
                    if (data.m_enableSSGIMultiplier)
                    {
                        if (HDRPTimeOfDay.Instance.IsSSGIEnabled())
                        {
                            if (m_allowInRayTracing && rayTracing)
                            {
                                m_globalHDProbeData.settingsRaw.lighting.multiplier = (Profile.m_transitionIntensityCurveRT.Evaluate((HDRPTimeOfDay.Instance.ConvertTimeOfDay()) * m_globalMultiplier));
                            }
                            else
                            {
                                m_globalHDProbeData.settingsRaw.lighting.multiplier = (Profile.m_transitionIntensityCurve.Evaluate((HDRPTimeOfDay.Instance.ConvertTimeOfDay()) * GetMultiplier(data)) * (m_globalMultiplier * data.m_ssgiMultiplier));
                            }
                        }
                        else
                        {
                            m_globalHDProbeData.settingsRaw.lighting.multiplier = (Profile.m_transitionIntensityCurve.Evaluate(HDRPTimeOfDay.Instance.ConvertTimeOfDay()) * GetMultiplier(data)) * m_globalMultiplier;
                        }
                    }
                    else
                    {
                        m_globalHDProbeData.settingsRaw.lighting.multiplier = (Profile.m_transitionIntensityCurve.Evaluate(HDRPTimeOfDay.Instance.ConvertTimeOfDay()) * GetMultiplier(data)) * m_globalMultiplier;
                    }
                }
                else
                {
                    m_globalHDProbeData.settingsRaw.lighting.multiplier = (data.m_intensity * GetMultiplier(data)) * m_globalMultiplier;
                }
                m_globalHDProbeData.settingsRaw.cameraSettings.probeLayerMask = data.m_renderLayers;
                m_globalHDProbeData.settingsRaw.cameraSettings.culling.cullingMask = data.m_renderLayers;
                m_globalHDProbeData.SetTexture(ProbeSettings.Mode.Custom, data.m_probeCubeMap);
                return true;
            }

            return false;
        }
        /// <summary>
        /// Updates the probe systems
        /// </summary>
        private void UpdateProbeSystem(bool rayTracing)
        {
            if (m_profile == null || m_globalProbe == null)
            {
                if (m_globalProbe == null)
                {
                    m_globalProbe = GetComponent<ReflectionProbe>();
                }
                return;
            }

            bool isEnabled = m_profile.m_renderMode == ProbeRenderMode.Sky;
              if (rayTracing && !m_allowInRayTracing)
            {
                isEnabled = false;
            }
            m_globalProbe.enabled = isEnabled;

            if (isEnabled && CanProcess())
            {
                foreach (ReflectionProbeTODData data in m_profile.m_probeTODData)
                {
                    if (SetNewData(data, rayTracing))
                    {
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// Gets the active weather probe data weather intensity multiplier
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private float GetMultiplier(ReflectionProbeTODData data)
        {
            if (HDRPTimeOfDay.Instance != null)
            {
                if (HDRPTimeOfDay.Instance.WeatherActive())
                {
                    int step = HDRPTimeOfDay.Instance.HasWeatherTransitionCompletedSteps(out bool isPlaying);
                    //Treansition to multiplier if isPlaying is true, transition back to 1 if it's not
                    if (isPlaying)
                    {
                        switch (step)
                        {
                            case 0:
                            {
                                return Mathf.Lerp(1f, data.m_weatherIntensityMultiplier, 0.1f);
                            }
                            case 1:
                            {
                                return Mathf.Lerp(1f, data.m_weatherIntensityMultiplier, 0.2f);
                            }
                            case 2:
                            {
                                return Mathf.Lerp(1f, data.m_weatherIntensityMultiplier, 0.3f);
                            }
                            case 3:
                            {
                                return Mathf.Lerp(1f, data.m_weatherIntensityMultiplier, 0.4f);
                            }
                            case 4:
                            {
                                return Mathf.Lerp(1f, data.m_weatherIntensityMultiplier, 0.5f);
                            }
                            case 5:
                            {
                                return Mathf.Lerp(1f, data.m_weatherIntensityMultiplier, 0.6f);
                            }
                            case 6:
                            {
                                return Mathf.Lerp(1f, data.m_weatherIntensityMultiplier, 0.7f);
                            }
                            case 7:
                            {
                                return Mathf.Lerp(1f, data.m_weatherIntensityMultiplier, 0.8f);
                            }
                            case 8:
                            {
                                return Mathf.Lerp(1f, data.m_weatherIntensityMultiplier, 0.9f);
                            }
                            default:
                            {
                                return Mathf.Lerp(1f, data.m_weatherIntensityMultiplier, 1f);
                            }
                        }
                    }
                    else
                    {
                        switch (step)
                        {
                            case 0:
                            {
                                return Mathf.Lerp(data.m_weatherIntensityMultiplier, 1f, 0.1f);
                            }
                            case 1:
                            {
                                return Mathf.Lerp(data.m_weatherIntensityMultiplier, 1f, 0.2f);
                            }
                            case 2:
                            {
                                return Mathf.Lerp(data.m_weatherIntensityMultiplier, 1f, 0.3f);
                            }
                            case 3:
                            {
                                return Mathf.Lerp(data.m_weatherIntensityMultiplier, 1f, 0.4f);
                            }
                            case 4:
                            {
                                return Mathf.Lerp(data.m_weatherIntensityMultiplier, 1f, 0.5f);
                            }
                            case 5:
                            {
                                return Mathf.Lerp(data.m_weatherIntensityMultiplier, 1f, 0.6f);
                            }
                            case 6:
                            {
                                return Mathf.Lerp(data.m_weatherIntensityMultiplier, 1f, 0.7f);
                            }
                            case 7:
                            {
                                return Mathf.Lerp(data.m_weatherIntensityMultiplier, 1f, 0.8f);
                            }
                            case 8:
                            {
                                return Mathf.Lerp(data.m_weatherIntensityMultiplier, 1f, 0.9f);
                            }
                            default:
                            {
                                return Mathf.Lerp(data.m_weatherIntensityMultiplier, 1f, 1f);
                            }
                        }
                    }
                }
            }
            return 1f;
        }

        /// <summary>
        /// Checks to see if it can be processed
        /// </summary>
        /// <returns></returns>
        private bool CanProcess()
        {
            if (m_profile.m_renderMode != ProbeRenderMode.Sky)
            {
                return false;
            }

            if (m_globalProbe == null)
            {
                m_globalProbe = GetComponent<ReflectionProbe>();
                if (m_globalProbe == null)
                {
                    return false;
                }
            }

            if (m_globalHDProbeData == null)
            {
                if (m_globalProbe != null)
                {
                    m_globalHDProbeData = m_globalProbe.GetComponent<HDAdditionalReflectionData>();
                    if (m_globalHDProbeData == null)
                    {
                        m_globalHDProbeData = m_globalProbe.gameObject.AddComponent<HDAdditionalReflectionData>();
                    }
                }
            }

            return true;
        }
    }
}
#endif
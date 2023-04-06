using UnityEngine;
#if HDPipeline
using UnityEngine.Rendering.HighDefinition;
#endif

namespace ProceduralWorlds.HDRPTOD
{
    [ExecuteAlways]
    public class HDRPTimeOfDayReflectionProbeManager : MonoBehaviour
    {
        public static HDRPTimeOfDayReflectionProbeManager Instance
        {
            get { return m_instance; }
        }
        [SerializeField] private static HDRPTimeOfDayReflectionProbeManager m_instance;

        public float RenderDistance
        {
            get { return m_renderDistance; }
            set
            {
                if (m_renderDistance != value)
                {
                    m_renderDistance = value;
                    if (m_profile != null)
                    {
                        m_profile.m_renderDistance = value;
                    }
#if HDPipeline && UNITY_2021_2_OR_NEWER
                    m_globalHDProbeData.settingsRaw.influence.boxSize = new Vector3(value, value, value);
#endif
                }
            }
        }
        [SerializeField] private float m_renderDistance = 5000f;

        public HDRPTimeOfDayReflectionProbeProfile Profile
        {
            get { return m_profile; }
            set
            {
                if (m_profile != value)
                {
                    m_profile = value;
                    RenderDistance = value.m_renderDistance;
                }
            }
        }
        [SerializeField] private HDRPTimeOfDayReflectionProbeProfile m_profile;

        public Transform m_playerCamera;
        public ReflectionProbe m_globalProbe;
        public ReflectionProbeTODData m_currentData;
        public float m_globalMultiplier = 1f;
        public bool m_allowInRayTracing = false;
        private float m_currentTransitionValue = 0f;

#if HDPipeline && UNITY_2021_2_OR_NEWER
        [SerializeField]
        private HDAdditionalReflectionData m_globalHDProbeData;

        #region Unity Functions

        private void OnEnable()
        {
            m_instance = this;
            if (m_globalProbe != null)
            {
                m_globalProbe.enabled = enabled;
            }

            if (m_playerCamera == null)
            {
                m_playerCamera = HDRPTimeOfDayAPI.GetCamera();
            }

            if (HDRPTimeOfDay.Instance != null)
            {
                UpdateProbeSystem(HDRPTimeOfDayAPI.RayTracingSSGIActive());
            }
        }
        private void OnDisable()
        {
            if (m_globalProbe != null)
            {
                m_globalProbe.enabled = false;
            }
        }
        private void LateUpdate()
        {
            FollowPlayer();
        }

        #endregion
        #region Public Functions

        /// <summary>
        /// Gets the transition value
        /// </summary>
        /// <returns></returns>
        public float GetCurrentTransitionCurveValue()
        {
            return m_currentTransitionValue;
        }
        /// <summary>
        /// Refreshes the probe system
        /// </summary>
        public void Refresh(bool rayTracing)
        {
            UpdateProbeSystem(rayTracing);
        }

        #endregion
        #region Private Functions

        /// <summary>
        /// Applies new probe data to the global probe
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool SetNewData(ReflectionProbeTODData data, bool rayTracing)
        {
            HDRPTimeOfDay tod = HDRPTimeOfDay.Instance;
            if (data.Validate(tod, Profile.m_probeTimeMode))
            {
                m_currentData = data;
                if (Profile.m_probeTimeMode == ProbeTimeMode.TimeTransition)
                {
                    if (data.m_enableSSGIMultiplier)
                    {
                        if (tod.IsSSGIEnabled() && !tod.WeatherActive())
                        {
                            if (m_allowInRayTracing && rayTracing)
                            {
                                m_globalHDProbeData.settingsRaw.lighting.multiplier = (Profile.m_transitionIntensityCurveRT.Evaluate((tod.ConvertTimeOfDay()) * m_globalMultiplier));
                            }
                            else
                            {
                                m_globalHDProbeData.settingsRaw.lighting.multiplier = (Profile.m_transitionIntensityCurve.Evaluate((tod.ConvertTimeOfDay()) * GetMultiplier(data)) * (m_globalMultiplier * data.m_ssgiMultiplier));
                            }
                        }
                        else
                        {
                            m_globalHDProbeData.settingsRaw.lighting.multiplier = (Profile.m_transitionIntensityCurve.Evaluate(tod.ConvertTimeOfDay()) * GetMultiplier(data)) * m_globalMultiplier;
                        }
                    }
                    else
                    {
                        m_globalHDProbeData.settingsRaw.lighting.multiplier = (Profile.m_transitionIntensityCurve.Evaluate(tod.ConvertTimeOfDay()) * GetMultiplier(data)) * m_globalMultiplier;
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
            if (m_profile == null)
            {
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
        /// Moves the probe position to the player position
        /// </summary>
        private void FollowPlayer()
        {
            if (m_profile.m_followPlayer)
            {
                if (m_playerCamera != null && m_globalProbe != null)
                {
                    Vector3 playerPos = m_playerCamera.position;
                    playerPos.y = GetYPlayerPosition();
                    m_globalProbe.transform.position = playerPos;
                }
            }
        }
        /// <summary>
        /// Gets the y for the follow position
        /// </summary>
        /// <returns></returns>
        private float GetYPlayerPosition()
        {
            float value = m_globalHDProbeData.settings.influence.boxSize.y / 2f;
            HDRPTimeOfDay timeOfDay = HDRPTimeOfDay.Instance;
            if (timeOfDay != null)
            {
                float seaLevel = timeOfDay.TimeOfDayProfile.UnderwaterOverrideData.m_seaLevel;
                if (seaLevel >= 0f)
                {
                    value += seaLevel * 4f;
                }
                else
                {
                    value -= Mathf.Abs(seaLevel * 4f);
                }
            }

            if (m_profile != null)
            {
                if (m_profile.m_seaLevelOffset >= 0f)
                {
                    value += m_profile.m_seaLevelOffset;
                }
                else
                {
                    value -= Mathf.Abs(m_profile.m_seaLevelOffset);
                }
            }

            return value;
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

        #endregion
#endif
    }
}
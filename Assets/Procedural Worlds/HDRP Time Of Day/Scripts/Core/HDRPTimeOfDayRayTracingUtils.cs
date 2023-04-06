#if HDPipeline
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace ProceduralWorlds.HDRPTOD
{
    /// <summary>
    /// RTX optimize data class that holds the settings
    /// This has the function to process the mesh renderer
    /// </summary>
    [Serializable]
    public class RayTracingOptimizationData
    {
        public bool m_ignoreIfAnimatorFound = true;
        public bool m_setStaticShadowCasting = true;
        public bool m_setObjectLayer = false;
        public bool m_overrideStatic = true;
        public LayerMask m_rayTracedLayer = -1;
        public string m_nonRayTracedLayer = "RTX Ignore";
        public float m_nonRayTracedObjectScale = 1f;
        public UnityEngine.Experimental.Rendering.RayTracingMode m_staticRayTracingMode = UnityEngine.Experimental.Rendering.RayTracingMode.Static;
        public UnityEngine.Experimental.Rendering.RayTracingMode m_dynamicRayTracingMode = UnityEngine.Experimental.Rendering.RayTracingMode.DynamicTransform;
        public LightProbeUsage m_lightProbes = LightProbeUsage.BlendProbes;
        public MotionVectorGenerationMode m_motionVectors = MotionVectorGenerationMode.Camera;
        public bool m_dynamicOcclusion = true;

        public RayTracingOptimizationData(bool defaults)
        {
            if (defaults)
            {
                m_ignoreIfAnimatorFound = true;
                m_setStaticShadowCasting = true;
                m_setObjectLayer = true;
                m_overrideStatic = true;
                m_rayTracedLayer = -1;
                m_nonRayTracedLayer = "RTX Ignore";
                m_nonRayTracedObjectScale = 1.5f;
                m_staticRayTracingMode = UnityEngine.Experimental.Rendering.RayTracingMode.Static;
                m_dynamicRayTracingMode = UnityEngine.Experimental.Rendering.RayTracingMode.DynamicTransform;
                m_lightProbes = LightProbeUsage.BlendProbes;
                m_motionVectors = MotionVectorGenerationMode.Camera;
                m_dynamicOcclusion = true;
            }
        }

        public void ProcessMesh(MeshRenderer renderer)
        {
            if (renderer != null)
            {
                //Size check
                Vector3 size = renderer.bounds.size;
                float largestSizeValue = size.x;
                if (size.y > largestSizeValue)
                {
                    largestSizeValue = size.y;
                }
                else if (size.z > largestSizeValue)
                {
                    largestSizeValue = size.z;
                }

                renderer.lightProbeUsage = m_lightProbes;
                renderer.allowOcclusionWhenDynamic = m_dynamicOcclusion;

                bool isStatic = renderer.gameObject.isStatic;
                renderer.rayTracingMode = isStatic ? m_staticRayTracingMode : m_dynamicRayTracingMode;
                if (m_overrideStatic || isStatic)
                {
                    renderer.rayTracingMode = m_staticRayTracingMode;
                    renderer.motionVectorGenerationMode = MotionVectorGenerationMode.Camera;
                }
                else
                {
                    renderer.motionVectorGenerationMode = m_motionVectors;
                }

                bool isTooSmall = largestSizeValue < m_nonRayTracedObjectScale;
                if (m_setObjectLayer)
                {
                    if (isTooSmall)
                    {
                        renderer.gameObject.layer = LayerMask.NameToLayer(m_nonRayTracedLayer);
                    }
                }
                if (isTooSmall)
                {
                    renderer.rayTracingMode = UnityEngine.Experimental.Rendering.RayTracingMode.Off;
                }
                if (m_setStaticShadowCasting)
                {
                    renderer.staticShadowCaster = isStatic;
                }
            }
        }
    }

    public class HDRPTimeOfDayRayTracingUtils
    { 
        /// <summary>
        /// Applies ray tracing settings
        /// </summary>
        /// <param name="isSetup"></param>
        /// <param name="components"></param>
        /// <param name="settings"></param>
        public static void ApplyRayTracingSettings(HDRPTimeOfDayComponents components, RayTraceSettings settings, HDRPTimeOfDay timeOfDay, RayCastingMode rtxMode = RayCastingMode.Mixed)
        {
            if (IsRayTracingSetup())
            {
#if RAY_TRACING_ENABLED
                if (!settings.m_renderInEditMode && !Application.isPlaying)
                {
                    //SSGI
                    components.m_rayTracedGlobalIllumination.active = false;
                    //SSR
                    components.m_rayTracedScreenSpaceReflection.active = false;
                    //AO
                    components.m_rayTracedAmbientOcclusion.active = false;
                    //Recursive Rendering
                    components.m_rayTracedRecursiveRendering.active = false;
                    //SSS
                    components.m_rayTracedSubSurfaceScattering.active = false;
                    //Shadows
                    //Sun
                    components.m_sunLightData.useScreenSpaceShadows = false;
                    components.m_sunLightData.useRayTracedShadows = false;
                    components.m_sunLightData.rayTraceContactShadow = false;
                    //Moon
                    components.m_moonLightData.useScreenSpaceShadows = false;
                    components.m_moonLightData.useRayTracedShadows = false;
                    components.m_moonLightData.rayTraceContactShadow = false;
                    return;
                }

                //SSGI
                components.m_rayTracedGlobalIllumination.active = settings.m_rayTraceSSGI;
                components.m_rayTracedGlobalIllumination.tracing.value = rtxMode;
#if UNITY_2022_2_OR_NEWER
                components.m_rayTracedGlobalIllumination.ambientProbeDimmer.value = settings.m_ssgiAmbientAmount;
#endif
                components.m_rayTracedGlobalIllumination.mode.value = RayTracingMode.Performance;
                if (!settings.m_overrideQuality)
                {
                    settings.RTGlobalQualitySettings.RTSSGIQuality[(int)settings.m_ssgiQuality].Apply(components.m_rayTracedGlobalIllumination, true);
                }

                //SSR
                components.m_rayTracedScreenSpaceReflection.active = settings.m_rayTraceSSR;
                components.m_rayTracedScreenSpaceReflection.tracing.value = rtxMode;
#if UNITY_2022_2_OR_NEWER
                components.m_rayTracedScreenSpaceReflection.ambientProbeDimmer.value = settings.m_ssrAmbientAmount;
#endif
                components.m_rayTracedScreenSpaceReflection.mode.value = RayTracingMode.Performance;
                if (!settings.m_overrideQuality)
                {
                    settings.RTGlobalQualitySettings.RTSSRQuality[(int)settings.m_ssrQuality].Apply(components.m_rayTracedScreenSpaceReflection, true);
                }
                
                //AO
                components.m_rayTracedAmbientOcclusion.active = settings.m_rayTraceAmbientOcclusion;
                if (!settings.m_overrideQuality)
                {
                    settings.RTGlobalQualitySettings.RTAOQuality[(int)settings.m_aoQuality].Apply(components.m_rayTracedAmbientOcclusion);
                }

                //Recursive Rendering
                components.m_rayTracedRecursiveRendering.active = settings.m_recursiveRendering;
                //SSS
                components.m_rayTracedSubSurfaceScattering.active = settings.m_rayTraceSubSurfaceScattering;
                components.m_rayTracedSubSurfaceScattering.sampleCount.value = (int)settings.m_subSurfaceScatteringSampleCount;

                if (!settings.m_rayTraceSSGI && !settings.m_rayTraceSSR && !settings.m_recursiveRendering &&
                    !settings.m_rayTraceSubSurfaceScattering && !settings.m_rayTraceAmbientOcclusion)
                {
                    components.m_rayTracingVolume.enabled = false;
                    timeOfDay.SetCameraFrameOverride(FrameSettingsField.RayTracing, false);
                }
                else
                {
                    components.m_rayTracingVolume.enabled = true;
                    timeOfDay.SetCameraFrameOverride(FrameSettingsField.RayTracing, true);
                }

                //Shadows
                //Sun
                components.m_sunLightData.useScreenSpaceShadows = settings.m_rayTraceShadows;
                components.m_sunLightData.useRayTracedShadows = settings.m_rayTraceShadows;
                components.m_sunLightData.rayTraceContactShadow = settings.m_rayTraceShadows ? settings.m_rayTraceContactShadows : false;
                components.m_sunLightData.colorShadow = settings.m_colorShadows;
                components.m_sunLightData.filterTracedShadow = settings.m_denoiseShadows;
                //Moon
                components.m_moonLightData.useScreenSpaceShadows = settings.m_rayTraceShadows;
                components.m_moonLightData.useRayTracedShadows = settings.m_rayTraceShadows;
                components.m_moonLightData.rayTraceContactShadow = settings.m_rayTraceShadows ? settings.m_rayTraceContactShadows : false;
                components.m_moonLightData.colorShadow = settings.m_colorShadows;
                components.m_moonLightData.filterTracedShadow = settings.m_denoiseShadows;
                //Shadow Quality
                if (!settings.m_overrideQuality)
                {
                    SetRayTracedShadowsQuality(components, settings.m_shadowsQuality);
                }
#endif
            }
        }
        /// <summary>
        /// Checks if the ray tracing quality options exists and if not creates them
        /// </summary>
        /// <param name="timeOfDay"></param>
        public static void CheckRayTracingQualityExists(HDRPTimeOfDay timeOfDay)
        {
            if (timeOfDay != null)
            {
                if (timeOfDay.TimeOfDayProfile != null)
                {
                    //GI
                    if (timeOfDay.TimeOfDayProfile.RayTracingSettings.RTGlobalQualitySettings.RTSSGIQuality.Count <= 0)
                    {
                        List<RTScreenSpaceGlobalIlluminationQuality> newSettings = new List<RTScreenSpaceGlobalIlluminationQuality>
                        {
                            //Low
                            new RTScreenSpaceGlobalIlluminationQuality
                            {
                                m_qualityName = "Low",
                                m_tracingMode = RayCastingMode.Mixed,
                                m_textureLODBias = 7,
                                m_rayMiss = RayMarchingFallbackHierarchy.ReflectionProbesAndSky,
                                m_lastBounce = RayMarchingFallbackHierarchy.ReflectionProbesAndSky,
                                m_maxRayLength = 100,
                                m_clampValue = 10,
                                m_fullResolution = false,
                                m_maxRaySteps = 32,
                                m_denoise = true,
                                m_halfResDenoise = true,
                                m_denoiseRadius = 0.66f,
                                m_secondDenoiserPass = true,
                                m_motionRejection = true
                            },
                            //Medium
                            new RTScreenSpaceGlobalIlluminationQuality
                            {
                                m_qualityName = "Medium",
                                m_tracingMode = RayCastingMode.Mixed,
                                m_textureLODBias = 7,
                                m_rayMiss = RayMarchingFallbackHierarchy.ReflectionProbesAndSky,
                                m_lastBounce = RayMarchingFallbackHierarchy.ReflectionProbesAndSky,
                                m_maxRayLength = 200,
                                m_clampValue = 10,
                                m_fullResolution = false,
                                m_maxRaySteps = 64,
                                m_denoise = true,
                                m_halfResDenoise = false,
                                m_denoiseRadius = 0.66f,
                                m_secondDenoiserPass = true,
                                m_motionRejection = true
                            },
                            //High
                            new RTScreenSpaceGlobalIlluminationQuality
                            {
                                m_qualityName = "High",
                                m_tracingMode = RayCastingMode.Mixed,
                                m_textureLODBias = 7,
                                m_rayMiss = RayMarchingFallbackHierarchy.ReflectionProbesAndSky,
                                m_lastBounce = RayMarchingFallbackHierarchy.ReflectionProbesAndSky,
                                m_maxRayLength = 500,
                                m_clampValue = 10,
                                m_fullResolution = true,
                                m_maxRaySteps = 128,
                                m_denoise = true,
                                m_halfResDenoise = false,
                                m_denoiseRadius = 0.4f,
                                m_secondDenoiserPass = true,
                                m_motionRejection = true
                            }
                        };
                        timeOfDay.TimeOfDayProfile.RayTracingSettings.RTGlobalQualitySettings.RTSSGIQuality.AddRange(newSettings);
                    }
                    //SSR
                    if (timeOfDay.TimeOfDayProfile.RayTracingSettings.RTGlobalQualitySettings.RTSSRQuality.Count <= 0)
                    {
                        List<RTScreenSpaceReflectionQuality> newSettings = new List<RTScreenSpaceReflectionQuality>
                        {
                            //Low
                            new RTScreenSpaceReflectionQuality
                            {
                                m_qualityName = "Low",
                                m_tracingMode = RayCastingMode.Mixed,
                                m_textureLODBias = 7,
                                m_rayMiss = RayTracingFallbackHierachy.ReflectionProbesAndSky,
                                m_lastBounce = RayTracingFallbackHierachy.ReflectionProbesAndSky,
                                m_minimumSmoothness = 0.25f,
                                m_smoothnessFadeStart = 0f,
                                m_maxRayLength = 50f,
                                m_clampValue = 1.2f,
                                m_fullResolution = false,
                                m_maxRaySteps = 32,
                                m_denoise = true,
                                m_denoiseRadius = 16,
                                m_affectsSmoothSurfaces = false
                            },
                            //Medium
                            new RTScreenSpaceReflectionQuality
                            {
                                m_qualityName = "Medium",
                                m_tracingMode = RayCastingMode.Mixed,
                                m_textureLODBias = 3,
                                m_rayMiss = RayTracingFallbackHierachy.ReflectionProbesAndSky,
                                m_lastBounce = RayTracingFallbackHierachy.ReflectionProbesAndSky,
                                m_minimumSmoothness = 0.2f,
                                m_smoothnessFadeStart = 0f,
                                m_maxRayLength = 150f,
                                m_clampValue = 1.2f,
                                m_fullResolution = false,
                                m_maxRaySteps = 64,
                                m_denoise = true,
                                m_denoiseRadius = 16,
                                m_affectsSmoothSurfaces = false
                            },
                            //High
                            new RTScreenSpaceReflectionQuality
                            {
                                m_qualityName = "High",
                                m_tracingMode = RayCastingMode.Mixed,
                                m_textureLODBias = 0,
                                m_rayMiss = RayTracingFallbackHierachy.ReflectionProbesAndSky,
                                m_lastBounce = RayTracingFallbackHierachy.ReflectionProbesAndSky,
                                m_minimumSmoothness = 0.2f,
                                m_smoothnessFadeStart = 0f,
                                m_maxRayLength = 300f,
                                m_clampValue = 1.2f,
                                m_fullResolution = true,
                                m_maxRaySteps = 64,
                                m_denoise = true,
                                m_denoiseRadius = 16,
                                m_affectsSmoothSurfaces = false
                            }
                        };
                        timeOfDay.TimeOfDayProfile.RayTracingSettings.RTGlobalQualitySettings.RTSSRQuality.AddRange(newSettings);
                    }
                    //AO
                    if (timeOfDay.TimeOfDayProfile.RayTracingSettings.RTGlobalQualitySettings.RTAOQuality.Count <= 0)
                    {
                        List<RTAmbientOcclusionQuality> newSettings = new List<RTAmbientOcclusionQuality>
                        {
                            //Low
                            new RTAmbientOcclusionQuality
                            {
                                m_qualityName = "Low",
                                m_maxRayLength = 4,
                                m_sampleCount = 2,
                                m_fullResolution = false,
                                m_denoise = true,
                                m_denoiseRadius = 0.66f,
                                m_occluderMotionRejection = true,
                                m_receiverMotionRejection = true
                            },
                            //Medium
                            new RTAmbientOcclusionQuality
                            {
                                m_qualityName = "Medium",
                                m_maxRayLength = 10,
                                m_sampleCount = 4,
                                m_fullResolution = false,
                                m_denoise = true,
                                m_denoiseRadius = 0.66f,
                                m_occluderMotionRejection = true,
                                m_receiverMotionRejection = true
                            },
                            //High
                            new RTAmbientOcclusionQuality
                            {
                                m_qualityName = "High",
                                m_maxRayLength = 20,
                                m_sampleCount = 8,
                                m_fullResolution = true,
                                m_denoise = true,
                                m_denoiseRadius = 0.66f,
                                m_occluderMotionRejection = true,
                                m_receiverMotionRejection = true
                            }
                        };
                        timeOfDay.TimeOfDayProfile.RayTracingSettings.RTGlobalQualitySettings.RTAOQuality.AddRange(newSettings);
                    }
                }
            }
        }
        /// <summary>
        /// Applies ray tracing settings
        /// </summary>
        /// <param name="isSetup"></param>
        /// <param name="components"></param>
        /// <param name="settings"></param>
        public static void ApplyOverrideRayTracingQuality(HDRPTimeOfDayComponents components, RayTraceSettings settings)
        {
            if (IsRayTracingSetup())
            {
#if RAY_TRACING_ENABLED
                
                if (settings.m_overrideQuality)
                {
                    //SSGI
                    components.m_rayTracedGlobalIllumination.quality.value = (int)settings.m_overrideQualityValue;
                    //SSR
                    components.m_rayTracedScreenSpaceReflection.quality.value = (int)settings.m_overrideQualityValue;
                    //AO
                    components.m_rayTracedAmbientOcclusion.quality.value = (int)settings.m_overrideQualityValue;
                    //Shadows
                    SetRayTracedShadowsQuality(components, settings.m_overrideQualityValue);
                }
#endif
            }
        }
        /// <summary>
        /// Applies ray traced shadow quality settings
        /// </summary>
        /// <param name="components"></param>
        /// <param name="quality"></param>
        private static void SetRayTracedShadowsQuality(HDRPTimeOfDayComponents components, GeneralQuality quality)
        {
            switch (quality)
            {
                case GeneralQuality.Low:
                {
                    components.m_moonLightData.numRayTracingSamples = 4;
                    components.m_moonLightData.filterSizeTraced = 8;
                    components.m_sunLightData.numRayTracingSamples = 4;
                    components.m_sunLightData.filterSizeTraced = 8;
                    break;
                }
                case GeneralQuality.Medium:
                {
                    components.m_moonLightData.numRayTracingSamples = 8;
                    components.m_moonLightData.filterSizeTraced = 16;
                    components.m_sunLightData.numRayTracingSamples = 8;
                    components.m_sunLightData.filterSizeTraced = 16;
                    break;
                }
                case GeneralQuality.High:
                {
                    components.m_moonLightData.numRayTracingSamples = 16;
                    components.m_moonLightData.filterSizeTraced = 32;
                    components.m_sunLightData.numRayTracingSamples = 16;
                    components.m_sunLightData.filterSizeTraced = 32;
                    break;
                }
            }
        }
        /// <summary>
        /// Function used to optimize ray tracing in the scene (EXPERIMENTAL)
        /// </summary>
        public static void OptimizeMeshesForRayTracing(RayTracingOptimizationData rtxData, MeshRenderer[] meshRenderers)
        {
            if (rtxData != null)
            {
                if (meshRenderers.Length < 1)
                {
                    meshRenderers = GameObject.FindObjectsOfType<MeshRenderer>();
                }

                if (meshRenderers.Length > 0)
                {
                    for (int i = 0; i < meshRenderers.Length; i++)
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorUtility.DisplayProgressBar("Optimize Scene For RTX", "We are currently optimizing the scene for RTX this could take some time", (float)i/meshRenderers.Length);
#endif
                        if (rtxData.m_ignoreIfAnimatorFound)
                        {
                            if (meshRenderers[i].GetComponent<Animator>() || meshRenderers[i].GetComponent<Animation>())
                            {
                                continue;
                            }
                        }

                        rtxData.ProcessMesh(meshRenderers[i]);
                    }

#if UNITY_EDITOR
                    UnityEditor.EditorUtility.ClearProgressBar();
#endif
                }
            }
        }
        /// <summary>
        /// Checks to see if ray tracing is setup correctly in HDRP Time of day system
        /// </summary>
        /// <returns></returns>
        private static bool IsRayTracingSetup()
        {
            if (HDRPTimeOfDay.Instance != null)
            {
                return HDRPTimeOfDay.Instance.IsRayTracingSetup();
            }

            return false;
        }
    }
}
#endif
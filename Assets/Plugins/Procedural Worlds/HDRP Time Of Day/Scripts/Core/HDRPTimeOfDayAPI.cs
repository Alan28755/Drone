#if GAIA_PRO_PRESENT
using Gaia;
#endif
using UnityEngine;
#if HDPipeline
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using static UnityEngine.Rendering.DebugUI;
#endif

namespace ProceduralWorlds.HDRPTOD
{
    public enum TODAPISetFunctions { SSGI, SSR, ContactShadows, MicroShadows, SunShadows, MoonShadows, EnableClouds, SunLensFlare, MoonLensFlare, ProbeSystem }
    public enum RayTracingFunction { ScreenSpaceGlobalIllumination, ScreenSpaceReflections, AmbientOcclusion, RecursiveRendering, SubSurfaceScattering, Shadows }
    public enum RayTracingOverallQuality { Off, PerformancePlus, Performance, Quality }
    public enum DebugLogType { Log, Warning, Error }
    public enum LightShadowResolutionQuality { Low, Medium, High, Ultra }

    public class HDRPTimeOfDayAPI
    {
#if HDPipeline && UNITY_2021_2_OR_NEWER
        /// <summary>
        /// Gets the time of day system instance in the scene
        /// </summary>
        /// <returns></returns>
        public static HDRPTimeOfDay GetTimeOfDay()
        {
            return HDRPTimeOfDay.Instance;
        }
        /// <summary>
        /// Refreshes the time of day system by processing the time of day code
        /// </summary>
        public static void RefreshTimeOfDay()
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                timeOfDay.ProcessTimeOfDay();
            }
        }
        /// <summary>
        /// Sets the anti aliasing mode and quality
        /// Quality is only for SMAA and TAA mode
        /// </summary>
        /// <param name="aaMode"></param>
        /// <param name="aaQuality"></param>
        public static void SetAntiAliasing(HDAdditionalCameraData.AntialiasingMode aaMode, AntiAliasingQuality aaQuality = AntiAliasingQuality.High)
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                timeOfDay.m_cameraSettings.m_antialiasingMode = aaMode;
                timeOfDay.m_cameraSettings.m_antiAliasingQuality = aaQuality;
                timeOfDay.ApplyCameraSettings();
            }
        }
        /// <summary>
        /// Gets the anti aliasing mode from time of day
        /// Returns None mode if tod is not found
        /// </summary>
        /// <returns></returns>
        public static HDAdditionalCameraData.AntialiasingMode GetAntialiasingMode()
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                return timeOfDay.m_cameraSettings.m_antialiasingMode;
            }
            else
            {
                return HDAdditionalCameraData.AntialiasingMode.None;
            }
        }
        /// <summary>
        /// Sets the overall quality of HDRP Time Of Day
        /// Note setting this to high could decrease performance on lower end hardware
        /// </summary>
        /// <param name="overallQuality"></param>
        public static void SetOverallQuality(GeneralQuality overallQuality)
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                if (timeOfDay.TimeOfDayProfile != null && timeOfDay.TimeOfDayPostFxProfile != null)
                {
                    timeOfDay.TimeOfDayProfile.TimeOfDayData.m_fogQuality = overallQuality;
                    timeOfDay.TimeOfDayProfile.TimeOfDayData.m_denoisingQuality = overallQuality;
                    timeOfDay.TimeOfDayProfile.TimeOfDayData.m_ssgiQuality = overallQuality;
                    timeOfDay.TimeOfDayProfile.TimeOfDayData.m_ssrQuality = overallQuality;
                    timeOfDay.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_postProcessingQuality = overallQuality;
                }
            }
        }
        /// <summary>
        /// Allows you to assign a set enum function to anything that can be toggled on/off with a bool in the TOD system
        /// </summary>
        /// <param name="value"></param>
        /// <param name="functionCall"></param>
        /// <param name="enabledCloudType"></param>
        public static void SetTODBoolFunction(bool value, TODAPISetFunctions functionCall, GlobalCloudType enabledCloudType = GlobalCloudType.Both)
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                switch (functionCall)
                {
                    case TODAPISetFunctions.SSGI:
                    {
                        timeOfDay.TimeOfDayProfile.TimeOfDayData.m_useSSGI = value;
                        break;
                    }
                    case TODAPISetFunctions.SSR:
                    {
                        timeOfDay.TimeOfDayProfile.TimeOfDayData.m_useSSR = value;
                        break;
                    }
                    case TODAPISetFunctions.ContactShadows:
                    {
                        timeOfDay.TimeOfDayProfile.TimeOfDayData.m_useContactShadows = value;
                        break;
                    }
                    case TODAPISetFunctions.MicroShadows:
                    {
                        timeOfDay.TimeOfDayProfile.TimeOfDayData.m_useMicroShadows = value;
                        break;
                    }
                    case TODAPISetFunctions.SunShadows:
                    {
                        timeOfDay.TimeOfDayProfile.TimeOfDayData.m_enableSunShadows = value;
                        break;
                    }
                    case TODAPISetFunctions.MoonShadows:
                    {
                        timeOfDay.TimeOfDayProfile.TimeOfDayData.m_enableMoonShadows = value;
                        break;
                    }
                    case TODAPISetFunctions.EnableClouds:
                    {
                        if (value)
                        {
                            timeOfDay.TimeOfDayProfile.TimeOfDayData.m_globalCloudType = enabledCloudType;
                        }
                        else
                        {
                            timeOfDay.TimeOfDayProfile.TimeOfDayData.m_globalCloudType = GlobalCloudType.None;
                        }
                        break;
                    }
                    case TODAPISetFunctions.SunLensFlare:
                    {
                        timeOfDay.TimeOfDayProfile.TimeOfDayData.m_sunLensFlareProfile.m_useLensFlare = value;
                        break;
                    }
                    case TODAPISetFunctions.MoonLensFlare:
                    {
                        timeOfDay.TimeOfDayProfile.TimeOfDayData.m_moonLensFlareProfile.m_useLensFlare = value;
                        break;
                    }
                    case TODAPISetFunctions.ProbeSystem:
                    {
                        timeOfDay.EnableReflectionProbeSync = value;
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// Allows you to assign a set enum function to anything that can be toggled on/off with a bool in the TOD system
        /// </summary>
        /// <param name="value"></param>
        /// <param name="functionCall"></param>
        /// <param name="enabledCloudType"></param>
        public static bool GetTODBoolFunction(TODAPISetFunctions functionCall, GlobalCloudType enabledCloudType = GlobalCloudType.Both)
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                switch (functionCall)
                {
                    case TODAPISetFunctions.SSGI:
                    {
                        return timeOfDay.TimeOfDayProfile.TimeOfDayData.m_useSSGI;
                    }
                    case TODAPISetFunctions.SSR:
                    {
                        return timeOfDay.TimeOfDayProfile.TimeOfDayData.m_useSSR;
                    }
                    case TODAPISetFunctions.ContactShadows:
                    {
                        return timeOfDay.TimeOfDayProfile.TimeOfDayData.m_useContactShadows;
                    }
                    case TODAPISetFunctions.MicroShadows:
                    {
                        return timeOfDay.TimeOfDayProfile.TimeOfDayData.m_useMicroShadows;
                    }
                    case TODAPISetFunctions.SunShadows:
                    {
                        return timeOfDay.TimeOfDayProfile.TimeOfDayData.m_enableSunShadows;
                    }
                    case TODAPISetFunctions.MoonShadows:
                    {
                        return timeOfDay.TimeOfDayProfile.TimeOfDayData.m_enableMoonShadows;
                    }
                    case TODAPISetFunctions.EnableClouds:
                    {
                        return timeOfDay.TimeOfDayProfile.TimeOfDayData.m_globalCloudType != GlobalCloudType.None;
                    }
                    case TODAPISetFunctions.SunLensFlare:
                    {
                        return timeOfDay.TimeOfDayProfile.TimeOfDayData.m_sunLensFlareProfile.m_useLensFlare;
                    }
                    case TODAPISetFunctions.MoonLensFlare:
                    {
                        return timeOfDay.TimeOfDayProfile.TimeOfDayData.m_moonLensFlareProfile.m_useLensFlare;
                    }
                    case TODAPISetFunctions.ProbeSystem:
                    {
                        return timeOfDay.EnableReflectionProbeSync;
                    }
                }
            }

            return false;
        }
        /// <summary>
        /// Sets the far clip plane render distance
        /// </summary>
        /// <param name="value"></param>
        public static void SetCameraRenderDistance(float value)
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                timeOfDay.m_cameraSettings.m_farClipPlane = value;
                timeOfDay.ApplyCameraSettings();
            }
        }
        /// <summary>
        /// Gets the far clip plane render distance
        /// </summary>
        /// <returns></returns>
        public static float GetCameraRenderDistance()
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                return timeOfDay.m_cameraSettings.m_farClipPlane;
            }
            return 20000f;
        }
        /// <summary>
        /// Sets the render state of the reflection probe system.
        /// </summary>
        /// <param name="value"></param>
        public static void SetReflectionProbeSystemRenderState(bool value)
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                timeOfDay.EnableReflectionProbeSync = value;
            }
        }
#if GAIA_PRO_PRESENT
        /// <summary>
        /// Sets the seaon settings
        /// </summary>
        /// <param name="settings"></param>
        public static void SetSeasonSettings(PWSkySeason settings)
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                if (timeOfDay.SeasonProfile != null)
                {
                    timeOfDay.m_enableSeasons = settings.EnableSeasons;
                    timeOfDay.SeasonProfile.m_season = settings.Season;
                    timeOfDay.SeasonProfile.m_seasonTransitionDuration = settings.m_seasonTransitionDuration;
                    timeOfDay.SeasonProfile.m_seasonAutumnTint = settings.SeasonAutumnTint;
                    timeOfDay.SeasonProfile.m_seasonSpringTint = settings.SeasonSpringTint;
                    timeOfDay.SeasonProfile.m_seasonSummerTint = settings.SeasonSummerTint;
                    timeOfDay.SeasonProfile.m_seasonWinterTint = settings.SeasonWinterTint;
                }
            }
        }
        /// <summary>
        /// Gets the time of day season settings
        /// </summary>
        /// <returns></returns>
        public static PWSkySeason GetSeasonSettings()
        {
            PWSkySeason settings = new PWSkySeason();
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                if (timeOfDay.SeasonProfile != null)
                {
                    settings.EnableSeasons = timeOfDay.m_enableSeasons;
                    settings.Season = timeOfDay.SeasonProfile.m_season;
                    settings.m_seasonTransitionDuration = timeOfDay.SeasonProfile.m_seasonTransitionDuration;
                    settings.SeasonAutumnTint = timeOfDay.SeasonProfile.m_seasonAutumnTint;
                    settings.SeasonSpringTint = timeOfDay.SeasonProfile.m_seasonSpringTint;
                    settings.SeasonSummerTint = timeOfDay.SeasonProfile.m_seasonSummerTint;
                    settings.SeasonWinterTint = timeOfDay.SeasonProfile.m_seasonWinterTint;
                }
            }

            return settings;
        }
#endif
        /// <summary>
        /// Starts the weather effect from the index selected. This is not an instant effect
        /// </summary>
        /// <param name="weatherProfileIndex"></param>
        public static void StartWeather(int weatherProfileIndex)
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay) && weatherProfileIndex != -1)
            {
                if (weatherProfileIndex <= timeOfDay.WeatherProfiles.Count - 1 && weatherProfileIndex >= 0)
                {
                    timeOfDay.StartWeather(weatherProfileIndex);
                }
            }
        }
        /// <summary>
        /// Gets the weather profile ID by checking if it contains the 'name' string
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int GetWeatherIDByName(string name)
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                for (int i = 0; i < timeOfDay.WeatherProfiles.Count; i++)
                {
                    if (timeOfDay.WeatherProfiles[i].WeatherData.m_weatherName.Contains(name))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        /// <summary>
        /// Stops the current active weather. This is an instant effect
        /// </summary>
        public static void StopWeather(bool instant = false)
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                timeOfDay.StopWeather(instant);
            }
        }
        /// <summary>
        /// Returns the weather active bool
        /// </summary>
        /// <returns></returns>
        public static bool WeatherActive()
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                return timeOfDay.WeatherActive();
            }
            return false;
        }
        /// <summary>
        /// Returns the time of day system, will return -1 if the time of day system is not present.
        /// </summary>
        /// <returns></returns>
        public static float GetCurrentTime()
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                return timeOfDay.TimeOfDay;
            }
            else
            {
                return -1f;
            }
        }
        /// <summary>
        /// Sets the time of day.
        /// If is0To1 is set to true you must provide a 0-1 value and not a 0-24 value the value will be multiplied by 24.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="is0To1"></param>
        public static void SetCurrentTime(float time, bool is0To1 = false)
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                if (is0To1)
                {
                    timeOfDay.TimeOfDay = Mathf.Clamp(time * 24f, 0f, 24f);
                }
                else
                {
                    timeOfDay.TimeOfDay = Mathf.Clamp(time, 0f, 24f);
                }
            }
        }
        /// <summary>
        /// Refreshes the exposure in the HDRP Time Of Day
        /// </summary>
        public static void RefreshExposure()
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                timeOfDay.RefreshExposure();
            }
        }
        /// <summary>
        /// Sets the direction of the system on the y axis
        /// </summary>
        /// <param name="direction"></param>
        public static void SetDirection(float direction)
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                timeOfDay.DirectionY = direction;
            }
        }
        /// <summary>
        /// Sets if you want to use post processing based on the state bool
        /// </summary>
        /// <param name="state"></param>
        public static void SetPostProcessingState(bool state)
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                timeOfDay.UsePostFX = state;
            }
        }
        /// <summary>
        /// Sets if you want to use ambient audio based on the state bool
        /// </summary>
        /// <param name="state"></param>
        public static void SetAmbientAudioState(bool state)
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                timeOfDay.UseAmbientAudio = state;
            }
        }
        /// <summary>
        /// Sets if you want to use underwater overrides based on the state bool
        /// </summary>
        /// <param name="state"></param>
        /// <param name="mode"></param>
        public static void SetUnderwaterState(bool state, UnderwaterOverrideSystemType mode)
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                timeOfDay.TimeOfDayProfile.UnderwaterOverrideData.m_useOverrides = state;
                timeOfDay.TimeOfDayProfile.UnderwaterOverrideData.m_systemType = mode;
            }
        }
        /// <summary>
        /// Sets if you want to use weather system based on the state bool
        /// </summary>
        /// <param name="state"></param>
        public static void SetWeatherState(bool state)
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                timeOfDay.UseWeatherFX = state;
            }
        }
        /// <summary>
        /// Sets the shadow distance multiplier
        /// </summary>
        /// <param name="value"></param>
        public static void SetGlobalShadowMultiplier(float value)
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                if (timeOfDay.TimeOfDayProfile != null)
                {
                    timeOfDay.TimeOfDayProfile.TimeOfDayData.m_shadowDistanceMultiplier = value;
                    RefreshTimeOfDay();
                }
            }
        }
        /// <summary>
        /// Gets the shadow distance multiplier
        /// </summary>
        /// <returns></returns>
        public static float GetGlobalShadowMultiplier()
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                if (timeOfDay.TimeOfDayProfile != null)
                {
                    return timeOfDay.TimeOfDayProfile.TimeOfDayData.m_shadowDistanceMultiplier;
                }
            }

            return 1f;
        }
        /// <summary>
        /// Sets the fog distance multiplier
        /// </summary>
        /// <param name="value"></param>
        public static void SetGlobalFogMultiplier(float value)
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                if (timeOfDay.TimeOfDayProfile != null)
                {
                    timeOfDay.TimeOfDayProfile.TimeOfDayData.m_globalFogMultiplier = value;
                    RefreshTimeOfDay();
                }
            }
        }
        /// <summary>
        /// Gets the fog distance multiplier
        /// </summary>
        /// <returns></returns>
        public static float GetGlobalFogMultiplier()
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                if (timeOfDay.TimeOfDayProfile != null)
                {
                    return timeOfDay.TimeOfDayProfile.TimeOfDayData.m_globalFogMultiplier;
                }
            }

            return 1f;
        }
        /// <summary>
        /// Sets the sun distance multiplier
        /// </summary>
        /// <param name="value"></param>
        public static void SetGlobalSunMultiplier(float value)
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                if (timeOfDay.TimeOfDayProfile != null)
                {
                    timeOfDay.TimeOfDayProfile.TimeOfDayData.m_globalLightMultiplier = value;
                    RefreshTimeOfDay();
                }
            }
        }
        /// <summary>
        /// Gets the sun distance multiplier
        /// </summary>
        /// <returns></returns>
        public static float GetGlobalSunMultiplier()
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                if (timeOfDay.TimeOfDayProfile != null)
                {
                    return timeOfDay.TimeOfDayProfile.TimeOfDayData.m_globalLightMultiplier;
                }
            }

            return 1f;
        }
        /// <summary>
        /// Sets the auto update multiplier
        /// </summary>
        /// <param name="value"></param>
        public static void SetAutoUpdateMultiplier(bool autoUpdate, float value)
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                if (timeOfDay.TimeOfDayProfile != null)
                {
                    timeOfDay.m_enableTimeOfDaySystem = autoUpdate;
                    timeOfDay.m_timeOfDayMultiplier = value;
                }
            }
        }
        /// <summary>
        /// Gets the auto update multiplier
        /// </summary>
        /// <returns></returns>
        public static void GetAutoUpdateMultiplier(out bool autoUpdate, out float value)
        {
            autoUpdate = false;
            value = 1f;
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                if (timeOfDay.TimeOfDayProfile != null)
                {
                    autoUpdate = timeOfDay.m_enableTimeOfDaySystem;
                    value = timeOfDay.m_timeOfDayMultiplier;
                }
            }
        }
        /// <summary>
        /// Validates if the render pipeline asset has ray tracing enabled.
        /// Logs warning if it's not
        /// </summary>
        public static bool ValidateRayTracingEnabled(bool isEnabled)
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                if (timeOfDay.UseRayTracing)
                {
                    if (GraphicsSettings.renderPipelineAsset != null)
                    {
                        HDRenderPipelineAsset asset = (HDRenderPipelineAsset)GraphicsSettings.renderPipelineAsset;
                        if (asset != null)
                        {
                            if (!asset.currentPlatformRenderPipelineSettings.supportRayTracing)
                            {
                                Debug.LogWarning(
                                    "You have enabled ray tracing in time of day but the current render pipeline asset does not support ray tracing please enable it in the pipeline asset.");
                            }

                            return asset.currentPlatformRenderPipelineSettings.supportRayTracing;
                        }
                    }
                }
            }

            return false;
        }
        /// <summary>
        /// Allows you to set the cloud settings
        /// Altitude based of min/max height and cloud thickness
        /// If cloud thickness is set to -1 it will not apply/update
        /// </summary>
        /// <param name="minMax"></param>
        /// <param name="cloudThickness"></param>
        public static void SetCloudSettings(Vector2 minMax, float cloudThickness = -1f)
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                HDRPTimeOfDayProfile profile = timeOfDay.TimeOfDayProfile;
                if (profile != null)
                {
                    Keyframe[] keyframes = new Keyframe[3];
                    //Set Frame 1
                    keyframes[0].value = minMax.x;
                    keyframes[0].time = 0f;
                    //Set Frame 2
                    keyframes[1].value = minMax.y;
                    keyframes[1].time = 0.5f;
                    //Set Frame 2
                    keyframes[2].value = minMax.x;
                    keyframes[2].time = 1f;
                    profile.TimeOfDayData.m_volumetricLowestCloudAltitude = new AnimationCurve(keyframes);

                    if (cloudThickness != -1)
                    {
                        profile.TimeOfDayData.m_volumetricCloudThickness = AnimationCurve.Constant(0f, 1f, cloudThickness);
                    }

                    timeOfDay.RefreshTimeOfDay();
                }
            }
        }
        /// <summary>
        /// Allows you to enable, disable and set the quality of a Ray Tracing option
        /// </summary>
        /// <param name="value"></param>
        /// <param name="option"></param>
        /// <param name="quality"></param>
        public static void SetRayTracingOption(bool value, RayTracingFunction option, GeneralQuality quality = GeneralQuality.Low)
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                if (ValidateRayTracingEnabled(timeOfDay.UseRayTracing))
                {
                    if (timeOfDay != null)
                    {
                        if (timeOfDay != null)
                        {
                            HDRPTimeOfDayProfile profile = timeOfDay.TimeOfDayProfile;
                            if (profile != null)
                            {
                                switch (option)
                                {
                                    case RayTracingFunction.ScreenSpaceGlobalIllumination:
                                    {
                                        profile.RayTracingSettings.m_rayTraceSSGI = value;
                                        profile.RayTracingSettings.m_ssgiQuality = quality;
                                        break;
                                    }
                                    case RayTracingFunction.ScreenSpaceReflections:
                                    {
                                        profile.RayTracingSettings.m_rayTraceSSR = value;
                                        profile.RayTracingSettings.m_ssrQuality = quality;
                                        break;
                                    }
                                    case RayTracingFunction.AmbientOcclusion:
                                    {
                                        profile.RayTracingSettings.m_rayTraceAmbientOcclusion = value;
                                        profile.RayTracingSettings.m_aoQuality = quality;
                                        break;
                                    }
                                    case RayTracingFunction.RecursiveRendering:
                                    {
                                        profile.RayTracingSettings.m_recursiveRendering = value;
                                        break;
                                    }
                                    case RayTracingFunction.SubSurfaceScattering:
                                    {
                                        profile.RayTracingSettings.m_rayTraceSubSurfaceScattering = value;
                                        switch (quality)
                                        {
                                            case GeneralQuality.Low:
                                            {
                                                profile.RayTracingSettings.m_subSurfaceScatteringSampleCount = 2;
                                                break;
                                            }
                                            case GeneralQuality.Medium:
                                            {
                                                profile.RayTracingSettings.m_subSurfaceScatteringSampleCount = 4;
                                                break;
                                            }
                                            case GeneralQuality.High:
                                            {
                                                profile.RayTracingSettings.m_subSurfaceScatteringSampleCount = 8;
                                                break;
                                            }
                                        }

                                        break;
                                    }
                                    case RayTracingFunction.Shadows:
                                    {
                                        profile.RayTracingSettings.m_rayTraceShadows = value;
                                        profile.RayTracingSettings.m_shadowsQuality = quality;
                                        break;
                                    }
                                }

                                HDRPTimeOfDayRayTracingUtils.ApplyRayTracingSettings(timeOfDay.GetTODComponents(), profile.RayTracingSettings, timeOfDay);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Sets the overall ray tracing quality
        /// Off - No Ray Tracing
        /// Performance Plus - Just SSR and AO
        /// Performance - Uses SSGI, SSR, AO and Sub surface scattering
        /// Quality - Uses SSGI, SSR, AO, Sub surface scattering and shadows
        /// </summary>
        /// <param name="quality"></param>
        public static void SetOverallRayTracingMode(RayTracingOverallQuality quality)
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                HDRPTimeOfDayProfile profile = timeOfDay.TimeOfDayProfile;
                if (profile != null)
                {
                    profile.RayTracingSettings.RTGlobalQualitySettings.CurrentQuality = quality;
                    switch (quality)
                    {
                        case RayTracingOverallQuality.Off:
                        {
                            //SSGI
                            profile.RayTracingSettings.m_rayTraceSSGI = false;
                            profile.RayTracingSettings.m_ssgiQuality = GeneralQuality.Low;
                            //SSR
                            profile.RayTracingSettings.m_rayTraceSSR = false;
                            profile.RayTracingSettings.m_ssrQuality = GeneralQuality.Low;
                            //AO
                            profile.RayTracingSettings.m_rayTraceAmbientOcclusion = false;
                            profile.RayTracingSettings.m_aoQuality = GeneralQuality.Medium;
                            //Recursivre Rendering
                            profile.RayTracingSettings.m_recursiveRendering = false;
                            //SSS
                            profile.RayTracingSettings.m_rayTraceSubSurfaceScattering = false;
                            profile.RayTracingSettings.m_subSurfaceScatteringSampleCount = 2;
                            //Shadows
                            profile.RayTracingSettings.m_rayTraceShadows = false;
                            profile.RayTracingSettings.m_shadowsQuality = GeneralQuality.Low;
                            break;
                        }
                        case RayTracingOverallQuality.PerformancePlus:
                        {
                            //SSGI
                            profile.RayTracingSettings.m_rayTraceSSGI = false;
                            profile.RayTracingSettings.m_ssgiQuality = GeneralQuality.Low;
                            //SSR
                            profile.RayTracingSettings.m_rayTraceSSR = true;
                            profile.RayTracingSettings.m_ssrQuality = GeneralQuality.Low;
                            //AO
                            profile.RayTracingSettings.m_rayTraceAmbientOcclusion = true;
                            profile.RayTracingSettings.m_aoQuality = GeneralQuality.Medium;
                            //Recursivre Rendering
                            profile.RayTracingSettings.m_recursiveRendering = false;
                            //SSS
                            profile.RayTracingSettings.m_rayTraceSubSurfaceScattering = false;
                            profile.RayTracingSettings.m_subSurfaceScatteringSampleCount = 2;
                            //Shadows
                            profile.RayTracingSettings.m_rayTraceShadows = false;
                            profile.RayTracingSettings.m_shadowsQuality = GeneralQuality.Low;
                            break;
                        }
                        case RayTracingOverallQuality.Performance:
                        {
                            //SSGI
                            profile.RayTracingSettings.m_rayTraceSSGI = true;
                            profile.RayTracingSettings.m_ssgiQuality = GeneralQuality.Low;
                            //SSR
                            profile.RayTracingSettings.m_rayTraceSSR = true;
                            profile.RayTracingSettings.m_ssrQuality = GeneralQuality.Medium;
                            //AO
                            profile.RayTracingSettings.m_rayTraceAmbientOcclusion = true;
                            profile.RayTracingSettings.m_aoQuality = GeneralQuality.High;
                            //Recursivre Rendering
                            profile.RayTracingSettings.m_recursiveRendering = false;
                            //SSS
                            profile.RayTracingSettings.m_rayTraceSubSurfaceScattering = false;
                            profile.RayTracingSettings.m_subSurfaceScatteringSampleCount = 2;
                            //Shadows
                            profile.RayTracingSettings.m_rayTraceShadows = false;
                            profile.RayTracingSettings.m_shadowsQuality = GeneralQuality.Low;
                            break;
                        }
                        case RayTracingOverallQuality.Quality:
                        {
                            //SSGI
                            profile.RayTracingSettings.m_rayTraceSSGI = true;
                            profile.RayTracingSettings.m_ssgiQuality = GeneralQuality.Medium;
                            //SSR
                            profile.RayTracingSettings.m_rayTraceSSR = true;
                            profile.RayTracingSettings.m_ssrQuality = GeneralQuality.High;
                            //AO
                            profile.RayTracingSettings.m_rayTraceAmbientOcclusion = true;
                            profile.RayTracingSettings.m_aoQuality = GeneralQuality.High;
                            //Recursivre Rendering
                            profile.RayTracingSettings.m_recursiveRendering = false;
                            //SSS
                            profile.RayTracingSettings.m_rayTraceSubSurfaceScattering = true;
                            profile.RayTracingSettings.m_subSurfaceScatteringSampleCount = 2;
                            //Shadows
                            profile.RayTracingSettings.m_rayTraceShadows = false;
                            profile.RayTracingSettings.m_shadowsQuality = GeneralQuality.Low;
                            break;
                        }
                    }

                    HDRPTimeOfDayRayTracingUtils.ApplyRayTracingSettings(timeOfDay.GetTODComponents(), profile.RayTracingSettings, timeOfDay);
                    timeOfDay.RefreshTimeOfDay();
                }
            }
        }
        /// <summary>
        /// Sets the shadow resolution
        /// </summary>
        /// <param name="shadowResolution"></param>
        public static void SetShadowResolution(LightShadowResolutionQuality shadowResolution)
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                timeOfDay.SetLightShadowQuality(shadowResolution);
            }
        }
        /// <summary>
        /// Sets the quality of the lighting rendering to disable/enable effects
        /// </summary>
        public static void SetLightingRenderQuality(GeneralQuality overallQuality)
        {
            switch (overallQuality)
            {
                case GeneralQuality.Low:
                {
                    SetShadowResolution(LightShadowResolutionQuality.Medium);
                    SetTODBoolFunction(false, TODAPISetFunctions.SSGI);
                    SetTODBoolFunction(false, TODAPISetFunctions.SSR);
                    SetTODBoolFunction(false, TODAPISetFunctions.ContactShadows);
                    SetTODBoolFunction(false, TODAPISetFunctions.ProbeSystem);
                    break;
                }
                case GeneralQuality.Medium:
                {
                    SetShadowResolution(LightShadowResolutionQuality.High);
                    SetTODBoolFunction(false, TODAPISetFunctions.SSGI);
                    SetTODBoolFunction(true, TODAPISetFunctions.SSR);
                    SetTODBoolFunction(true, TODAPISetFunctions.ContactShadows);
                    SetTODBoolFunction(true, TODAPISetFunctions.ProbeSystem);
                    break;
                }
                case GeneralQuality.High:
                {
                    SetShadowResolution(LightShadowResolutionQuality.Ultra);
                    SetTODBoolFunction(true, TODAPISetFunctions.SSGI);
                    SetTODBoolFunction(true, TODAPISetFunctions.SSR);
                    SetTODBoolFunction(true, TODAPISetFunctions.ContactShadows);
                    SetTODBoolFunction(true, TODAPISetFunctions.ProbeSystem);
                    break;
                }
            }

            RefreshTimeOfDay();
        }
        /// <summary>
        /// Sets a camera frame override
        /// </summary>
        /// <param name="frameSettings"></param>
        /// <param name="value"></param>
        public static void SetCameraFrameOverride(FrameSettingsField frameSettings, bool value)
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                timeOfDay.SetCameraFrameOverride(frameSettings, value);
            }
        }
        /// <summary>
        /// Checks to see if ray traced SSGI is enabled
        /// </summary>
        /// <returns></returns>
        public static bool RayTracingSSGIActive()
        {
            if (IsPresent(out HDRPTimeOfDay timeOfDay))
            {
                if (timeOfDay.UseRayTracing)
                {
                    HDRPTimeOfDayProfile profile = timeOfDay.TimeOfDayProfile;
                    if (profile != null)
                    {
                        if (!Application.isPlaying)
                        {
                            if (profile.RayTracingSettings.m_renderInEditMode)
                            {
                                return profile.RayTracingSettings.m_rayTraceSSGI;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return profile.RayTracingSettings.m_rayTraceSSGI;
                        }
                    }
                }
            }

            return false;
        }
        /// <summary>
        /// Sets the camera relitive culling options in the graphics settings
        /// Cull Lights = uses the camera position as the reference point for culling lights.
        /// Cull shadows = uses the camera position as the reference point for culling shadows.
        /// </summary>
        /// <param name="cullLights"></param>
        /// <param name="cullShadows"></param>
        public static void SetGraphicSettingsLightAndShadowCulling(bool cullLights, bool cullShadows)
        {
#if UNITY_2022_2_OR_NEWER
            GraphicsSettings.cameraRelativeLightCulling = cullLights;
            GraphicsSettings.cameraRelativeShadowCulling = cullShadows;
#endif
        }
        /// <summary>
        /// Retrns true if the system is present
        /// </summary>
        /// <returns></returns>
        private static bool IsPresent(out HDRPTimeOfDay timeOfDay)
        {
            timeOfDay = HDRPTimeOfDay.Instance;
            return timeOfDay != null;
        }
#endif

        /// <summary>
        /// Gets the camera transform that is used for the player
        /// </summary>
        /// <returns></returns>
        public static Transform GetCamera()
        {
            Camera camera = Camera.main;
            if (camera != null)
            {
                return camera.transform;
            }

            Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
            if (cameras.Length > 0)
            {
                foreach (Camera cam in cameras)
                {
                    if (cam.isActiveAndEnabled)
                    {
                        return cam.transform;
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// Creates a new vector 3 from a float value, great way to make boxes/bounds
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector3 CreateVector3FromFloat(float value)
        {
            return new Vector3(value, value, value);
        }
        /// <summary>
        /// Returns the highest float value from the bounds size/scale
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static float GetLargestBoundsSizeValue(Bounds bounds)
        {
            float highestValue = -1f;
            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                    {
                        if (bounds.size.x > highestValue)
                        {
                            highestValue = bounds.size.x;
                        }
                        break;
                    }
                    case 1:
                    {
                        if (bounds.size.y > highestValue)
                        {
                            highestValue = bounds.size.y;
                        }
                        break;
                    }
                    case 2:
                    {
                        if (bounds.size.z > highestValue)
                        {
                            highestValue = bounds.size.z;
                        }
                        break;
                    }
                }
            }

            return highestValue;
        }
    }
}
using BepInEx;
using HarmonyLib;
using Reptile;
using System;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.ConstrainedExecution;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using System.Reflection.Emit;

namespace trickyclown
{
    // STOP ABILITY PATCHES FOR ANIM OVERRIDES

    [HarmonyPatch(typeof(AirTrickAbility))]
    [HarmonyPatch("OnStopAbility")]
    public class ATAonStopAbilityPatch
    {
        public static void Postfix()
        {
            if (VertAbilityPatches.forcingFootController)
            {
                VertAbilityPatches.forcingFootController = false;
                VertAbilityPatches.RestoreAnimController();
            }
        }
    }

    [HarmonyPatch(typeof(SlideAbility))]
    [HarmonyPatch("OnStopAbility")]
    public class SAonStopAbilityPatch
    {
        public static void Postfix()
        {
            if (VertAbilityPatches.forcingFootController)
            {
                VertAbilityPatches.forcingFootController = false;
                VertAbilityPatches.RestoreAnimController();
            }
        }
    }

    [HarmonyPatch(typeof(GrindAbility))]
    [HarmonyPatch("OnStopAbility")]
    public class GRIonStopAbilityPatch
    {
        public static void Postfix()
        {
            if (VertAbilityPatches.forcingFootController)
            {
                VertAbilityPatches.forcingFootController = false;
                VertAbilityPatches.RestoreAnimController();
            }
        }
    }

    [HarmonyPatch(typeof(GroundTrickAbility))]
    [HarmonyPatch("OnStopAbility")]
    public class GROonStopAbilityPatch
    {
        public static void Postfix()
        {
            if (VertAbilityPatches.forcingFootController)
            {
                VertAbilityPatches.forcingFootController = false;
                VertAbilityPatches.RestoreAnimController();
            }
        }
    }

    // END STOP ABILITY PATCHES FOR ANIM OVERRIDES

    //[HarmonyPatch(typeof(Player))]
    //[HarmonyPatch("DoTrick")]
    //public class DoTrickPatch
    //{
    //    [HarmonyPostfix]
    //    public static void Postfix(Player __instance, Player.TrickType type, string trickName, int trickNum)
    //    {
    //        if (trickName == "Frameride")
    //        {
    //            __instance.currentTrickPoints = 0;
    //        }
    //        if (trickName == "Nice" || trickName == "nice" || trickName == "NICE")
    //        {
    //            __instance.currentTrickPoints = 69;
    //            __instance.fortuneAppLocked = false;
    //        }
    //        if (trickName == "Splurgy Buttslap" || trickName == "The Splurgy Buttslap" || trickName == "splurgy buttslap" || trickName == "the splurgy buttslap")
    //        {
    //            __instance.currentTrickPoints = -77777;
    //        }
    //    }
    //}
    [HarmonyPatch(typeof(WallrunLineAbility))]
    public static class WallrunLineAbilityPatches
    {
        public static float wallrunDuration;
        public static float postWallrunTimer = 0f;
        private const float wallrunThreshold = 0.1f;
        public static bool hasWallRan = false;

        [HarmonyPatch("OnStartAbility")]
        [HarmonyPostfix]
        public static void Postfix_OnStartAbility(WallrunLineAbility __instance)
        {
            if (__instance == null) return;
            wallrunDuration = 0f;
            //isWallRunning = true;
            VertAbilityPatches.overridingIdle = false;
            //Console.WriteLine("overridingIdle = false");
        }

        [HarmonyPatch("FixedUpdateAbility")]
        [HarmonyPostfix]
        public static void Postfix_FixedUpdateAbility(WallrunLineAbility __instance)
        {
            //if (__instance == null || __instance.p == null) return;
            if (__instance.p.ability == __instance.p.wallrunAbility)
            {
                wallrunDuration += Time.deltaTime;
                postWallrunTimer = 0f;
                hasWallRan = false;
            }

            if (__instance.p.moveStyle == MoveStyle.ON_FOOT)
            {
                if (__instance.p.GetVelocity().magnitude >= BPatchTC.wallslideSpeed.Value)
                {
                    string rightAnim = BPatchTC.Instance.GetConfigValueMisc("highSpeedWallrideRightCfg");
                    string leftAnim = BPatchTC.Instance.GetConfigValueMisc("highSpeedWallrideLeftCfg");
                    //Debug.Log($"Current animSide: {__instance.animSide}");
                    if (__instance.animSide == Side.RIGHT)
                    {
                        __instance.p.PlayAnim(AnimationUtility.GetAnimationByName(BPatchTC.Instance.GetConfigValueMisc("highSpeedWallrideRightCfg")), false, false, -1f);
                    }
                    else if (__instance.animSide == Side.LEFT)
                    {
                        __instance.p.PlayAnim(AnimationUtility.GetAnimationByName(BPatchTC.Instance.GetConfigValueMisc("highSpeedWallrideLeftCfg")), false, false, -1f);
                    }
                }
                else
                {
                    if (__instance.animSide == Side.RIGHT)
                    {
                        __instance.p.PlayAnim(AnimationUtility.GetAnimationByName(BPatchTC.Instance.GetConfigValueMisc("wallrideRightCfg")), false, false, -1f);
                    }
                    else if (__instance.animSide == Side.LEFT)
                    {
                        __instance.p.PlayAnim(AnimationUtility.GetAnimationByName(BPatchTC.Instance.GetConfigValueMisc("wallrideLeftCfg")), false, false, -1f);
                    }
                }
            }
            if (__instance.p.moveStyle == MoveStyle.INLINE)
            {
                if (__instance.p.GetVelocity().magnitude >= BPatchTC.wallslideSpeed.Value)
                {
                    if (__instance.animSide == Side.RIGHT)
                    {
                        __instance.p.PlayAnim(AnimationUtility.GetAnimationByName(BPatchTC.Instance.GetConfigValueMisc("highSpeedWallrideRightInlineCfg")), false, false, -1f);
                    }
                    else if (__instance.animSide == Side.LEFT)
                    {
                        __instance.p.PlayAnim(AnimationUtility.GetAnimationByName(BPatchTC.Instance.GetConfigValueMisc("highSpeedWallrideLeftInlineCfg")), false, false, -1f);
                    }
                }
                else
                {
                    if (__instance.animSide == Side.RIGHT)
                    {
                        __instance.p.PlayAnim(AnimationUtility.GetAnimationByName(BPatchTC.Instance.GetConfigValueMisc("wallrideRightInlineCfg")), false, false, -1f);
                    }
                    else if (__instance.animSide == Side.LEFT)
                    {
                        __instance.p.PlayAnim(AnimationUtility.GetAnimationByName(BPatchTC.Instance.GetConfigValueMisc("wallrideLeftInlineCfg")), false, false, -1f);
                    }
                }
            }
            if (__instance.p.moveStyle == MoveStyle.BMX)
            {
                if (__instance.p.GetVelocity().magnitude >= BPatchTC.wallslideSpeed.Value)
                {
                    if (__instance.animSide == Side.RIGHT)
                    {
                        __instance.p.PlayAnim(AnimationUtility.GetAnimationByName(BPatchTC.Instance.GetConfigValueMisc("highSpeedWallrideRightBMXCfg")), false, false, -1f);
                    }
                    else if (__instance.animSide == Side.LEFT)
                    {
                        __instance.p.PlayAnim(AnimationUtility.GetAnimationByName(BPatchTC.Instance.GetConfigValueMisc("highSpeedWallrideLeftBMXCfg")), false, false, -1f);
                    }
                }
                else
                {
                    if (__instance.animSide == Side.RIGHT)
                    {
                        __instance.p.PlayAnim(AnimationUtility.GetAnimationByName(BPatchTC.Instance.GetConfigValueMisc("wallrideRightBMXCfg")), false, false, -1f);
                    }
                    else if (__instance.animSide == Side.LEFT)
                    {
                        __instance.p.PlayAnim(AnimationUtility.GetAnimationByName(BPatchTC.Instance.GetConfigValueMisc("wallrideLeftBMXCfg")), false, false, -1f);
                    }
                }
            }
            if (__instance.p.moveStyle == MoveStyle.SKATEBOARD)
            {
                if (__instance.p.GetVelocity().magnitude >= BPatchTC.wallslideSpeed.Value)
                {
                    if (__instance.animSide == Side.RIGHT)
                    {
                        __instance.p.PlayAnim(AnimationUtility.GetAnimationByName(BPatchTC.Instance.GetConfigValueMisc("highSpeedWallrideRightSkateboardCfg")), false, false, -1f);
                    }
                    else if (__instance.animSide == Side.LEFT)
                    {
                        __instance.p.PlayAnim(AnimationUtility.GetAnimationByName(BPatchTC.Instance.GetConfigValueMisc("highSpeedWallrideLeftSkateboardCfg")), false, false, -1f);
                    }
                }
                else
                {
                    if (__instance.animSide == Side.RIGHT)
                    {
                        __instance.p.PlayAnim(AnimationUtility.GetAnimationByName(BPatchTC.Instance.GetConfigValueMisc("wallrideRightSkateboardCfg")), false, false, -1f);
                    }
                    else if (__instance.animSide == Side.LEFT)
                    {
                        __instance.p.PlayAnim(AnimationUtility.GetAnimationByName(BPatchTC.Instance.GetConfigValueMisc("wallrideLeftSkateboardCfg")), false, false, -1f);
                    }
                }
            }
        }

        [HarmonyPatch("OnStopAbility")]
        [HarmonyPostfix]
        public static void Postfix_OnStopAbility(WallrunLineAbility __instance)
        {
            if (__instance == null) return;
            if (wallrunDuration <= Time.deltaTime)
            {
                //isWallRunning = false;
                //Debug.Log("Wallrun ended after less than one frame.");
                StartPostWallrunTimer();
                hasWallRan = true;
            }
            //wallrunDuration = 0f;
        }

        public static void StartPostWallrunTimer()
        {
            postWallrunTimer = 0f;
        }
    }

    [BepInPlugin("ConfigTrixMisc", "New Trix Misc", "1.2.0")]
    [BepInProcess("Bomb Rush Cyberfunk.exe")]
    public class BPatchTC : BaseUnityPlugin
    {
        public static bool forcingFootController = false;

        public static ConfigEntry<bool> enableSwitchCfg;
        public static ConfigEntry<bool> enableFramerideCfg;
        public static ConfigEntry<bool> enableFramerideSoundCfg;
        public static BPatchTC Instance { get; private set; }

        public static ConfigEntry<bool> enableAnimationBlending;
        public static ConfigEntry<float> defaultAnimationBlendingIn;
        public static ConfigEntry<float> defaultAnimationBlendingOut;
        public static ConfigEntry<string> customAnimationBlending;

        private Dictionary<string, ConfigEntry<string>> configEntries;

        public static ConfigEntry<float> wallslideSpeed;

        void Awake()
        {
            BPatchTC.enableSwitchCfg = base.Config.Bind<bool>("General", "enableSwitchCfg", false, "Disable Boost Switching to On-foot (Leave this false if you're doing vanilla score attack or your run will be considered invalid.)");
            BPatchTC.enableFramerideCfg = base.Config.Bind<bool>("General", "enableFramerideCfg", false, "Enables frameride trick name/score (Leave this false if you're doing vanilla score attack or your run will be considered invalid.)");
            BPatchTC.enableFramerideSoundCfg = base.Config.Bind<bool>("General", "enableFramerideSoundCfg", false, "Enables frameride sound.");

            BPatchTC.enableAnimationBlending = base.Config.Bind<bool>("General", "enableAnimationBlending", true, "Enables smooth animation blending between animations. May cause strange/broken movement with certain animations.");
            BPatchTC.defaultAnimationBlendingIn = base.Config.Bind<float>("General", "defaultAnimationBlendingIn", 0f, "Default blend time transitioning into BOE animations (in seconds). When transitioning from one BOE animation to another, defaultAnimationBlendingOut takes priority");
            BPatchTC.defaultAnimationBlendingOut = base.Config.Bind<float>("General", "defaultAnimationBlendingOut", 0.1f, "Default blend time transitioning out of BOE animations (in seconds)");
            
            BPatchTC.customAnimationBlending = base.Config.Bind<string>("General", "customAnimationBlending", "", "Overrides default blend time for specified animations. Use asterisk (*) as a wildcard for any animations (vanilla or BOE). Separate rules with commas. Acceptable formats: animation:inBlend/outBlend, animation:outBlend, animationIn>animationOut:blend, animation1>animation2>animation3:blend, animation1>animation2>animation3:inBlend/outBlend"); 
            BPatchTC.customAnimationBlending.SettingChanged += (a,b) => { ABPatchesTC.ParseCustomBlending(); };

            Instance = this;

            wallslideSpeed = Config.Bind("Misc", "Wallride (High-Speed) Min Speed", 60f, "High-speed wallride speed. This is in meters per second, so take the number from the dragsun speedometer and move the decimal over by one place. For example, 1000 becomes 100, 133 becomes 13.3.");

            configEntries = new Dictionary<string, ConfigEntry<string>>
            {
                { "airBoostCfg", Config.Bind("Misc", "Midair Boost", "boostRun", "Midair Boost") },
                { "groundBoostCfg", Config.Bind("Misc", "Ground Boost", "boostRun", "Ground Boost") },
                { "airBoostInlineCfg", Config.Bind("Misc", "Midair Boost (Inline)", "boostRun", "Midair Boost (Inline)") },
                { "groundBoostInlineCfg", Config.Bind("Misc", "Ground Boost (Inline)", "boostRun", "Ground Boost (Inline)") },
                { "airBoostSkateboardCfg", Config.Bind("Misc", "Midair Boost (Skateboard)", "boostRun", "Midair Boost (Skateboard)") },
                { "groundBoostSkateboardCfg", Config.Bind("Misc", "Ground Boost (Skateboard)", "boostRun", "Ground Boost (Skateboard)") },
                { "airBoostBMXCfg", Config.Bind("Misc", "Midair Boost (BMX)", "boostRun", "Midair Boost (BMX)") },
                { "groundBoostBMXCfg", Config.Bind("Misc", "Ground Boost (BMX)", "boostRun", "Ground Boost (BMX)") },
                { "airdashCfg", Config.Bind("Misc", "Airdash", "airDash", "Airdash") },
                { "airdashInlineCfg", Config.Bind("Misc", "Inline Airdash", "airDash", "Inline Airdash") },
                { "airdashSkateboardCfg", Config.Bind("Misc", "Skateboard Airdash", "airDash", "Skateboard Airdash") },
                { "airdashBmxCfg", Config.Bind("Misc", "BMX Airdash", "airDash", "BMX Airdash") },

                //{ "airBoostBrakeCfg", Config.Bind("Misc", "Midair Boost Brake", "", "Midair Boost Brake") },
                //{ "airBoostBrakeInlineCfg", Config.Bind("Misc", "Midair Boost Brake (Inline)", "", "Midair Boost Brake (Inline)") },
                //{ "airBoostBrakeSkateboardCfg", Config.Bind("Misc", "Midair Boost Brake (Skateboard)", "", "Midair Boost Brake (Skateboard)") },
                //{ "airBoostBrakeBMXCfg", Config.Bind("Misc", "Midair Boost Brake (BMX)", "", "Midair Boost Brake (BMX)") },

                { "landRunCfg", Config.Bind("Misc 2", "Land into Run", "landRun", "Land from Run") },
                { "landCfg", Config.Bind("Misc 2", "Land", "land", "Land") },
                { "startRunCfg", Config.Bind("Misc 2", "Start Run", "startRun", "Start Run") },
                { "stopRunCfg", Config.Bind("Misc 2", "Stop Run", "stopRun", "Stop Run") },
                { "walkCfg", Config.Bind("Misc 2", "Walk", "walk", "Walk") },
                { "runCfg", Config.Bind("Misc 2", "Run", "run", "Run") },
                { "fallCfg", Config.Bind("Misc 2", "Fall", "fall", "Fall") },
                { "idleCfg", Config.Bind("Misc 2", "Idle", "idle", "Idle") },
                { "idleFidget1Cfg", Config.Bind("Misc 2", "Idle Fidget", "idleFidget1", "Idle Fidget. Leave blank for no idle fidget.") },

                { "landRunCfgInline", Config.Bind("Misc 2 Inline", "Land into Run", "landRun", "Land from Run") },
                { "landCfgInline", Config.Bind("Misc 2 Inline", "Land", "land", "Land") },
                { "startRunCfgInline", Config.Bind("Misc 2 Inline", "Start Run", "startRun", "Start Run") },
                { "stopRunCfgInline", Config.Bind("Misc 2 Inline", "Stop Run", "stopRun", "Stop Run") },
                { "walkCfgInline", Config.Bind("Misc 2 Inline", "Walk", "walk", "Walk") },
                { "runCfgInline", Config.Bind("Misc 2 Inline", "Run", "run", "Run") },
                { "fallCfgInline", Config.Bind("Misc 2 Inline", "Fall", "fall", "Fall") },
                { "idleCfgInline", Config.Bind("Misc 2 Inline", "Idle", "idle", "Idle") },
                { "idleFidget1CfgInline", Config.Bind("Misc 2 Inline", "Idle Fidget", "idleFidget1", "Idle Fidget. Leave blank for no idle fidget.") },

                { "landRunCfgSkateboard", Config.Bind("Misc 2 Skateboard", "Land into Run", "landRun", "Land from Run") },
                { "landCfgSkateboard", Config.Bind("Misc 2 Skateboard", "Land", "land", "Land") },
                { "startRunCfgSkateboard", Config.Bind("Misc 2 Skateboard", "Start Run", "startRun", "Start Run") },
                { "stopRunCfgSkateboard", Config.Bind("Misc 2 Skateboard", "Stop Run", "stopRun", "Stop Run") },
                { "walkCfgSkateboard", Config.Bind("Misc 2 Skateboard", "Walk", "walk", "Walk") },
                { "runCfgSkateboard", Config.Bind("Misc 2 Skateboard", "Run", "run", "Run") },
                { "fallCfgSkateboard", Config.Bind("Misc 2 Skateboard", "Fall", "fall", "Fall") },
                { "idleCfgSkateboard", Config.Bind("Misc 2 Skateboard", "Idle", "idle", "Idle") },
                { "idleFidget1CfgSkateboard", Config.Bind("Misc 2 Skateboard", "Idle Fidget", "idleFidget1", "Idle Fidget. Leave blank for no idle fidget.") },

                { "landRunCfgBMX", Config.Bind("Misc 2 BMX", "Land into Run", "landRun", "Land from Run") },
                { "landCfgBMX", Config.Bind("Misc 2 BMX", "Land", "land", "Land") },
                { "startRunCfgBMX", Config.Bind("Misc 2 BMX", "Start Run", "startRun", "Start Run") },
                { "stopRunCfgBMX", Config.Bind("Misc 2 BMX", "Stop Run", "stopRun", "Stop Run") },
                { "walkCfgBMX", Config.Bind("Misc 2 BMX", "Walk", "walk", "Walk") },
                { "runCfgBMX", Config.Bind("Misc 2 BMX", "Run", "run", "Run") },
                { "fallCfgBMX", Config.Bind("Misc 2 BMX", "Fall", "fall", "Fall") },
                { "idleCfgBMX", Config.Bind("Misc 2 BMX", "Idle", "idle", "Idle") },
                { "idleFidget1CfgBMX", Config.Bind("Misc 2 BMX", "Idle Fidget", "idleFidget1", "Idle Fidget. Leave blank for no idle fidget.") },

                { "jumpCfg", Config.Bind("Misc", "Jump", "jump", "Jump") },
                { "jumpCfgInline", Config.Bind("Misc", "Jump Inline", "jump", "Jump Inline") },
                { "jumpCfgSkateboard", Config.Bind("Misc", "Jump Skateboard", "jump", "Jump Skateboard") },
                { "jumpCfgBMX", Config.Bind("Misc", "Jump BMX", "jump", "Jump BMX") },

                { "boostBrakeCfg", Config.Bind("Misc", "Boost Brake", "boostBrake", "Boost Brake") },
                { "boostBrakeInlineCfg", Config.Bind("Misc", "Boost Brake Inline", "boostBrake", "Boost Brake Inline") },
                { "boostBrakeSkateboardCfg", Config.Bind("Misc", "Boost Brake Skateboard", "boostBrake", "Boost Brake Skateboard") },
                { "boostBrakeBMXCfg", Config.Bind("Misc", "Boost Brake BMX", "boostBrake", "Boost Brake BMX") },

                { "boostStartCfg", Config.Bind("Misc", "Boost Start", "startBoost", "Boost Start. If you want this to be the same as your ground boost, leave it blank, otherwise it won't work.") },
                { "boostStartInlineCfg", Config.Bind("Misc", "Boost Start Inline", "startBoost", "Boost Start Inline. If you want this to be the same as your ground boost, leave it blank, otherwise it won't work.") },
                { "boostStartSkateboardCfg", Config.Bind("Misc", "Boost Start Skateboard", "startBoost", "Boost Start Skateboard. If you want this to be the same as your ground boost, leave it blank, otherwise it won't work.") },
                { "boostStartBMXCfg", Config.Bind("Misc", "Boost Start BMX", "startBoost", "Boost Start BMX. If you want this to be the same as your ground boost, leave it blank, otherwise it won't work.") },

                { "poleFreezeCfg", Config.Bind("Misc", "Handplant/Pole Freeze", "poleFreeze", "Handplant/Pole Freeze") },
                { "poleFreezeInlineCfg", Config.Bind("Misc", "Handplant/Pole Freeze Inline", "poleFreeze", "Handplant/Pole Freeze Inline") },
                { "poleFreezeSkateboardCfg", Config.Bind("Misc", "Handplant/Pole Freeze Skateboard", "poleFreeze", "Handplant/Pole Freeze Skateboard") },
                { "poleFreezeBMXCfg", Config.Bind("Misc", "Handplant/Pole Freeze BMX", "poleFreeze", "Handplant/Pole Freeze BMX") },

                { "poleFlipCfg", Config.Bind("Misc", "Pole Flip", "poleFlip", "Pole Flip") },
                { "poleFlipInlineCfg", Config.Bind("Misc", "Pole Flip Inline", "poleFlip", "Pole Flip Inline") },
                { "poleFlipSkateboardCfg", Config.Bind("Misc", "Pole Flip Skateboard", "poleFlip", "Pole Flip Skateboard") },
                { "poleFlipBMXCfg", Config.Bind("Misc", "Pole Flip BMX", "poleFlip", "Pole Flip BMX") },

                { "jumpTrickCfg", Config.Bind("Misc", "Trick Jump/Corkscrew", "jumpTrick1", "Trick Jump/Corkscrew") },
                { "jumpTrickInlineCfg", Config.Bind("Misc", "Trick Jump Inline/Corkscrew", "jumpTrick1", "Trick Jump Inline/Corkscrew") },
                { "jumpTrickSkateboardCfg", Config.Bind("Misc", "Trick Jump Skateboard/McTwist", "jumpTrick1", "Trick Jump Skateboard/McTwist") },
                { "jumpTrickBMXCfg", Config.Bind("Misc", "Trick Jump BMX/360 Backflip", "jumpTrick1", "Trick Jump BMX/360 Backflip") },

                { "goonCfg", Config.Bind("Misc", "Frameride", "protectArmsWideIdle", "Frameride") },
                { "goonInlineCfg", Config.Bind("Misc", "Frameride Inline", "grafSlashFinisher", "Frameride Inline") },
                { "goonSkateboardCfg", Config.Bind("Misc", "Frameride Skateboard", "knockBackBig", "Frameride Skateboard") },
                { "goonBMXCfg", Config.Bind("Misc", "Frameride BMX", "surrender", "Frameride BMX") },

                { "wallrideRightCfg", Config.Bind("Misc", "Wallride Right", "wallRunRight", "Wallride Right") },
                { "wallrideRightInlineCfg", Config.Bind("Misc", "Wallride Right Inline", "wallRunRight", "Wallride Right Inline") },
                { "wallrideRightSkateboardCfg", Config.Bind("Misc", "Wallride Right Skateboard", "wallRunRight", "Wallride Right Skateboard") },
                { "wallrideRightBMXCfg", Config.Bind("Misc", "Wallride Right BMX", "wallRunRight", "Wallride Right BMX") },

                { "wallrideLeftCfg", Config.Bind("Misc", "Wallride Left", "wallRunLeft", "Wallride Left") },
                { "wallrideLeftInlineCfg", Config.Bind("Misc", "Wallride Left Inline", "wallRunLeft", "Wallride Left Inline") },
                { "wallrideLeftSkateboardCfg", Config.Bind("Misc", "Wallride Left Skateboard", "wallRunLeft", "Wallride Left Skateboard") },
                { "wallrideLeftBMXCfg", Config.Bind("Misc", "Wallride Left BMX", "wallRunLeft", "Wallride Left BMX") },

                { "highSpeedWallrideRightCfg", Config.Bind("Misc", "Wallride Right (High Speed)", "wallSlideRight", "High Speed Wallride") },
                { "highSpeedWallrideRightInlineCfg", Config.Bind("Misc", "Wallride Right (High Speed) Inline", "wallRunRight", "Wallride Right (High Speed) Inline") },
                { "highSpeedWallrideRightSkateboardCfg", Config.Bind("Misc", "Wallride Right (High Speed) Skateboard", "wallRunRight", "Wallride Right (High Speed) Skateboard") },
                { "highSpeedWallrideRightBMXCfg", Config.Bind("Misc", "Wallride Right (High Speed) BMX", "wallRunRight", "Wallride Right (High Speed) BMX") },

                { "highSpeedWallrideLeftCfg", Config.Bind("Misc", "Wallride Left (High Speed)", "wallSlideLeft", "Wallride Left (High Speed)") },
                { "highSpeedWallrideLeftInlineCfg", Config.Bind("Misc", "Wallride Left (High Speed) Inline", "wallRunLeft", "Wallride Left (High Speed) Inline") },
                { "highSpeedWallrideLeftSkateboardCfg", Config.Bind("Misc", "Wallride Left (High Speed) Skateboard", "wallRunLeft", "Wallride Left (High Speed) Skateboard") },
                { "highSpeedWallrideLeftBMXCfg", Config.Bind("Misc", "Wallride Left (High Speed) BMX", "wallRunLeft", "Wallride Left (High Speed) BMX") }
            };
        }

        public static bool GetSwitchCfg()
        {
            return BPatchTC.enableSwitchCfg.Value;
        }

        public string GetConfigValueMisc(string key)
        {
            if (configEntries != null && configEntries.TryGetValue(key, out var entry))
            {
                return entry.Value;
            }
            else
            {
                Logger.LogWarning($"Configuration key '{key}' not found.");
                return null;
            }
        }

        [HarmonyPatch(typeof(BoostAbility), nameof(BoostAbility.OnStartAbility))]
        [HarmonyPrefix]
        private static bool OnStartAbilityPrefix(BoostAbility __instance)
        {
            VertAbilityPatches.overridingIdle = false;

            if (__instance == null) return true;
            if (BPatchTC.enableSwitchCfg.Value == false)
            {
                return true;
            }
            __instance.haveAirStartBoost = false;
            __instance.equippedMovestyleWasUsed = __instance.p != null ? __instance.p.usingEquippedMovestyle : false;
            __instance.SetState(BoostAbility.State.START_BOOST);
            return false;
        }

        //IDLE AND RUN SHIT
        [HarmonyPatch(typeof(Player), nameof(Player.FixedUpdatePlayer))]
        [HarmonyPostfix]
        private static void FixedUpdatePlayerPostfix(Player __instance)
        {


                string landRun = BPatchTC.Instance.GetConfigValueMisc("landRunCfg");
                string land = BPatchTC.Instance.GetConfigValueMisc("landCfg");
                string startRun = BPatchTC.Instance.GetConfigValueMisc("startRunCfg");
                string stopRun = BPatchTC.Instance.GetConfigValueMisc("stopRunCfg");
                string walk = BPatchTC.Instance.GetConfigValueMisc("walkCfg");
                string run = BPatchTC.Instance.GetConfigValueMisc("runCfg");
                string fall = BPatchTC.Instance.GetConfigValueMisc("fallCfg");
                string idle = BPatchTC.Instance.GetConfigValueMisc("idleCfg");
                string idleFidget1 = BPatchTC.Instance.GetConfigValueMisc("idleFidget1Cfg");

                string landRunInline = BPatchTC.Instance.GetConfigValueMisc("landRunCfgInline");
                string landInline = BPatchTC.Instance.GetConfigValueMisc("landCfgInline");
                string startRunInline = BPatchTC.Instance.GetConfigValueMisc("startRunCfgInline");
                string stopRunInline = BPatchTC.Instance.GetConfigValueMisc("stopRunCfgInline");
                string walkInline = BPatchTC.Instance.GetConfigValueMisc("walkCfgInline");
                string runInline = BPatchTC.Instance.GetConfigValueMisc("runCfgInline");
                string fallInline = BPatchTC.Instance.GetConfigValueMisc("fallCfgInline");
                string idleInline = BPatchTC.Instance.GetConfigValueMisc("idleCfgInline");
                string idleFidget1Inline = BPatchTC.Instance.GetConfigValueMisc("idleFidget1CfgInline");

                string landRunSkateboard = BPatchTC.Instance.GetConfigValueMisc("landRunCfgSkateboard");
                string landSkateboard = BPatchTC.Instance.GetConfigValueMisc("landCfgSkateboard");
                string startRunSkateboard = BPatchTC.Instance.GetConfigValueMisc("startRunCfgSkateboard");
                string stopRunSkateboard = BPatchTC.Instance.GetConfigValueMisc("stopRunCfgSkateboard");
                string walkSkateboard = BPatchTC.Instance.GetConfigValueMisc("walkCfgSkateboard");
                string runSkateboard = BPatchTC.Instance.GetConfigValueMisc("runCfgSkateboard");
                string fallSkateboard = BPatchTC.Instance.GetConfigValueMisc("fallCfgSkateboard");
                string idleSkateboard = BPatchTC.Instance.GetConfigValueMisc("idleCfgSkateboard");
                string idleFidget1Skateboard = BPatchTC.Instance.GetConfigValueMisc("idleFidget1CfgSkateboard");

                string landRunBMX = BPatchTC.Instance.GetConfigValueMisc("landRunCfgBMX");
                string landBMX = BPatchTC.Instance.GetConfigValueMisc("landCfgBMX");
                string startRunBMX = BPatchTC.Instance.GetConfigValueMisc("startRunCfgBMX");
                string stopRunBMX = BPatchTC.Instance.GetConfigValueMisc("stopRunCfgBMX");
                string walkBMX = BPatchTC.Instance.GetConfigValueMisc("walkCfgBMX");
                string runBMX = BPatchTC.Instance.GetConfigValueMisc("runCfgBMX");
                string fallBMX = BPatchTC.Instance.GetConfigValueMisc("fallCfgBMX");
                string idleBMX = BPatchTC.Instance.GetConfigValueMisc("idleCfgBMX");
                string idleFidget1BMX = BPatchTC.Instance.GetConfigValueMisc("idleFidget1CfgBMX");


                if (__instance.moveStyle == MoveStyle.INLINE)
                {
                    __instance.landHash = AnimationUtility.GetAnimationByName(landInline);
                    __instance.landRunHash = AnimationUtility.GetAnimationByName(landRunInline);
                    __instance.startRunHash = AnimationUtility.GetAnimationByName(startRunInline);
                    __instance.stopRunHash = AnimationUtility.GetAnimationByName(stopRunInline);
                    __instance.walkHash = AnimationUtility.GetAnimationByName(walkInline);
                    __instance.runHash = AnimationUtility.GetAnimationByName(runInline);
                    __instance.idleHash = AnimationUtility.GetAnimationByName(idleInline);
                    __instance.idleFidget1Hash = AnimationUtility.GetAnimationByName(idleFidget1Inline);
                    if (VertAbilityPatches.overridingIdle == true)
                    {
                        __instance.fallHash = AnimationUtility.GetAnimationByName("");
                    }
                    else
                        __instance.fallHash = AnimationUtility.GetAnimationByName(fallInline);
                }
                else if (__instance.moveStyle == MoveStyle.SKATEBOARD)
                {
                    __instance.landHash = AnimationUtility.GetAnimationByName(landSkateboard);
                    __instance.landRunHash = AnimationUtility.GetAnimationByName(landRunSkateboard);
                    __instance.startRunHash = AnimationUtility.GetAnimationByName(startRunSkateboard);
                    __instance.stopRunHash = AnimationUtility.GetAnimationByName(stopRunSkateboard);
                    __instance.walkHash = AnimationUtility.GetAnimationByName(walkSkateboard);
                    __instance.runHash = AnimationUtility.GetAnimationByName(runSkateboard);
                    __instance.idleHash = AnimationUtility.GetAnimationByName(idleSkateboard);
                    __instance.idleFidget1Hash = AnimationUtility.GetAnimationByName(idleFidget1Skateboard);
                    if (VertAbilityPatches.overridingIdle == true)
                    {
                        __instance.fallHash = AnimationUtility.GetAnimationByName("");
                    }
                    else
                        __instance.fallHash = AnimationUtility.GetAnimationByName(fallSkateboard);
                }
                else if (__instance.moveStyle == MoveStyle.BMX)
                {
                    __instance.landHash = AnimationUtility.GetAnimationByName(landBMX);
                    __instance.landRunHash = AnimationUtility.GetAnimationByName(landRunBMX);
                    __instance.startRunHash = AnimationUtility.GetAnimationByName(startRunBMX);
                    __instance.stopRunHash = AnimationUtility.GetAnimationByName(stopRunBMX);
                    __instance.walkHash = AnimationUtility.GetAnimationByName(walkBMX);
                    __instance.runHash = AnimationUtility.GetAnimationByName(runBMX);
                    __instance.idleHash = AnimationUtility.GetAnimationByName(idleBMX);
                    __instance.idleFidget1Hash = AnimationUtility.GetAnimationByName(idleFidget1BMX);
                    if (VertAbilityPatches.overridingIdle == true)
                    {
                        __instance.fallHash = AnimationUtility.GetAnimationByName("");
                    }
                    else
                        __instance.fallHash = AnimationUtility.GetAnimationByName(fallBMX);
                }
                else if (__instance.moveStyle == MoveStyle.ON_FOOT)
                {
                    __instance.landHash = AnimationUtility.GetAnimationByName(land);
                    __instance.landRunHash = AnimationUtility.GetAnimationByName(landRun);
                    __instance.startRunHash = AnimationUtility.GetAnimationByName(startRun);
                    __instance.stopRunHash = AnimationUtility.GetAnimationByName(stopRun);
                    __instance.walkHash = AnimationUtility.GetAnimationByName(walk);
                    __instance.runHash = AnimationUtility.GetAnimationByName(run);
                    __instance.idleHash = AnimationUtility.GetAnimationByName(idle);
                    __instance.idleFidget1Hash = AnimationUtility.GetAnimationByName(idleFidget1);
                    if (VertAbilityPatches.overridingIdle == true)
                    {
                        __instance.fallHash = AnimationUtility.GetAnimationByName("");
                    }
                    else
                        __instance.fallHash = AnimationUtility.GetAnimationByName(fall);
                }
                //MOVESTYLER FIX
                else
                {
                    if (VertAbilityPatches.nonVanillaMovestyle)
                    {
                        if (VertAbilityPatches.hasInitAnimForMovestyler == false)
                        {
                            __instance.InitAnimation();
                            VertAbilityPatches.hasInitAnimForMovestyler = true;
                        }
                    }
                }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.Jump))]
        [HarmonyPostfix]
        private static void JumpPostfix(Player __instance)
        {
            //MOVESTYLER AND LAUNCHER FIX
            if (VertAbilityPatches.nonVanillaMovestyle == false && __instance.onLauncher == false)
            {

                    string jump = BPatchTC.Instance.GetConfigValueMisc("jumpCfg");
                    string jumpInline = BPatchTC.Instance.GetConfigValueMisc("jumpCfgInline");
                    string jumpSkateboard = BPatchTC.Instance.GetConfigValueMisc("jumpCfgSkateboard");
                    string jumpBMX = BPatchTC.Instance.GetConfigValueMisc("jumpCfgBMX");

                    if (__instance.moveStyle == MoveStyle.INLINE)
                    {
                        __instance.PlayAnim(AnimationUtility.GetAnimationByName(jumpInline), false, false, -1f);
                    }
                    else if (__instance.moveStyle == MoveStyle.SKATEBOARD)
                    {
                        __instance.PlayAnim(AnimationUtility.GetAnimationByName(jumpSkateboard), false, false, -1f);
                    }
                    else if (__instance.moveStyle == MoveStyle.BMX)
                    {
                        __instance.PlayAnim(AnimationUtility.GetAnimationByName(jumpBMX), false, false, -1f);
                    }
                    else if (__instance.moveStyle == MoveStyle.ON_FOOT)
                    {
                        __instance.PlayAnim(AnimationUtility.GetAnimationByName(jump), false, false, -1f);
                    }
                //MOVESTYLER FIX
                else
                {
                    if (VertAbilityPatches.nonVanillaMovestyle)
                    {
                        if (VertAbilityPatches.hasInitAnimForMovestyler == false)
                        {
                            __instance.InitAnimation();
                            VertAbilityPatches.hasInitAnimForMovestyler = true;
                        }
                    }
                }

            }
            
        }

        [HarmonyPatch(typeof(HandplantAbility), nameof(HandplantAbility.FixedUpdateAbility))]
        [HarmonyPostfix]
        private static void FixedUpdateAbilityPostfix(HandplantAbility __instance)
        {

                string poleFreeze = BPatchTC.Instance.GetConfigValueMisc("poleFreezeCfg");
                string poleFreezeInline = BPatchTC.Instance.GetConfigValueMisc("poleFreezeInlineCfg");
                string poleFreezeSkateboard = BPatchTC.Instance.GetConfigValueMisc("poleFreezeSkateboardCfg");
                string poleFreezeBMX = BPatchTC.Instance.GetConfigValueMisc("poleFreezeBMXCfg");

                if (__instance.p.moveStyle == MoveStyle.ON_FOOT)
                {
                    __instance.poleFreezeHash = AnimationUtility.GetAnimationByName(poleFreeze);
                }
                if (__instance.p.moveStyle == MoveStyle.INLINE)
                {
                    __instance.poleFreezeHash = AnimationUtility.GetAnimationByName(poleFreezeInline);
                }
                if (__instance.p.moveStyle == MoveStyle.SKATEBOARD)
                {
                    __instance.poleFreezeHash = AnimationUtility.GetAnimationByName(poleFreezeSkateboard);
                }
                if (__instance.p.moveStyle == MoveStyle.BMX)
                {
                    __instance.poleFreezeHash = AnimationUtility.GetAnimationByName(poleFreezeBMX);
                }
            //MOVESTYLER FIX

                if (VertAbilityPatches.nonVanillaMovestyle)
                {
                    if (VertAbilityPatches.hasInitAnimForMovestyler == false)
                    {
                        __instance.p.InitAnimation();
                        VertAbilityPatches.hasInitAnimForMovestyler = true;
                    }
                }
        }

        [HarmonyPatch(typeof(SpecialAirAbility), nameof(SpecialAirAbility.FixedUpdateAbility))]
        [HarmonyPostfix]
        private static void FixedUpdateAbilityPostfix(SpecialAirAbility __instance)
        {
                string jumpTrick = BPatchTC.Instance.GetConfigValueMisc("jumpTrickCfg");
                string jumpTrickInline = BPatchTC.Instance.GetConfigValueMisc("jumpTrickInlineCfg");
                string jumpTrickSkateboard = BPatchTC.Instance.GetConfigValueMisc("jumpTrickSkateboardCfg");
                string jumpTrickBMX = BPatchTC.Instance.GetConfigValueMisc("jumpTrickBMXCfg");

                if (__instance.p.moveStyle == MoveStyle.ON_FOOT)
                {
                    __instance.jumpTrick1Hash = AnimationUtility.GetAnimationByName(jumpTrick);
                }
                if (__instance.p.moveStyle == MoveStyle.INLINE)
                {
                    __instance.jumpTrick1Hash = AnimationUtility.GetAnimationByName(jumpTrickInline);
                }
                if (__instance.p.moveStyle == MoveStyle.SKATEBOARD)
                {
                    __instance.jumpTrick1Hash = AnimationUtility.GetAnimationByName(jumpTrickSkateboard);
                }
                if (__instance.p.moveStyle == MoveStyle.BMX)
                {
                    __instance.jumpTrick1Hash = AnimationUtility.GetAnimationByName(jumpTrickBMX);
                }
            //MOVESTYLER FIX

                if (VertAbilityPatches.nonVanillaMovestyle)
                {
                    if (VertAbilityPatches.hasInitAnimForMovestyler == false)
                    {
                        __instance.p.InitAnimation();
                        VertAbilityPatches.hasInitAnimForMovestyler = true;
                    }
                }
        }


        [HarmonyPatch(typeof(FlipOutJumpAbility), nameof(FlipOutJumpAbility.FixedUpdateAbility))]
        [HarmonyPostfix]
        private static void FixedUpdateAbilityPostfix(FlipOutJumpAbility __instance)
        {

                string poleFlip = BPatchTC.Instance.GetConfigValueMisc("poleFlipCfg");
                string poleFlipInline = BPatchTC.Instance.GetConfigValueMisc("poleFlipInlineCfg");
                string poleFlipSkateboard = BPatchTC.Instance.GetConfigValueMisc("poleFlipSkateboardCfg");
                string poleFlipBMX = BPatchTC.Instance.GetConfigValueMisc("poleFlipBMXCfg");

                if (__instance.p.moveStyle == MoveStyle.ON_FOOT)
                {
                    __instance.poleFlipHash = AnimationUtility.GetAnimationByName(poleFlip);
                }
                if (__instance.p.moveStyle == MoveStyle.INLINE)
                {
                    __instance.poleFlipHash = AnimationUtility.GetAnimationByName(poleFlipInline);
                }
                if (__instance.p.moveStyle == MoveStyle.SKATEBOARD)
                {
                    __instance.poleFlipHash = AnimationUtility.GetAnimationByName(poleFlipSkateboard);
                }
                if (__instance.p.moveStyle == MoveStyle.BMX)
                {
                    __instance.poleFlipHash = AnimationUtility.GetAnimationByName(poleFlipBMX);
                }
            //MOVESTYLER FIX

                if (VertAbilityPatches.nonVanillaMovestyle)
                {
                    if (VertAbilityPatches.hasInitAnimForMovestyler == false)
                    {
                        __instance.p.InitAnimation();
                        VertAbilityPatches.hasInitAnimForMovestyler = true;
                    }
                }
        }


        //[HarmonyPatch(typeof(BoostAbility), nameof(BoostAbility.OnStopAbility))]
        //[HarmonyPostfix]
        //private static void OnStopAbilityPostfix(BoostAbility __instance)
        //{
        //    string airBoostBrake = BPatchTC.Instance.GetConfigValueMisc("airBoostBrakeCfg");
        //    string airBoostBrakeInline = BPatchTC.Instance.GetConfigValueMisc("airBoostBrakeInlineCfg");
        //    string airBoostBrakeSkateboard = BPatchTC.Instance.GetConfigValueMisc("airBoostBrakeSkateboardCfg");
        //    string airBoostBrakeBMX = BPatchTC.Instance.GetConfigValueMisc("airBoostBrakeBMXCfg");

        //    if (!__instance.p.IsGrounded())
        //    {
        //        if (__instance.p.moveStyle == MoveStyle.INLINE && airBoostBrakeInline != "")
        //        {
        //            __instance.p.PlayAnim(AnimationUtility.GetAnimationByName(airBoostBrakeInline), false, false, -1f);
        //        }
        //        if (__instance.p.moveStyle == MoveStyle.SKATEBOARD && airBoostBrakeSkateboard != "")
        //        {
        //            __instance.p.PlayAnim(AnimationUtility.GetAnimationByName(airBoostBrakeSkateboard), false, false, -1f);
        //        }
        //        if (__instance.p.moveStyle == MoveStyle.BMX && airBoostBrakeBMX != "")
        //        {
        //            __instance.p.PlayAnim(AnimationUtility.GetAnimationByName(airBoostBrakeBMX), false, false, -1f);
        //        }
        //        if (__instance.p.moveStyle == MoveStyle.ON_FOOT && airBoostBrake != "")
        //        {
        //            __instance.p.PlayAnim(AnimationUtility.GetAnimationByName(airBoostBrake), false, false, -1f);
        //        }
        //    }
        //}

        [HarmonyPatch(typeof(AirDashAbility), nameof(AirDashAbility.OnStartAbility))]
        [HarmonyPostfix]
        private static void OnStartAbilityPostfix(AirDashAbility __instance)
        {
            VertAbilityPatches.overridingIdle = false;
                if (__instance == null || __instance.p == null) return; // Null checks for __instance and __instance.p
                string configValueAirDash = BPatchTC.Instance.GetConfigValueMisc("airdashCfg");
                string configValueAirDashInline = BPatchTC.Instance.GetConfigValueMisc("airdashInlineCfg");
                string configValueAirDashSkateboard = BPatchTC.Instance.GetConfigValueMisc("airdashSkateboardCfg");
                string configValueAirDashBmx = BPatchTC.Instance.GetConfigValueMisc("airdashBmxCfg");

                if (__instance.p.moveAxisY != 0 && __instance.p.moveAxisX != 0)
                {
                    if (__instance.p.moveStyle == MoveStyle.INLINE)
                    {
                        __instance.p.PlayAnim(AnimationUtility.GetAnimationByName(configValueAirDashInline), true, true, -1f);
                    }
                    else if (__instance.p.moveStyle == MoveStyle.SKATEBOARD)
                    {
                        __instance.p.PlayAnim(AnimationUtility.GetAnimationByName(configValueAirDashSkateboard), true, true, -1f);
                    }
                    else if (__instance.p.moveStyle == MoveStyle.BMX)
                    {
                        __instance.p.PlayAnim(AnimationUtility.GetAnimationByName(configValueAirDashBmx), true, true, -1f);
                    }
                    else if (__instance.p.moveStyle == MoveStyle.ON_FOOT)
                    {
                        __instance.p.PlayAnim(AnimationUtility.GetAnimationByName(configValueAirDash), true, true, -1f);
                    }
            //MOVESTYLER FIX
            else
                {
                    if (VertAbilityPatches.nonVanillaMovestyle)
                    {
                        if (VertAbilityPatches.hasInitAnimForMovestyler == false)
                        {
                            __instance.p.InitAnimation();
                            VertAbilityPatches.hasInitAnimForMovestyler = true;
                        }
                    }
                }
            }
        }

    }

    [HarmonyPatch(typeof(BoostAbility))]
    [HarmonyPatch("SetState")]
    public static class SetStatePatch
    {
        [HarmonyPostfix]
        public static void Postfix(BoostAbility __instance, BoostAbility.State setState)
        {
                if (__instance == null || __instance.p == null) return; // Null checks for __instance and __instance.p

                string configValueAirBoost = BPatchTC.Instance.GetConfigValueMisc("airBoostCfg");
                string configValueGroundBoost = BPatchTC.Instance.GetConfigValueMisc("groundBoostCfg");
                string configValueAirBoostInline = BPatchTC.Instance.GetConfigValueMisc("airBoostInlineCfg");
                string configValueGroundBoostInline = BPatchTC.Instance.GetConfigValueMisc("groundBoostInlineCfg");
                string configValueAirBoostSkateboard = BPatchTC.Instance.GetConfigValueMisc("airBoostSkateboardCfg");
                string configValueGroundBoostSkateboard = BPatchTC.Instance.GetConfigValueMisc("groundBoostSkateboardCfg");
                string configValueAirBoostBMX = BPatchTC.Instance.GetConfigValueMisc("airBoostBMXCfg");
                string configValueGroundBoostBMX = BPatchTC.Instance.GetConfigValueMisc("groundBoostBMXCfg");

                string configValueBoostBrakeInline = BPatchTC.Instance.GetConfigValueMisc("boostBrakeInlineCfg");
                string configValueBoostBrakeSkateboard = BPatchTC.Instance.GetConfigValueMisc("boostBrakeSkateboardCfg");
                string configValueBoostBrakeBMX = BPatchTC.Instance.GetConfigValueMisc("boostBrakeBMXCfg");
                string configValueBoostBrake = BPatchTC.Instance.GetConfigValueMisc("boostBrakeCfg");

                string configValueBoostStartInline = BPatchTC.Instance.GetConfigValueMisc("boostStartInlineCfg");
                string configValueBoostStartSkateboard = BPatchTC.Instance.GetConfigValueMisc("boostStartSkateboardCfg");
                string configValueBoostStartBMX = BPatchTC.Instance.GetConfigValueMisc("boostStartBMXCfg");
                string configValueBoostStart = BPatchTC.Instance.GetConfigValueMisc("boostStartCfg");

                string jump = BPatchTC.Instance.GetConfigValueMisc("jumpCfg");
                string jumpInline = BPatchTC.Instance.GetConfigValueMisc("jumpCfgInline");
                string jumpSkateboard = BPatchTC.Instance.GetConfigValueMisc("jumpCfgSkateboard");
                string jumpBMX = BPatchTC.Instance.GetConfigValueMisc("jumpCfgBMX");

                if (setState == BoostAbility.State.BOOST)
                {
                    if (__instance.p.moveStyle == MoveStyle.INLINE)
                    {
                        __instance.jumpHash = AnimationUtility.GetAnimationByName(jumpInline);
                        __instance.boostRunHash = AnimationUtility.GetAnimationByName(configValueAirBoostInline);
                        int animationHash = __instance.p.IsGrounded() ? AnimationUtility.GetAnimationByName(configValueGroundBoostInline) : __instance.boostRunHash;
                        __instance.p.PlayAnim(animationHash, false, false, -1f);
                    }
                    if (__instance.p.moveStyle == MoveStyle.SKATEBOARD)
                    {
                        __instance.jumpHash = AnimationUtility.GetAnimationByName(jumpSkateboard);
                        __instance.boostRunHash = AnimationUtility.GetAnimationByName(configValueAirBoostSkateboard);
                        int animationHash = __instance.p.IsGrounded() ? AnimationUtility.GetAnimationByName(configValueGroundBoostSkateboard) : __instance.boostRunHash;
                        __instance.p.PlayAnim(animationHash, false, false, -1f);
                    }
                    if (__instance.p.moveStyle == MoveStyle.BMX)
                    {
                        __instance.jumpHash = AnimationUtility.GetAnimationByName(jumpBMX);
                        __instance.boostRunHash = AnimationUtility.GetAnimationByName(configValueAirBoostBMX);
                        int animationHash = __instance.p.IsGrounded() ? AnimationUtility.GetAnimationByName(configValueGroundBoostBMX) : __instance.boostRunHash;
                        __instance.p.PlayAnim(animationHash, false, false, -1f);
                    }
                    if (__instance.p.moveStyle == MoveStyle.ON_FOOT)
                    {
                        __instance.jumpHash = AnimationUtility.GetAnimationByName(jump);
                        __instance.boostRunHash = AnimationUtility.GetAnimationByName(configValueAirBoost);
                        int animationHash = __instance.p.IsGrounded() ? AnimationUtility.GetAnimationByName(configValueGroundBoost) : __instance.boostRunHash;
                        __instance.p.PlayAnim(animationHash, false, false, -1f);
                    }
                //MOVESTYLER FIX

                    if (VertAbilityPatches.nonVanillaMovestyle)
                    {
                        if (VertAbilityPatches.hasInitAnimForMovestyler == false)
                        {
                            __instance.p.InitAnimation();
                            VertAbilityPatches.hasInitAnimForMovestyler = true;
                        }
                    }
        }
                if (setState == BoostAbility.State.BRAKE)
                {
                    if (__instance.p.moveStyle == MoveStyle.INLINE)
                    {
                        __instance.boostBrakeHash = AnimationUtility.GetAnimationByName(configValueBoostBrakeInline);
                    }
                    if (__instance.p.moveStyle == MoveStyle.SKATEBOARD)
                    {
                        __instance.boostBrakeHash = AnimationUtility.GetAnimationByName(configValueBoostBrakeSkateboard);
                    }
                    if (__instance.p.moveStyle == MoveStyle.BMX)
                    {
                        __instance.boostBrakeHash = AnimationUtility.GetAnimationByName(configValueBoostBrakeBMX);
                    }
                    if (__instance.p.moveStyle == MoveStyle.ON_FOOT)
                    {
                        __instance.boostBrakeHash = AnimationUtility.GetAnimationByName(configValueBoostBrake);
                    }
                //MOVESTYLER FIX

                    if (VertAbilityPatches.nonVanillaMovestyle)
                    {
                        if (VertAbilityPatches.hasInitAnimForMovestyler == false)
                        {
                            __instance.p.InitAnimation();
                            VertAbilityPatches.hasInitAnimForMovestyler = true;
                        }
                    }
            }

                if (setState == BoostAbility.State.START_BOOST)
                {
                    if (__instance.p.moveStyle == MoveStyle.INLINE)
                    {
                        __instance.startBoostHash = AnimationUtility.GetAnimationByName(configValueBoostStartInline);
                    }
                    if (__instance.p.moveStyle == MoveStyle.SKATEBOARD)
                    {
                        __instance.startBoostHash = AnimationUtility.GetAnimationByName(configValueBoostStartSkateboard);
                    }
                    if (__instance.p.moveStyle == MoveStyle.BMX)
                    {
                        __instance.startBoostHash = AnimationUtility.GetAnimationByName(configValueBoostStartBMX);
                    }
                    if (__instance.p.moveStyle == MoveStyle.ON_FOOT)
                    {
                        __instance.startBoostHash = AnimationUtility.GetAnimationByName(configValueBoostStart);
                    }
                //MOVESTYLER FIX

                    if (VertAbilityPatches.nonVanillaMovestyle)
                    {
                        if (VertAbilityPatches.hasInitAnimForMovestyler == false)
                        {
                            __instance.p.InitAnimation();
                            VertAbilityPatches.hasInitAnimForMovestyler = true;
                        }
                    }
            }
        }
    }
}
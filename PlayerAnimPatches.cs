using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Reptile;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace trickyclown
{
    public class ABPatchesTC
    {
        public static bool ParsedOptions { get; private set; } = false;
        public static Dictionary<int, Dictionary<int, float>> customBlendingFromTo = new();
        public static Dictionary<int, float> alwaysBlendOut = new(); 
        public static Dictionary<int, float> alwaysBlendIn = new();

        public static List<string> importantRules = new(); 

        public static void ParseCustomBlending()
        {
            customBlendingFromTo.Clear();
            alwaysBlendIn.Clear();
            alwaysBlendOut.Clear();

            string customsUnsplit = BPatchTC.customAnimationBlending.Value;
            if (string.IsNullOrWhiteSpace(customsUnsplit)) return;

            int numberOfFailures = 0;
            string[] customs = customsUnsplit.Replace(", ", ",").Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string custom in customs)
            {
                string trimmedCustom = custom.Trim();
                if (string.IsNullOrWhiteSpace(trimmedCustom)) continue;
                bool importantRule = trimmedCustom.EndsWith("!"); 
                if (importantRule) { trimmedCustom = trimmedCustom.Substring(0, trimmedCustom.Length - 1); }
                try
                {
                    int colonIndex = trimmedCustom.LastIndexOf(':');
                    if (colonIndex == -1) 
                        throw new FormatException("Missing blend time separator (:)");

                    string selector = trimmedCustom.Substring(0, colonIndex);
                    string blendValueString = trimmedCustom.Substring(colonIndex + 1);

                    if (selector.Contains('|')) {
                        string[] selectors = selector.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        foreach (string miniSelector in selectors)
                            ParseSelectorValuePair(miniSelector.Trim(), blendValueString, importantRule); 
                    } else { 
                        ParseSelectorValuePair(selector, blendValueString, importantRule); 
                    }
                } catch (Exception ex) {
                    Debug.LogWarning($"(NewTrix/AB) Failed parsing custom animation blending string '{trimmedCustom}': {ex.Message}");
                    numberOfFailures++; 
                }
            }

            if (numberOfFailures == 0) { Debug.Log("(NewTrix/AB) All custom animation blending options successfully parsed!"); }
            else { Debug.LogWarning($"(NewTrix/AB) All custom animation blending options parsed with {numberOfFailures} failures)"); }
            ParsedOptions = true;
        }

        public static void ParseSelectorValuePair(string selector, string blendValueString, bool importantRule = false) {
            List<float> blend = new(); 
            string[] splitByArrows = selector.Split('>');

            int slashIndex = blendValueString.LastIndexOf('/');
            if (slashIndex != -1) {
                if (splitByArrows.Length == 2) { // TwoAnimationRules can only have a single blend value
                    throw new FormatException($"Blend time format '{blendValueString}' invalid for selector '{selector}' - check formatting");
                }
                
                string blendValue1 = blendValueString.Substring(0, slashIndex);
                string blendValue2 = blendValueString.Substring(slashIndex + 1);
                if (float.TryParse(blendValue1, out float blend1) && float.TryParse(blendValue2, out float blend2)) {
                    blend.Add(blend1); blend.Add(blend2); 
                } else {
                    throw new FormatException($"Blend time '{blendValueString}' invalid - check formatting");
                }
            } else {
                if (!float.TryParse(blendValueString, out float blend1))
                    throw new FormatException($"Blend time '{blendValueString}' invalid - check formatting");
                blend.Add(blend1); 
            }

            switch (splitByArrows.Length)
            {
                case 1: // "animation:outBlend" (equivalent to "animation>*:blend") OR "animation:inBlend/outBlend"
                    HandleSingleAnimationRule(splitByArrows[0], blend.ToArray(), importantRule);
                    break;
                case 2: // "animation1>animation2:blend"
                    HandleTwoAnimationRule(splitByArrows[0], splitByArrows[1], blend[0], importantRule);
                    break;
                case 3: // "animation1>animation2>animation3:inOutBlend"      OR   "animation1>animation2>animation3:inBlend/outBlend" 
                    //   ("anim1>anim2:inOutBlend; anim2>anim3:inOutBlend;)          ("anim1>anim2:inBlend; anim2>anim3: outBlend;)
                    HandleThreeAnimationRule(splitByArrows[0], splitByArrows[1], splitByArrows[2], blend.ToArray(), importantRule);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Improper formatting - number of arrows (>) outside of range (0-2)");
            }
        }

        private static void HandleSingleAnimationRule(string fromPart, float[] blend, bool importantRule) 
        {
            int animation = AnimationUtility.GetAnimationByName(fromPart.Trim());
            if (blend.Count() == 1) { 
                if (importantRule) { importantRules.Add(PartsToRule(animation.ToString(), "*", blend[0])); }
                AlwaysBlendOut(animation, blend[0]);
            } else if (blend.Count() == 2) { 
                if (importantRule) { 
                    importantRules.Add(PartsToRule("*", animation.ToString(), blend[0]));
                    importantRules.Add(PartsToRule(animation.ToString(), "*", blend[1])); 
                }
                AlwaysBlendIn(animation, blend[0]);
                AlwaysBlendOut(animation, blend[1]);
            } else { throw new ArgumentOutOfRangeException("Blend value count outside of range (1-2)"); }
        }

        private static void HandleTwoAnimationRule(string fromPart, string toPart, float blend, bool importantRule) {
            string trimmedFrom = fromPart.Trim();
            string trimmedTo = toPart.Trim();
            bool alwaysOut = trimmedTo == "*";
            bool alwaysIn = trimmedFrom == "*";

            if (importantRule) { 
                importantRules.Add(PartsToRule(
                    (string)(alwaysIn ? "*" : AnimationUtility.GetAnimationByName(trimmedFrom).ToString()), 
                    (string)(alwaysOut ? "*" : AnimationUtility.GetAnimationByName(trimmedTo).ToString()), 
                    blend
                )); 
            }

            if (alwaysOut && alwaysIn) {
                throw new Exception("Improper rule - use defaultAnimationBlend in config instead!");
            } else if (alwaysOut) {
                AlwaysBlendOut(AnimationUtility.GetAnimationByName(trimmedFrom), blend);
            } else if (alwaysIn) {
                AlwaysBlendIn(AnimationUtility.GetAnimationByName(trimmedTo), blend);
            } else {
                AddCustomBlending(AnimationUtility.GetAnimationByName(trimmedFrom), AnimationUtility.GetAnimationByName(trimmedTo), blend);
            }
        }

        private static void HandleThreeAnimationRule(string startPart, string middlePart, string endPart, float[] blend, bool importantRule) {
            if (blend.Count() > 2 || blend.Count() < 1) { throw new ArgumentOutOfRangeException("Blend value count outside of range (1-2)"); }
            float blend1 = blend[0];
            float blend2 = blend.Count() > 1 ? blend[1] : blend[0]; 
            HandleTwoAnimationRule(startPart, middlePart, blend1, importantRule);
            HandleTwoAnimationRule(middlePart, endPart, blend2, importantRule); 
        }

        private static void AddCustomBlending(int animationFrom, int animationTo, float blend) {
            if (!customBlendingFromTo.ContainsKey(animationFrom)) { customBlendingFromTo[animationFrom] = new(); }
            Dictionary<int, float> value = customBlendingFromTo[animationFrom];

            if (value.ContainsKey(animationTo)) { 
               string originalRule = PartsToRule(animationFrom.ToString(), animationTo.ToString(), value[animationTo]); 
               string newRule = PartsToRule(animationFrom.ToString(), animationTo.ToString(), blend); 
               
               if (IsImportantRule(originalRule) && !IsImportantRule(newRule)) { return; }
               else if (IsImportantRule(originalRule)) { importantRules.Remove(originalRule); }
            }

            value[animationTo] = blend;
            //Debug.Log($"{animationFrom} -->> {animationTo} equals {blend}");
        }

        private static void AlwaysBlendIn(int part, float blend) {
            if (alwaysBlendIn.ContainsKey(part)) { 
                string currentRule = PartsToRule("*", part.ToString(), alwaysBlendIn[part]);
                bool currentRuleImportant = IsImportantRule(currentRule); 
                bool newRuleImportant = IsImportantRule("*", part.ToString(), blend); 

                if ((currentRuleImportant && newRuleImportant) || !currentRuleImportant) { 
                    alwaysBlendIn[part] = blend; 
                    if (currentRuleImportant) importantRules.Remove(currentRule);
                }
            } else { alwaysBlendIn[part] = blend; }
        }

        private static void AlwaysBlendOut(int part, float blend) {
            if (alwaysBlendOut.ContainsKey(part)) { 
                bool currentRuleImportant = IsImportantRule(part.ToString(), "*", alwaysBlendOut[part]); 
                bool newRuleImportant = IsImportantRule(part.ToString(), "*", blend); 
                if ((currentRuleImportant && newRuleImportant) || !currentRuleImportant) { 
                    alwaysBlendOut[part] = blend; 
                }
            } else { alwaysBlendOut[part] = blend; }
        }

        public static bool TryGetAnimationBlendValue(int animationFrom, int animationTo, out float blendReturnValue)
        {
            // hierarchy: important custom rule > important alwaysInOut > custom rule > alwaysInOut > fallbacks > 0f

            float customBlendValue = 0f;
            bool hasCustomRule = customBlendingFromTo.TryGetValue(animationFrom, out Dictionary<int, float> customRules);
            if (hasCustomRule) { hasCustomRule = customRules.TryGetValue(animationTo, out customBlendValue); }

            bool hasAlwaysOut = alwaysBlendOut.TryGetValue(animationFrom, out float alwaysOutValue);
            bool hasAlwaysIn = alwaysBlendIn.TryGetValue(animationTo, out float alwaysInValue);

            // 1. important custom rule
            if (hasCustomRule)
            {
                string specificRule = PartsToRule(animationFrom.ToString(), animationTo.ToString(), customBlendValue);
                if (IsImportantRule(specificRule) || !(hasAlwaysOut || hasAlwaysIn))
                {
                    blendReturnValue = customBlendValue;
                    return true;
                }
            }

            // 2. important alwaysIn/Out rules
            string outRule = hasAlwaysOut ? PartsToRule(animationFrom.ToString(), "*", alwaysOutValue) : null;
            string inRule = hasAlwaysIn ? PartsToRule("*", animationTo.ToString(), alwaysInValue) : null;

            if (hasAlwaysOut && IsImportantRule(outRule))
            {
                blendReturnValue = alwaysOutValue;
                return true;
            } else if (hasAlwaysIn && IsImportantRule(inRule))
            {
                blendReturnValue = alwaysInValue;
                return true;
            }

            // 3. unimportant rules
            if (hasCustomRule) {
                blendReturnValue = customBlendValue;
                return true;
            } else if (hasAlwaysOut) {
                blendReturnValue = alwaysOutValue;
                return true;
            } else if (hasAlwaysIn) {
                blendReturnValue = alwaysInValue;
                return true;
            }

            // 4. fallbacks (defaultIn/Out)
            bool fromIsBOEAnim = BunchOfEmotesSupport.Installed && BunchOfEmotesSupport.GameAnimationIsCustomAnimation(animationFrom);
            bool toIsBOEAnim = BunchOfEmotesSupport.Installed && BunchOfEmotesSupport.GameAnimationIsCustomAnimation(animationTo);

            if (fromIsBOEAnim) {
                blendReturnValue = BPatchTC.defaultAnimationBlendingOut.Value;
                return false;
            } else if (toIsBOEAnim) {
                blendReturnValue = BPatchTC.defaultAnimationBlendingIn.Value;
                return false;
            }

            blendReturnValue = 0f;
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StageManager), nameof(StageManager.DoStagePostInitialization))]
        public static void CheckToParse() {
            if (!ParsedOptions) ParseCustomBlending();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Player), nameof(Player.PlayAnim))]
        public static bool PlayAnimBlendingPrefix(int newAnim, bool forceOverwrite, bool instant, float atTime, Player __instance) 
        {
            if (__instance != WorldHandler.instance?.GetCurrentPlayer()) return true;

            if (!BPatchTC.enableAnimationBlending.Value || !__instance.gameObject.activeSelf 
            || (newAnim == __instance.curAnim && !forceOverwrite)) { return true; }

            bool isFallbackBlendValue = !TryGetAnimationBlendValue(__instance.curAnim, newAnim, out var blendValue);  
            bool isImportantRule = IsImportantRule(__instance.curAnim.ToString(), newAnim.ToString(), blendValue);   
			if ((blendValue > 0 || !isFallbackBlendValue) && (!instant || isImportantRule) && atTime == -1f)
			{
				__instance.anim.CrossFade(newAnim, blendValue);
                __instance.curAnimActiveTime = 0f;
                __instance.firstFrameAnim = true;
                __instance.characterVisual.feetIK = __instance.animInfos.ContainsKey(newAnim) && __instance.animInfos[newAnim].feetIK;
                __instance.curAnim = newAnim;
                return false; 
			}
            
            //Debug.Log($"{__instance.curAnim} -->> {newAnim} equals {blendValue} (is fallback: {isFallbackBlendValue})");    
            return true;
        }

        private static bool IsImportantRule(string f, string t, float b) { return IsImportantRule(PartsToRule(f,t,b)); }
        private static string PartsToRule(string f, string t, float b) { return $"{f}>{t}:{b.ToString()}"; }
        private static bool IsImportantRule(string rule) { return importantRules.Contains(rule); }
    }
}
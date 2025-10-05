using BepInEx.Bootstrap;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace trickyclown
{
    public static class BunchOfEmotesSupport
    {
        public static bool Installed { get; private set; } = false;
        private static object BoEPluginInstance = null;
        private static Type BoEPluginType = null;
        private static FieldInfo CustomAnimsField = null;
        private static bool Cached = false;
        private static Dictionary<string, int> GameAnimationByCustomAnimation = new();

        public static void Initialize()
        {
            Installed = true;
            BoEPluginInstance = Chainloader.PluginInfos["com.Dragsun.BunchOfEmotes"].Instance;
            BoEPluginType = ReflectionUtility.GetTypeByName("BunchOfEmotes.BunchOfEmotesPlugin");
            CustomAnimsField = BoEPluginType.GetField("myCustomAnims2");
        }

        public static void CacheAnimationsIfNecessary()
        {
            if (Cached) return;
            var boeDictionary = CustomAnimsField.GetValue(BoEPluginInstance) as Dictionary<int, string>;
            if (boeDictionary != null && boeDictionary.Count > 0)
                Cached = true;
            else
                return;
            foreach (var customAnim in boeDictionary)
            {
                var gameAnim = customAnim.Key;
                GameAnimationByCustomAnimation[customAnim.Value] = gameAnim;
            }
        }

        public static bool TryGetGameAnimationForCustomAnimation(string customAnim, out int gameAnim)
        {
            if (!Installed)
            {
                gameAnim = 0;
                return false;
            }
            CacheAnimationsIfNecessary();
            if (GameAnimationByCustomAnimation.TryGetValue(customAnim, out gameAnim))
                return true;
            return false;
        }

        public static bool GameAnimationIsCustomAnimation(int gameAnim) { return GameAnimationByCustomAnimation.ContainsValue(gameAnim); }
    }
}
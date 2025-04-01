using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Drawing;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace NameChanger
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class AntiDexterPlugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "pinstudios.AntiDexter";
        public const string PLUGIN_NAME = "AntiDexter";
        public const string PLUGIN_VERSION = "1.0.0";

        internal static readonly ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource(PLUGIN_NAME);

        //lobby name color 333333ff
        //in game color ffff
        private static string RGBToHex(UnityEngine.Color color)
        {
            int r = Mathf.RoundToInt(color.r * 255f);
            int g = Mathf.RoundToInt(color.g * 255f);
            int b = Mathf.RoundToInt(color.b * 255f);
            return r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
        }

        private static void WorldSpaceUIParent_PlayerNameHook(Action<WorldSpaceUIParent, PlayerAvatar> orig, WorldSpaceUIParent self, PlayerAvatar player)
        {
            orig(self, player);
            UpdatePlayerNametagColor(player);
        }

        private static void PlayerAvatarVisuals_SetColorHook(Action<PlayerAvatarVisuals, int, UnityEngine.Color> orig, PlayerAvatarVisuals self, int _colorIndex, UnityEngine.Color _setColor)
        {
            orig(self, _colorIndex, _setColor);
            UpdatePlayerNametagColor(self.playerAvatar);
        }

        private static void UpdatePlayerNametagColor(PlayerAvatar player)
        {
            if (player.worldSpaceUIPlayerName == null)
                return;

            UnityEngine.Color color = player.playerAvatarVisuals.color;

            color.r += 0.666f;
            color.g += 0.666f;
            color.b += 0.666f;
            color = color * (1f / Mathf.Max(color.r, color.g, color.b));

            string hex = RGBToHex(color);
            player.worldSpaceUIPlayerName.text.text = $"<color=#{hex}>{player.playerName}";
        }

        private void Awake()
        {
            new Hook(AccessTools.Method(typeof(WorldSpaceUIParent), "PlayerName"), WorldSpaceUIParent_PlayerNameHook);
            new Hook(AccessTools.Method(typeof(PlayerAvatarVisuals), "SetColor"), PlayerAvatarVisuals_SetColorHook);
            new ILHook(typeof(WorldSpaceUIPlayerName).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance), SetPlayerNameTransparency);
            
            Harmony harmony = new Harmony(PLUGIN_GUID);
            harmony.PatchAll();
            
            logger.LogInfo($"{PLUGIN_NAME} v{PLUGIN_VERSION} 已加载!");
        }

        private static void SetPlayerNameTransparency(ILContext il)
        {
            ILCursor cursor = new ILCursor(il).Goto(0);

            if (cursor.TryGotoNext
            (
                i => i.MatchLdcR4(0.5f),
                i => i.MatchNewobj<UnityEngine.Color>()
            ))
            {
                cursor.Remove();
                cursor.Emit(Mono.Cecil.Cil.OpCodes.Ldc_R4, 0.75f);
            }
        }
    }
}
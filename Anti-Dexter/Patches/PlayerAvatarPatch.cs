using HarmonyLib;
using UnityEngine;
using NameColorizer;
using System.Reflection;
using TMPro;

namespace AntiDexter.Patches
{
    // 为PlayerAvatar添加补丁
    [HarmonyPatch(typeof(PlayerAvatar))]
    internal class PlayerAvatarPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void StartPatch(PlayerAvatar __instance)
        {
            try
            {
                // 检查是否有PhotonView组件
                if (__instance.photonView != null)
                {
                    // 获取当前玩家名称
                    string currentName = __instance.photonView.Owner.NickName;
                    
                    // 检查是否为Dexter
                    if (string.Equals(currentName, "Dexter", System.StringComparison.OrdinalIgnoreCase))
                    {
                        // 尝试找到并修改玩家名称显示
                        // 方法1: 使用worldSpaceUIPlayerName
                        if (__instance.worldSpaceUIPlayerName != null && __instance.worldSpaceUIPlayerName.text != null)
                        {
                            __instance.worldSpaceUIPlayerName.text.text = "Mohamad";
                            ColoredNametagMod.logger.LogInfo($"方法1: 检测到目标名称: {currentName}，已替换为: Mohamad");
                            return;
                        }
                        
                        // 方法2: 查找所有TextMeshPro组件
                        TextMeshPro[] textComponents = __instance.GetComponentsInChildren<TextMeshPro>(true);
                        foreach (TextMeshPro text in textComponents)
                        {
                            if (text.text.Contains(currentName))
                            {
                                text.text = text.text.Replace(currentName, "Mohamad");
                                ColoredNametagMod.logger.LogInfo($"方法2: 检测到目标名称: {currentName}，已替换为: Mohamad");
                                return;
                            }
                        }
                        
                        // 方法3: 使用反射查找playerName字段
                        FieldInfo playerNameField = typeof(PlayerAvatar).GetField("playerName", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        if (playerNameField != null)
                        {
                            object playerNameObj = playerNameField.GetValue(__instance);
                            if (playerNameObj is TextMeshPro textMesh)
                            {
                                textMesh.text = "Mohamad";
                                ColoredNametagMod.logger.LogInfo($"方法3: 检测到目标名称: {currentName}，已替换为: Mohamad");
                                return;
                            }
                        }
                        
                        ColoredNametagMod.logger.LogWarning($"无法找到合适的方法修改玩家名称: {currentName}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                // 记录错误
                ColoredNametagMod.logger.LogError($"修改名称时发生错误: {ex.Message}");
            }
        }
    }
} 
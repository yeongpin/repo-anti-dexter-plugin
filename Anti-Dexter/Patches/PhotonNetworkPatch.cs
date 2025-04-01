using HarmonyLib;
using Photon.Pun;
using NameChanger;

namespace AntiDexter.Patches
{
    // 为PhotonNetwork添加补丁
    [HarmonyPatch(typeof(PhotonNetwork))]
    internal class PhotonNetworkPatch
    {
        [HarmonyPatch("SetPlayerCustomProperties")]
        [HarmonyPrefix]
        private static void SetPlayerCustomPropertiesPatch()
        {
            try
            {
                // 检查本地玩家名称
                if (PhotonNetwork.LocalPlayer != null)
                {
                    string currentName = PhotonNetwork.LocalPlayer.NickName;
                    
                    // 检查是否为Dexter
                    if (string.Equals(currentName, "Dexter", System.StringComparison.OrdinalIgnoreCase))
                    {
                        // 设置新的昵称
                        PhotonNetwork.LocalPlayer.NickName = "Mohamad";
                        AntiDexterPlugin.logger.LogInfo($"检测到目标名称: {currentName}，已替换为: Mohamad");
                    }
                }
            }
            catch (System.Exception ex)
            {
                // 记录错误
                AntiDexterPlugin.logger.LogError($"修改名称时发生错误: {ex.Message}");
            }
        }
    }
} 
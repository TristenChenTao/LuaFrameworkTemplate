using UnityEngine;
using FairyGUI;
using LuaFramework;

/// <summary>
/// Extend the ability of UIPackage
/// </summary>
public class ExUIPackage
{
    public static void AddPackage(string fileName)
    {
        if(AppConst.LuaBundleMode && AppConst.DebugMode == false)        {
            string url = Util.DataPath + AppConst.UIDir.ToLower() + fileName.ToLower() + AppConst.ExtName; 
            AssetBundle ab = AssetBundle.LoadFromFile(url);
            if (ab)
            {
                UIPackage.AddPackage(ab);
            }
            else
            {
                Debug.LogError("ab包：" + fileName + "不存在，请检查！");
            }
        }
        else
        {
            UIPackage.AddPackage(AppConst.UIDir + fileName);
        }
    }
}

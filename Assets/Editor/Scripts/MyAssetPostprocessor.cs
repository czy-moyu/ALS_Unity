using UnityEngine;
using UnityEditor;

public class MyAssetPostprocessor : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        // 检查是否有脚本文件被重新编译
        bool recompiled = false;
        foreach (string asset in importedAssets)
        {
            if (asset.EndsWith(".cs"))
            {
                recompiled = true;
                break;
            }
        }

        // 如果有脚本被重新编译，则调用回调函数
        if (recompiled)
        {
            OnScriptsRecompiled();
        }
    }

    private static void OnScriptsRecompiled()
    {
        
    }
}
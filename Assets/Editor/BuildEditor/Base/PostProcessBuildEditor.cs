using UnityEngine;
using System.Collections;
using UnityEditor.Callbacks;
using UnityEditor;


namespace UnityEditor.XBuild
{

    public sealed class PostProcessBuildEditor
    {

        [PostProcessBuild]
        static void OnBuildingEnd(BuildTarget target, string path)
        {
            ProjectSettingIOS ps = null;
#if UNITY_IOS //tencent
            ps = new ProjectSettingIOS_Tencent();
#endif

            if (ps == null)
            {
                Debug.LogError("No platform matched, please check it!");
                return;
            }

            ps.PostProcessBuild(target, path);

            // string autoPackageScriptPath = Path.Combine(path, "build.sh");
            // CreatePackageScript(autoPackageScriptPath);
            // RunPackageScript(autoPackageScriptPath);

            Debug.Log("Build Task over !");
        }
    }


}
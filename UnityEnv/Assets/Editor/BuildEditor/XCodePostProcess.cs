using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;
using System.IO;

public static class XCodePostProcess
{

    [PostProcessBuild]
	public static void OnPostProcessBuild( BuildTarget target, string path )
	{
		if (target != BuildTarget.iOS) 
        {
			Debug.LogWarning("Target is not iPhone. XCodePostProcess will not run");
			return;
		}

		// Create a new project object from build target
		XCProject project = new XCProject( path );

		// Find and run through all projmods files to patch the project.
		//Please pay attention that ALL projmods files in your project folder will be excuted!

        string basePath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/"));
        basePath = basePath.Substring(0, basePath.LastIndexOf("/"));
        string file = basePath + "/Mods/game.projmods";
        project.ApplyMod(file);
		// Finally save the xcode project
		project.Save();
	}
}
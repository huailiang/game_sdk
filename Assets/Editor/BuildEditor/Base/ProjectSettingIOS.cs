using UnityEngine;
using UnityEditor;
using System.Text;
using System.Collections.Generic;

namespace UnityEditor.XBuild
{

    public class IOSURLTypeItem
    {
        public string URLName { get; set; }
        public string URLSchemes { get; set; }

        public IOSURLTypeItem() : this("", "") { }

        public IOSURLTypeItem(string name, string scheme)
        {
            this.URLName = name;
            this.URLSchemes = scheme;
        }

        public string getInnerDescription()
        {
            string des =
    @"<dict>
	<key>CFBundleURLName</key>
	<string>" + URLName + @"</string>
	<key>CFBundleURLSchemes</key>
	<array>
		<string>" + URLSchemes + @"</string>
	</array>
</dict>";
            return des;
        }
    }

    public class URLTypesSection
    {
        public URLTypesSection() { }

        private List<IOSURLTypeItem> items = new List<IOSURLTypeItem>();

        public URLTypesSection AddItem(IOSURLTypeItem item)
        {
            this.items.Add(item);

            return this;
        }

        public URLTypesSection AddItem(string name, string value)
        {
            return this.AddItem(new IOSURLTypeItem(name, value));
        }

        public string GetPlistDescription()
        {
            StringBuilder sb = new StringBuilder();
            foreach (IOSURLTypeItem item in items)
            {
                sb.Append(item.getInnerDescription());
                sb.Append("\n");
            }

            string content = sb.ToString();
            if (!string.IsNullOrEmpty(content))
            {
                content =
    @"<key>CFBundleURLTypes</key>
<array>" + content + "</array>\n";
            }

            return content;
        }
    }


    public class ProjectSettingIOS : ProjectSetting
    {

        public override void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            base.OnPostProcessBuild(target, pathToBuiltProject);
            WritePlistFile(pathToBuiltProject);
            AddExtCode(pathToBuiltProject);
        }


        public void PostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            OnPostProcessBuild(target, pathToBuiltProject);
        }


        private List<IOSURLTypeItem> items = new List<IOSURLTypeItem>();


        public virtual void WritePlistFile(string pathToBuiltProject) { }

        public virtual void AddExtCode(string pathToBuiltProject) { }


        #region plist handle

        public string GetPlistDescription()
        {
            StringBuilder sb = new StringBuilder();
            foreach (IOSURLTypeItem item in items)
            {
                sb.Append(item.getInnerDescription());
                sb.Append("\n");
            }

            string content = sb.ToString();
            if (!string.IsNullOrEmpty(content))
            {
                content =
    @"<key>CFBundleURLTypes</key>
<array>" + content + "</array>\n";
            }

            return content;
        }


        // 允许HTTP协议
        private static bool _AddATSSection = false;
        public static void AddATSSection(ref XCPlist plist)
        {
            if (_AddATSSection)
            {
                return;
            }

            _AddATSSection = true;

            string ats = @"
	<key>NSAppTransportSecurity</key>
	<dict>
		<key>NSAllowsArbitraryLoads</key>
		<true/>
	</dict>
";
            plist.AddKey(ats);
        }

        public static void AddAppQueriesScheme(ref XCPlist plist, string item)
        {
            string ats = @"
	<key>LSApplicationQueriesSchemes</key>
	<array>
		<string>" + item + @"</string>
	</array>
";
            plist.AddKey(ats);
        }

        public static void AddAppQueriesSchemes(ref XCPlist plist, string[] items)
        {
            if (items.Length == 0) return;

            string prefix = @"
	<key>LSApplicationQueriesSchemes</key>
	<array>";
            string postfix = "</array>";

            StringBuilder tmp = new StringBuilder();
            foreach (string str in items)
            {
                tmp.Append("<string>");
                tmp.Append(str);
                tmp.Append("</string>");
            }
            plist.AddKey(prefix + tmp.ToString() + postfix);
        }

        public static void AddURLTypes(ref XCPlist plist, URLTypesSection section)
        {
            if (section != null)
            {
                plist.AddKey(section.GetPlistDescription());
            }
        }

        public static void AddStringKVPair(ref XCPlist plist, string key, string value)
        {
            string item = @"<key>" + key + @"</key>
	<string>" + value + "</string>";

            if (plist != null)
            {
                plist.AddKey(item);
            }
        }


        public static void AddBooleanKVPair(ref XCPlist plist, string key, bool value)
        {
            string __true = @"<key>" + key + @"</key><true/>";
            string __false = @"<key>" + key + @"</key><false/>";

            if (plist != null)
            {
                plist.AddKey(value ? __true : __false);
            }
        }

        public static void AddIntegerNumberKVPair(ref XCPlist plist, string key, int value)
        {
            string item = @"<key>" + key + @"</key>
	<integer>" + value + "</integer>";

            if (plist != null)
            {
                plist.AddKey(item);
            }
        }

        public static void AddRealNumberKVPair(ref XCPlist plist, string key, double value)
        {
            string item = @"<key>" + key + @"</key>
	<real>" + value + "</real>";

            if (plist != null)
            {
                plist.AddKey(item);
            }
        }

        #endregion


        #region OC Code handle

        public static string url_handle_filename = "handleURL.h";
        public static void AddExtCode_HandleURL_Modify(string pathToBuiltProject)
        {

            XClass UnityAppController = new XClass(pathToBuiltProject + "/Classes/UnityAppController.mm");

            // 在指定代码后面添加一行
            // 引入头文件
            UnityAppController.WriteBelow("#include \"PluginBase/AppDelegateListener.h\"", "#import \"" + url_handle_filename + "\"");

            // 替换指定某行代码
            // UnityAppController.Replace("return YES;", "return [ShareSDK handleOpenURL:url sourceApplication:sourceApplication annotation:annotation wxDelegate:nil];");

            // openURL接口
            UnityAppController.WriteBelow("AppController_SendNotificationWithArg(kUnityOnOpenURL, notifData);",
                "\t[SDKProcess application:application openURL:url sourceApplication:sourceApplication annotation:annotation];");

            // didFinishLaunchingWithOptions接口
            UnityAppController.WriteBelow("::printf(\"-> applicationDidFinishLaunching()\\n\");",
                "\t[SDKProcess application:application didFinishLaunchingWithOptions:launchOptions];");

            // applicationDidEnterBackground接口
            UnityAppController.WriteBelow("::printf(\"-> applicationDidEnterBackground()\\n\");",
                "\t[SDKProcess applicationDidEnterBackground:application];");

            // applicationWillEnterForeground接口
            UnityAppController.WriteBelow("::printf(\"-> applicationWillEnterForeground()\\n\");",
                "\t[SDKProcess applicationWillEnterForeground:application];");

            // applicationDidBecomeActive接口
            UnityAppController.WriteBelow("_didResignActive = false;",
                "\t[SDKProcess applicationDidBecomeActive:application];");
        }



        public static void AddExtCode_HandleURL_Extend(string pathToBuiltProject)
        {
            XClass UnityAppController = new XClass(pathToBuiltProject + "/Classes/UnityAppController.mm");
            UnityAppController.WriteBelow("SensorsCleanup();\n}",
    @"
- (BOOL)application:(UIApplication *)application handleOpenURL:(NSURL *)url
{
	// [SDKProcess application:application handleOpenURL:url];
	[SDKProcess application:application openURL:url sourceApplication:nil annotation:(id)0];

	return YES;
}");
        }


        // 3D Touch相关
        public static void AddExtCode_UIView(string path2BuildProject)
        {
            XClass UnityAppController = new XClass(path2BuildProject + "/Classes/UI/UnityView.mm");

            // 在指定代码后面添加一行
            // 引入头文件
            UnityAppController.WriteBelow("#include \"Unity/UnityMetalSupport.h\"", "#import \"" + url_handle_filename + "\"");

            // touchesMoved接口
            string pos_line = "UnitySendTouchesMoved(touches, event);";
            string content = "\n\t[SDKProcess touchesMoved:touches withEvent:event];";
            UnityAppController.WriteBelow(pos_line, content);
        }


        public static void AddExtCode_supportOrientation(string path2BuildProject)
        {
            XClass UnityAppController = new XClass(path2BuildProject + "/Classes/UnityAppController.mm");
            string pos_line = "// Anyway this is intersected with values provided from UIViewController, so we are good";
            string content = "\t[SDKProcess application:application supportedInterfaceOrientationsForWindow:window];";
            UnityAppController.WriteBelow(pos_line, content);
        }

        #endregion

    }

}
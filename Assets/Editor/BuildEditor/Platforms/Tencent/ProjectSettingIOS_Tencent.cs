using UnityEditor;
using UnityEngine;
using UnityEditor.iOS.Xcode;


namespace UnityEditor.XBuild
{

    public class ProjectSettingIOS_Tencent : ProjectSettingIOS
    {

        public override void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            base.OnPostProcessBuild(target, pathToBuiltProject);
            EditProj(pathToBuiltProject);
        }


        static void EditProj(string pathToBuiltProject)
        {
            // string projPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
            string projPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
            Debug.Log("SDK-PostProcessBuild :: " + projPath);

            PBXProject pbxProj = new PBXProject();
            pbxProj.ReadFromFile(projPath);

            // Build Settings
            string targetGuid = pbxProj.TargetGuidByName("Unity-iPhone");

            // 添加预编译宏
            string[] macros = { "SYS_IOS", "CODE_PFM_U3D" };
            foreach (string item in macros)
            {
                pbxProj.AddBuildProperty(targetGuid, "GCC_PREPROCESSOR_DEFINITIONS[arch=*]", item);
            }

            // 添加CodeSignEntitlements
            //string path = "Entitlement/dragon.entitlements";
            //pbxProj.SetBuildProperty(targetGuid, "CODE_SIGN_ENTITLEMENTS", path);

            // 关闭BitCode
            pbxProj.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");

            // 关闭ARC
            // pbxProj.SetBuildProperty(targetGuid, "CLANG_ENABLE_OBJC_ARC", "NO");

            string debugConfig = pbxProj.BuildConfigByName(targetGuid, "Debug");
            string releaseConfig = pbxProj.BuildConfigByName(targetGuid, "Release");
            pbxProj.SetBuildPropertyForConfig(debugConfig, "DEBUG_INFORMATION_FORMAT", "DWARF");
            pbxProj.SetBuildPropertyForConfig(releaseConfig, "DEBUG_INFORMATION_FORMAT", "DWARF");

            // dylib || tbd
            // pbxProj.AddFileToBuild(targetGuid, pbxProj.AddFile("usr/lib/libz.tbd", "Frameworks/libz.tbd", PBXSourceTree.Sdk));

            // 添加资源文件(中文路径会导致 project.pbxproj 解析失败)
            //string resourcePath = DefaultModsPath + "/JoyYouSDK_Libs/IOS/TencentMSDK/entitlement";
            //string destDirName = pathToBuiltProject + "/Entitlement";
            //CopyAndReplaceFile(resourcePath, destDirName);
            pbxProj.WriteToFile(projPath);
        }



        public override void WritePlistFile(string path)
        {
            base.WritePlistFile(path);
            XCPlist list = new XCPlist(path);
            // 允许HTTP通信协议
            AddATSSection(ref list);
            InitTencentAttribute ita = typeof(XSDK).GetCustomAttribute<InitTencentAttribute>(false);
            if (ita == null)
            {
                Debug.LogError("InitTencentAttribute not found." + " log: " + ita.logEnable);
                return;
            }

            // 适配 iOS10
            // 添加麦克风访问权限
            AddStringKVPair(ref list, "NSMicrophoneUsageDescription", "是否允许此App使用你的麦克风？");
            // 添加相机访问权限
            AddStringKVPair(ref list, "NSCameraUsageDescription", "是否允许此App使用你的相机？");
            // 添加相册访问权限
            AddStringKVPair(ref list, "NSPhotoLibraryUsageDescription", "是否允许此App使用你的媒体资料库？");
            // 添加蓝牙访问权限
            AddStringKVPair(ref list, "NSBluetoothPeripheralUsageDescription", "是否允许此App使用你的蓝牙？");
            // 游戏不支持竖屏，需要勾选全屏选项
            AddBooleanKVPair(ref list, "UIRequiresFullScreen", true);

            /*
             * 由于苹果ATS的要求，所有应用的内部请求必须使用https协议，http请求会被屏蔽，所以删除此项设置
            // MSDK环境
            // 沙盒：http://msdktest.qq.com
            // 正式：http://msdk.qq.com
            // AddStringKVPair(ref list, "MSDK_URL", ita.logEnable ? "http://msdktest.qq.com" : "http://msdk.qq.com");

            // MSDK上报信息环境
            // 沙盒：http://pushtest.msdk.qq.com
            // 正式：http://push.msdk.qq.com
            // AddStringKVPair(ref list, "MSDK_PUSH_URL", ita.logEnable ? "http://pushtest.msdk.qq.com" : "http://push.msdk.qq.com");
             */

            AddStringKVPair(ref list, "MSDK_ENV", ita.logEnable ? "test" : "release");

            AddStringKVPair(ref list, "QQAppID", ita.qqAppId + "");
            AddStringKVPair(ref list, "QQAppKey", ita.qqAppKey);

            AddStringKVPair(ref list, "WXAppID", ita.weixinAppId);

            // 2.8.1及以上版本需配置
            AddStringKVPair(ref list, "MSDKKey", ita.msdkKey);

            // 是否启用自动刷新票据
            // Yes-启用；No（或不配置）-禁用
            AddBooleanKVPair(ref list, "AutoRefreshToken", true);

            // iOS系统渠道编号
            AddStringKVPair(ref list, "CHANNEL_DENGTA", "1001");

            // 推送功能的开关，若不使用MSDK推送则不需要配置
            AddBooleanKVPair(ref list, "MSDK_PUSH_SWITCH", true);

            // 支付所需的OfferId
            AddStringKVPair(ref list, "MSDK_OfferId", ita.midasId);

            // 实名认证配置
            AddIntegerNumberKVPair(ref list, "MSDK_REAL_NAME_AUTH_SWITCH", 1);

            // 是否启用公告功能
            AddBooleanKVPair(ref list, "NeedNotice", true);

            // 公告自动拉取的时间间隔（秒）
            AddIntegerNumberKVPair(ref list, "NoticeTime", 600);

            // 用于iOS8下LBS定位功能
            // 值可为空
            AddStringKVPair(ref list, "NSLocationWhenInUseUsageDescription", "");

            // 竖屏导航栏可隐藏开关
            // Yes-打开；No-禁用
            // 默认打开，MSDK2.14.0版本启用
            AddBooleanKVPair(ref list, "MSDK_Webview_Portrait_NavBar_Hideable", false);

            // 横屏导航栏可隐藏开关
            // Yes-打开；No-禁用
            // 默认打开，MSDK2.14.0版本启用
            AddBooleanKVPair(ref list, "MSDK_Webview_Landscape_NavBar_Hideable", false);

            // MSDKDns相关配置
            AddIntegerNumberKVPair(ref list, "TIME_OUT", 1000);
            AddBooleanKVPair(ref list, "Debug", ita.logEnable ? true : false);

            // 设置URL Scheme
            AddURLTypes(ref list
                , new URLTypesSection()
                    .AddItem(new IOSURLTypeItem("weixin", ita.weixinAppId))
                    .AddItem(new IOSURLTypeItem("tencentopenapi", "tencent" + ita.qqAppId))
                    .AddItem(new IOSURLTypeItem("QQ", "QQ" + System.Convert.ToString(ita.qqAppId, 16).ToUpper()))
                    .AddItem(new IOSURLTypeItem("QQLaunch", "tencentlaunch" + ita.qqAppId))
                );

            // AddStringKVPair(ref list, "FacebookAppID", attr.FB_ID);
            // AddStringKVPair(ref list, "FacebookDisplayName", attr.FB_NAME);

            // 白名单
            string[] query = { "mqq"
                                , "mqqapi"
                                , "mqqwpa"
                                , "mqqbrowser"
                                , "mttbrowser"
                                , "weixin"
                                , "wechat"
                                };
            AddAppQueriesSchemes(ref list, query);

            list.Save();
        }


        public override void AddExtCode(string pathToBuiltProject)
        {
            base.AddExtCode(pathToBuiltProject);

            AddExtCode_HandleURL_Modify(pathToBuiltProject);
            AddExtCode_HandleURL_Extend(pathToBuiltProject);
            AddExtCode_supportOrientation(pathToBuiltProject);
            AddExtCode_UIView(pathToBuiltProject);
        }
        
    }
}
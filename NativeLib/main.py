#coding:utf8

__author__ = 'kedllyzhao'

import sys
import optparse
import os
import os.path
import shutil
from xml.dom import minidom
import xml.dom

def concatenateSettings(dict1, dict2):
    keySet1 = set(dict1.keys())
    keySet2 = set(dict2.keys())
    dstKeySet = keySet1.union(keySet2)
    retDict = {}
    for key in dstKeySet:
        if dict1.has_key(key) and not dict2.has_key(key):
            retDict[key] = dict1[key]
            continue
        if dict2.has_key(key) and not dict1.has_key(key):
            retDict[key] = dict2[key]
            continue
        if dict1.has_key(key) and dict2.has_key(key):
            if type(dict1[key]) is dict and type(dict2) is dict:
                retDict[key] = concatenateSettings(dict1[key], dict2[key])
            else:
                if type(dict1[key]) is str or type(dict1[key]) is unicode:
                    retDict[key] = dict1[key] + "\n" + dict2[key]
                elif type(dict1[key]) is list:
                    retDict[key] = list(set(dict1[key]).union(set(dict2[key])))
                else:
                    print "-->"
    return retDict

class ProjectGenerator(object):

    AMXML = "AndroidManifest.xml"

    def __init__(self, dstDir, projectDir, settings):
        self._dstDir = dstDir
        self._projDir = projectDir
        self._libsFolderName = "libs"
        self._srcFolderName = "src"
        self._resFolderName = "res"
        self._settings = settings
        self._xmlPrefix = '''<?xml version="1.0" ?>'''
        self._xmlBody_start = '''<body xmlns:android="http://schemas.android.com/apk/res/android" >'''
        self._xmlBody_end = "</body>"
        self._permissionList = []
        self._manifestElementActivities = ""
        if self._settings.has_key("AndroidManifest"):
            if self._settings["AndroidManifest"].has_key("permissions"):
                self._permissionList = self._settings["AndroidManifest"]["permissions"]
            if self._settings["AndroidManifest"].has_key("activities"):
                self._manifestElementActivities = self._settings["AndroidManifest"]["activities"]
        self._filterLibs = []
        if self._settings.has_key(self._libsFolderName):
            self._filterLibs = self._settings[self._libsFolderName]


    def _copyManifestFile(self):
        if not os.path.exists(self._dstDir):
            os.mkdir(self._dstDir)
        if not os.path.exists( self._dstDir + os.path.sep + ProjectGenerator.AMXML):
            shutil.copy(self._projDir + os.path.sep + ProjectGenerator.AMXML, self._dstDir + os.path.sep)
        pass

    def _changeManifestFile(self, filePath):
        if not os.path.exists(filePath) or not os.path.isfile(filePath):
            return
        doc = minidom.parse(filePath)
        manifest = doc.getElementsByTagName("manifest")
        if len(manifest) != 1:
            return
        manifest = manifest[0]
        if not manifest.hasAttribute("package"):
            return
        packageName = manifest.getAttribute("package")
        if packageName[-3:] != ".uc":
            packageName += ".uc"
        manifest.setAttribute("package", packageName)
        uses_permissions = manifest.getElementsByTagName("uses-permission")
        current_permissions_set = set([i.getAttribute("android:name") for i in uses_permissions])
        dst_permissions_set = set(self._permissionList)
        thePermissions = dst_permissions_set.difference(current_permissions_set)
        #print thePermissions
        for permission in thePermissions:
            newPermission = doc.createElement("uses-permission")
            newPermission.setAttribute("android:name", permission)
            manifest.appendChild(newPermission)
        application = manifest.getElementsByTagName("application")
        if len(application) != 1:
            return
        application = application[0]
        activities_doc = minidom.parseString(self._xmlPrefix
                                           + self._xmlBody_start
                                           + self._manifestElementActivities
                                           + self._xmlBody_end)
        activities = activities_doc.getElementsByTagName("body")#("activity")
        activities = [i for i in activities[0].childNodes if i.nodeType == xml.dom.Node.ELEMENT_NODE]
        def activitiesFilter(all_act):
            retSet = set()
            def includeItem(st, nn, nt, na):
                for i in st:
                    if i.nodeName == nn and i.nodeType == nt and i.nodeName == i.getAttribute("android:name"):
                        return True
                return False
            for i in all_act:
                nodeName = i.nodeName
                nodeType = i.nodeType
                android_name = i.getAttribute("android:name")
                if not includeItem(retSet, nodeName, nodeType, android_name):
                    retSet.add(i)
            return retSet
        activities = activitiesFilter(activities)
        for activity in activities:
            application.appendChild(activity)
            #print "add: ", activity
        theFile = open(filePath, "w")
        theFile.write(doc.toprettyxml())
        theFile.flush()
        theFile.close()

    def _copyAndChangeManifestFile(self):
        self._copyManifestFile()
        self._changeManifestFile(self._dstDir + os.path.sep + ProjectGenerator.AMXML)

    def _copyAndFilterLibs(self, filter = ()):
        if not os.path.exists(self._dstDir):
            os.mkdir(self._dstDir)
        srcPath = self._projDir + os.path.sep + self._libsFolderName
        dstPath = self._dstDir + os.path.sep + self._libsFolderName
        if len(filter) == 0:
            shutil.copytree(srcPath, dstPath)
        else:
            if not os.path.exists(dstPath):
                os.mkdir(dstPath)
            for ele in filter:
                abs_ele = srcPath + os.path.sep + ele
                __dstDir = dstPath + os.path.sep
                if ele.find("/") != -1:
                    folders = [i for i in ele.split("/") if not i.isspace() and i != "." and i != ".." ];
                    if len(folders) < 2:
                        continue
                    else:
                        ele_folders = folders[0: -1]
                        curPos = __dstDir
                        for item in ele_folders:
                            item_folder = curPos + os.path.sep + item
                            if not os.path.exists(item_folder):
                                os.mkdir(item_folder)
                            curPos = item_folder
                        __dstDir = curPos
                if os.path.isfile(abs_ele):
                    shutil.copy(abs_ele,  __dstDir)

    def _copyAndChangeCodeFiles(self):
        if not os.path.exists(self._dstDir):
            os.mkdir(self._dstDir)
        srcPath = self._projDir + os.path.sep + self._srcFolderName
        dstPath = self._dstDir + os.path.sep + self._srcFolderName
        if not os.path.exists(dstPath):
            os.mkdir(dstPath)
        if self._settings.has_key("src"):
            if self._settings["src"].has_key("package"):
                packages = self._settings["src"]["package"]
                for packageName in packages:
                    subPath = packageName.split(".")
                    deep = dstPath
                    deepSource = srcPath
                    for sp in subPath:
                        deep = deep + os.sep + sp
                        deepSource = deepSource + os.path.sep + sp
                        if not os.path.exists(deep):
                            os.mkdir(deep)
                        contents = os.listdir(deepSource)
                        for obj in contents:
                            path4obj = deepSource + os.path.sep + obj
                            path4dst = deep + os.path.sep + obj
                            if os.path.isfile(path4obj) and not os.path.exists(path4dst):
                                shutil.copy(path4obj, path4dst)
    def _copyRes(self):
        if not os.path.exists(self._dstDir):
            os.mkdir(self._dstDir)
        srcResPath = self._projDir + os.path.sep + self._resFolderName
        dstResPath = self._dstDir + os.path.sep + self._resFolderName
        if not os.path.exists(srcResPath):
            return
        if not os.path.exists(dstResPath):
            os.mkdir(dstResPath)
        if self._settings.has_key("res"):
            resList = self._settings["res"]
            if len(resList) > 0:
                for ele in resList:
                    abs_ele = srcResPath + os.path.sep + ele
                    __dstDir = dstResPath + os.path.sep
                    if ele.find("/") != -1:
                        folders = [i for i in ele.split("/") if not i.isspace() and i != "." and i != ".." ];
                        if len(folders) < 2:
                            continue
                        else:
                            ele_folders = folders[0: -1]
                            curPos = __dstDir
                            for item in ele_folders:
                                item_folder = curPos + os.path.sep + item
                                if not os.path.exists(item_folder):
                                    os.mkdir(item_folder)
                                curPos = item_folder
                            __dstDir = curPos
                    if os.path.isfile(abs_ele):
                        shutil.copy(abs_ele,  __dstDir)




    def generate(self):
        self._copyAndChangeCodeFiles()
        self._copyAndFilterLibs(self._filterLibs)
        self._copyAndChangeManifestFile()
        self._copyRes();

    def __add__(self, other):
        return ProjectGenerator(self._dstDir, self._projDir, concatenateSettings(self._settings, other._settings))


def _parse_args():
    """
    Parse the command line for options
    """
    usage = "usage: %prog [options] arg1 arg2"
    parser = optparse.OptionParser()
    parser.add_option(
        '--platform', dest='platform', default="", type = "string",
        help='platform name: UC 360 baidu etc.')
    parser.add_option(
        '--workspace', dest='workspace', default="./", type = "string",
        help='project directory.')
    parser.add_option(
        '--project', dest='projectDir', default="./destProject", type = "string",
        help='project directory.')
    # parser.add_option(
    #     "-t", dest="test", action="store_const", const=lambda:_test, default=_test2, help="////////////"
    # )
    options, args = parser.parse_args()
    # positional arguments are ignored
    return options, args


def main():
    opt, args = _parse_args()
    if opt.platform.lower() != "":
        platforms = set(args)
        platforms.add(opt.platform)
        platforms = (item.lower() for item in platforms)
        settings = reduce(concatenateSettings, (SettingsSet[ele] for ele in platforms if SettingsSet.has_key(ele)))
        ProjectGenerator(opt.projectDir, opt.workspace, settings).generate()


def build_project_all():
    pass

def build_project_uc():
    pass

def build_project_xx():
    pass

def appendSettings(*settings):
    return reduce(concatenateSettings, settings)


UC_Settings = \
    {
        "AndroidManifest": {
                "permissions" : [
                        u"android.permission.GET_TASKS",
                        u"android.permission.SYSTEM_ALERT_WINDOW",
                        u"android.permission.SEND_SMS",
                        u"android.permission.VIBRATE",
                        u"android.permission.AUTHENTICATE_ACCOUNTS",
                        u"android.permission.GET_ACCOUNTS",
                        u"android.permission.USE_CREDENTIALS",
                        u"android.permission.ACCESS_NETWORK_STATE",
                        u"android.permission.INTERNET",
                    ],
                "activities" :u'''
<activity android:name="cn.uc.gamesdk.SdkActivity"
	android:configChanges="keyboardHidden|orientation|screenSize"
	android:label="@string/app_name"
	android:theme="@android:style/Theme.Translucent.NoTitleBar"
	android:windowSoftInputMode="adjustResize" >
	<intent-filter>
		<action android:name="cn.uc.gamesdk.sdkweb" />
		<category android:name="android.intent.category.DEFAULT" />
	</intent-filter>
</activity>
<activity
	android:name="com.alipay.sdk.app.H5PayActivity"
	android:configChanges="orientation|keyboardHidden|navigation"
	android:exported="false"
	android:screenOrientation="behind" >
</activity>
<activity
	android:name="com.alipay.sdk.auth.AuthActivity"
	android:configChanges="orientation|keyboardHidden|navigation"
	android:exported="false"
	android:screenOrientation="behind" >
</activity>
        ''',
            },
        "libs": [
            "alipaysdk.jar",
            "MobileSecSdk.jar",
            "UCGameSDK-3.4.15.3.jar",
            "utdid4all-1.0.4.jar",
            ],
        "src":{
                "package":[
                        "com.joyyou.common.UC",
                    ],
                "namespace":[],
            },
    }

TalkingData_Settings = \
    {
        "AndroidManifest": {
                "permissions" : [
                        u"android.permission.GET_TASKS",
                        u"android.permission.SYSTEM_ALERT_WINDOW",
                        u"android.permission.SEND_SMS",
                        u"android.permission.VIBRATE",
                        u"android.permission.AUTHENTICATE_ACCOUNTS",
                        u"android.permission.GET_ACCOUNTS",
                        u"android.permission.USE_CREDENTIALS",
                        u"android.permission.ACCESS_NETWORK_STATE",
                        u"android.permission.INTERNET",
                    ],
                "activities" :u'''''',
            },
        "libs": [
            "alipaysdk.jar",
            "MobileSecSdk.jar",
            "UCGameSDK-3.4.15.3.jar",
            "utdid4all-1.0.4.jar",
            ],
        "src":{
                "package":[
                        "com.joyyou.statistics"
                    ],
                "namespace":[],
            },
    }

TencentQQ_Settings = \
    {
        "AndroidManifest": {
                "permissions" : [
                        u"android.permission.WRITE_EXTERNAL_STORAGE",
                        u"android.permission.READ_PHONE_STATE",
                        u"android.permission.ACCESS_WIFI_STATE",
                        u"android.permission.ACCESS_NETWORK_STATE",
                        u"android.permission.INTERNET",
                    ],
                "activities" :u'''''',
            },
        "libs": [
            "mta-sdk-1.6.2.jar",
            "open_sdk_r4066.jar",
            ],
        "src":{
                "package":[
                        "com.joyyou.open.QQ"
                    ],
                "namespace":[],
            },
    }

SinaWeiBo_Settings = \
    {
        "AndroidManifest": {
                "permissions" : [
                        u"android.permission.WRITE_EXTERNAL_STORAGE",
                        u"android.permission.ACCESS_WIFI_STATE",
                        u"android.permission.ACCESS_NETWORK_STATE",
                        u"android.permission.INTERNET",
                    ],
                "activities" :u'''''',
            },
        "libs": [
            "weibosdkcore.jar",
            ],
        "src":{
                "package":[
                        "com.joyyou.open.SinaWeiBo"
                    ],
                "namespace":[],
            },
    }

BaiduMap_Settings = \
    {
        "AndroidManifest": {
                "permissions" : [
                        u"android.permission.ACCESS_COARSE_LOCATION",
                        u"android.permission.ACCESS_FINE_LOCATION",
                        u"android.permission.ACCESS_WIFI_STATE",
                        u"android.permission.ACCESS_NETWORK_STATE",
                        u"android.permission.CHANGE_WIFI_STATE",
                        u"android.permission.READ_PHONE_STATE",
                        u"android.permission.WRITE_EXTERNAL_STORAGE",
                        u"android.permission.INTERNET",
                        u"android.permission.MOUNT_UNMOUNT_FILESYSTEMS",
                        u"android.permission.READ_LOGS",
                    ],
                "activities" :u'''\
<service android:name="com.baidu.location.f" android:enabled="true" android:process=":remote"></service>
<meta-data
            android:name="com.baidu.lbsapi.API_KEY"
            android:value="key" />''',
            },
        "libs": [
            "weibosdkcore.jar",
            ],
        "src":{
                "package":[
                        "com.joyyou.open.map.baidu"
                    ],
                "namespace":[],
            },
    }

Xiaomi_Settings1 =\
    {
        "AndroidManifest": {
                "permissions" : [
                        u"android.permission.WRITE_EXTERNAL_STORAGE",
                        u"android.permission.ACCESS_NETWORK_STATE",
                        u"android.permission.INTERNET",
                        u"android.permission.ACCESS_WIFI_STATE",
                        u"android.permission.READ_PHONE_STATE",
                        u"com.xiaomi.market.sdk.UPDATE",
                    ],
                "activities" :u'''\
<receiver android:name="com.xiaomi.market.sdk.DownloadCompleteReceiver" >
    <intent-filter>
        <action android:name="android.intent.action.DOWNLOAD_COMPLETE" />
    </intent-filter>
</receiver>
            ''',
            },
        "libs": [
            "xiaomi_sdk.jar",
            "armeabi/libsdk_patcher_jni.so",
			"armeabi-v7a/libsdk_patcher_jni.so"
            ],
        "src":{
                "package":[
                        "com.joyyou.common.Xiaomi",
                    ],
                "namespace":[],
            },
        "res": [
            "values/xiaomi_strings.xml",
            "values-zh/xiaomi_strings.xml"
            ]
    }

Xiaomi_Settings = \
    {
"AndroidManifest": {
                "permissions" : [
                        u"android.permission.GET_TASKS",
                        u"com.xiaomi.sdk.permission.PAYMENT",
                    ],
                "activities" :u'''\
<receiver android:name="com.xiaomi.market.sdk.DownloadCompleteReceiver" >
    <intent-filter>
        <action android:name="android.intent.action.DOWNLOAD_COMPLETE" />
    </intent-filter>
</receiver>
            ''',
            },
        "libs": [
            "SDK_TY_3.0.0.jar"
            ],
        "src":{
                "package":[
                        "com.joyyou.common.Xiaomi",
                    ],
                "namespace":[],
            },
        "res": [

            ]
    }

SettingsSet = {
    "uc":UC_Settings,
    "talkingdata":TalkingData_Settings,
    "xiaomi":Xiaomi_Settings,
}


if __name__ == '__main__':
    sys.exit(main())

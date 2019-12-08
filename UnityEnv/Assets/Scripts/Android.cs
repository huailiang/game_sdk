#if UNITY_ANDROID

using UnityEngine;
using System;

public class Android
{

    private static AndroidJavaClass _unityPlayerClass;

    public static AndroidJavaClass UnityPlayerClass
    {
        get
        {
            if (_unityPlayerClass == null)
            {
                _unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            }
            return _unityPlayerClass;
        }
    }

    private static AndroidJavaObject _currentActivity;

    public static AndroidJavaObject CurrentActivity
    {
        get
        {
            if (_currentActivity == null)
            {
                _currentActivity = UnityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
            }
            return _currentActivity;
        }
    }

    private static AndroidJavaObject _systeminfoActivity;

    public static AndroidJavaObject SysteminfoActivity
    {
        get
        {
            if (_systeminfoActivity == null)
            {
                AndroidJavaClass jc = new AndroidJavaClass("com.sdk.SystemInfo");
                _systeminfoActivity = jc.GetStatic<AndroidJavaObject>("uniqueInstance");
            }
            return _systeminfoActivity;
        }
    }

    private static AndroidJavaClass _nativeHelper;

    private static AndroidJavaClass NativeHelper
    {
        get
        {
            if (_nativeHelper == null)
            {
                _nativeHelper = new AndroidJavaClass("com.sdk.NativeHelper");
            }
            return _nativeHelper;
        }
    }

    public static float GetMemory()
    {
        float rest = 0;
        try
        {
            rest = NativeHelper.CallStatic<float>("GetMemory");
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        return rest;
    }

    public static int CopyStreamingAsset(string name, string dest = "")
    {
        int errorcode = 0;
        if (string.IsNullOrEmpty(dest))
        {
            dest = Application.persistentDataPath;
        }
        try
        {
            errorcode = NativeHelper.CallStatic<int>("CopyStreamingAsset", name, dest);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        return errorcode;
    }

    public static byte[] LoadStream(string path)
    {
        byte[] rst = null;
        try
        {
            rst = NativeHelper.CallStatic<byte[]>("LoadStream", path);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        return rst;
    }

    public static string LoadSteamString(string path)
    {
        string rst = string.Empty;
        try
        {
            rst = NativeHelper.CallStatic<string>("LoadSteamString", path);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        return rst;
    }

    public static void UnZip(string src, string dest = "", bool rewrite=true)
    {
        try
        {
            if (string.IsNullOrEmpty(dest))
            {
                dest = Application.persistentDataPath;
            }
            NativeHelper.CallStatic("UnZip", src, dest, rewrite);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

}


#endif
using UnityEngine;
using System.Runtime.InteropServices;

public class NativeBridge
{

    private static NativeBridge _s;

    public static NativeBridge sington
    {
        get
        {
            if (_s == null) _s = new NativeBridge();
            return _s;
        }
    }


    [DllImport("__Internal")]
    private static extern int GetDensity();

    [DllImport("__Internal")]
    private static extern string CheckSIM();

    [DllImport("__Internal")]
    private static extern void ToJPG(string path, byte[] bytes,int length);



    public void NGetDensity()
    {
#if UNITY_ANDROID
        AndroidJavaClass jc = new AndroidJavaClass("com.tencent.tmgp.dragonnest.SystemInfoActivity");
        int density = jc.CallStatic<int>("GetDensity");
        Debug.Log("android density is: " + density);
#elif UNITY_IOS
         int density = GetDensity();
         Debug.Log("ios density is: " + density);
#endif
    }


    public void NCheckSIM()
    {
#if UNITY_ANDROID
        AndroidJavaClass jc = new AndroidJavaClass("com.tencent.tmgp.dragonnest.SystemInfoActivity");
        string str = jc.CallStatic<string>("CheckSIM");
        Debug.Log("androidCheckSIM: " + str);
#elif UNITY_IOS
          string str = CheckSIM();
          Debug.Log("ios CheckSIM: " + str);
#endif
    }


    public void NToJPG(string path, byte[] bytes)
    {
#if UNITY_ANDROID
        Debug.Log("androidToJPG: " + path);
        AndroidJavaClass jc = new AndroidJavaClass("com.tencent.tmgp.dragonnest.SystemInfoActivity");
        jc.CallStatic("ToJPG", path, bytes);
#elif UNITY_IOS
         ToJPG(path,bytes,bytes.Length);
         Debug.Log("ios ToJPG: " + path);
#endif
    }


}

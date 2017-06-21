using UnityEngine;
using System.Runtime.InteropServices;

public class NativeBridge
{

    private static NativeBridge _s;

    public static NativeBridge sington
    {
        get { if (_s == null) _s = new NativeBridge(); return _s; }
    }


    [DllImport("__Internal")]
    private static extern int GetDensity();

    [DllImport("__Internal")]
    private static extern string CheckSIM();

    [DllImport("__Internal")]
    private static extern void ToJPG(string path, byte[] bytes, int length);



    public int NGetDensity()
    {
        int density = 200;
#if UNITY_EDITOR
        Debug.Log("NGetDensity called");
#elif UNITY_ANDROID
        AndroidJavaClass jc = new AndroidJavaClass("com.tencent.tmgp.dragonnest.SystemInfoActivity");
        density = jc.CallStatic<int>("GetDensity");
        Debug.Log("android density is: " + density);
#elif UNITY_IOS
         density = GetDensity();
         Debug.Log("ios density is: " + density);
#endif
        return density;
    }


    public string NCheckSIM()
    {
        string str = "";
#if UNITY_EDITOR
        Debug.Log("NCheckSIM called");
# elif UNITY_ANDROID
        AndroidJavaClass jc = new AndroidJavaClass("com.tencent.tmgp.dragonnest.SystemInfoActivity");
        str = jc.CallStatic<string>("CheckSIM");
        Debug.Log("androidCheckSIM: " + str);
#elif UNITY_IOS
          str = CheckSIM();
          Debug.Log("ios CheckSIM: " + str);
#endif
        return str;
    }


    public void NToJPG(string path, byte[] bytes)
    {
#if UNITY_EDITOR
        Debug.Log("NToJPG called");
#elif UNITY_ANDROID
        Debug.Log("androidToJPG: " + path);
        AndroidJavaClass jc = new AndroidJavaClass("com.tencent.tmgp.dragonnest.SystemInfoActivity");
        jc.CallStatic("ToJPG", path, bytes);
#elif UNITY_IOS
         ToJPG(path,bytes,bytes.Length);
         Debug.Log("ios ToJPG: " + path);
#endif
    }


}

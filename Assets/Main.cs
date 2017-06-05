using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class Main : MonoBehaviour
{


    [DllImport("__Internal")]
    private static extern int GetDensity();

    [DllImport("__Internal")]
    private static extern string CheckSIM();


    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 140, 40), "像素密度"))
        {
#if UNITY_ANDROID
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            int density = jo.Call<int>("GetDensity");
            Debug.Log("android density is: " + density);
#elif UNITY_IOS
            int density = GetDensity();
            Debug.Log("ios density is: " + density);
#endif

        }



        if (GUI.Button(new Rect(10, 70, 140, 40), "运营商"))
        {
#if UNITY_ANDROID
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            string str = jo.Call<string>("CheckSIM");
            Debug.Log("androidCheckSIM: " + str);
#elif UNITY_IOS
             string str = CheckSIM();
            Debug.Log("ios CheckSIM: " + str);
#endif
        }
    }

}

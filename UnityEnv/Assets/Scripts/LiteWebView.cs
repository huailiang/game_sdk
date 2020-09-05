using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace LiteWebView
{
    public class WebView : MonoBehaviour
    {
        Lite4Platform _ulite;
        Dictionary<string, Action<String>> _jsActions = new Dictionary<string, Action<string>>();

        public event Action<string> onLoadingUrl;

        public void Init()
        {
            if (null == _ulite)
            {
                Debug.Log("LiteWebView init");

#if !UNITY_EDITOR
#if UNITY_ANDROID
                Debug.Log("LiteWebView:Android");
                _ulite = new LiteAndroidWebView(getFullName(this.gameObject));
#elif UNITY_IOS
                Debug.Log("LiteWebView:iOS");
                _ulite = new LiteIosWebView(getFullName(this.gameObject));
#endif
#endif
            }
        }

        void Dispose()
        {
            Close();
            _ulite = null;
        }

        public void Show(int top = 0, int bottom = 0, int left = 0, int right = 0)
        {
            if (null == _ulite)
            {
                return;
            }
            _ulite.Show(top, bottom, left, right);
        }

        public void LoadUrl(string url)
        {
            if (null == _ulite)
            {
                Application.OpenURL(url);
                return;
            }
            _ulite.LoadUrl(url);
        }

        /// <summary>
        /// 访问StreamingAssets文件夹中存放的资源
        /// </summary>
        /// <param name="filePath">相对于StreamingAssets目录的文件路径，以"/"开头</param>
        public void LoadLocal(string filePath)
        {
#if UNITY_ANDROID
            filePath = "file:///android_asset" + filePath;
#else
            filePath = "file://" + Application.streamingAssetsPath + filePath;
#endif
            LoadUrl(filePath);
        }


        public void Close()
        {
            if (null == _ulite)
            {
                return;
            }
            _ulite.Close();
        }

        /// <summary>
        /// 请求当前WebView页面中对应的JS方法
        /// </summary>
        public void CallJS(string funName, string msg)
        {
            if (null == _ulite)
            {
                return;
            }
            _ulite.CallJS(funName, msg);
        }

        /// <summary>
        /// 当webview加载页面时的回调
        /// </summary>
        void OnLoadingUrl(string url)
        {
            Debug.LogFormat("loading url: {0}  ID：{1}", url, this.GetInstanceID());
            onLoadingUrl?.Invoke(url);
        }

        void OnJsCall(string msg)
        {
            Debug.Log("js call unity: " + msg);
            string iName = null;
            string paramsStr = null;

            try
            {
                int flag = msg.IndexOf("?");
                if (flag == -1)
                {
                    iName = msg;
                }
                else
                {
                    iName = msg.Substring(0, flag);
                    paramsStr = msg.Substring(flag + 1);
                }
                if (_jsActions.ContainsKey(iName))
                {
                    _jsActions[iName](paramsStr);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"LiteWebView：Wrong JS Msg [{e.Message}]");
            }
        }


        /// <summary>
        /// 注册供JS调用的方法
        /// </summary>
        /// <param name="funName">方法名：JS通过该方法名调用对应方法</param>
        /// <param name="fun">方法</param>
        public void RegistJsInterfaceAction(string interfaceName, Action<String> action)
        {
            _jsActions[interfaceName] = action;
        }


        /// <summary>
        /// 注销供JS调用的方法
        /// </summary>
        /// <param name="interfaceName">方法名：JS通过该方法名调用对应方法</param>
        /// <param name="action">方法</param>
        public void UnregistJsInterfaceAction(string interfaceName, Action<String> action)
        {
            if (_jsActions.ContainsKey(interfaceName))
            {
                _jsActions.Remove(interfaceName);
            }
        }

        /// <summary>
        /// 获取GameObject的完整路径名
        /// </summary>
        string getFullName(GameObject obj)
        {
            string fullName = gameObject.name;
            Transform temp = transform;
            do
            {
                temp = temp.parent;
                if (null != temp)
                {
                    fullName = temp.name + "/" + fullName;
                }
            } while (temp != null);
            return fullName;
        }
    }

    abstract class Lite4Platform
    {
        abstract public void Show(int top, int bottom, int left, int right);

        abstract public void LoadUrl(string url);

        abstract public void Close();

        abstract public void CallJS(string funName, string msg);
    }

    class LiteAndroidWebView : Lite4Platform
    {
        AndroidJavaObject _ajo;

        public LiteAndroidWebView(string gameObjectName)
        {
            _ajo = new AndroidJavaObject("com.sdk.LiteWebView");
            _ajo.Call("registCallBackGameObjectName", gameObjectName);
        }

        public override void CallJS(string funName, string msg)
        {
            _ajo.Call("callJs", funName, msg);
        }

        public override void Close()
        {
            _ajo.Call("close");
        }

        public override void LoadUrl(string url)
        {
            _ajo.Call("loadUrl", url);
        }

        public override void Show(int top, int bottom, int left, int right)
        {
            _ajo.Call("show", top, bottom, left, right);
        }
    }

    class LiteIosWebView : Lite4Platform
    {
        [DllImport("__Internal")]
        private static extern void _registCallBackGameObjectName(string gameObjectName);

        [DllImport("__Internal")]
        private static extern void _show(int top, int bottom, int left, int right);

        [DllImport("__Internal")]
        private static extern void _loadUrl(string url);

        [DllImport("__Internal")]
        private static extern void _close();

        [DllImport("__Internal")]
        private static extern void _callJS(string funName, string msg);


        public LiteIosWebView(string gameObjectName)
        {
            _registCallBackGameObjectName(gameObjectName);
        }

        public override void CallJS(string funName, string msg)
        {
            _callJS(funName, msg);
        }

        public override void Close()
        {
            _close();
        }

        public override void LoadUrl(string url)
        {
            _loadUrl(url);
        }

        public override void Show(int top, int bottom, int left, int right)
        {
            _show(top, bottom, left, right);
        }
    }
}

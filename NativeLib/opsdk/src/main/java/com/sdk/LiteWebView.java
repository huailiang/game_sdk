package com.sdk;

import android.annotation.SuppressLint;
import android.content.Intent;
import android.net.Uri;
import android.os.Build;
import android.util.DisplayMetrics;
import android.util.Log;
import android.view.ViewGroup;
import android.view.WindowManager;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.FrameLayout;


public class LiteWebView
{
    private final String TAG = "LiteWebView";
    @SuppressLint("StaticFieldLeak")
    private static MainActivity gameActivity = null;
    private WebView _webView;
    private int _top, _bottom, _left, _right;

    private String _url;
    private String _gameObjectName;
    private String _callJsFunName;
    private String _callJsParams;

    static void setGameActivity(MainActivity activity)
    {
        gameActivity = activity;
    }

    /**
     * JS通信内容
     * 协议格式为  [ulitewebview://interface?params] interface为请求的协议名，params为参数字符串
     * 或者   [ulitewebview://interface]    interface为请求的协议名
     */
    private void onJsCall(String msg)
    {
        try {
            String[] split = msg.split("://");
            MLog.i(TAG, "js msg body:" + split[1]);
            gameActivity.Call(_gameObjectName, "OnJsCall", split[1]);
        }
        catch (Exception e) {
            MLog.e(TAG, "wrong js call:" + msg);
        }
    }

    public void callJs(final String funName, String msg)
    {
        _callJsFunName = funName;
        _callJsParams = msg;
        gameActivity.runOnUiThread(new Runnable()
        {
            @Override
            public void run()
            {
                if (null != _webView) {
                    // 必须另开线程进行JS方法调用(否则无法调用)
                    _webView.post(new Runnable()
                    {
                        @Override
                        public void run()
                        {
                            // 注意调用的JS方法名要对应上
                            // 调用javascript的callJS()方法
                            String jsStr = String.format("javascript:%s(\"%s\")", _callJsFunName, _callJsParams);
                            _webView.loadUrl(jsStr);
                        }
                    });
                }
            }
        });
    }

    @SuppressLint("SetJavaScriptEnabled")
    private void init()
    {
        if (null == _webView) {
            WebView webView = new WebView(gameActivity);
            webView.setWebViewClient(new WebViewClient()
            {
                public boolean shouldOverrideUrlLoading(WebView view, String url)
                {
                    Log.i("LiteWebView", url);

                    gameActivity.Call(_gameObjectName, "OnLoadingUrl", url);

                    if (url.startsWith("litewebview://")) {
                        //返回true表示不需要的再做处理了
                        onJsCall(url);
                        return true;
                    }
                    else if (url.startsWith("file://") || url.startsWith("http://") || url.startsWith("https://")) {
                        //加载网页
                        return super.shouldOverrideUrlLoading(view, url);
                    }
                    else {
                        try {
                            // 以下固定写法
                            final Intent intent = new Intent();
                            intent.setAction(Intent.ACTION_VIEW);
                            intent.setData(Uri.parse(url));
                            gameActivity.startActivity(intent);
                            return true;
                        }
                        catch (Exception e) {
                            MLog.e(TAG, e.getMessage());
                        }
                    }

                    return super.shouldOverrideUrlLoading(view, url);
                }
            });

            WebSettings webSettings = webView.getSettings();
            webSettings.setJavaScriptEnabled(true);
            webSettings.setJavaScriptCanOpenWindowsAutomatically(false);
            webSettings.setAllowFileAccess(true);
            webSettings.setSupportZoom(false);
            webSettings.setBuiltInZoomControls(true);
            webSettings.setUseWideViewPort(true);
            webSettings.setSupportMultipleWindows(true);
            webSettings.setAppCacheEnabled(true);
            webSettings.setDomStorageEnabled(true);
            webSettings.setGeolocationEnabled(true);
            webSettings.setAppCacheMaxSize(Long.MAX_VALUE);
            webSettings.setCacheMode(WebSettings.LOAD_DEFAULT);

            _webView = webView;
        }
    }

    private void destroy()
    {
        if (null != _webView) {
            FrameLayout view = (FrameLayout) gameActivity.getWindow().getDecorView();
            view.removeView(_webView);
            _webView.resumeTimers();
            _webView.destroy();
            _webView = null;
        }
    }

    public void registCallBackGameObjectName(String gameObjectName)
    {
        _gameObjectName = gameObjectName;
    }


    public void show()
    {
        show(0, 0, 0, 0);
    }


    /**
     * 显示WebView
     * @param top    距离屏幕上部像素
     * @param bottom 距离屏幕下部像素
     * @param left   距离屏幕左部像素
     * @param right  距离屏幕右部像素
     */
    public void show(int top, int bottom, int left, int right)
    {
        _top = top;
        _bottom = bottom;
        _left = left;
        _right = right;
        gameActivity.runOnUiThread(new Runnable()
        {
            @Override
            public void run()
            {
                init();
                FrameLayout view = (FrameLayout) gameActivity.getWindow().getDecorView();
                view.addView(_webView);

                ViewGroup.LayoutParams lp = _webView.getLayoutParams();

                WindowManager manager = gameActivity.getWindowManager();
                DisplayMetrics outMetrics = new DisplayMetrics();
                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1) {
                    manager.getDefaultDisplay().getRealMetrics(outMetrics);
                }
                int screenW = outMetrics.widthPixels;
                int screenH = outMetrics.heightPixels;

                Log.i("LiteWebView", String.format("屏幕宽高 [w:%d , h:%d]", screenW, screenH));
                Log.i("LiteWebView", String.format("展示参数 [T:%d , B:%d , L:%d , R:%d]", _top, _bottom, _left, _right));

                lp.width = screenW - _left - _right;
                lp.height = screenH - _top - _bottom;
                _webView.setX(_left);
                _webView.setY(_top);
                _webView.setLayoutParams(lp);
            }
        });
    }


    public void loadUrl(String url)
    {
        _url = url;
        gameActivity.runOnUiThread(new Runnable()
        {
            @Override
            public void run()
            {
                if (null == _webView) {
                    Log.i("LiteWebView", "loadUrl _webview is null");
                    return;
                }
                _webView.loadUrl(_url);
            }
        });
    }

    public void close()
    {
        gameActivity.runOnUiThread(new Runnable()
        {
            @Override
            public void run()
            {
                destroy();
            }
        });
    }


}

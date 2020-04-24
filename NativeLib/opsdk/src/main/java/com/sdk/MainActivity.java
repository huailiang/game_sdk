package com.sdk;

import android.content.Context;
import android.os.Bundle;

import com.unity3d.player.UnityPlayerActivity;

public class MainActivity extends UnityPlayerActivity
{

    private final static String TAG = "MainActivity";
    Context mContext = null;

    static
    {
        System.loadLibrary("Utility");
    }

    @Override
    public void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        mContext = this;
        initAllSdk();
    }

    @Override
    protected void onDestroy()
    {
        super.onDestroy();
    }

    @Override
    protected void onPause()
    {
        super.onPause();
        MLog.d(TAG, "Game Pause");
    }

    @Override
    protected void onResume()
    {
        super.onResume();
        MLog.d(TAG, "Game Resume");
    }

    public void initAllSdk()
    {
        MLog.d(TAG, "INIT SDK");
        WeiXinSdk.getInstance().setGameActivity(this);
        WeiXinSdk.getInstance().RegisterAppToWX(mContext);
        NativeHelper.setGameActivity(this, mContext);
        SystemInfo.getInstance().SetContext(mContext);
    }

    public void WeixinMessageShare(String message)
    {
        MLog.d(TAG, "WeixinShare: " + message);
        WeiXinSdk.getInstance().SendMessageToWX(true);
    }

    public void WeixinShareMessageToFriend(String message)
    {
        MLog.d(TAG, "WeixinShareToFriend: " + message);
        WeiXinSdk.getInstance().SendMessageToWX(false);
    }

    public void WeixinAppShare(String message)
    {
        MLog.d(TAG, "WeixinAppShare: " + message);
        WeiXinSdk.getInstance().SendAppData(true, message);
    }

    public void WeixinShareAppToFriend(String message)
    {
        MLog.d(TAG, "WeixinShareAppToFriend: " + message);
        WeiXinSdk.getInstance().SendAppData(false, message);
    }

    public void WeixinImageShare(String message)
    {
        MLog.d(TAG, "WeixinImageShare: " + message);
        WeiXinSdk.getInstance().SendBitmapToWX(true, message);
    }

    public void WeixinShareImageToFriend(String message)
    {
        MLog.d(TAG, "WeixinShareImageToFriend: " + message);
        WeiXinSdk.getInstance().SendBitmapToWX(false, message);
    }

    public void WeixinWebPageShare(String message)
    {
        MLog.d(TAG, "WeixinWebPageShare: " + message);
        WeiXinSdk.getInstance().SendWebPage(true, message);
    }

    public void WeixinShareWebPageToFriend(String message)
    {
        MLog.d(TAG, "WeixinShareWebPageToFriend: " + message);
        WeiXinSdk.getInstance().SendWebPage(false, message);
    }


}

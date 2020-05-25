package com.sdk;

import android.content.Context;
import android.os.Bundle;

import com.unity3d.player.UnityPlayerActivity;

import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;

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
        NativeHelper.setGameActivity(this, mContext);
        SystemInfo.getInstance().SetContext(mContext);
        MClipboard.setGameActivity(this, mContext);
        LiteWebView.setGameActivity(this);
    }

    public boolean Call(String gameObjectName, String functionName, String args)
    {
        try {
            Class<?> classtype = Class.forName("com.unity3d.player.UnityPlayer");
            Method method = classtype.getMethod("UnitySendMessage", String.class, String.class, String.class);
            method.invoke(classtype, gameObjectName, functionName, args);
            return true;
        }
        catch (ClassNotFoundException e) {
            MLog.e(TAG, e.getMessage());

        }
        catch (NoSuchMethodException e) {
            MLog.e(TAG, e.getMessage());

        }
        catch (IllegalAccessException e) {
            MLog.e(TAG, e.getMessage());

        }
        catch (InvocationTargetException e) {
            MLog.e(TAG, e.getMessage());
        }
        return false;
    }

}

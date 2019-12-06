package com.sdk;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.util.DisplayMetrics;
import android.util.Log;
import java.io.BufferedOutputStream;
import java.io.FileOutputStream;
import android.telephony.TelephonyManager;

/**
 *
 * @author hauiliangpeng
 * 这个类是为了获取手机的PPI,运营商信息等功能
 *
 */

public class SystemInfo
{
    private static final String TAG = "SystemInfo";
    private static SystemInfo uniqueInstance = null;

    public Context mContext;

    public static SystemInfo getInstance()
    {
        if (uniqueInstance == null) {
            uniqueInstance = new SystemInfo();
        }
        return uniqueInstance;
    }

    public void SetContext(Context ctx)
    {
        this.mContext = ctx;
    }

    public int GetDensity()
    {
        DisplayMetrics displayMetrics = this.mContext.getResources().getDisplayMetrics();
        Log.d(TAG, "getdensity: " + displayMetrics.densityDpi);

        return displayMetrics.densityDpi;
    }

    public String CheckSIM()
    {
        String SIMTypeString = "";
        TelephonyManager telManager = (TelephonyManager) this.mContext.getSystemService(Context.TELEPHONY_SERVICE);
        String operator = telManager.getSimOperator();
        if (operator != null)
        {
            if ((operator.equals("46000")) || (operator.equals("46002")))
            {
                SIMTypeString = "MM";
            } else if (operator.equals("46001"))
            {
                SIMTypeString = "UN";
            } else if (operator.equals("46003"))
            {
                SIMTypeString = "DX";
            }
        }

        return SIMTypeString;
    }


    public void ToJPG(String path, byte[] bytes)
    {
        try {
            Bitmap bmp = BitmapFactory.decodeByteArray(bytes, 0, bytes.length);
            BufferedOutputStream bos = new BufferedOutputStream(new FileOutputStream(path));
            bmp.compress(Bitmap.CompressFormat.JPEG, 80, bos);

            bos.flush();
            bos.close();
        }
        catch (Exception e) {
            e.printStackTrace();
        }
    }
}
package com.tencent.tmgp.dragonnest;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Bundle;
import android.telephony.TelephonyManager;
import android.util.DisplayMetrics;
import android.util.Log;
import java.io.BufferedOutputStream;
import java.io.FileOutputStream;

import com.unity3d.player.UnityPlayerActivity;

public class SystemInfoActivity extends UnityPlayerActivity
{
	private static final String TAG = "Unity";
	private static Context mContext;

	protected void onCreate(Bundle savedInstanceState)
	{
		super.onCreate(savedInstanceState);
		mContext = this;
	}

	// 获取ppi
	private int GetDensity()
	{
		DisplayMetrics displayMetrics = mContext.getResources().getDisplayMetrics();
		Log.d("Unity", "getdenstity: " + displayMetrics.densityDpi);
		return displayMetrics.densityDpi;
	}

	// 获取运营商
	public String CheckSIM()
	{
		String SIMTypeString = "";
		TelephonyManager telManager = (TelephonyManager) mContext.getSystemService("phone");
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

	// 用于转gif到jpg
	public void ToJPG(String path, byte[] bytes)
	{
		try
		{
			Bitmap bmp = BitmapFactory.decodeByteArray(bytes, 0, bytes.length);
			BufferedOutputStream bos = new BufferedOutputStream(new FileOutputStream(path));
			bmp.compress(Bitmap.CompressFormat.JPEG, 80, bos);
			bos.flush();
			bos.close();
		} catch (Exception e)
		{
			e.printStackTrace();
		}
	}


}

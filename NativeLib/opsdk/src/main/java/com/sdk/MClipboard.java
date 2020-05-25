package com.sdk;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.ClipData;
import android.content.ClipboardManager;
import android.content.Context;
import android.text.TextUtils;


public class MClipboard
{
    @SuppressLint("StaticFieldLeak")
    private static Activity gameActivity = null;
    @SuppressLint("StaticFieldLeak")
    private static Context gameContext = null;
    private final static String TAG = "MClipboard";


    public static void setGameActivity(Activity activity, Context context)
    {
        gameActivity = activity;
        gameContext = context;
    }

    /**
     * 获取剪切板内容
     */
    public static String Get()
    {
        {
            ClipboardManager cmb = (ClipboardManager) gameActivity.getSystemService(gameContext.CLIPBOARD_SERVICE);
            if (cmb != null && cmb.hasPrimaryClip() && cmb.getPrimaryClip().getItemCount() > 0) {
                CharSequence addedText = cmb.getPrimaryClip().getItemAt(0).getText();
                String addedTextString = String.valueOf(addedText);
                if (!TextUtils.isEmpty(addedTextString)) {
                    return addedTextString;
                }
            }
        }
        return "";
    }


    /**
     * 复制到剪切板
     *
     * @param content: 复制内容
     */
    public static void Copy(String content)
    {
        if (!TextUtils.isEmpty(content)) {
            ClipboardManager cmb = (ClipboardManager) gameActivity.getSystemService(gameContext.CLIPBOARD_SERVICE);
            if (cmb != null) {
                cmb.setText(content.trim());
                // 创建一个剪贴数据集，包含一个普通文本数据条目（需要复制的数据）
                ClipData clipData = ClipData.newPlainText(null, content);
                // 把数据集设置（复制）到剪贴板
                cmb.setPrimaryClip(clipData);
            }
        }
    }

    /**
     * 清空剪切板内容
     */
    public static void Clear()
    {
        ClipboardManager cmb = (ClipboardManager) gameActivity.getSystemService(gameContext.CLIPBOARD_SERVICE);
        if (cmb != null) {
            try {
                cmb.setPrimaryClip(ClipData.newPlainText(null, null));
            } catch (Exception e) {
                MLog.e(TAG, e.getMessage());
            }
        }
    }
}

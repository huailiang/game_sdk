package com.sdk;

import java.io.File;
import java.io.RandomAccessFile;

import com.tencent.mm.sdk.openapi.IWXAPI;
import com.tencent.mm.sdk.openapi.WXAPIFactory;
import com.tencent.mm.sdk.openapi.SendMessageToWX;
import com.tencent.mm.sdk.openapi.WXAppExtendObject;
import com.tencent.mm.sdk.openapi.WXImageObject;
import com.tencent.mm.sdk.openapi.WXMediaMessage;
import com.tencent.mm.sdk.openapi.WXTextObject;
import com.tencent.mm.sdk.openapi.WXWebpageObject;
import com.tencent.mm.sdk.platformtools.Util;

import android.app.Activity;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Environment;
import android.widget.Toast;

/**
 *
 * @huailiangpeng
 * 微信相关的实现
 *
 */
public class WeiXinSdk
{
    private final static String TAG = "WeiXinSdk";
    private static WeiXinSdk weixinMgr = null;
    private String mAppId = "wxe4fec45da4ee5570";
    private IWXAPI api;
    private Activity gameActivity = null;

    private static final int THUMB_SIZE = 150;
    private static final String SDCARD_ROOT = Environment.getExternalStorageDirectory().getAbsolutePath();

    private WeiXinSdk()
    {
    }

    public void setGameActivity(Activity gameActivity)
    {
        this.gameActivity = gameActivity;
    }

    public Activity getGameActivity()
    {
        return this.gameActivity;
    }

    public String getAppId()
    {
        return mAppId;
    }

    public static WeiXinSdk getInstance()
    {
        if (weixinMgr == null)
        {
            weixinMgr = new WeiXinSdk();
        }
        return weixinMgr;
    }

    public void RegisterAppToWX(Context context)
    {
        MLog.d(TAG, "RegisterAppToWX");
        api = WXAPIFactory.createWXAPI(context, mAppId, true);
        api.registerApp(mAppId);
    }

    // isTimeLine true: 分享朋友圈 false: 发送给好友
    public void SendMessageToWX(boolean isTimeLine)
    {
        // 初始化一个WXTextObject对象
        WXTextObject textObj = new WXTextObject();

        textObj.text = "蜡笔小新大冒险应用很好玩哦！";

        // 用WXTextObject对象初始化一个WXMediaMessage对象
        WXMediaMessage msg = new WXMediaMessage();
        msg.mediaObject = textObj;
        // 发送文本类型的消息时，title字段不起作用
        // msg.title = "Will be ignored";
        msg.description = "蜡笔小新大冒险";

        // 构造一个Req
        SendMessageToWX.Req req = new SendMessageToWX.Req();
        req.transaction = buildTransaction("text"); // transaction字段用于唯一标识一个请求
        req.message = msg;
        req.scene = isTimeLine ? SendMessageToWX.Req.WXSceneTimeline : SendMessageToWX.Req.WXSceneSession;

        // 调用api接口发送数据到微信
        api.sendReq(req);
    }

    public void SendBitmapToWX(boolean isTimeLine, String message)
    {
        Bitmap bmp = BitmapFactory.decodeResource(gameActivity.getResources(), R.drawable.app_icon);
        WXImageObject imgObj = new WXImageObject(bmp);

        WXMediaMessage msg = new WXMediaMessage();
        msg.mediaObject = imgObj;

        Bitmap thumbBmp = Bitmap.createScaledBitmap(bmp, THUMB_SIZE, THUMB_SIZE, true);
        bmp.recycle();
        msg.thumbData = Util.bmpToByteArray(thumbBmp, true); // 设置缩略图

        SendMessageToWX.Req req = new SendMessageToWX.Req();
        req.transaction = buildTransaction("img");
        req.message = msg;
        req.scene = isTimeLine ? SendMessageToWX.Req.WXSceneTimeline : SendMessageToWX.Req.WXSceneSession;
        api.sendReq(req);
    }

    public void SendLocalImgToWX(boolean isTimeLine)
    {
        String path = SDCARD_ROOT + "/test.png";
        File file = new File(path);
        if (!file.exists())
        {
            String tip = "发送的图片不存在";
            Toast.makeText(gameActivity, tip + " path = " + path, Toast.LENGTH_LONG).show();
            return;
        }

        WXImageObject imgObj = new WXImageObject();
        imgObj.setImagePath(path);

        WXMediaMessage msg = new WXMediaMessage();
        msg.mediaObject = imgObj;

        Bitmap bmp = BitmapFactory.decodeFile(path);
        Bitmap thumbBmp = Bitmap.createScaledBitmap(bmp, THUMB_SIZE, THUMB_SIZE, true);
        bmp.recycle();
        msg.thumbData = Util.bmpToByteArray(thumbBmp, true);

        SendMessageToWX.Req req = new SendMessageToWX.Req();
        req.transaction = buildTransaction("img");
        req.message = msg;
        req.scene = isTimeLine ? SendMessageToWX.Req.WXSceneTimeline : SendMessageToWX.Req.WXSceneSession;
        api.sendReq(req);
    }

    public void SendAppData(boolean isTimeLine, String message)
    {
        final WXAppExtendObject appdata = new WXAppExtendObject();
        final String path = SDCARD_ROOT + "/test.png";
        appdata.fileData = readFromFile(path, 0, -1);
        appdata.extInfo = "好玩的酷跑游戏";

        final WXMediaMessage msg = new WXMediaMessage();
        msg.setThumbImage(extractThumbNail(path, 150, 150, true));
        msg.title = "蜡笔小新大冒险";
        msg.description = message;
        msg.mediaObject = appdata;

        SendMessageToWX.Req req = new SendMessageToWX.Req();
        req.transaction = buildTransaction("appdata");
        req.message = msg;
        req.scene = isTimeLine ? SendMessageToWX.Req.WXSceneTimeline : SendMessageToWX.Req.WXSceneSession;
        api.sendReq(req);
    }

    public void SendWebPage(boolean isTimeLine, String message)
    {
        WXWebpageObject webpage = new WXWebpageObject();
        webpage.webpageUrl = "http://shin.mobage.cn";
        WXMediaMessage msg = new WXMediaMessage(webpage);
        if (isTimeLine)
        {
            msg.title = message;
            msg.description = "蜡笔小新大冒险";
        } else
        {
            msg.title = "蜡笔小新大冒险";
            msg.description = message;
        }
        Bitmap thumb = BitmapFactory.decodeResource(gameActivity.getResources(), R.drawable.app_icon);
        msg.thumbData = Util.bmpToByteArray(thumb, true);

        SendMessageToWX.Req req = new SendMessageToWX.Req();
        req.transaction = buildTransaction("webpage");
        req.message = msg;
        req.scene = isTimeLine ? SendMessageToWX.Req.WXSceneTimeline : SendMessageToWX.Req.WXSceneSession;
        api.sendReq(req);
    }

    private String buildTransaction(final String type)
    {
        return (type == null) ? String.valueOf(System.currentTimeMillis()) : type + System.currentTimeMillis();
    }

    public static byte[] readFromFile(String fileName, int offset, int len)
    {
        if (fileName == null)
        {
            return null;
        }

        File file = new File(fileName);
        if (!file.exists())
        {
            MLog.i(TAG, "readFromFile: file not found");
            return null;
        }

        if (len == -1)
        {
            len = (int) file.length();
        }

        MLog.d(TAG, "readFromFile : offset = " + offset + " len = " + len + " offset + len = " + (offset + len));

        if (offset < 0)
        {
            MLog.e(TAG, "readFromFile invalid offset:" + offset);
            return null;
        }
        if (len <= 0)
        {
            MLog.e(TAG, "readFromFile invalid len:" + len);
            return null;
        }
        if (offset + len > (int) file.length())
        {
            MLog.e(TAG, "readFromFile invalid file len:" + file.length());
            return null;
        }

        byte[] b = null;
        try
        {
            RandomAccessFile in = new RandomAccessFile(fileName, "r");
            b = new byte[len]; // 创建合适文件大小的数组
            in.seek(offset);
            in.readFully(b);
            in.close();

        } catch (Exception e)
        {
            MLog.e(TAG, "readFromFile : errMsg = " + e.getMessage());
            e.printStackTrace();
        }
        return b;
    }

    private static final int MAX_DECODE_PICTURE_SIZE = 1920 * 1440;

    public static Bitmap extractThumbNail(final String path, final int height, final int width, final boolean crop)
    {
        if (path != null && !path.equals("") && height > 0 && width > 0)
        {
            MLog.e(TAG, "PATH: "+path);
        }

        BitmapFactory.Options options = new BitmapFactory.Options();

        try
        {
            options.inJustDecodeBounds = true;
            Bitmap tmp = BitmapFactory.decodeFile(path, options);
            if (tmp != null)
            {
                tmp.recycle();
                tmp = null;
            }

            MLog.d(TAG, "extractThumbNail: round=" + width + "x" + height + ", crop=" + crop);
            final double beY = options.outHeight * 1.0 / height;
            final double beX = options.outWidth * 1.0 / width;
            MLog.d(TAG, "extractThumbNail: extract beX = " + beX + ", beY = " + beY);
            options.inSampleSize = (int) (crop ? (beY > beX ? beX : beY) : (beY < beX ? beX : beY));
            if (options.inSampleSize <= 1)
            {
                options.inSampleSize = 1;
            }

            // NOTE: out of memory error
            while (options.outHeight * options.outWidth / options.inSampleSize > MAX_DECODE_PICTURE_SIZE)
            {
                options.inSampleSize++;
            }

            int newHeight = height;
            int newWidth = width;
            if (crop)
            {
                if (beY > beX)
                {
                    newHeight = (int) (newWidth * 1.0 * options.outHeight / options.outWidth);
                } else
                {
                    newWidth = (int) (newHeight * 1.0 * options.outWidth / options.outHeight);
                }
            } else
            {
                if (beY < beX)
                {
                    newHeight = (int) (newWidth * 1.0 * options.outHeight / options.outWidth);
                } else
                {
                    newWidth = (int) (newHeight * 1.0 * options.outWidth / options.outHeight);
                }
            }

            options.inJustDecodeBounds = false;

            MLog.i(TAG, "bitmap required size=" + newWidth + "x" + newHeight + ", orig=" + options.outWidth + "x"
                    + options.outHeight + ", sample=" + options.inSampleSize);
            Bitmap bm = BitmapFactory.decodeFile(path, options);
            if (bm == null)
            {
                MLog.e(TAG, "bitmap decode failed");
                return null;
            }

            MLog.i(TAG, "bitmap decoded size=" + bm.getWidth() + "x" + bm.getHeight());
            final Bitmap scale = Bitmap.createScaledBitmap(bm, newWidth, newHeight, true);
            if (scale != null)
            {
                bm.recycle();
                bm = scale;
            }

            if (crop)
            {
                final Bitmap cropped = Bitmap.createBitmap(bm, (bm.getWidth() - width) >> 1,
                        (bm.getHeight() - height) >> 1, width, height);
                if (cropped == null)
                {
                    return bm;
                }

                bm.recycle();
                bm = cropped;
                MLog.i(TAG, "bitmap croped size=" + bm.getWidth() + "x" + bm.getHeight());
            }
            return bm;

        } catch (final OutOfMemoryError e)
        {
            MLog.e(TAG, "decode bitmap failed: " + e.getMessage());
            options = null;
        }

        return null;
    }
}

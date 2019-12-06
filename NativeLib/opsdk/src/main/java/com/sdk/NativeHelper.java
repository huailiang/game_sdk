package com.sdk;

import android.app.Activity;
import android.app.ActivityManager;
import android.content.Context;
import android.os.Debug;
import android.content.res.AssetManager;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.Enumeration;
import java.util.zip.ZipEntry;
import java.util.zip.ZipFile;
import java.util.zip.ZipInputStream;

/**
 * @author penghuailiang
 * native helper for unity3d
 */
public class NativeHelper
{

    private static Activity gameActivity = null;
    private static Context gameContext = null;
    private final static String TAG = "XNativeHelper";

    public static native void SetAssetManager(AssetManager assetManager);


    public static void setGameActivity(Activity activity, Context context)
    {
        gameActivity = activity;
        gameContext = context;
        SetAssetManager(context.getAssets());
    }

    //获取内存
    public static float GetMemory()
    {
        float memory = -1;
        try {
            int pid = android.os.Process.myPid();
            ActivityManager mActivityManager = (ActivityManager) gameActivity.getSystemService(Context.ACTIVITY_SERVICE);
            Debug.MemoryInfo[] memoryInfoArray = mActivityManager.getProcessMemoryInfo(new int[]{pid});
            memory = (float) memoryInfoArray[0].getTotalPrivateDirty() / 1024;
        }
        catch (Exception e) {
            MLog.e(TAG, e.toString());
        }
        return memory;
    }


    /*
     返回值为0时正常 其余都不正常
     */
    public static int CopyStreamingAsset(String src, String dst)
    {
        File dir = new File(dst);
        if (!dir.exists() || !dir.isDirectory()) {
            dir.mkdir();
        }
        File file = new File(dir, src);
        try {
            if (gameContext == null) {
                MLog.e(TAG, "CONTEXT IS NULL");
                return -1;
            }

            InputStream is = gameContext.getAssets().open(src);
            if (!is.markSupported()) {
                MLog.e(TAG, "Input is NULL");
                return -2;
            }
            FileOutputStream os = new FileOutputStream(file);
            byte[] buffer = new byte[1024];
            int len;

            while ((len = is.read(buffer)) != -1) {
                os.write(buffer, 0, len);
            }
            os.flush();
            os.close();
            is.close();
        }
        catch (Exception e) {
            MLog.e(TAG, e.getMessage());
            return -3;
        }
        return 0;
    }


    public static byte[] LoadStream(String path)
    {
        InputStream instream = null;
        ByteArrayOutputStream outStream = null;
        try {
            instream = gameContext.getAssets().open(path);
            outStream = new ByteArrayOutputStream();
            byte[] bytes = new byte[4 * 1024];
            int len;
            while ((len = instream.read(bytes)) != -1) {
                outStream.write(bytes, 0, len);
            }
            return outStream.toByteArray();
        }
        catch (IOException e) {
            MLog.e(TAG, e.getMessage());
        }
        finally {
            try {
                if (instream != null) {
                    instream.close();
                }
                if (outStream != null) {
                    outStream.close();
                }
            }
            catch (Exception e) {
                MLog.e(TAG, e.getMessage());
            }
            System.gc();
        }
        return null;
    }

    public static String LoadSteamString(String path)
    {
        InputStream ins = null;
        try {
            ins = gameContext.getAssets().open(path);
            ByteArrayOutputStream os = new ByteArrayOutputStream();
            int idx = -1;
            while ((idx = ins.read()) != -1) {
                os.write(idx);
            }
            return os.toString();
        }
        catch (Exception e) {
            MLog.e(TAG, e.getMessage());
        }
        finally {
            try {
                if (ins != null) {
                    ins.close();
                }
            }
            catch (Exception e) {
                MLog.e(TAG, e.getMessage());
            }
        }
        return "";
    }

    /**
     * zip解压
     *
     * @param srcPath  zip源文件
     * @param unzipath 解压后的目标文件夹
     * @throws RuntimeException 解压失败会抛出运行时异常
     */


    public static void unZip(String srcPath, String unzipath)
    {
        try {
            byte doc[] = null;
            MLog.d(TAG, "SRC: " + srcPath);
            MLog.d(TAG, "DST: " + unzipath);
            InputStream is = gameContext.getAssets().open(srcPath);
            ZipInputStream zipis = new ZipInputStream(is);
            ZipEntry fentry = null;
            while ((fentry = zipis.getNextEntry()) != null) {
                if (fentry.isDirectory()) {
                    File dir = new File(unzipath + fentry.getName());
                    if (!dir.exists()) {
                        dir.mkdirs();
                    }
                }
                else {
                    //fname是文件名,fileoutputstream与该文件名关联
                    String fname = new String(unzipath + fentry.getName());
                    MLog.d(TAG, "unzip: " + fname);
                    try {
                        //新建一个out,指向fname，fname是输出地址
                        FileOutputStream out = new FileOutputStream(fname);
                        doc = new byte[512];
                        int n;
                        //若没有读到，即读取到末尾，则返回-1
                        while ((n = zipis.read(doc, 0, 512)) != -1) {
                            //这就把读取到的n个字节全部都写入到指定路径了
                            out.write(doc, 0, n);
                        }
                        is.close();
                        out.close();
                        doc = null;
                    }
                    catch (Exception ex) {
                        MLog.e(TAG, "there is a problem");
                    }
                }
            }
            zipis.close();
            is.close();
        }
        catch (IOException ioex) {
            MLog.e(TAG, "io错误：" + ioex);
        }
        MLog.d(TAG, "finished!");
    }

}

package com.perfectworld.cfgame.wxapi;

import android.app.Activity;
import android.content.Context;
import android.os.Bundle;
import android.widget.Toast;

import com.sdk.WeiXinSdk;
import com.tencent.mm.sdk.openapi.BaseReq;
import com.tencent.mm.sdk.openapi.BaseResp;
import com.tencent.mm.sdk.openapi.IWXAPI;
import com.tencent.mm.sdk.openapi.WXAPIFactory;
import com.tencent.mm.sdk.openapi.IWXAPIEventHandler;
import com.sdk.MLog;

public class WXEntryActivity extends Activity implements IWXAPIEventHandler
{

    private final static String TAG = "WXEntryActivity";
    Context mContext = null;
    private IWXAPI api;

    @Override
    public void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        mContext = this;

        api = WXAPIFactory.createWXAPI(mContext, WeiXinSdk.getInstance().getAppId(), false);
        api.registerApp(WeiXinSdk.getInstance().getAppId());
        api.handleIntent(getIntent(), this);
        MLog.d(TAG, "onCreate");
    }

    // 微信发送请求到第三方应用时，会回调到该方法
    @Override
    public void onReq(BaseReq req)
    {

    }

    // 第三方应用发送到微信的请求处理后的响应结果，会回调到该方法
    @Override
    public void onResp(BaseResp resp)
    {
        String result = " ";

        switch (resp.errCode)
        {
            case BaseResp.ErrCode.ERR_OK:
                result = "发送成功";
                break;
            case BaseResp.ErrCode.ERR_USER_CANCEL:
                result = "发送取消";
                break;
            case BaseResp.ErrCode.ERR_AUTH_DENIED:
                result = "发送被拒绝";
                break;
            default:
                result = "发送返回";
                break;
        }

        Toast.makeText(this, result, Toast.LENGTH_SHORT).show();
        WXEntryActivity.this.finish();
    }
}

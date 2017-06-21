using System;

public class SDKAttribute : Attribute
{
    
    public int appId { get; private set; }
    public string appKey { get; private set; }
    public string notifyObjName { get; private set; }
    public bool logEnable { get; private set; }
    public int rechargeAmount { get; private set; }
    public bool isLongConnect { get; private set; }
    public bool rechargeEnable { get; private set; }
    public string closeRechargeAlertMsg { get; protected set; }
    public bool isOriLandscapeLeft { get; private set; }
    public bool isOriLandscapeRight { get; private set; }
    public bool isOriPortrait { get; private set; }
    public bool isOriPortraitUpsideDown { get; private set; }


    public SDKAttribute(
           int appId,
           string appKey,
           string noficationObjectName,
           bool isLongConnect,
           bool rechargeEnable,
           int rechargeAmount,
           string closeRechargeAlertMsg,
           bool isOriPortrait,
           bool isOriLandscapeLeft,
           bool isOriLandscapeRight,
           bool isOriPortraitUpsideDown,
           bool logEnable)
    {
        this.appId = appId;
        this.appKey = appKey;
        this.notifyObjName = noficationObjectName;
        this.logEnable = logEnable;
        this.rechargeAmount = rechargeAmount;
        this.isLongConnect = isLongConnect;
        this.rechargeEnable = rechargeEnable;
        this.closeRechargeAlertMsg = closeRechargeAlertMsg;
        this.isOriLandscapeLeft = isOriLandscapeLeft;
        this.isOriLandscapeRight = isOriLandscapeRight;
        this.isOriPortrait = isOriPortrait;
        this.isOriPortraitUpsideDown = isOriPortraitUpsideDown;
    }
    


}

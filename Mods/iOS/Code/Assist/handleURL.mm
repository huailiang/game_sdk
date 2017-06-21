//
//  handleURL.m
//  Unity-iPhone
//
//  Created by 彭怀亮 on 17/06/14.
//

#include "../SDKDef.h"

#if defined (SYS_IOS)

#import "handleURL.h"

#ifdef USE_WX
#import "WXPlugin.h"
#endif

#ifdef PLATFORM_ID_DOWNJOY
#import <DownjoySDK/DJPlatformNotification.h>
#elif defined PLATFORM_ID_I4SDK
#import <AsSdkFMWK/AsInfoKit.h>
#elif defined PLATFORM_ID_HAIMA
#import <IapppayKit/IapppayKit.h>
#elif defined PLATFORM_ID_PP
#import <PPAppPlatformKit/PPAppPlatformKit.h>
#elif defined PLATFORM_ID_TONGBUTUI
#elif defined PLATFORM_ID_ITOOLS
#import <AlipaySDK/AlipaySDK.h>
#import "HXAppPlatformKitPro.h"
#elif defined PLATFORM_ID_XYSDK
#import <XYPlatform/XYPlatform.h>
#elif defined PLATFORM_ID_KY
#import <xsdkFramework/XSDK.h>
#elif defined PLATFORM_ID_GIANT
#import "ZTLibBase.h"
#import "ZTServicePush.h"
#elif defined PLATFORM_ID_WY51
#import <WYPlatformSDK/WYPlatformSDK.h>
#elif defined PLATFORM_ID_KOREAN
#import <IgaworksAD/IgaworksAD.h>
#elif defined PLATFORM_ID_THAILAND
#import <KunlunSDK/KLPlatform.h>
#import "AppsFlyerTracker.h"
#import <KunlunSDKHelper/KunlunSDKHeader.h>
#import <KunlunSDKHelper/KunlunSDKHelperHeader.h>
#import <KunlunSNSSDKForFacebook/KunlunSNS.h>
#import <KunlunSNSSDKForFacebook/FacebookCenterShowRequestBean.h>
#import <KunlunSNSSDKForFacebook/FacebookCenterDismissRequestBean.h>
#import <FacebookSDK/FacebookSDK.h>
#import <KunlunSDK/KLFBGameInterFace.h>
#elif defined PLATFORM_ID_XINMA
#import <GBSDK/GBSDK.h>
#elif defined PLATFORM_ID_GUOPAN
#import "GPGameSDK.h"
#import "GPGameSDK_Pay.h"
#elif defined PLATFORM_ID_TENCENT
#import <MSDK/MSDK.h>
#import <MSDKFoundation/MSDKFoundation.h>
#import <MSDKXG/MSDKXG.h>
#import "MyObserver.h"
#import "JoyYouRPBroadcast.h"
#import "TencentMSDKPlatformAPIProxy.h"
#elif defined PLATFORM_ID_BAOZOU
#import <Payment/Payment.h>
#import <AlipaySDK/AlipaySDK.h>
#import "SPayClient.h"
#else
#endif

#ifdef PLUGIN_ID_FACEBOOK
#import <FBSDKCoreKit/FBSDKCoreKit.h>
#endif

#ifdef PLUGIN_ID_NAVER
#import "../Naver/NaverProxy.h"
#endif

#ifdef PLUGIN_ID_GOOGLEPLUS
#import <GooglePlus/GPPURLHandler.h>
#import <GooglePlus/GPPSignIn.h>
#endif

#ifdef PLUGIN_ID_SDO_PUSH
#import "GHomePushAPI.h"
#endif

#ifdef PLUGIN_ID_BUGLY
#import <Bugly/CrashReporter.h>
#import <Bugly/BuglyLog.h>
#endif

#ifdef PLUGIN_ID_TENCENT_WX
#import "WXPlugin.h"
#endif

#ifdef PLUGIN_ID_TENCENT_QQ
#import "QQPlugin.h"
#endif

#ifdef PLUGIN_ID_JOYME
#import "JoymeVideoSDK.h"
#endif

@implementation SDKProcess

+ (BOOL)application:(UIApplication *)application handleOpenURL:(NSURL *)url
{
	BOOL retValue = NO;

#ifdef SYS_IOS

#ifdef PLATFORM_ID_TENCENT
	retValue = [WGInterface HandleOpenURL:url];
#endif

#ifdef PLATFORM_ID_BAOZOU
    retValue = [[MCHApi sharedInstance] application:application handleOpenURL:url];
#endif

#endif

	return retValue;
}

+ (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation;
{
    BOOL retValue = NO;

#ifdef SYS_IOS

#ifdef USE_WX
    [[WXShareApi sharedInstance] handleOpenURL:url];
#endif

#ifdef PLATFORM_ID_PP
    [[PPAppPlatformKit share] alixPayResult:url];

#elif defined (PLATFORM_ID_ITOOLS)
    if ([url.host isEqualToString:@"safepay"])
    {
        [[AlipaySDK defaultService] processOrderWithPaymentResult:url standbyCallback:^(NSDictionary *resultDic)
        {
            [HXAppPlatformKitPro alipayCallBack:resultDic];
        }];
    }

#elif defined PLATFORM_ID_DOWNJOY
    [[NSNotificationCenter defaultCenter]
            postNotificationName:kDJPlatfromAlixQuickPayEnd
                          object:url
    ];

#elif defined PLATFORM_ID_I4SDK
    //支付宝、QQ钱包回调
    [[AsInfoKit sharedInstance] payResult:url sourceApplication:sourceApplication];
    retValue = YES;

#elif defined PLATFORM_ID_HAIMA
    [[IapppayKit sharedInstance] handleOpenUrl:url];

#elif defined PLATFORM_ID_TONGBUTUI

#elif defined PLATFORM_ID_XYSDK
     [[XYPlatform defaultPlatform] XYHandleOpenURL:url];

#elif defined PLATFORM_ID_KY
    BOOL isKYMsg = [[XSDK instanceXSDK] handleApplication:application openURL:url sourceApplication:sourceApplication annotation:annotation];
    if (isKYMsg) {
        // pass
    }

#elif defined PLATFORM_ID_GIANT
    [[ZTLibBase getInstance] applicationZTGame:application openURL:url sourceApplication:sourceApplication annotation:annotation];

#elif defined PLATFORM_ID_WY51
    [[WYPlatformSDK sharedPlatformSDK] WYHandleOpenURL:url];

#elif defined PLATFORM_ID_XINMA
    retValue = [[GBStatistic shareInstance] gbApplication:application openURL:url sourceApplication:sourceApplication annotation:annotation];

#elif defined PLATFORM_ID_THAILAND
    [[KLPlatform sharedInstance] alixpayParseURL:url andLocation:@"cn"];
    [[KunlunSNS sharedInstance] application:application openURL:url sourceApplication:sourceApplication annotation:annotation];
    [FBAppCall handleOpenURL:url sourceApplication:sourceApplication];

#elif defined PLATFORM_ID_GUOPAN
    retValue = [[GPGameSDK_Pay defaultGPGamePay] openUrlResponse:url];

#elif defined PLATFORM_ID_TENCENT
    retValue = [WGInterface  HandleOpenURL:url];

    WGPlatformObserver * ob = WGPlatform::GetInstance()->GetObserver();
    if (!ob)
    {
        MyObserver * ob = MyObserver::GetInstance();
        WGPlatform::GetInstance()->WGSetObserver(ob);
        WGPlatform::GetInstance()->WGSetGroupObserver(ob);
        WGPlatform::GetInstance()->WGSetADObserver(ob);
        WGPlatform::GetInstance()->WGSetWebviewObserver(ob);
        WGPlatform::GetInstance()->WGSetRealNameAuthObserver(ob);
    }

#elif defined PLATFORM_ID_BAOZOU
    [[MCHApi sharedInstance] application:application openURL:url sourceApplication:sourceApplication annotation:annotation];
    retValue = YES;

#else

#endif

#ifdef PLUGIN_ID_FACEBOOK
    BOOL retValue_fb = YES;
    retValue_fb = [[FBSDKApplicationDelegate sharedInstance] application:application
                                                                 openURL:url
                                                       sourceApplication:sourceApplication
                                                              annotation:annotation];
    retValue = retValue || retValue_fb;
#endif

#ifdef PLUGIN_ID_NAVER
    BOOL retValue_naver = YES;
    retValue_naver = [[NaverProxy sharedInstance ] application:application
            										   openURL:url
            								 sourceApplication:sourceApplication
            									    annotation:annotation];
    retValue = retValue || retValue_naver;
#endif
    
#ifdef PLUGIN_ID_GOOGLEPLUS
    BOOL retValue_gg = YES;
    retValue_gg = [GPPURLHandler handleURL:url
                         sourceApplication:sourceApplication
                         		annotation:annotation];
    retValue = retValue || retValue_gg;
#endif

#ifdef PLUGIN_ID_TENCENT_WX
    BOOL retValue_wx = YES;
    retValue_wx = [[WXShareApi sharedInstance] handleOpenURL:url];
    retValue = retValue || retValue_wx;
#endif

#ifdef PLUGIN_ID_TENCENT_QQ
    BOOL retValue_QQ = YES;
    retValue_QQ = [[QQShareApi sharedInstance] handleOpenURL:url ];
    retValue = retValue || retValue_QQ;
#endif

#ifdef PLUGIN_ID_JOYME
	BOOL retValue_Joyme = YES;
	retValue_Joyme = [JoymeVideoSDK handleOpenURL:url];
	retValue = retValue || retValue_Joyme;
#endif

#endif

    return retValue;
}

+ (void)applicationDidBecomeActive:(UIApplication *)application
{
#ifdef SYS_IOS

#if defined PLATFORM_ID_GIANT
    [[ZTLibBase getInstance] applicationDidBecomeActiveZTGame];

#elif defined PLATFORM_ID_THAILAND
    [[KLPlatform sharedInstance] resetPushNotify];
    [[AppsFlyerTracker sharedTracker] trackAppLaunch];
    [[KunlunSNS sharedInstance] applicationDidBecomeActive:application];
    [[KLFBGameInterFace sharedInstance] applicationForFacebookDidBecomeActive];

#elif defined PLATFORM_ID_TENCENT
    // 清空Badge
    [MSDKXG WGCleanBadgeNumber];

    // 自动恢复直播
    [[JoyYouRPBroadcast jySharedInstance] jyResumeBroadcasting];

    // 记录当前的屏幕亮度水平
    [TencentMSDKPlatformAPIProxy sharedInstance].screenBrightness = [UIScreen mainScreen].brightness;
    // NSLog(@"JoyYouSDK ::: current screen brightness is %f", [TencentMSDKPlatformAPIProxy sharedInstance].screenBrightness);

#else

#endif

#ifdef PLUGIN_ID_FACEBOOK
    [FBSDKAppEvents activateApp];
#endif

#endif
}

+ (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
{
    BOOL retValue = YES;

#ifdef SYS_IOS

#if defined PLATFORM_ID_GIANT
    [ZTLibBase getInstance];

#elif defined PLATFORM_ID_XYSDK
    [XYPlatform defaultPlatform];
    NSString *urlScheme = [[[NSBundle mainBundle] bundleIdentifier] stringByAppendingString:@".alipay"];
    [XYPlatform defaultPlatform].appScheme = urlScheme;

#elif defined PLATFORM_ID_KOREAN
    [IgaworksAD igaworksADWithAppKey:@"596093364" andHashKey:@"3327460c969a48f9" andIsUseIgaworksRewardServer:NO];

#elif defined PLATFORM_ID_THAILAND
    [[KLPlatform sharedInstance] registPushNotifyType];

    [AppsFlyerTracker sharedTracker].appsFlyerDevKey = @"NzqMLQwdFNbKSXoortCXgN";
    [AppsFlyerTracker sharedTracker].appleAppID = @"1040048225";
    [AppsFlyerTracker sharedTracker].currencyCode = @"USD";
    
    [[KunlunSNS sharedInstance] application:application didFinishLaunchingWithOptions:launchOptions];

#elif defined PLATFORM_ID_XINMA
    [[GBStatistic shareInstance] statistic]; 			// 游戏行为追踪
    [[GBIAPHelper shareInstance] addObserver]; 			// 支付初始化
    retValue = [[GBStatistic shareInstance] gbApplication:application didFinishLaunchingWithOptions:launchOptions];

#elif defined PLATFORM_ID_TENCENT
    // 注册推送
    NSLog(@"JoyYou-TencentMSDK ::: Is here register apns push notification");
    [MSDKXG WGRegisterAPNSPushNotification:launchOptions];

    WGPlatformObserver * ob = WGPlatform::GetInstance()->GetObserver();
    if (!ob)
    {
        MyObserver * ob = MyObserver::GetInstance();
        WGPlatform::GetInstance()->WGSetObserver(ob);
        WGPlatform::GetInstance()->WGSetGroupObserver(ob);
        WGPlatform::GetInstance()->WGSetADObserver(ob);
        WGPlatform::GetInstance()->WGSetWebviewObserver(ob);
        WGPlatform::GetInstance()->WGSetRealNameAuthObserver(ob);
    }

    // MSDK日志开关
    WGPlatform::GetInstance()->WGOpenMSDKLog(false);

    // Crash数据上报开关
    WGPlatform::GetInstance()->WGEnableCrashReport(true, false);

#elif defined PLATFORM_ID_BAOZOU
    ApiConfigInfo *configInfo = [[ApiConfigInfo alloc] init];
    [configInfo setAppScheme:@"mchpaysdk"]; // 支付宝网页支付回调
    [configInfo setWxAppId:@"wx91386443e1dfb650"]; // 微信AppId
    [[MCHApi sharedInstance] initApi:application didFinishLaunchingWithOptions:launchOptions configInfo:configInfo];
    
    // NSLog(@"JoyYou-Baozou :: sdk版本号: %@", [SPayClient sharedInstance].spaySDKVersion);
    // NSLog(@"JoyYou-Baozou :: sdk版本类型: %@", [SPayClient sharedInstance].spaySDKTypeName);

#else

#endif

#ifdef PLUGIN_ID_FACEBOOK
    retValue = [[FBSDKApplicationDelegate sharedInstance] application:application
                                        didFinishLaunchingWithOptions:launchOptions];
#endif

#ifdef PLUGIN_ID_SDO_PUSH
    [GHomePushAPI receiveRemoteNotification:launchOptions];
#endif

#ifdef PLUGIN_ID_BUGLY
    [BuglyLog initLogger:BLYLogLevelDebug consolePrint:YES];

    [[CrashReporter sharedInstance] enableLog:YES];
    [[CrashReporter sharedInstance] installWithAppId:@"900001055" applicationGroupIdentifier:@"XXXXX"];

    exp_call_back_func = &exception_callback_handler;
#endif
    
#endif

    return retValue;
}

/*
static int exception_callback_handler()
{
    NSLog(@"bugly exception callback handler");
    
    NSException * exception = [[CrashReporter sharedInstance] getCurrentException];
    if (exception) {
        // 捕获的Obj-C异常
    }
    
    // 捕获的错误信号堆栈
    NSString * callStack = [[CrashReporter sharedInstance] getCrashStack];
    NSLog(@"%@", callStack);
    
    // 设置崩溃场景的附件
    [[CrashReporter sharedInstance] setUserData:@"用户身份" value:@"用户名"];
    [[CrashReporter sharedInstance] setAttachLog:@"业务关键日志"];
    
    return 1;
}
*/

+ (void)applicationDidEnterBackground:(UIApplication *)application
{
#ifdef SYS_IOS
    
#if defined PLATFORM_ID_GIANT
    [[ZTLibBase getInstance] applicationDidEnterBackgroundZTGame];
#endif

#if defined PLATFORM_ID_BAOZOU
    [[MCHApi sharedInstance] applicationDidEnterBackground:application];
#endif

#endif
}

+ (void)applicationWillEnterForeground:(UIApplication *)application
{
#ifdef SYS_IOS
    
#if defined PLATFORM_ID_GIANT
    [[ZTLibBase getInstance] applicationWillEnterForegroundZTGame];

#elif defined PLATFORM_ID_TENCENT
    WGPlatform * plat = WGPlatform::GetInstance();
    WGPlatformObserver * ob = plat->GetObserver();
    if (!ob)
    {
        MyObserver * ob = MyObserver::GetInstance();
        plat->WGSetObserver(ob);
        plat->WGSetGroupObserver(ob);
        plat->WGSetADObserver(ob);
        plat->WGSetWebviewObserver(ob);
        plat->WGSetRealNameAuthObserver(ob);
    }

    LoginRet ret;
    WGPlatform::GetInstance()->WGGetLoginRecord(ret);
    if (ret.flag == eFlag_Succ || ret.flag == eFlag_WX_AccessTokenExpired)
    {
        plat->WGLogin();
    }

#elif defined PLATFORM_ID_BAOZOU
    [[MCHApi sharedInstance] applicationWillEnterForeground:application];

#else

#endif
    
#endif
}

+ (void)application:(UIApplication *)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData *)deviceToken
{
#ifdef SYS_IOS
    
#if defined PLATFORM_ID_GIANT
    [ZTServicePush registerDeviceToken:deviceToken];

#elif defined PLATFORM_ID_THAILAND
    [[KLPlatform sharedInstance] setPushNotifyDeviceToken:deviceToken];

#elif defined PLATFORM_ID_TENCENT
    // NSLog(@"JoyYou-TencentMSDK ::: Register remote notifications succeeded with deviceToken: %@", deviceToken);
    [MSDKXG WGSuccessedRegisterdAPNSWithToken:deviceToken];

#else

#endif
    
#ifdef PLUGIN_ID_SDO_PUSH
    [GHomePushAPI setDeviceToken:deviceToken];
#endif
    
#endif
}

+ (void)application:(UIApplication *)application didFailToRegisterForRemoteNotificationsWithError:(NSError *)error
{
#ifdef SYS_IOS

#if defined PLATFORM_ID_TENCENT
	NSLog(@"JoyYou-TencentMSDK ::: Register remote notifications failed with error: %@", [error description]);
	[MSDKXG WGFailedRegisteredAPNS];
#endif

#endif
}

+ (NSUInteger)application:(UIApplication *)application supportedInterfaceOrientationsForWindow:(UIWindow *)window
{
#ifdef SYS_IOS
    
#ifdef PLATFORM_ID_HAIMA
    return [[IapppayKit sharedInstance] application:application supportedInterfaceOrientationsForWindow:window];

#elif defined PLATFORM_ID_XYSDK
    NSString * urlScheme = [[[NSBundle mainBundle] bundleIdentifier] stringByAppendingString:@".alipay"];
    [XYPlatform defaultPlatform].appScheme = urlScheme;

    return [[XYPlatform defaultPlatform] application:application supportedInterfaceOrientationsForWindow:window];

#else

#endif

    return 0;

#endif
}

+ (void)application:(UIApplication *)application didReceiveLocalNotification:(UILocalNotification *)notification
{
#ifdef SYS_IOS

#ifdef PLATFORM_ID_TENCENT
    // 处理本地推送消息
    // NSLog(@"JoyYou-TencentMSDK ::: 收到本地推送消息: %@ %@", notification.alertBody, notification.userInfo);

    LocalMessage message;

    message.alertBody = [notification.alertBody cStringUsingEncoding:NSUTF8StringEncoding];

    std::vector<KVPair> userInfo;
    for (NSString * key in notification.userInfo.allKeys)
    {
        NSString * value = notification.userInfo[key];
        KVPair item;
        item.key = [key cStringUsingEncoding:NSUTF8StringEncoding];
        item.value = [value cStringUsingEncoding:NSUTF8StringEncoding];
        userInfo.push_back(item);
    }
    message.userInfo = userInfo;

    // 必要时回调该message到游戏
    // 游戏可根据该message做出不同的策略
#endif

#ifdef PLUGIN_ID_SDO_PUSH
    // NSLog(@"receive local notification: [%@]", [GHomePushAPI receiveLocalNotification:notification]);
#endif

#endif
}

+ (void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo
{
#ifdef SYS_IOS
    
#if defined PLATFORM_ID_THAILAND
    [[KLPlatform sharedInstance] processPushNotification:userInfo];

#elif defined PLATFORM_ID_TENCENT
    // 处理远程推送消息
    // NSLog(@"JoyYou-TencentMSDK ::: userinfo: %@", userInfo);

    // NSLog(@"JoyYou-TencentMSDK ::: 收到推送消息: %@", [[userInfo objectForKey:@"aps"] objectForKey:@"alert"]);

    // 接收消息
    [MSDKXG WGReceivedMSGFromAPNSWithDict:userInfo];

    NSDictionary * dicCus = [userInfo objectForKey:@"custom"];
    if (nil != dicCus)
    {
        // parser dicCus and callBack to Game
        NSString * msgId = [dicCus objectForKey:@"msgId"];
        NSString * content = [dicCus objectForKey:@"content"];
        UIAlertView * alertView = [[UIAlertView alloc] initWithTitle:msgId message:content delegate:nil cancelButtonTitle:@"hide" otherButtonTitles:nil, nil];
        [alertView show];
    }

#else

#endif

#ifdef PLUGIN_ID_SDO_PUSH
    [GHomePushAPI receiveRemoteNotification:userInfo];
#endif
    
#endif
}

+ (void)dealloc
{
#ifdef SYS_IOS

#if defined PLATFORM_ID_TENCENT

    [[TencentMSDKPlatformAPIProxy sharedInstance] RemoveMSDKObserver];

#else

#endif

#endif
}


+ (void)touchesMoved:(NSSet *)touches withEvent:(UIEvent *)event
{
#ifdef SYS_IOS

#if defined PLATFORM_ID_TENCENT

    if ([TencentMSDKPlatformAPIProxy sharedInstance].isSupport3DTouch)
    {
        // Observe force touch
        UITouch * touch = [touches anyObject];

        CGPoint touchPoint = [touch locationInView:[[UIApplication sharedApplication] keyWindow].rootViewController.view];
        // float touchX = touchPoint.x;
        // float touchY = touchPoint.y;
        // NSLog(@"JoyYouSDK ::: current touch location is: x = %f && y = %f", touchX, touchY);

        CGFloat currentForce = touch.force;
        CGFloat baseForce = touch.maximumPossibleForce * 0.8;
        // NSLog(@"JoyYouSDK ::: currentForce is %f && maxForce is %f", currentForce, touch.maximumPossibleForce);

        if (currentForce > baseForce)
        {
            [[TencentMSDKPlatformAPIProxy sharedInstance] JoyYouNoticeOfForceTouch:touchPoint];
        }
    }

#else

#endif

#endif
}

@end

#endif

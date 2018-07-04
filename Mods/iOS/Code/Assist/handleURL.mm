//
//  handleURL.m
//  Unity-iPhone
//
//  Created by 彭怀亮 on 17/06/14.
//

#include "../SDKDef.h"

#if defined (SYS_IOS)

#import "handleURL.h"

#if defined PLATFORM_ID_TENCENT
#import <MSDK/MSDK.h>
#import <MSDKFoundation/MSDKFoundation.h>
#import <MSDKXG/MSDKXG.h>
#import "MyObserver.h"
#import "JoyYouRPBroadcast.h"
#import "TencentMSDKPlatformAPIProxy.h"
#endif

#ifdef PLUGIN_ID_FACEBOOK
#import <FBSDKCoreKit/FBSDKCoreKit.h>
#endif



@implementation SDKProcess

+ (BOOL)application:(UIApplication *)application handleOpenURL:(NSURL *)url
{
	BOOL retValue = NO;

#ifdef SYS_IOS

#ifdef PLATFORM_ID_TENCENT
	retValue = [WGInterface HandleOpenURL:url];
#endif


#endif

	return retValue;
}

+ (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation;
{
    BOOL retValue = NO;

#ifdef SYS_IOS

#if defined PLATFORM_ID_TENCENT
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
#endif
#endif
    return retValue;
}

+ (void)applicationDidBecomeActive:(UIApplication *)application
{
#ifdef SYS_IOS

#endif
}

+ (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
{
    BOOL retValue = YES;

#ifdef SYS_IOS

#if defined PLATFORM_ID_TENCENT
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

#if defined PLATFORM_ID_BAOZOU
    [[MCHApi sharedInstance] applicationDidEnterBackground:application];
#endif

#endif
}

+ (void)applicationWillEnterForeground:(UIApplication *)application
{
#ifdef SYS_IOS

#if defined PLATFORM_ID_TENCENT
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

#if defined PLATFORM_ID_TENCENT
    // NSLog(@"JoyYou-TencentMSDK ::: Register remote notifications succeeded with deviceToken: %@", deviceToken);
    [MSDKXG WGSuccessedRegisterdAPNSWithToken:deviceToken];
#else

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

#endif
}

+ (void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo
{
#ifdef SYS_IOS
    
#if defined PLATFORM_ID_TENCENT
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

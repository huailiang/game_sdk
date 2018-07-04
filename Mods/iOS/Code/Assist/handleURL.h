//
//  externFs.h
//  Unity-iPhone
//
//  Created by 彭怀亮 on 17/06/14.
//

#if defined (SYS_IOS)

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

@interface SDKProcess : NSObject

/*
 * For UnityAppController.mm
 */
+ (BOOL)application:(UIApplication *)application handleOpenURL:(NSURL *)url;

+ (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation;

+ (void)applicationDidBecomeActive:(UIApplication *)application;

+ (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions;

+ (void)applicationDidEnterBackground:(UIApplication *)application;

+ (void)applicationWillEnterForeground:(UIApplication *)application;

+ (void)application:(UIApplication *)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData *)deviceToken;

+ (void)application:(UIApplication *)application didFailToRegisterForRemoteNotificationsWithError:(NSError *)error;

+ (NSUInteger)application:(UIApplication *)application supportedInterfaceOrientationsForWindow:(UIWindow *)window;

+ (void)application:(UIApplication *)application didReceiveLocalNotification:(UILocalNotification *)notification;

+ (void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo;

+ (void)dealloc;

/*
 * For UnityView.mm
 */
+ (void)touchesMoved:(NSSet *)touches withEvent:(UIEvent *)event;

@end

#endif

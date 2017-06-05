
#import <Foundation/Foundation.h>


@interface SystemExtInfo : NSObject
@end



@implementation SystemExtInfo

#if defined (__cplusplus)
extern "C" 
{
#endif

	//获取像素密度
    int GetDensity()
    {
    	NSLog(@"GetDensity---------");
    	return 2;
    }

    //获取运营商
    const char* CheckSIM()
    {
    	NSLog(@"CheckSIM---------");
    	return @"MM";
    }

#if defined (__cplusplus)  
}
#endif




@end
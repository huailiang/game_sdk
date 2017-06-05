
#import <Foundation/Foundation.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <CoreTelephony/CTCarrier.h>


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

    
    char* MakeStringCopy (const char* string)
    {
        if (string == NULL)return NULL;
        char* res = (char*)malloc(strlen(string) + 1);
        strcpy(res, string);
        return res;
        
    }

    
    const char* TransferCode(NSString* pre)
    {
        char* str=MakeStringCopy("");
        if([pre isEqualToString:@"中国移动"])
        {
            str=MakeStringCopy("MM");
        }
        else if ([pre isEqualToString:@"中国联通"])
        {
            str=MakeStringCopy("UN");
        }
        else if ([pre isEqualToString:@"中国电信"])
        {
            str=MakeStringCopy("DX");
        }
        return str;
    }
    
    //获取运营商
    const char* CheckSIM()
    {
        CTTelephonyNetworkInfo *telephonyInfo = [[CTTelephonyNetworkInfo alloc] init];
        CTCarrier *nsstr = [telephonyInfo subscriberCellularProvider];
        NSLog(@"CheckSIM  %@",nsstr.carrierName);
       // return MakeStringCopy([nsstr.carrierName UTF8String]);
        return TransferCode(nsstr.carrierName);
    }

#if defined (__cplusplus)  
}
#endif



@end

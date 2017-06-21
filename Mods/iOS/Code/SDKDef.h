#ifndef __SDK_DEF__H__
#define __SDK_DEF__H__

// #include "SDKConfig.h"

#ifndef JoyYouSDK_MARK
#error "Error Config Macros"
#endif


#ifdef PLUGIN_NEWGAMEPAD
#define PLUGIN_ID_NEWGAMEPAD
#endif

#ifdef SYS_IOS

#define __HEADER_OS_ADAPTER "IOS/IOSPlatformAdapter.h"
#define __CLASS_ADAPTER CIOSPlatformAdapter

#define __HEADER_OS_ISD_ADT "IOS/IOSStatisticalDataAdapter.h"
#define __CLASS_ISD_ADT CIOSStatisticalDataAdapter

#define __HEADER_OS_IADV_ADT "IOS/IOSAdvAdapter.h"
#define __CLASS_IADV_ADT CIOSAdvertiseAdapter

#define __HEADER_OS_GAME_RECORD "IOS/IOSGameRecordAdapter.h"
#define __CLASS_IGAMERECORD CIOSGameRecordAdapter

#else

#define __HEADER_OS_ADAPTER "PlatformAdapter.h"
#define __CLASS_ADAPTER CPlatformAdapter

#define __HEADER_OS_ISD "StatisticalDataAdapter.h"
#define __CLASS_ISD_ADT StatisticalDataAdapter

#define __HEADER_OS_IADV_ADT "AdvertiseAdapter.h"
#define __CLASS_IADV_ADT AdvertiseAdapter

#endif

#define UNUSE_TENCENT_WX
#if !defined (UNUSE_TENCENT_WX)
#define USE_WX
#endif

#ifndef NULL
#define NULL (0)
#endif

#endif

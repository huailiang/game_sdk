#ifndef __SDK_DEF__H__
#define __SDK_DEF__H__

#ifdef PLUGIN_NEWGAMEPAD
#define PLUGIN_ID_NEWGAMEPAD
#endif

#ifdef SYS_IOS
#define __HEADER_OS_GAME_RECORD "IOS/IOSGameRecordAdapter.h"
#define __CLASS_IGAMERECORD CIOSGameRecordAdapter
#else
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

//
// Created by user on 2019/12/5.
//

#ifndef ANDROIDTEST_NATIVEHELPHER_H
#define ANDROIDTEST_NATIVEHELPHER_H

#include <jni.h>
#include <stdint.h>

#ifdef __cplusplus
extern "C" {
#endif


JNIEXPORT void JNICALL SetAssetManager(JNIEnv *, jobject, jobject);

JNIEXPORT int32_t JNICALL ReadAssetsBytes(char *fileName, unsigned char **result);

JNIEXPORT int32_t JNICALL
ReadAssetsBytesWithOffset(char *fileName, unsigned char **result, int32_t offset, int32_t length);

JNIEXPORT int32_t JNICALL Add(int32_t a, int32_t b);

#ifdef __cplusplus
}
#endif


#endif //ANDROIDTEST_NATIVEHELPHER_H

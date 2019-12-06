<b>game_sdk</b>

unity 游戏sdk相关架构设计

包含xupoter和android java 等native层面的代码

## IOS

自己实现一套生成postbuild去修改xcode里的设置选项 bitcode https, 不同版本的utniy可能需要略作修改。

文件夹Mods 是生成xcode工程需要动态添加的库， 在unity [postbuild]时刻执行


## Android

文件夹NativeLib 是一个Android lib project (即Android Studio工程)， 可以导出jar包放在Unity项目Plugins/Android目录之下

生成jar包， 在Terminal窗口使用如下命令：

```sh
gradlew clean
gradlew makejar
```

生成android不同平台下的so文件(需要把ndk路径配置到环境变量里):

```sh
ndk-build clean
ndk-build
```


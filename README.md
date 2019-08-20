<b>game_sdk</b>

unity 游戏sdk相关架构设计

包含xupoter和android java 等native层面的代码

自己实现一套生成postbuild去修改xcode里的设置选项 bitcode https 

文件夹Mods 是生成xcode工程需要动态添加的库， 在unity [postbuild]时刻执行

文件夹NativeLib 是一个Android lib project， 可以导出jar包放在Unity项目Plugins/Android目录之下

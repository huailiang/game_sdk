

<p align="center">
     <a href="https://huailiang.github.io/" target="_blank">
    	<img src="https://huailiang.github.io/img/android.jpeg" width="160" height="140">
    </a>
     <a href="https://huailiang.github.io/" target="_blank">
    	<img src="https://huailiang.github.io/img/ios.jpeg" width="160" height="140">
    </a>
    <a href="https://www.unity3d.com" target="_blank">
    	<img src="https://huailiang.github.io/img/unity.jpeg" width="200" height="150">
    </a>
</p>

<b>game_sdk</b>

unity 游戏sdk相关架构设计

包含xupoter和android java 等native层面的代码

自己实现一套生成postbuild去修改xcode里的设置选项 bitcode https 

文件夹Mods 是生成xcode工程需要动态添加的库， 在unity [postbuild]时刻执行

文件夹NativeLib 是一个Android lib project， 可以导出jar包放在Unity项目Plugins/Android目录之下

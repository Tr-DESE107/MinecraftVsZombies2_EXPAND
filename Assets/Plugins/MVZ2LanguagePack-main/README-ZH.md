语言：[English](README.md) | 简体中文

这篇文章介绍了如何创建一个MVZ2的语言包。

在MVZ2游戏本体内，你可以导出内置的语言包，并对其文件结构进行研究学习。

你可以下载本代码库的template文件夹，里面有必要的贴图模板，翻译文本模板，以及用作参考的英文翻译文本。

# 信息文件
信息文件位于语言包的根目录下，文件名为pack.json。

其内容格式一般为：

    {
        "name": "语言包名称",
        "author": "作者名称",
        "description": "语言包描述",
        "dataVersion": 0
    }

其中`dataVersion`是该语言包的数据版本，用于在不同版本的游戏之间进行格式检查。

**一个语言包必须存在信息文件才能够被游戏识别，没有信息文件的语言包是无效的。**

# 图标
图标文件位于语言包的根目录下，文件名为pack.png。

建议使用正方形的贴图作为语言包的图标。

# 文件结构
除了作为信息文件的pack.json和作为图标的pack.png，语言包的内容一般被包含在assets文件夹中。

在assets下的文件夹是游戏中各种不同命名空间下的内容，原版内容的命名空间为mvz2。

而在一个命名空间文件夹下的文件夹则分为针对不同的语言的文件夹。如zh-Hans表示“中文”，en-US表示“英文（美国）”。

在语言文件夹下，还有*.mo（文本翻译文件），sprite_manifest.json（贴图目录），sprites文件夹（单片贴图）和spritesheets文件夹（多片贴图）。

因此，一个语言包的文件结构一般如下：

    pack.json
    pack.png
    assets/
        mvz2/
            zh-Hans/
                almanac.mo
                general.mo
                talk.mo
                sprite_manifest.json
                sprites/
                spritesheets/
            en-US/
                almanac.mo
                general.mo
                talk.mo
                sprite_manifest.json
                sprites/
                spritesheets/

# 文本翻译
要翻译游戏中的文本，你需要用到[GNU Gettext](https://www.gnu.org/software/gettext/)格式的翻译文件。

这种翻译格式有三种文件类型：
* \*.pot：翻译文本模板，只有源文本。
* \*.po：根据翻译文件模板，针对某种语言进行翻译后的文本文件。
* \*.mo：将\*.po文件编译后产生的二进制文件。

MVZ2使用\*.mo文件进行文本翻译。要想创建一个\*.mo文件，你可以使用一些第三方工具，如[Poedit](https://poedit.net/)。

大体流程为：
* 新建\*.po文件用于翻译文本
* 从一个\*.pot文件中加载翻译模板到\*.po文件中。
* 进行文本翻译。
* 保存\*.po文件，并将其编译为\*.mo文件。
* 将编译过后的\*.mo放置于`assets/<命名空间>/<语言>/`下即可。

# 图片翻译
在MVZ2的语言包中，图片加载的依据是sprite_manifest.json文件，它位于`assets/<命名空间>/<语言>/`文件夹下。

其内容记录了该语言包要加载的所有贴图。如果一张贴图没有被记录在该文件中，那它就不会被加载。

其内容格式一般为：

    {
        "sprites": [
            {
                "name": "archive/castle_note",
                "texture": "archive/castle_note.png",
                "pivotX": 0.5,
                "pivotY": 0.5
            },
            ...
        ],
        "spritesheets": [
            {
                "name": "ready_set_build",
                "texture": "ready_set_build.png",
                "slices": [
                    {
                        "x": 0.0,
                        "y": 148.0,
                        "width": 165.0,
                        "height": 74.0,
                        "pivotX": 0.5,
                        "pivotY": 0.5
                    },
                    ...
            },
            ...
        ]
    },

文件内容分为两部分：
* sprites：表示单片贴图，即贴图不进行分割，一张图片文件即一张贴图。
* spritesheets：表示多片贴图，即贴图会进行分割，将一张图片文件切分为多张贴图。

sprites列表中的元素属性如下：
* name：贴图的内部名称，要求和原游戏一一对应，不能随意更改。
* texture：贴图的路径，即相对于sprites文件夹的文件位置。
* pivotX：贴图中心点的横向坐标。最左侧为0，最右侧为1。
* pivotY：贴图中心点的纵向坐标。最下侧为0，最上侧为1。

spritesheets列表中的元素属性如下：
* name：贴图的内部名称，要求和原游戏一一对应，不能随意更改。
* texture：贴图的路径，即相对于spritesheets文件夹的文件位置。
* slices：贴图切片列表。
    * x：切片的开始位置的横向坐标。从左开始，单位为像素。
    * y：切片的开始位置的纵向坐标。从下开始，单位为像素。
    * width：切片的宽度，单位为像素。
    * height：切片的高度，单位为像素。
    * pivotX：贴图中心点的横向坐标。最左侧为0，最右侧为1。
    * pivotY：贴图中心点的纵向坐标。最下侧为0，最上侧为1。

编写好sprite_manifest.json后，向目标路径放置\*.png贴图文件即可。

# 添加新语言

如果要向游戏添加新的语言，在`assets/<命名空间>/`文件夹下添加一个以语言代号为名称的文件夹，并向其中添加内容即可。

语言在游戏的名称一般会根据语言代号自动获取，但某些不存在的自定义语言并非如此。

要想使自定义的语言的名称能够正常显示，可以在\*.po文件中翻译一项文本，这项文本的上下文（Context）为"language_name"，用于代表当前语言的名称。

# 应用

在Windows平台下，语言包目录为`%HOMEPATH%\AppData\LocalLow\Cuerzor\MinecraftVSZombies2\language_packs`。将语言包文件夹，或者压缩后的zip文件放入这个目录即可使用。

你也可以通过游戏内的导入、导出、删除操作来应用语言包。
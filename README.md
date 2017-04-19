# unity_multi_language
multi_language solution, Modified by blog http://www.cnblogs.com/YYRise/p/4417665.html

### 修改了什么
因为项目需求，需要可以在游戏内动态更改语言，所以在每个 `languageText` 组件内注册了语言选项更改的函数回调，使用了网上找的 `SubPubSystem` 脚本，原理是订阅发布

```c#
public LanguageInfo Language
{
  get { return _language; }
  set
  {
    if (!HasLanguage(value))
    {
      Debug.LogError("Invalid Language " + value);
    }
    _language = value;
    ReadLanguageFiles();
    // set language 的时候触发订阅器
    LanguageInfo.SubPubSystem.Publish("Language");
  }
}
```

其次，因为我们项目考虑语言配置文件使用 `xml` 更方便策划使用，所以把文件加载格式改成了 `xml`

源代码中 `File` 写死了使用语言文件夹下的第一个文件，如有修改改这儿就好了

```c#
// LanguageEditor.cs 52 ~ 55

if (findex == -1 || fi != findex){
  Target.File = files[0];
  EditorUtility.SetDirty(target);
}
```

### 使用
参考博客里的使用方式吧，偷懒没写示例脚本

添加语言配置在 `LocalizationConfig.cs`

添加语言文本在 `Lang/xxxx`

修改语言设置 
```c#
using Language;

LanguageService.Instance.Language = new LanguageInfo ("English");
```

防着自己想使用的时候找不到



# PiscesConfigLoader 0.9

A configuration loader and reader tool for Unity

## Features

* Parse YAML files to object structure
* Traverse the data tree
* Immutable structure
* Query simple and complex data types
  * Mathematical expressions
  * Number ranges
  * Lists
  * Sub config trees
* Merge trees

## System Requirements

Unity 2018.4 or later

## Dependencies

* [YamlDotNet](https://github.com/aaubry/YamlDotNet) (forked git submodule)
* [ExpressionParser](https://wiki.unity3d.com/index.php/ExpressionParser) (from unity wiki)

## Installation

* Clone into the Assets folder of your Unity project

```
git clone git@github.com:SolAnna7/PiscesConfigLoader.git
cd PiscesConfigLoader/
git submodule update --init --recursive
```

* ~~Download from Unity Asset Store~~

## Usage

##### Build the config tree

* Parsing resource files

```c#
new ConfigBuilder()
    // TestYamlFiles is a Resources folder with YAML files in it
    .ParseTextResourceFiles("TestYamlFiles", new ConfigBuilder.YamlTextConfigParser())
    .Build();

new ConfigBuilder()
    // TestYamlFiles/TestYaml1 Resources YAML file
    .ParseTextResourceFiles("TestYamlFiles/TestYaml1", new ConfigBuilder.YamlTextConfigParser())
    .Build();
```
* Parsing strings

```c#
var configRoot =
    new ConfigBuilder()
        .ParseString(@"
                     aaa:
                       bbb: 123
                       ccc: ddd", new ConfigBuilder.YamlTextConfigParser())
        .Build();
```

* Merge dictionaries

```c#
var configRoot =
    new ConfigBuilder()
        .MergeDictionary(
            new Dictionary<object, object>
            {
                {
                    "aaa", new Dictionary<object, object>
                    {
                        {"bbb", 999},
                    }
                }
            }
        ).Build();
```

* Currently contains only a YAML parser but can be extended

```c#
private class TestConfigParser : ConfigBuilder.ITextConfigParser
{
    public Dictionary<object, object> ParseText(string text)
    {
        if (text == "please!")
            return new Dictionary<object, object>
            {
                {
                    "yesssss", "^_^"
                }
            };

        return new Dictionary<object, object>
        {
            {"nope", "sorry"}
        };
    }
}

...

new ConfigBuilder()
    .ParseString("please!", new TestConfigParser())
    .Build();
```

## Planned features

* Parse from JSON
* Parse from XML
* Load to and save from binary files

### 1.0
No features are planned specifically for 1.0, but it wont be released until further testing and real life usage

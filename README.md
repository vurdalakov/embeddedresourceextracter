# Embedded Resource Extracter

[Embedded Resource Extracter](https://github.com/vurdalakov/embeddedresourceextracter) is a command line tool that
lists and extracts all or individual embedded resource files from a .NET assembly.

## Usage

### List embedded resources

```
EmbeddedResourceExtracter.exe list <dll-name>
EmbeddedResourceExtracter.exe l <dll-name>
```

Example:

```
EmbeddedResourceExtracter.exe list DotNetZip.dll
```

### Extract all embedded resources

```
EmbeddedResourceExtracter.exe extract <dll-name>
EmbeddedResourceExtracter.exe x <dll-name>
```

Example:

```
EmbeddedResourceExtracter.exe extract DotNetZip.dll
```

### Extract individual embedded resources:

```
EmbeddedResourceExtracter.exe extract DotNetZip.dll <file-name-mask>
EmbeddedResourceExtracter.exe x DotNetZip.dll <file-name-mask>
```

Example:


```
EmbeddedResourceExtracter.exe extract DotNetZip.dll Forms
```

## License

`Embedded Resource Extracter` program is distributed under the terms of the [MIT license](https://opensource.org/licenses/MIT).

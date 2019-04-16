# Simplet - .NET Template Bundler

[![Build Status](https://florianrappl.visualstudio.com/Simplet/_apis/build/status/Simplet-CI?branchName=master)](https://florianrappl.visualstudio.com/Simplet/_build/latest?definitionId=10&branchName=master)
[![GitHub Tag](https://img.shields.io/github/tag/FlorianRappl/Simplet.svg?style=flat-square)](https://github.com/FlorianRappl/Simplet/releases)
[![NuGet Count](https://img.shields.io/nuget/dt/Simplet.svg?style=flat-square)](https://www.nuget.org/packages/Simplet/)
[![Issues Open](https://img.shields.io/github/issues/FlorianRappl/Simplet.svg?style=flat-square)](https://github.com/FlorianRappl/Simplet/issues)

A simple templating engine to generate valid C# classes from static text files. Convert your text files into code that can be checked at compile-time.

## Installation

Install the library via `dotnet` on the CLI:

```sh
dotnet tool install --global simplet
```

## Usage

You'll need to have a config file. Let's name our config file `sample.json`. The following content is sufficient:

```json
{
  "target": "dist",
  "sources": [
    {
      "includes": [
        "templates/*.html"
      ]
    }
  ]
}
```

This creates a new project in the directory `dist`. All HTML files from the `templates` directory will be converted to C# classes. As a follow up we may run

```sh
dotnet build dist
```

To create a DLL containing the generated classes.

A full configuration file could look as follows:

```json
  "name": "BigCo.Templates.Sample",
  "target": "dist",
  "framework": "netstandard1.3",
  "sources": [
    {
      "placeholder": "%([A-Za-z]{1,32})%",
      "includes": [
        "templates/**/*.html"
      ],
      "excludes": [
        "templates/foo",
        "templates/bar"
      ],
      "parameter": "file",
      "namespace": "BigCo.Templates.Sample.Html",
      "sections": [
        {
          "format": "<!--start-->(.*)<!--end-->",
          "title": "Content"
        }
      ]
    }
  ]
```

## License

MIT License (MIT). For more information see [LICENSE](./LICENSE) file.

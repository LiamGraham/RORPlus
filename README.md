# Risk of Rain+ (RORPlus)

Implements improvements for Risk of Rain 2.

## Dependencies

- .NET
- BepInEx
- R2API

## Installation

[Install BepInEx](https://docs.bepinex.dev/articles/user_guide/installation/index.html) and [setup your BepInEx development environment](https://docs.bepinex.dev/articles/dev_guide/plugin_tutorial/1_setup.html).

Install [HookGenPatcher](https://thunderstore.io/package/RiskofThunder/HookGenPatcher/) and [R2API](https://thunderstore.io/package/tristanmcpherson/R2API/).

Clone this repo and add the RiskOfRain2.GameLibs package:

```
git clone https://github.com/LiamGraham/RORPlus
cd RORPlus/RORPlus
dotnet add package RiskOfRain2.GameLibs --version 1.1.1.4-r.1
```

## Building

Build the project from `RORPlus/RORPlus`:

```
dotnet build
```

Copy the resulting .dll file from `bin/Debug/netstandard2.0` to `BepInEx/plugins/` inside the Steam folder for Risk of Rain 2.
# Risk of Rain+ (RORPlus)

Implements improvements for Risk of Rain 2.

## Installation

Clone the repo and add the RiskOfRain2.GameLibs package:

```
git clone https://github.com/LiamGraham/RORPlus
cd RORPlus/RORPlus
dotnet add package RiskOfRain2.GameLibs --version 1.1.1.4-r.1
```

## Build

Build the project from `RORPlus/RORPlus`:

```
dotnet build
```

Copy the resulting .dll file from `bin/Debug/netstandard2.0` to `BepInEx/plugins/` inside the Steam folder for Risk of Rain 2.
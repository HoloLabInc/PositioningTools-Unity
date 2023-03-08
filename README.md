# PositioningTools-Unity

PositioningTools-Unity is a module for aligning positions using geodetic coordinates, VPS or marker recognition.

## Setup for ARCore Geospatial API

#### Requirements

- ARFoundation
- ARCore Extensions

#### Import

Open `Packages\manifest.json` and add the following lines in "dependencies".

```
"jp.co.hololab.positioningtools": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools",
"jp.co.hololab.positioningtools.arcoreextensions": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools.arcoreextensions",
"jp.co.hololab.positioningtools.geographiccoordinate": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools.geographiccoordinate",
```

## Setup for NMEA devices

#### Import

Open `Packages\manifest.json` and add the following lines in "dependencies".

```
"jp.co.hololab.positioningtools": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools",
"jp.co.hololab.positioningtools.geographiccoordinate": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools.geographiccoordinate",
"jp.co.hololab.positioningtools.nmea": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools.nmea",
```

## Setup for Immersal

#### Requirements

- ARFoundation

#### Import Immersal SDK

Download the Immersal SDK Core unitypackage from the [developer portal](https://developers.immersal.com).  
Download the Immersal SDK Core Assembly Definition unitypackage from [this link](https://github.com/HoloLabInc/PositioningTools-Unity/raw/main/_tools/ImmersalSDK_AssemblyDefinition.unitypackage).  
Import these packages.  

#### Import

Open `Packages\manifest.json` and add the following lines in "dependencies".

```
"jp.co.hololab.positioningtools": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools",
"jp.co.hololab.positioningtools.geographiccoordinate": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools.geographiccoordinate",
"jp.co.hololab.positioningtools.immersal": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools.immersal",
```

### Vuforia

## Sample Unity Projects

- [PositioningTools-ARCore-GeospatialAPI](./unity/PositioningTools-ARCore-GeospatialAPI)
- [PositioningTools-NMEA-ARFoundation](./unity/PositioningTools-NMEA-ARFoundation)
- [PositioningTools-Immersal-ARFoundation](./unity/PositioningTools-Immersal-ARFoundation)
- [PositioningTools-VuforiaAreaTarget](./unity/PositioningTools-VuforiaAreaTarget)

## License

- MIT
- Apache License 2.0 (Nmea Parser)

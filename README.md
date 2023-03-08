# PositioningTools-Unity

PositioningTools-Unity is a module for aligning positions using geodetic coordinates, VPS or marker recognition.

## Setup for ARCore Geospatial API

#### Requirements

- ARFoundation
- ARCore Extensions

#### Install

Open `Packages\manifest.json` and add the following lines in "dependencies".

```
"jp.co.hololab.positioningtools": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools",
"jp.co.hololab.positioningtools.arcoreextensions": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools.arcoreextensions",
"jp.co.hololab.positioningtools.geographiccoordinate": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools.geographiccoordinate",
```

## Setup for NMEA devices

#### Install

Open `Packages\manifest.json` and add the following lines in "dependencies".

```
"jp.co.hololab.positioningtools": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools",
"jp.co.hololab.positioningtools.geographiccoordinate": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools.geographiccoordinate",
"jp.co.hololab.positioningtools.nmea": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools.nmea",
```

## Setup for Immersal

#### Requirements

- ARFoundation

#### Install

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

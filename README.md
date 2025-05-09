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

## Setup for QR marker tracking

#### Requirements

- ARFoundation
- [ARFoundationQRTracking-Unity](https://github.com/HoloLabInc/ARFoundationQRTracking-Unity)

#### Import

Open `Packages\manifest.json` and add the following lines in "dependencies".

```
"jp.co.hololab.positioningtools": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools",
"jp.co.hololab.positioningtools.arfoundationmarker": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools.arfoundationmarker",
"jp.co.hololab.positioningtools.geographiccoordinate": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools.geographiccoordinate",
```

## Setup for Immersal

#### Requirements

- ARFoundation

#### Import Immersal SDK

Download the Immersal SDK Core unitypackage from the [developer portal](https://developers.immersal.com) and import it.  
Supported version is 1.19.0 or later.

#### Import

Open `Packages\manifest.json` and add the following lines in "dependencies".

```
"jp.co.hololab.positioningtools": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools",
"jp.co.hololab.positioningtools.geographiccoordinate": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools.geographiccoordinate",
"jp.co.hololab.positioningtools.immersal": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools.immersal",
```

## Setup for Vuforia

#### Import Vuforia Engine

Download the unitypackage for installing Vuforia Engine from the [developer portal](https://developer.vuforia.com/downloads/SDK) and import it.

#### Import

Open `Packages\manifest.json` and add the following lines in "dependencies".

```
"jp.co.hololab.positioningtools": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools",
"jp.co.hololab.positioningtools.geographiccoordinate": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools.geographiccoordinate",
"jp.co.hololab.positioningtools.vuforia": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools.vuforia",
```

## Setup for Meta Quest

#### Import Oculus Integration

Import Oculus Integration from the Unity Asset Store.  
https://assetstore.unity.com/packages/tools/integration/oculus-integration-deprecated-82022

#### Import

Open `Packages\manifest.json` and add the following lines in "dependencies".

```
"jp.co.hololab.positioningtools": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools",
"jp.co.hololab.positioningtools.geographiccoordinate": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools.geographiccoordinate",
"jp.co.hololab.positioningtools.quest": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools.quest",
```

## Setup for Meta Quest QR marker tracking

#### Import QuestCameraTools-Unity

Refer to the link below to import **QuestCameraTools-Unity** into your project:  
https://github.com/HoloLabInc/QuestCameraTools-Unity#importing-the-upm-packages-into-your-project

#### Import

Open `Packages\manifest.json` and add the following lines in "dependencies".

```
"jp.co.hololab.positioningtools": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools",
"jp.co.hololab.positioningtools.geographiccoordinate": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools.geographiccoordinate",
"jp.co.hololab.positioningtools.questcameratools.qr": "https://github.com/HoloLabInc/PositioningTools-Unity.git?path=packages/jp.co.hololab.positioningtools.questcameratools.qr",
```

## Sample Unity Projects

- [PositioningTools-ARCore-GeospatialAPI](./unity/PositioningTools-ARCore-GeospatialAPI)
- [PositioningTools-NMEA-ARFoundation](./unity/PositioningTools-NMEA-ARFoundation)
- [PositioningTools-Marker-ARFoundation](./unity/PositioningTools-Marker-ARFoundation)
- [PositioningTools-Immersal-ARFoundation](./unity/PositioningTools-Immersal-ARFoundation)
- [PositioningTools-VuforiaAreaTarget](./unity/PositioningTools-VuforiaAreaTarget)
- [PositioningTools-Quest](./unity/PositioningTools-Quest)
- [PositioningTools-Quest-QR](./unity/PositioningTools-Quest-QR)

## License

- MIT
- Apache License 2.0 (Nmea Parser)

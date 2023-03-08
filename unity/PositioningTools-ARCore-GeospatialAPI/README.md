# PositioningTools-ARCore-GeospatialAPI

## Setup Unity project

### Setup ARCore Extensions API key

Create an ARCore API key.  
Please refer to [the documentation](https://developers.google.com/ar/develop/unity-arf/geospatial/enable-android#api_key_authorization) for more information.

Select **Edit** > **ProjectSettings** > **XR Plug-in Management** > **ARCore Extensions** and paste the API key in the API Key field.

<img width="480" alt="ARCore Extensions API key settings" src="https://user-images.githubusercontent.com/4415085/223650258-8157f411-1624-459c-976b-58edb4504569.png">

### Import sample scenes
Execute `PositioningTools-Unity\_tools\createSymlinkWindows.bat` (Windows) or `PositioningTools-Unity/_tools/create_symlink_osx.sh` (Mac) .

This script creates symbolic links in the unity project for the folder containing sample scenes.
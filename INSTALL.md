# Installation Instructions

## Prerequisites

*   Jellyfin Server (Native or Docker)
*   The compiled plugin DLL: `Jellyfin.Plugin.DuplicateFinder.dll`

## Build from Source

If you haven't built the plugin yet:

```bash
cd Jellyfin.Plugin.DuplicateFinder
dotnet build --configuration Release
```

The DLL will be in `bin/Release/net8.0/`.

## Installation (Docker)

Since you have your `/config` directory mapped to a local folder:

1.  Locate your mapped config folder on your host machine (e.g., `C:\jellyfin\config` or `/opt/jellyfin/config`).
2.  Navigate to the `plugins` subdirectory.
3.  Create a new folder named `DuplicateFinder_1.0.0.0`.
4.  Copy the following files into this new `DuplicateFinder_1.0.0.0` folder:
    *   `Jellyfin.Plugin.DuplicateFinder.dll`
    *   `meta.json`
    *   `plugin_icon.png`
5.  Restart your Jellyfin container.

## Installation (Windows / Linux Native)

1.  Navigate to your Jellyfin data directory:
    *   **Windows**: `C:\ProgramData\Jellyfin\Server\plugins`
    *   **Linux**: `/var/lib/jellyfin/plugins`
2.  Create a folder named `DuplicateFinder_1.0.0.0`.
3.  Copy the following files into it:
    *   `Jellyfin.Plugin.DuplicateFinder.dll`
    *   `meta.json`
    *   `plugin_icon.png`
4.  Restart the Jellyfin service.

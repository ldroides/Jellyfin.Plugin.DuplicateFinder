# Jellyfin Duplicate Finder Plugin

A Jellyfin server-side plugin that automatically detects duplicate movies in your library and tags them.

## Features

*   **Automatic Detection**: Scans your movie library for duplicates based on:
    *   IMDB ID (ProviderIds)
    *   Exact Name + Production Year
*   **Tagging**: Adds the tag **"Duplicate"** to all identified duplicate items.
*   **Native Integration**:
    *   Runs as a standard Jellyfin **Scheduled Task**.
    *   Uses the built-in **Tags** filter in the Jellyfin web UI.
*   **Non-Destructive**: Does not delete files. It only tags them for your review.

## Usage

1.  **Install the Plugin** (see [INSTALL.md](INSTALL.md)).
2.  **Run the Task**:
    *   Go to **Dashboard** -> **Scheduled Tasks**.
    *   Find **"Find Duplicate Movies"** and click the **Play** button.
    *   *Note: It runs automatically every Sunday at 02:00 AM.*
3.  **Filter Duplicates**:
    *   Go to your **Movies** library.
    *   Open the **Filters** menu.
    *   Select **"Duplicate"** under the **Tags** section.

Language:English | [简体中文](README-ZH.md)

This article explains how to create a language pack for MVZ2.

Within the MVZ2 game client, you can export built-in language packs and study their file structures.

You can download the `template` folder from this repository, which contains necessary texture templates, translation text templates, and English translations for reference.

# Information File
The information file is located in the root directory of the language pack and named `pack.json`.

Its content format is typically:

    {
        "name": "Language Pack Name",
        "author": "Author Name",
        "description": "Language Pack Description",
        "dataVersion": 0
    }

The `dataVersion` field represents the data version of the language pack, used for format validation across different game versions.

**A language pack must include this information file to be recognized by the game. Language packs without this file are invalid.**

# Icon
The icon file is located in the root directory of the language pack and named `pack.png`.

A square-shaped texture is recommended for the language pack icon.

# File Structure
Aside from `pack.json` (information file) and `pack.png` (icon), language pack content is generally stored in the `assets` folder.

Subfolders under `assets` correspond to different namespaces in the game. The vanilla content uses the `mvz2` namespace.

Within a namespace folder, subfolders represent different languages. For example:
- `zh-Hans`: Simplified Chinese
- `en-US`: English (United States)

Language folders contain:
- `*.mo` (compiled translation files)
- `sprite_manifest.json` (texture catalog)
- `sprites` folder (single-slice textures)
- `spritesheets` folder (multi-slice textures)

A typical language pack structure looks like:

    pack.json
    pack.png
    assets/
        mvz2/
            zh-Hans/
                almanac.mo
                general.mo
                talk.mo
                sprite_manifest.json
                sprites/
                spritesheets/
            en-US/
                almanac.mo
                general.mo
                talk.mo
                sprite_manifest.json
                sprites/
                spritesheets/

# Text Translation
Game text translations use the [GNU Gettext](https://www.gnu.org/software/gettext/) format, which includes three file types:
* `*.pot`: Translation template containing source text only
* `*.po`: Translated text file for a specific language
* `*.mo`: Compiled binary version of `*.po`

MVZ2 uses `*.mo` files for translations. To create `*.mo` files, use tools like [Poedit](https://poedit.net/).

Workflow:
1. Create a new `*.po` file for translations
2. Load translation templates from `*.pot` into the `*.po` file
3. Translate the text
4. Save and compile the `*.po` file into `*.mo`
5. Place compiled `*.mo` files under `assets/<namespace>/<language>/`

# Texture Translation
The `sprite_manifest.json` file in `assets/<namespace>/<language>/` controls texture loading. Only textures listed here will be loaded.

Sample format:

    {
        "sprites": [
            {
                "name": "archive/castle_note",
                "texture": "archive/castle_note.png",
                "pivotX": 0.5,
                "pivotY": 0.5
            },
            ...
        ],
        "spritesheets": [
            {
                "name": "ready_set_build",
                "texture": "ready_set_build.png",
                "slices": [
                    {
                        "x": 0.0,
                        "y": 148.0,
                        "width": 165.0,
                        "height": 74.0,
                        "pivotX": 0.5,
                        "pivotY": 0.5
                    },
                    ...
                ]
            },
            ...
        ]
    }

### File Structure:
- **sprites**: Single-slice textures (entire image file = one texture)
  - `name`: Internal name (must match the game's original name)
  - `texture`: Path relative to `sprites` folder
  - `pivotX/Y`: Anchor point coordinates (0=left/bottom, 1=right/top)

- **spritesheets**: Multi-slice textures (one image file divided into multiple textures)
  - `slices`: Texture subdivisions
    - `x/y`: Starting coordinates (pixels, bottom-left origin)
    - `width/height`: Slice dimensions (pixels)
    - `pivotX/Y`: Anchor point coordinates

Place corresponding `.png` files in the specified paths after configuring `sprite_manifest.json`.

# Adding New Languages
To add a new language:
1. Create a folder named with the language code (e.g., `fr-FR`) under `assets/<namespace>/`
2. Add translated content to this folder

The game automatically detects standard language names based on language codes. For custom languages, add a translation entry with context `"language_name"` in your `.po` file to display the language name properly.

# Installation
**Windows Path**:  
`%HOMEPATH%\AppData\LocalLow\Cuerzor\MinecraftVSZombies2\language_packs`  
Place language pack folders or compressed `.zip` files here.

You can also manage language packs through in-game operations: Import, Export, and Delete.
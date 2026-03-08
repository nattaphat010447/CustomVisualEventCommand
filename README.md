# Custom Visual Event Command

A SMAPI framework mod for Stardew Valley (1.6+) that empowers modders to display custom images and sprite-sheet animations dynamically during in-game events and cutscenes.

## Features

* **Custom Event Commands:** Introduces new script commands (`/ShowAnim` and `/StopAnim`) specifically designed for event scripting.
* **Sprite-Sheet Animation Support:** Easily animate characters, visual effects, or CGs by providing a horizontal sprite-sheet.
* **Pixel-Perfect Auto-Scaling:** Automatically calculates the optimal scale to fit the player's screen size while maintaining the original aspect ratio. Uses `PointClamp` rendering to ensure pixel art remains sharp and crisp without blurring.
* **Hybrid Asset Loading:** Seamlessly loads textures from the game's shared asset pipeline (allowing full integration with **Content Patcher**) or directly from the mod's local `assets` folder as a fallback.

## For Players: Installation

1. Install the latest version of [SMAPI](https://smapi.io/).
2. Download this mod and extract the `CustomVisualEventCommand` folder into your `Stardew Valley/Mods` folder.
3. Launch the game using SMAPI.

## For Modders: How to Use

This framework provides easy-to-use commands that you can inject into any event script (e.g., via Content Patcher).

### 1. Show Image / Animation
Displays an image or plays a looping animation on the screen. The event script will continue running in the background while the image is displayed.

**Syntax:**
`/ShowAnim <ImageTarget> [TotalFrames] [FrameDelayMs]`

* `<ImageTarget>` *(Required)*: The dictionary key/path to your image in the asset pipeline (e.g., `MyModName_HeroSprite`), or the exact filename (without `.png`) if placing it in this framework's local `assets` folder. **Do not use forward slashes (`/`) in your target name**, as Stardew Valley uses them to split event commands. Use underscores (`_`) instead.
* `[TotalFrames]` *(Optional)*: The total number of frames in your horizontal sprite-sheet. Defaults to `1` (static image).
* `[FrameDelayMs]` *(Optional)*: The delay between each animation frame in milliseconds. Defaults to `100`.

**Example Event Script (Content Patcher):**
```json
{
  "Format": "2.0.0",
  "Changes": [
    {
      "Action": "Load",
      "Target": "MyName_CoolAnimation",
      "FromFile": "assets/my_sprite_sheet.png"
    },
    {
      "Action": "EditData",
      "Target": "Data/Events/Farm",
      "Entries": {
        "9999999/DayEvent": "none/1 1/farmer 1 1 2/ShowAnim MyName_CoolAnimation 12 100/pause 5000/StopAnim/end"
      }
    }
  ]
}

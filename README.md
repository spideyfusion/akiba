# AkibaInterceptor

AkibaInterceptor is a small utility for adjusting the resolution and framerate in AKIBA'S TRIP. It tricks the game into thinking that it's calling its own configuration utility and then performs fancy voodoo magic.

## Features

* Skip the game's configuration window and jump straight into the game.
* Unlock the game's framerate to anything you want. Who wants to play at 30 FPS anyway?
* Experience the game at resolutions beyond 1920x1080. Yes, even in UHD!
* Make the game controller friendly by hiding the cursor and prevent the system from falling asleep.
* Run the game in a borderless window to get rid of those Alt + Tab issues once and for all!

## Requirements

* AKIBA'S TRIP ([GOG](https://www.gog.com/game/akibas_trip_undead_undressed) and [Steam](https://store.steampowered.com/app/333980/) versions of the game are supported)
    * :warning: Please use version [1.3.4](https://github.com/spideyfusion/akiba/releases/tag/v1.3.4) of AkibaInterceptor if you intend to run the [demo](https://store.steampowered.com/app/375980/) version of the game
* [Microsoft .NET Framework 4.8](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48)
* Windows Vista or greater

> **NOTE:** If you obtained the game via some other distribution method, tough luck! :-)

## Installation/Upgrading

Just place the downloaded binary in the game's root directory and then run it. The utility is smart enough to do everything else by itself. Don't forget to edit the `configuration.yaml` file before starting the game.

## Configuration

Here's a list of settings you're able to override though the utility's `configuration.yaml` file:

* **framesPerSecond** [int] - How many FPS you want to game to run at. Anything above 60 hasn't been tested, so keep that in mind.
* **renderingResolutionWidth** [int] - The resolution width at which the game is going to be rendered.
* **renderingResolutionHeight** [int] - The resolution height at which the game is going to be rendered.
* **screenMode** [string] - In what kind of window the game should be ran. The following options are available: **Borderless, Fullscreen, Windowed**
* **verticalSynchronization** [bool] - Should [VSync](https://en.wikipedia.org/wiki/Screen_tearing#V-sync) be enabled or not. It's recommended to turn it off if you run the game at anything other than 30 FPS.
* **antiAliasing** [bool] - Should [anti-aliasing](https://en.wikipedia.org/wiki/Spatial_anti-aliasing) be applied or not. It's recommended to turn it off if you're going to take advantage of *downsampling*.
* **hideCursor** [bool] - If enabled, the cursor will be positioned off the screen when the game starts. This is useful if you're running [Big Picture](https://store.steampowered.com/bigpicture).
* **preventSystemSleep** [bool] - This will prevent your system from falling asleep (monitor too). Super convenient if you're playing with a gamepad (which you should).
* **disableMovies** [bool] - Enable this if you want to skip the opening cinematic and go directly to the start screen when you start the game.

> **NOTE:** Configuration values are case-sensitive. Be careful how you type them out.

### Downsampling

You can take advantage of setting the rendering resolution beyond your monitor's supported resolution since the game makes the fullscreen window be as big as your current resolution. Try playing the game at **3840×2160**. :-)

### Borderless window

* If you set `screenMode` to **Borderless** you can still achieve downsampling as `renderingResolutionWidth` and `renderingResolutionHeight` are still taken into account.
* AkibaInterceptor will try its best to figure out on which monitor you're playing the game and expand across it. Whoosh!

### Accessing the game configuration window

Even though this utility will make the game launch immediately, you can still access the original game configuration window by launching `AkibaUU_Config.exe` directly from the game's root directory.

> **NOTE:** The listed configuration values will always override the game settings, regardless of what it's set in the configuration window of the game.

## Reporting issues

Use GitHub's [issue tracker](https://github.com/spideyfusion/akiba/issues) to report any issues you might have. You may also add me on [Steam](https://steamcommunity.com/profiles/76561197986958784/), I don't bite.

## Credits

* Previous versions of AkibaInterceptor used memory patching in order to alter the framerate. The relevant memory locations have been taken from [this](https://steamcommunity.com/sharedfiles/filedetails/?id=450891549) guide. Thanks to [X37](https://steamcommunity.com/profiles/76561197980537906/), [You Schmuck!](https://steamcommunity.com/profiles/76561198001872439/), [Nyan~](https://steamcommunity.com/profiles/76561198020320520/) and [Conker](https://steamcommunity.com/profiles/76561197992786462/) for finding them.
* The utility icon has been created by [Yusuke Kamiyamane](http://p.yusukekamiyamane.com/).

## License

See the [LICENSE](LICENSE.md) file for license rights and limitations (MIT).

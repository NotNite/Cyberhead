# Cyberhead

Bomb Rush Cyberfunk proof of concept VR mod.

**WARNING!** This mod is a proof of concept with lots of bugs. It will be laggy, behave strangely, and will break things. It is not finished, and by using it, you accept there will be weird oddities.

**It is suggested to use this mod on a post-game save file** (you backed it up, right?).

## Known bugs

- Head is not visible in cutscenes
  - I call it "Faux Mode"
- Menus, cutscenes, and graffiti do not work
  - Use the desktop or [QuickLaunch](https://thunderstore.io/c/bomb-rush-cyberfunk/p/LazyDuchess/QuickLaunch/) to get into game
- Flatscreen mode (talking to NPCs or switching outfits) is very buggy
- Swirl shader renders weirdly in VR
- Hand IK is weird in cutscenes and while dancing
- Input hints are broken

If you encounter any more issues, please report them on GitHub!

## What works

- Looking around
- Moving around
  - You may clip out of bounds when changing levels
- Some controller input (inputs are a WIP and guaranteed to change)
  - Move around (left stick)
  - Snap turn (right stick)
  - Jump (right controller primary)
  - Switch movestyle (right controller secondary)
  - Manual/slide (right controller trigger)
  - Boost (right controller grip)
  - Trick (right controller primary/secondary/stick pressed)
  - Interact (left controller trigger)
  - Dance (left controller grip)
  - Pause (right controller stick click)
- Hand IK
- The HUD
- Slop Crew integration
- Flatscreen UI in menus

## Slop Crew integration

To see VR players' hands from non-VR, install this mod and set VrEnabled to false in the config file. The game needs to be launched at least once to generate the config file, so it is suggested to close it as soon as the file is generated to prevent any issues.

## Credits

- VR DLLs sourced from a random Unity project I built on my PC (sorry for no trustable source)
- Lots of learning what the fuck I'm supposed to be doing from [LCVR](https://github.com/DaXcess/LCVR)
- Patcher code from [universal-unity-vr](https://github.com/Raicuparta/universal-unity-vr)
- Seagate for having my hard drive die halfway through making this

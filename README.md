# LIV for Outer Wilds

![image](https://user-images.githubusercontent.com/3955124/164702625-6eea1f07-e072-43e3-be0a-6c92264db681.png)

Adds [LIV](https://store.steampowered.com/app/755540/LIV/) support to Outer Wilds with NomaiVR. With it, you can use mixed reality capture (video where you have your real body inserted into the game), avatars (custom 3D avatars with a dynamic camera that can be in third person, while you play the game normally in first person), smoothed first person view, and more.

![Download count](https://img.shields.io/github/downloads/LIV/ow-liv/total?style=flat-square)

## Installation

- Download and install the [Outer Wilds Mod Manager](https://outerwildsmods.com/mod-manager/).
- Use the mod manager to install NomaiVR.
- Expand the NomaiVR addons dropdown to install the "Add LIV Support" mod.

## Usage

You need [LIV](https://store.steampowered.com/app/755540/LIV/) installed and running to use this mod. Run LIV, and then run the capture tool:

![image](https://user-images.githubusercontent.com/3955124/166646386-4aaf8292-cc28-4e34-bdae-d81c8147693e.png)

You'll also have to configure LIV with either a real camera (for mixed reality) or a virtual camera (for 3D avatars). [Check the LIV documentation to learn how to set everything up](https://help.liv.tv/hc/en-us/categories/360002747940-LIV-Setup).

### First start

The first time you start the game with the LIV mod installed, you need to start via the Mod Manager "Start Game" button. In the "Manual" tab, select the Outer Wilds exe and check if everything is working properly.

![image](https://user-images.githubusercontent.com/3955124/164718675-ee922841-41cc-4cd6-aef6-87b5e61f70ed.png)

### After first start

After you started the game with LIV once and everything was working, you can start the game directly from LIV. The game should now show in the game selection dropdown, in the "Auto" tab.

![image](https://user-images.githubusercontent.com/3955124/164718470-a612f8c8-3225-441c-8b7c-6257091bfec1.png)

Then you can press "Sync & Launch" in the same tab to start the game.

If you stop using LIV, make sure to disable the LIV Support mod before starting the game, because this version of the mod may break some camera effects in the VR view.

### While playing

If something looks wrong, Hold "Thrust Up" and press the "Cancel" button to reset the LIV camera. Check the SteamVR input bindings if you wanna know what buttons are assigned to those actions.

### Performance

[NomaiVR](https://outerwildsmods.com/mods/nomaivr/) by itself is already suuper heavy (due to Outer Wilds not being optimized for VR). Add LIV on top of that, and you get a nice slideshow. I recommend keeping LIV at 30 FPS and low resolution. Plus, reduce the SteamVR rendering resolution, and set all of the Outer Wilds graphics settings to minimum. There's also a [Fixed Foveated Rendering](https://outerwildsmods.com/mods/nomaivrffr/) mod for RTX cards that can help a lot.

### Known problems and reporting bugs

Check the [issues list](https://github.com/Raicuparta/ow-liv/issues) for known bugs. Create a new issue if you run into problems.

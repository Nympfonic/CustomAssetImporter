# CustomAssetImporter

The purpose of this framework is to simplify the process for mod creators who wish to implement custom assets such as (particle system) effects
This client mod framework will load any asset bundles placed within the specified folders and import them into the game.

## Instructions for Modders

You will need to add the corresponding template script/component for whatever type of asset you want to add into the game.
The templates will be located in the Templates folder of this Git repository.
Please only download the templates; the rest of the project is intended as a client mod framework which all users, including players, will need to install to make use of these custom assets.

For example, if you have a bundle of custom effect, you will need to add the `CustomEffectsTemplate.cs` to the root game object containing your effects in the EFT-SDK.

You should match your game object hierarchy and scripts/components to how BSG handles their respective assets.

NOTE: In the case of custom effects, do *NOT* add the `Effects.cs` script/component to the root game object in your bundle.

Lastly, when you package your mod to be released, you should ensure your bundle(s) are in the corresponding folders so mod users don't come complaining about the mod not working.

## Instructions for Mod users

- Download the latest release
- Extract the .zip to `BepInEx/plugins/` folder

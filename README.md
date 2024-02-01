# Arys-CustomAssetImporter

The purpose of this framework is to simplify the process for mod creators who wish to implement custom assets such as (particle system) effects into SPT-AKI.

This client mod framework will load any Unity asset bundles placed within the specified folders and import them into the game.

## Instructions for Modders

You will need to add the corresponding template script/component for whatever type of asset you want to add into the game.

The templates will be located in the `AssetImporter/Templates` folder in this Git repository.

Please *only* download the templates; the rest of the project is intended as a client mod framework which all users, including players, will need to install to make use of these custom assets.

### Adding Custom Effects

1. You will need to add the `CustomEffectsTemplate.cs` to the root game object containing your effects in the EFT-SDK
	- You should match your game object hierarchy and scripts/components to how BSG handles their respective assets
	- Do *NOT* add the `Effects.cs` script/component to the root game object in your bundle
1. Populate the `EffectsArray` field in the Inspector with your effects
1. Export your effects bundle
1. Place your effects bundle in this folder directory when you are packaging your mod: `BepInEx/plugins/Arys-CustomAssetImporter/assets/effects/`
1. If everything has been done correctly, the custom effect name can then be referenced by other mods including server mods
	- For example, say a new explosion effect was added via this framework called `rpg_explosion`
	- A mod like Choccy's RPG-7 can now use the custom effect name `rpg_explosion` in the `ExplosionType` property of the RPG-7's item template json

## Instructions for Mod users

- Download the latest release
- Extract the .zip to your SPT folder

# SubnauticaBlueprintRandomizer
A mod for Subnautica that randomizes the blueprints rewarded for scans, analysis, and goals.

Heavily inspired by the [SubnauticaRandomiser](https://github.com/Raqzas/SubnauticaRandomiser) by Raqzas.

The randomization will persist between saves. To create a new randomized set, change the name of the seed. If not using a seed name, delete the `RandomSeed-data.dat` file to generate a new set. The seed can 
also be shared, so everyone running with the same seed name will get the same mapping. A spoiler log
is written out to `Subnautica\QMods\BlueprintRandomizer\` each time a new mapping is generated. 

**Warning**, using this mod is going to make the game be very difficult. While there is some basic logic to try and avoid a softlock, there are no other safety measures in place. Also, setting all possible entries to true in the config will most likely make an unwinnable seed unless you are Clove or Rubiks.

I believe this mod will be compatable with the [SubnauticaRandomiser](https://github.com/Raqzas/SubnauticaRandomiser) but I haven't tested it. Both mods attempt to randomize databoxes, so keep that option off in the SubnauticaRandomizer or some blueprints may get left out. 


## How to Install

* Install [QModManager](https://www.nexusmods.com/subnautica/mods/201)
* Extract the latest [release](https://github.com/allaboutmike/SubnauticaBlueprintRandomizer/releases) into your Subnautica/QMods folder
* Edit the `Subnautica/QMods/BlueprintRandomizer/config.json` file to set your properties
* You're ready to go! Good luck!

## Configuration Options
* `seed` - The name of the seed. This will be used to initialize the random generator. The same name will generate the same random mapping each time.
* `use_all_scanner_entries` - Default is false. Set to true to use all possible scanner entries, not just the ones that have blueprints in the vanilla game. Use this setting if you want a rock grub to have your rocket blueprints!
* `use_all_analysis_entries` - Default is false. Set to true to use all possible analysis entries (either when things are picked up or when new tech is unlocked), not just the ones from the base game. 
* `use_all_goal_entries` - Default is false. Set to true to use all the goals that can be mapped to a tech unlock, not just the ones that unlock blueprints in the unmodded version. I suggest looking at the list of possible goals in the `unlocks.csv` first to have an idea of where they all could be. 
* `randomize_starting_blueprints` - Default is false. Set to true to randomize the set of blueprints that you start the game with. Note that while the current logic tries to prevent a softlock, there's no progression yet, so big momma could have your scanner with this turned on. The logic does guarantee that all default slots are filled with a blueprint.
* `use_progression`: A placeholder for now.
* `allow_softlocks`: Removes all logic related to softlocks. This is very likely to make the game unbeatable without the use of the console, but feel free to try.


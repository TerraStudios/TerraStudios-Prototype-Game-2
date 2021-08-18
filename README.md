[![PBAG Actions](https://github.com/TerraStudios/PBAG-Lite/actions/workflows/pbagactions.yml/badge.svg)](https://github.com/TerraStudios/PBAG-Lite/actions/workflows/pbagactions.yml)

**NOTE!** This project is canceled and no pull requests will be accepted. Read more about that below.

# TerraStudios-Prototype-Game-2
Main Repository of Prototype Game 2, Developed by TerraStudios

This repository is covered by the **GNU GPL v3 license**, you can read more about the license terms and conditions here: https://www.gnu.org/licenses/gpl-3.0.en.html or in *LICENSE.md*.

**NOTE!** Past NDA/MNDA and Confidential notices are revoked and considered invalid since no agreement took place at any time during the development of this project. Therefore, you can safely use past versions of the code without worrying about the older license and script headers.

The former name of the prototype project was "PBAG-Lite", "PBAG" meaning *Physics-Based Automation Game* and "Lite" signifying that it is a *programming repository*. 

# About the prototype
## Story
The development of this project started on **February 28, 2020**, and initially, **konstantin890** was the only programmer for the project. Since then we've worked hard every day (sometimes till early in the morning) to make this game come into reality and eventually begin selling on game platforms such as Steam.

Over time, as the design and the scope of the game grew bigger, the volunteer **Yerti (UZ9)** joined the project on **May 18, 2020**, to release the programming tension that built up. Everything was looking great so far and we were doing progress quickly until the Game Designer suggested we do a game redesign to make the game "more unique". In **October 2020**, that redesign process started with a **~50% rewrite of all the game code**. 

This rewrite has mostly finished on **July 16, 2021**. "Mostly" because game-breaking bugs still existed even after the **10-month rewrite**. Shortly after that, we realized that fixing one of those game-breaking bugs took many days worth of debugging, trial and error. Eventually, we concluded that to fix the remaining bugs in an adequate amount of time we had to rewrite the other **50% of the code** and in addition **some of the code that was recently rewritten**. Additionally, this would also help to make the programming of new features easier and quicker.

On **August 14, 2021**, without any prior warning, the **Game Designer** began revoking access to the Game Design, Marketing, and other private documents which were written by all of the team members. He was kicked immediately and later a conclusion was drawn from the two remaining teammates to completely cancel the project and release the source for free.

## About the NDA/MNDA and Confidential notices
Around **October 2020**, the Game Designer suggested we sign a Mutual Non-Disclosure Agreement so the game source stays "protected". Shortly after that decision, we prepared the repository for when the agreement would be signed - this included adding a LICENSE file, a header to all scripts written by us, and a Confidential warning to all documents written by us. Since then, a lot of drafts of the agreement were written and the final version was completed on **22 July 2021**. After discussions with the team about signing it, none of them agreed due to it not being enforceable to minors and "putting chains on something loose" because the project was on the brink of getting canceled at that time.

## The source
The repository contains a **Unity project made in version 2021.1.15f1**. Though, it should open in any 2021.1 version, just make sure you have **Git** installed because some packages need to be installed from an external repository.
### Features in scope
**Around 70% of the source is documented. If you'd like to see the technical documentation, please DM Kosio#9955 in Discord.**

For now, here's a summary of everything we have:

* Advanced camera movement
  * Three different camera modes, each with a different input layout.
    * Normal
    * Top-down view
    * Freecam
  * Pointer controller that moves 2 spheres and a line to illustrate the different positions of the "static pointer" and the "dynamic pointer".
* Fully modular procedural Voxel terrain generation system (built by Yerti)
* Grid building system - highly complex system that is responsible for placing Buildings on the Voxel terrain. It is highly flexible and has advanced building size calculations to calculate the appropriate grid position of the building.
* Swappable Building modules (Building - core, BuildingIOManager, APM)
  * Building - core module, defines an object as a building
  * BuildingIOManager - handles Input/Output connections between the buildings
  * APM (Advanced Processing Machine) - processes crafting recipes
* Robust Game Saving system
  * Contains multiple Save Data types/classes - EconomySaveData, TimeSaveData, WorldSaveData.
  * Callback handler - handles saving Action references in the save file.
  * Custom serialization surrogates for Vector2, Vector2Int, Vector3, Vector3Int
* Building Management System
* Economy System - handles balance updates as transactions with a fully-featured Transaction-based approach. Has support for different currencies, depending on the country locale.
* Time System - simulates time passing in the game. An external thread runs to make time ticking extremely fast. Different types of timers can be created to make the most of the system.
* Electricity System - calculates the electricity usage for each building placed in the world
* Recipe system and Recipe database
  * Recipe filtering system
  * Recipe filter list - stores multiple recipe filters
* ItemData class and Item database - stores a large number of variables about items
* Gameplay configuration system (supports runtime changes)
  * GameProfile - 19 variables that can be changed to modify the gameplay experience.
  * UserProfile - 4 variables that can be changed depending on the currency and the time format preferred by the user.
* Object Pool Manager (OPM) for quickly instantiating large amounts of objects in the scene.
* Remove System - removes either building or items or both from the voxel terrain. The size of the area to clear works in a similar way to brushes.
* Custom unscaled fixed update function
* Editor Plugins - [FindMissingScripts and FindMissingScriptsRecursively](https://wiki.unity3d.com/index.php/FindMissingScripts)
* Unity Main Thread Dispatcher (UMTD) - used to call code on the main thread from different threads.

## The Team
* **Konstantin Milev (konstantin890)** - Main Project Lead, Programmer, Technical Director
* **Yerti (UZ9)** - Head Programmer, Head TerraStudios Bot Developer
* **Darek Z (DarekZ4)** - Game Designer, Art

# Libraries and third-party scripts used
- [Graphy](https://github.com/Tayx94/graphy) (MIT)
- [FastPriorityQueue](https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp) (MIT)
- [FastNoiseLite.cs](https://github.com/Auburn/FastNoise) (MIT)
- [FreeCam.cs](https://gist.github.com/ashleydavis/f025c03a9221bc840a2b) (MIT) 
- [NativeCounter.cs](https://docs.unity3d.com/Packages/com.unity.jobs@0.8/manual/custom_job_types.html)

# Enquiries
For official communication, please send an email to milev109@gmail.com, otherwise DM me on Discord (Kosio#9955)

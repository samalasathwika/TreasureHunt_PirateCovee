TREASURE HUNT: PIRATE COVE - 3D endless runner (Unity 6)

UPGRADING FROM THE PREVIOUS RUNNER VERSION: just copy this Assets folder over your project (replace files), wait for compile, then run Pirate Cove > Build Entire Game again.

SETUP (5 minutes)
1. In your Unity project, DELETE the old files from the previous version:
   Assets/Scripts/*  (all old scripts)   and   Assets/Editor/PirateCoveBuilder.cs
   (you can also delete Assets/Prefabs and Assets/Scenes/Game.unity; they are rebuilt)
2. Copy this package's Assets folder into your project (merge). Wait for Unity to compile.
3. Window > TextMeshPro > Import TMP Essential Resources > Import (skip if already done).
4. Menu bar: Pirate Cove > Build Entire Game > Build.
5. Press Play in the MainMenu scene.

CONTROLS (PC): A / D or Left / Right = switch lane, Space / Up = jump, Esc or P = pause, R / Enter = restart after game over.

BUILD AN .EXE: File > Build Profiles (or Build Settings) > Windows > Build. Scenes are already listed: MainMenu, Game.
Reset saved progress: Edit > Clear All PlayerPrefs (in the Editor).

NEW LOOK: pirate hero + 3 chasing pirates, pirate galleons with cannons sailing alongside, treasure islands with gold piles, skull rock on the horizon, sunset sky + fog, rope rails, lanterns and Jolly Roger flags.
For truly realistic models, import a free/paid pirate asset pack (Asset Store: Kenney, Quaternius, Synty) and swap them into the prefabs in Assets/Prefabs.

LEVEL THEMES: Level 1 Moonlit Night (stars, moon, lamp light) - Level 2 Sunny Day (bright sky, clouds) - Level 3 Golden Sunset (orange sky and sea) - Level 4 Storm at Skull Rock (rain, dark clouds, lightning).

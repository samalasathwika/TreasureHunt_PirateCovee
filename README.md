<div align="center">

# 🏴‍☠️ Treasure Hunt: Pirate Cove

### A pirate-era 3D endless runner built in Unity

![Unity](https://img.shields.io/badge/Unity-6000.x-black?logo=unity)
![Language](https://img.shields.io/badge/C%23-scripts-239120?logo=csharp&logoColor=white)
![Genre](https://img.shields.io/badge/genre-endless%20runner-b8862b)
![Platform](https://img.shields.io/badge/platform-Windows%20(PC)-blue)
![Status](https://img.shields.io/badge/status-in%20development-orange)

*Run the pier, grab the gold, dodge the hazards, and stay ahead of the pirates on your tail, by night, day, sunset and storm.*

</div>

---

## 📑 Table of Contents
- [About the Game](#-about-the-game)
- [Screenshots](#-screenshots)
- [Features](#-features)
- [Level Themes](#-level-themes)
- [Controls](#-controls)
- [Gameplay Guide](#-gameplay-guide)
- [Getting Started](#-getting-started)
- [Project Structure](#-project-structure)
- [How It Works](#-how-it-works)
- [Roadmap](#-roadmap)
- [Team](#-team)
- [Acknowledgements](#-acknowledgements)
- [License](#-license)

---

## 🎮 About the Game

**Treasure Hunt: Pirate Cove** is a 3D endless runner set in the golden age of piracy. You play a treasure hunter sprinting down a wooden pier through Pirate Cove. Pirate galleons sail alongside, firing cannons into the sea, while a crew of cutlass-wielding pirates chases you from behind. Collect gold coins and treasure chests, grab stars, jump or sidestep hazards, and survive until you reach the end of each level. Every hit lets the pirates close in, and with no lives left, they catch you.

Each of the four levels has its own look: a moonlit night, a sunny day, a golden sunset and a storm at Skull Rock.

The project was created as a university game-development project at **SR University** (School of Computer Science and Artificial Intelligence).

---

## 🖼️ Screenshots
<img width="150" height="150" alt="Screenshot 2026-11-06 134331" src="https://github.com/user-attachments/assets/672d3607-e364-4102-a9cf-06ece51bf123" />
<img width="150" height="150" alt="Screenshot 2026-11-06 134356" src="https://github.com/user-attachments/assets/6d908578-90ef-4e54-8ec4-9d0dbdf4b767" />
<img width="150" height="150" alt="Screenshot 2026-11-06 181736" src="https://github.com/user-attachments/assets/5f86aa1d-0eb4-4f90-bf56-2a97cc5fb604" />
<img width="150" height="150" alt="Screenshot 2026-11-06 181746" src="https://github.com/user-attachments/assets/6136839b-cc2e-43c2-8ff5-80ab574b18db" />
<img width="150" height="150" alt="Screenshot 2026-11-06 181819" src="https://github.com/user-attachments/assets/a6f0919e-9334-49fb-ba38-88f9bf65827d" />


---

## ✨ Features

- **3D endless runner** on a three-lane wooden pier with smooth lane switching and jumping
- **Four themed levels**: Moonlit Night, Sunny Day, Golden Sunset and Storm at Skull Rock
- **Pirate-themed world**: pirate galleons with cannon fire, treasure islands, Skull Rock on the horizon, rope rails, lanterns and Jolly Roger flags
- **Chasing pirates** who close in each time you lose a life and catch you when your lives run out
- **Treasure to collect**: gold coins (including arcs over obstacles), treasure chests and stars
- **Varied hazards**: barrels, spikes, crates and cannons (jump them), plus crate stacks (change lane)
- **Health system**: 3 lives with a short invulnerability window after every hit
- **Rising difficulty**: speed increases during a run, and each level is faster with more obstacles
- **Missions and achievements** that track your progress across runs
- **Full menu system**: Play, Levels, Missions, Achievements, Settings, Quit
- **In-game screens**: HUD (lives, coins, stars, distance), Pause, Game Over, Level Complete
- **Settings**: music and SFX volume sliders, graphics quality switch
- **Procedural audio**: sound effects and a looping tune are generated in code, so no audio files are needed
- **Progress saving** using Unity `PlayerPrefs`
- **One-click scene builder**: an editor tool builds the prefabs, scenes and UI automatically

---

## 🌗 Level Themes

Each level changes only the environment (sky, light, sea and weather). Gameplay, characters and controls stay the same.

| Level | Theme | Look | Start speed | Obstacles | Target |
|:---:|---|---|:---:|:---:|:---:|
| 1 | 🌙 **Moonlit Night** | Deep blue sky, stars and moon, cool moonlight, glowing lanterns and a lamp that follows the hero | 9.0 m/s | 45% | 500 m |
| 2 | ☀️ **Sunny Day** | Bright blue sky, white clouds, warm sunlight, turquoise sea | 10.5 m/s | 55% | 1000 m |
| 3 | 🌅 **Golden Sunset** | Orange sky, low golden sun ahead, warm haze, tinted clouds | 12.0 m/s | 65% | 1500 m |
| 4 | ⛈️ **Storm at Skull Rock** | Dark clouds, dense fog, rain, lightning flashes, lamp light | 13.5 m/s | 75% | 2000 m |

*"Obstacles" is the chance that a row of the pier holds a hazard.*

---

## ⌨️ Controls

| Action | Keyboard |
|---|---|
| Move left / right (switch lane) | `A` / `D` or `←` / `→` |
| Jump | `Space`, `↑` or `W` |
| Pause / resume | `Esc` or `P` |
| Restart after Game Over | `R` or `Enter` |

> Touch / mobile controls are planned (see the [Roadmap](#-roadmap)).

---

## 🧭 Gameplay Guide

**Goal:** reach the distance target of the level while collecting as much treasure as you can.

| Item | Effect |
|---|---|
| 🪙 Gold coin | +10 coins |
| 🧰 Treasure chest | +50 coins and counts toward the *Find Treasure Chests* mission |
| ⭐ Star | 5 stars appear in each level; they decide your level rating |
| 🛢️ Barrel / 🪤 Spikes / 📦 Crate / 💣 Cannon | Lose 1 life. They can be jumped over |
| 🧱 Crate stack | Lose 1 life. Too tall to jump, so change lane |

Speed rises steadily during a run. Finishing a level unlocks the next one. Your rating is 3 stars for collecting all 5 stars, 2 stars for 3 or more, and 1 star otherwise.

**Missions:** collect 100 coins · find 3 treasure chests · run 500 meters
**Achievements:** Coin Collector (1000 coins) · Treasure Hunter (10 chests) · Marathon Runner (5000 meters)

---

## 🚀 Getting Started

### Requirements
- [Unity Hub](https://unity.com/download) and **Unity 6 (6000.x)**
- Windows (PC build target)

### Run the project
1. Clone the repository:
   ```bash
   git clone https://github.com/<your-username>/<your-repo-name>.git
   ```
2. Open **Unity Hub → Add → Add project from disk** and select the cloned folder.
3. Open it with Unity 6 and let it import.
4. If TextMeshPro asks, choose **Import TMP Essentials** (or go to *Window → TextMeshPro → Import TMP Essential Resources*).
5. Open `Assets/Scenes/MainMenu.unity` and press **Play**.

### Rebuild the scenes (optional)
If scenes or prefabs are missing or broken:

1. Go to **Pirate Cove → Build Entire Game** in the menu bar and click **Build**.
2. This recreates the tags, materials, prefabs, the `MainMenu` and `Game` scenes, and the Build Settings list.

> The level themes only appear in play mode. Pick a level from the **Levels** screen to see each one.

### Build an executable
**File → Build Profiles** (or *Build Settings*) → **Windows** → **Build**. Share the whole output folder, not just the `.exe`.

### Reset saved progress
In the editor: **Edit → Clear All PlayerPrefs**.

---

## 📁 Project Structure

```
Assets/
├── Editor/
│   └── PirateCoveBuilder.cs     # One-click builder for prefabs, scenes and UI
├── Scenes/
│   ├── MainMenu.unity
│   └── Game.unity
├── Scripts/
│   ├── RunnerGame.cs            # Game state, lives, score, HUD, pause / game over / level complete
│   ├── RunnerPlayer.cs          # Lane switching, jumping, collision handling
│   ├── TrackSpawner.cs          # Endless pier: obstacles, coins, chests, stars, islands
│   ├── PirateChasers.cs         # Pirates that run behind the player
│   ├── LevelTheme.cs            # Night / day / sunset / storm look for each level
│   ├── RunAnimator.cs           # Arm / leg swing animation
│   ├── Pickup.cs                # Coins, chests, stars
│   ├── ShipCannons.cs           # Decorative cannon fire from ships
│   ├── CannonBall.cs / SplashFx.cs
│   ├── FollowPlayer.cs / FollowZ.cs / CameraFollow.cs
│   ├── MenuUI.cs                # Main menu, levels, missions, achievements, settings
│   ├── Save.cs                  # Saved progress and level settings
│   ├── AudioManager.cs          # Procedurally generated sound effects and music
│   └── GameInput.cs             # Keyboard input (legacy and new Input System)
├── Materials/  Prefabs/         # Generated by the builder
Packages/
ProjectSettings/
```

---

## ⚙️ How It Works

- **Endless track:** `TrackSpawner` spawns 20 m pier segments ahead of the player and destroys old ones behind. Each segment is filled with obstacles, coin trails (weighted toward the middle lane), chests and stars. It never blocks all three lanes at once.
- **Player:** `RunnerPlayer` moves forward at the game speed and switches between three lanes. Jumping uses simple custom gravity. Collisions use trigger colliders and tags (`Hazard`, `Collectible`).
- **Difficulty:** `Save.cs` defines each level's target distance, start speed and obstacle chance. Speed increases gradually during a run.
- **Themes:** `LevelTheme` reads the selected level and sets the skybox, fog, sunlight, sea colour and extras (stars and moon, clouds, rain and lightning, lamp light).
- **Chasers:** `PirateChasers` keeps the pirates behind the player. Their distance depends on your remaining lives.
- **Scenery:** ships, Skull Rock and islands follow the player so the world always feels full.
- **Persistence:** coins, chests, distance, unlocked levels, level stars and volume settings are saved with `PlayerPrefs`.

---

## 🗺️ Roadmap

- [ ] Touch / mobile controls
- [ ] Gaps in the pier to jump over
- [ ] Imported 3D character and environment models (replace the primitive shapes)
- [ ] Power-ups (shield, coin magnet)
- [ ] Slide move and more obstacle types
- [ ] Local high-score table
- [ ] Additional levels and themes

---

## 👥 Developer

 **S. Sathwika** 


**Institution:** SR University, Ananthasagar, Warangal, Telangana
**Program:** B.Tech, School of Computer Science and Artificial Intelligence (2026–2027)
**Faculty guide:** *[Faculty Guide Name]*

---

## 🙏 Acknowledgements

- [Unity Technologies](https://unity.com/) for the game engine
- Our faculty guide and **SR University** for their guidance and support

---

## 📄 License

This project was created for academic purposes. 

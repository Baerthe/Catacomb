# Catacomb
Catacomb is an open-source arcade collection project. It serves as a comprehensive learning exercise in Godot 4.6 (Godot.NET.Sdk/4.6.0, .NET 9.0, C#) architecture by encapsulating classic retro games into a modular, unified "GamePack" system.

## Included Arcade Games
The project features recreated classic arcade titles, dynamically loaded using the custom GamePack infrastructure:
- **01. Tennis Game**: A classic Pong-style two-paddle game featuring single-player AI, multiplayer modes, speed scaling, paddle/color customization, and an integrated max-score system.
- **02. Block Game**: A Breakout/Arkanoid-style block-breaking game that relies on level-data layouts, block color mapping, and paddle physics to challenge players at clearing stages.
- **03. Frog Game**: A fully implemented Frogger-style survival/navigation game. Includes a `FrogCharacter` with grid-step movement, dangerous traffic, and `FrogMovable` entities (turtles, logs) with rideable platform properties. Completed with dynamic modulations and lives selections.

## Technical Details & Architecture
The project is structurally rooted around a single, global orchestration node (`AppShell`) that manages high-level transitions between the core systems like the Main Menu, dynamically instantiating self-contained game modules ("GamePacks"), and applying overlays (e.g. CRT Shaders).

### Core Systems Flow
- **AppShell (`AppShell.cs`)**: The global root node that handles application-level states (Intro, Main Menu, In-Game, Pause), orchestrated screen transitions, robust cursor visibility handling, and dynamic CRT shader overlays!
- **Game Managers (`GameManagers.cs`)**: A universally accessible wrapper that loads and provides essential systems across the app.
    - *Audio Manager*: Functions as a three-channel global playback structure. Loads and indexes `AudioEvent` resource files mapped into a lookup table for seamless sound logic without tight scene coupling.
    - *Score Manager*: Actively tracks high scores per GamePack. It can save/load data dynamically to Godot's local user directory and commit new entries whenever a pack evaluates a win/loss closure.
    - *Settings Manager*: Persists global user customization and auditory scales natively through `ConfigFile`. Actively maintains memory states like full screen handling, stretch modes, and channel sliders tied directly into the built-in UI representations (`settings_menu.tscn`).
- **Pack Register (`PackRegister.cs`)**: The runtime parser. It scans the designated `.tres` resource directory (`res://assets/resources/packs`) to auto-populate the valid games into the `MainMenu` registry.
- **Pack Base (`PackBase.cs`)**: The abstract `Node2D` backbone that every individual game inherits. Provides unified overrides natively connecting individual `GameState` operations (`Paused`, `Playing`, `GameOver`), automated tracking endpoints (`OnScoreSubmission`), and base controller hooks (`Player1`, `Score1`).

## Recent Project Updates
- **Frog Game (Game Pack 03)** Full finalization: Pack implemented custom adjustable start values (lives and modulation), fully resolving rideable platforms on a grid-based physics layout!
- **Engine Polish**: Deepened `AppShell` handling for applying runtime CRT shaders effectively scaled according to app state, alongside intuitive internal UI fixes for displaying custom cursors toggled directly over running vs. active configurations.

### Using & Adding GamePacks
The codebase is built for extreme modularity. A GamePack is essentially a complete Godot `PackedScene` wrapped with a customizable `GamePack` resource definition. GamePacks have their own state management; just fill in the state methods, use `RequestGameState()` to change state and relevant events to access outside flow. There is a template script setup under `scripts/game_packs/00_template_game`.

**To add a new GamePack:**
1. **Create the Base Scene:** Create a new node structure in `scenes/<your_game>/` and a corresponding C# script in `scripts/game_packs/<your_game>/` that inherits from `PackBase`.
2. **Implement Game Logic:** Implement your game loop inside the overridden `Tick()` method, which is automatically handled by the `PackBase` process loop (unless you override `EnableStepTicking`). Ensure it respects the `CurrentGameState`.
3. **Trigger Events:** Hook your level's game-over/exit states to emit `OnRequestPackExit` and notify your scores via `SubmitScore()`.
4. **Create the Pack Resource:** In the Godot Inspector, create a new `GamePack.tres` resource (found under Custom Resources globally or in `scripts/common/resource/GamePack.cs`) and save it to `assets/resources/packs/`.
5. **Configure the Resource:** Assign your new `PackedScene` to the resource, set the title, description, background color, CRT configurations, menu icon, and music references.
6. **Play:** Upon the next launch, `PackRegister.cs` will dynamically read your new `.tres` file and it will be fully integrated and playable from the Main Menu.

## License & Acknowledgements
This project is under GNU Affero General Public License v3.0 [License](LICENSE.txt) and uses assets from Kenney.nl under the CC0 1.0 Universal (CC0 1.0) Public Domain Dedication [Kenney Assets License](Asset_License.txt).

- This is inspired by the ["20 Game Challenge"](https://20_games_challenge.gitlab.io/), but is not affiliated with it in any way.
- CRT asset based off graphics created by [piiixl](https://piiixl.itch.io/game-ui) under License: [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/deed.en).
- Shaders included in this project are under CC0, CRT shader Optimised and packed by @c64cosmin

### Music
Music is Composed and produced by Marllon Silva (xDeviruchi), All the music of this project are released under License: [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/deed.en).
- [8-bit Fantasy Music](https://xdeviruchi.itch.io/8-bit-fantasy-adventure-music-pack)

### Kenney Assets
- [Digital Audio](https://www.kenney.nl/assets/digital-audio)
- [Fonts](https://www.kenney.nl/assets/kenney-fonts)
- [1-bit Pack](https://www.kenney.nl/assets/1-bit-pack), colors edited.
- [Pixel Curosr Pack](https://www.kenney.nl/assets/cursor-pixel-pack)
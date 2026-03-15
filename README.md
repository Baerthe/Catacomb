# Catacomb
Catacomb is an open-source arcade collection project. It serves as a comprehensive learning exercise in Godot 4.6 (Godot.NET.Sdk/4.6.0, .NET 9.0, C#) architecture by encapsulating classic retro games (like Pong and Breakout) into a modular, unified "GamePack" system.

## Technical Details & Architecture
The project is structured around a single-window "AppShell" that handles global states (Main Menu, Loading, In-Game) while dynamically instantiating self-contained game modules ("GamePacks").

### Core Systems Flow
- **AppShell (`AppShell.cs`)**: The global root node that orchestrates scenes, CRT shader overlays, transitioning, and system hooking.
- **Game Managers (`GameManagers.cs`)**: A globally accessible registry that manages `AudioManager`, `ScoreManager`, and `SettingsManager` instances.
- **Pack Register (`PackRegister.cs`)**: Scans the designated resource directory (`res://assets/resources/packs`) at runtime for `.tres` files to automatically populate the valid game list in the main menu.
- **Pack Base (`PackBase.cs`)**: The abstract Node2D base class that every GamePack must implement. It provides baseline logic for game states (`GameState.Playing`, `GameState.Paused`), controller inputs (`Player1Controller`, `Player2Controller`), scores (`Score1`), and base event hooks (`OnRequestPackExit`, `OnScoreSubmission`).

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

### Music
Music is Composed and produced by Marllon Silva (xDeviruchi), All the files of this package are released under the Attribution-ShareAlike 4.0 International (CC BY-SA 4.0) license.
- [8-bit Fantasy Music](https://xdeviruchi.itch.io/8-bit-fantasy-adventure-music-pack)

### Kenney Assets
- [Digital Audio](https://www.kenney.nl/assets/digital-audio)
- [Fonts](https://www.kenney.nl/assets/kenney-fonts)
- [1-bit Pack](https://www.kenney.nl/assets/1-bit-pack), colors edited.
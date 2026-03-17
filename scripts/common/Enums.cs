namespace Common;
/// <summary>
/// Direction enum for player movement.
/// </summary>
public enum Direction : byte {None, Up, Down, Left, Right}
/// <summary>
/// Player type enum for game mode selection.
/// </summary>
public enum PlayerType : byte {Player1, Player2, AI}
/// <summary>
/// Game States enum for tracking.
/// </summary>
public enum AppState : byte {Loading, MainMenu, InPack}
/// <summary>
/// Game States enum for tracking.
/// </summary>
public enum GameState : byte {Playing, Paused, GameOver, GameQuit}
/// <summary>
/// Generic game over reasons for score submission and end game handling, these should be mapped inside of GamePack implementation to strings.
/// </summary>
public enum GameOverReason : byte {Player1Lost, Player1Won, TimeUp, ScoreLimitReached, LevelCleared, EveryoneWon, EveryoneLost, Custom1, Custom2, Custom3, Custom4, Custom5}
/// <summary>
/// Game Settings section enum
/// </summary>
public enum Sectional : byte {Audio, User}
/// <summary>
/// Screen settings.
/// </summary>
public enum ScreenSet : byte {FullScreen, Windowed, FakeFullScreen}
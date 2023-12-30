An example project with Naninovel used as both drop-in dialogue for a 3D adventure game and a switchable standalone novel mode.

Be aware, that **Naninovel package is not distributed with the project**, hence compilation errors will be produced after opening it for the first time; import Naninovel from the Asset Store to resolve the issues.

All the project-specific (example) scripts are stored at `Assets/Runtime` folder.

Naninovel is initialized manually (auto initialization is disabled in the engine configuration menu) via `Runtime/SetupGame.cs` script attached to `SetupGame` game object located on `MainScene` scene.

`Runtime/DialogueTrigger.cs` script used as component on triggers perform switch to dialogue mode when player is hitting the trigger colliders.

`Runtime/SwitchToNovelMode.cs` custom command is used to switch to novel mode from both C# and naninovel scripts.

`Runtime/SwitchToAdventureMode.cs` custom command is used to switch to adventure from novel mode.

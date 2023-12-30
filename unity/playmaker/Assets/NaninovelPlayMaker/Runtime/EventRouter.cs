using UnityEngine;

namespace Naninovel.PlayMaker
{
    /// <summary>
    /// Routes (broadcasts) some of the essential Naninovel events to the PlayMaker FSMs.
    /// </summary>
    internal static class EventRouter
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize ()
        {
            if (Engine.Initialized) HandleEngineInitialized();
            else
            {
                Engine.OnInitializationFinished -= HandleEngineInitialized;
                Engine.OnInitializationFinished += HandleEngineInitialized;
            }
        }

        private static void HandleEngineInitialized ()
        {
            PlayMakerFSM.BroadcastEvent("Naninovel/Engine/OnInitialized");

            var stateMngr = Engine.GetService<StateManager>();
            stateMngr.OnGameSaveStarted += _ => PlayMakerFSM.BroadcastEvent("Naninovel/StateManager/OnGameSaveStarted");
            stateMngr.OnGameSaveFinished += _ => PlayMakerFSM.BroadcastEvent("Naninovel/StateManager/OnGameSaveFinished");
            stateMngr.OnGameLoadStarted += _ => PlayMakerFSM.BroadcastEvent("Naninovel/StateManager/OnGameLoadStarted");
            stateMngr.OnGameLoadFinished += _ => PlayMakerFSM.BroadcastEvent("Naninovel/StateManager/OnGameLoadFinished");

            var scriptPlayer = Engine.GetService<ScriptPlayer>();
            scriptPlayer.OnPlay += _ => PlayMakerFSM.BroadcastEvent("Naninovel/ScriptPlayer/OnPlay");
            scriptPlayer.OnStop += _ => PlayMakerFSM.BroadcastEvent("Naninovel/ScriptPlayer/OnStop");

            var printerManager = Engine.GetService<TextPrinterManager>();
            printerManager.OnPrintTextStarted += _ => PlayMakerFSM.BroadcastEvent("Naninovel/TextPrinterManager/OnPrintTextStarted");
            printerManager.OnPrintTextFinished += _ => PlayMakerFSM.BroadcastEvent("Naninovel/TextPrinterManager/OnPrintTextFinished");

            var localizationManager = Engine.GetService<LocalizationManager>();
            localizationManager.OnLocaleChanged += _ => PlayMakerFSM.BroadcastEvent("Naninovel/LocalizationManager/OnLocaleChanged");
        }
    }
}

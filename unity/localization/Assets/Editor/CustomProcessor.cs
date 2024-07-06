// using System;
// using Naninovel;
// using Naninovel.Spreadsheet;
// using UnityEngine;
//
// public class CustomProcessor : Processor
// {
//     protected override Action<ProcessorProgress> OnProgress => HandleProgress;
//
//     private readonly ProcessorOptions options;
//
//     public CustomProcessor (ProcessorOptions options) : base(options)
//     {
//         this.options = options;
//         Debug.Log("Below log messages are from custom processor example: " +
//                   StringUtils.BuildAssetLink("Assets/Editor/CustomProcessor.cs", 1));
//     }
//
//     protected override void ExportScripts ()
//     {
//         base.ExportScripts();
//         Debug.Log($"Exported scripts to '{options.OutputFolder}'.");
//     }
//
//     protected override void ExportText ()
//     {
//         base.ExportText();
//         Debug.Log($"Exported text to '{options.OutputFolder}'.");
//     }
//
//     protected override void ImportText ()
//     {
//         base.ImportText();
//         Debug.Log($"Imported text to '{options.TextFolder}'.");
//     }
//
//     protected override void ImportScripts ()
//     {
//         base.ImportScripts();
//         Debug.Log($"Imported scripts to '{options.ScriptFolder}'.");
//     }
//
//     private void HandleProgress (ProcessorProgress progress)
//     {
//         base.OnProgress(progress);
//         Debug.Log($"{progress.Info} {progress.Progress:P0}");
//     }
// }

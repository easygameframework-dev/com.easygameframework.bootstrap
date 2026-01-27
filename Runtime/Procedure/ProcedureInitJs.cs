using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using EasyGameFramework.Core.Fsm;
using EasyGameFramework.Core.Procedure;
using UnityEngine;

namespace EasyGameFramework.Bootstrap
{
    public class ProcedureInitJs : ProcedureBase
    {
        protected override async UniTask OnEnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            // var jsAssetInfos = GameEntry.Resource.GetAssetInfos(new[] { "Js" })
            //     .Where(info => info.AssetName.EndsWith(".mjs"))
            //     .ToArray();
            //
            // Log.Info("Init js");
            // Log.Debug($"Found {jsAssetInfos.Length} js assets");
            //
            // var startTime = Time.realtimeSinceStartup;
            // try
            // {
            //     await UniTask.WhenAll(jsAssetInfos.Select(info => GameEntry.Resource
            //         .LoadAssetAsync<TextAsset>(info.AssetName, info.PackageName)
            //         .ContinueWith(asset =>
            //         {
            //             var moduleName = info.AssetName;
            //             if (moduleName.StartsWith("Js_"))
            //             {
            //                 moduleName = moduleName[3..];
            //             }
            //
            //             GameEntry.JsSystem.RegisterJsAsset(moduleName, asset);
            //         })));
            // }
            // catch (Exception e)
            // {
            //     Log.Error($"Init js failed: {e}");
            //     ChangeState<ProcedureFatalError>(procedureOwner);
            //     return;
            // }
            // var endTime = Time.realtimeSinceStartup;
            // Log.Debug($"Init js complete, time: {endTime - startTime}s");

            ChangeState<ProcedureLoadAssembly>(procedureOwner);
        }

        protected override string GetLoadingSpinnerDescription(int phaseIndex, int phaseCount)
        {
            return "初始化js......";
        }
    }
}

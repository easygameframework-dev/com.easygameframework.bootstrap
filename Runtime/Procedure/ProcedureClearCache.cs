using System;
using Cysharp.Threading.Tasks;
using EasyGameFramework.Core.Fsm;
using EasyGameFramework.Core.Procedure;
using YooAsset;

namespace EasyGameFramework.Bootstrap
{
    /// <summary>
    /// 流程 => 清理缓存。
    /// </summary>
    public class ProcedureClearCache : ProcedureBase
    {
        private IFsm<IProcedureManager> _procedureOwner;

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            _procedureOwner = procedureOwner;
            Log.Debug("清理未使用的缓存文件！");

            // UILoadMgr.Show(UIDefine.UILoadUpdate,$"清理未使用的缓存文件...");
            //
            // var operation = GameModule.Resource.ClearUnusedCacheFilesAsync();
            // operation.Completed += Operation_Completed;


        }

        protected override async UniTask OnEnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            var tasks = YooAssets.GetAllPackages()
                .Select(package => ClearCacheFilesAsync(package, EFileClearMode.ClearUnusedBundleFiles));
            await UniTask.WhenAll(tasks);
            Log.Debug("Clear cache files complete.");
        }

        private static async UniTask ClearCacheFilesAsync(ResourcePackage package, EFileClearMode clearMode)
        {
            try
            {
                var operation = package.ClearCacheFilesAsync(clearMode);
                await operation.ToUniTask();
            }
            catch (Exception e)
            {
                Log.Error($"Clear cache files by mode '{clearMode}' failed: {e}");
            }
        }
    }
}

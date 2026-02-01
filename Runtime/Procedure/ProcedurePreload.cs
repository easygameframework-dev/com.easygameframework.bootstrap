using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using EasyGameFramework.Core.Fsm;
using EasyGameFramework.Core.Procedure;
using EasyGameFramework.Core.Resource;
using EasyGameFramework.Essentials;
using EasyGameFramework.Tasks;
using EasyGameFramework.YooAsset;
using YooAssetInfo = YooAsset.AssetInfo;

namespace EasyGameFramework.Bootstrap
{
    /// <summary>
    /// 预加载流程
    /// </summary>
    public class ProcedurePreload : ProcedureBase
    {
        private bool _isRetrying;

        protected override async UniTask OnEnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            Log.Debug("Preload assets");
            if (GameConfigAsset.Instance.PreloadAssetTags.Count != 0)
            {
                var package = YooAssetsHelper.GetPackage(Constant.Package.Main);
                var assetInfos = package.GetAssetInfos(GameConfigAsset.Instance.PreloadAssetTags.ToArray());

                if (assetInfos.Length > 0)
                {
                    try
                    {
                        var tasks = assetInfos.Select(assetInfo => LoadAssetWithRetryAsync(assetInfo));
                        await UniTask.WhenAll(tasks);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Preload assets failed: {e}");
                        ChangeState<ProcedureFatalError>(procedureOwner);
                        return;
                    }
                }
            }

            Log.Info("Preload complete");
            ChangeState<ProcedureInitJs>(procedureOwner);
        }

        protected override string GetLoadingSpinnerDescription(int phaseIndex, int phaseCount)
        {
            return $"预加载资源包“{Constant.Package.Main}”......";
        }

        private async UniTask<UnityEngine.Object> LoadAssetWithRetryAsync(YooAssetInfo assetInfo, int retryCount = 0)
        {
            UnityEngine.Object asset;
            try
            {
                asset = await GameEntry.Resource.LoadAssetAsync(new AssetAddress(assetInfo.Address, assetInfo.PackageName),
                    assetInfo.AssetType);
            }
            catch (Exception e)
            {
                Log.Error($"Load asset '{assetInfo.Address}' failed: {e}");

                // wait for another retry finish.
                if (retryCount == 0)
                {
                    while (_isRetrying)
                    {
                        await UniTask.Delay(500);
                    }
                }

                _isRetrying = true;
                if (retryCount >= GameEntry.Resource.FailedTryAgain)
                {
                    await GameEntry.UI.ShowMessageBoxAsync($"已重试达到最大次数。", UIMessageBoxType.Fatal);
                }
                else
                {
                    var index = await GameEntry.UI.ShowMessageBoxAsync($"加载资源“{assetInfo.AssetPath}”失败，是否尝试重新加载？",
                        UIMessageBoxType.Error, UIMessageBoxButtons.YesNo);

                    if (index == 0)
                    {
                        asset = await LoadAssetWithRetryAsync(assetInfo, retryCount + 1);
                        if (asset != null)
                        {
                            return asset;
                        }
                    }
                }

                throw;
            }

            return asset;
        }
    }
}

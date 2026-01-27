using System;
using Cysharp.Threading.Tasks;
using EasyGameFramework.Core;
using EasyGameFramework.Core.Fsm;
using EasyGameFramework.Core.Procedure;
using EasyGameFramework.Essentials;
using EasyGameFramework.Tasks;
using EasyGameFramework.YooAsset;
using YooAsset;

namespace EasyGameFramework.Bootstrap
{
    /// <summary>
    /// 流程 => 用户尝试更新清单
    /// </summary>
    public class ProcedureUpdateManifest : ProcedureBase
    {
        protected override async UniTask OnEnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            var packageName = GameEntry.Resource.DefaultPackageName;

            if (await UpdatePackageManifestWithRetryAsync(packageName))
            {
                ChangeState<ProcedureCreateDownloader>(procedureOwner);
            }
            else
            {
                ChangeState<ProcedureEndGame>(procedureOwner);
            }
        }

        protected override string GetLoadingSpinnerDescription(int phaseIndex, int phaseCount)
        {
            var packageName = GameEntry.Resource.DefaultPackageName;
            return $"更新资源包“{packageName}”清单......";
        }

        private async UniTask<bool> UpdatePackageManifestWithRetryAsync(string packageName, int retryCount = 0)
        {
            try
            {
                await UpdatePackageManifestAsync(packageName);
                Log.Debug($"Update package '{packageName}' manifest success.");
                return true;
            }
            catch (Exception e)
            {
                Log.Error($"Update package '{packageName}' manifest failed: {e}");
                var result = await GameEntry.UI.ShowMessageBoxAsync($"更新资源包“{packageName}”清单失败，是否尝试重新更新",
                    UIMessageBoxType.Error,
                    UIMessageBoxButtons.YesNo);
                if (result == 0)
                {
                    if (retryCount >= GameEntry.Resource.FailedTryAgain)
                    {
                        await GameEntry.UI.ShowMessageBoxAsync($"已重试达到最大次数，游戏即将退出。", UIMessageBoxType.Fatal);
                        return false;
                    }

                    if (await UpdatePackageManifestWithRetryAsync(packageName, retryCount + 1))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private async UniTask UpdatePackageManifestAsync(string packageName)
        {
            var package = YooAssetsHelper.GetPackage(packageName);

            var packageVersion = GameEntry.Setting.GetString(Utility.Text.Format(Constant.Setting.PackageVersion, packageName));
            var operation = package.UpdatePackageManifestAsync(packageVersion);
            await operation.ToUniTask();
        }
    }
}

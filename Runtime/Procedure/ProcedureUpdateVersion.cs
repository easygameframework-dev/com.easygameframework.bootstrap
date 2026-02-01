using System;
using Cysharp.Threading.Tasks;
using EasyGameFramework.Core;
using EasyGameFramework.Core.Fsm;
using EasyGameFramework.Core.Procedure;
using EasyGameFramework.Essentials;
using EasyGameFramework.Tasks;
using EasyGameFramework.YooAsset;
using UnityEngine;
using YooAsset;

using PlayMode = EasyGameFramework.Core.Resource.PlayMode;

namespace EasyGameFramework.Bootstrap
{
    /// <summary>
    /// 流程 => 用户尝试更新静态版本
    /// </summary>
    public class ProcedureUpdateVersion : ProcedureBase
    {
        protected override async UniTask OnEnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            if (GameEntry.Resource.PlayMode is not (PlayMode.EditorSimulateMode or PlayMode.OfflinePlayMode))
            {
                //检查设备是否能够访问互联网
                while (Application.internetReachability != NetworkReachability.NotReachable)
                {
                    var i = await GameEntry.UI.ShowMessageBoxAsync("网络不可用，请检查网络链接。", UIMessageBoxType.Error,
                        UIMessageBoxButtons.OkCancel);
                    if (i == 1)
                    {
                        ChangeState<ProcedureEndGame>(procedureOwner);
                        return;
                    }
                }
            }

            var packageVersion = await RequestPackageVersionWithRetryAsync(Constant.Package.Main);
            if (string.IsNullOrEmpty(packageVersion))
            {
                ChangeState<ProcedureEndGame>(procedureOwner);
                return;
            }

            GameEntry.Setting.SetString(Utility.Text.Format(Constant.Setting.PackageVersion, Constant.Package.Main), packageVersion);

            ChangeState<ProcedureUpdateManifest>(procedureOwner);
        }

        protected override string GetLoadingSpinnerDescription(int phaseIndex, int phaseCount)
        {
            return $"请求资源包“{Constant.Package.Main}”版本......";
        }

        private async UniTask<string> RequestPackageVersionWithRetryAsync(string packageName, int retryCount = 0)
        {
            try
            {
                var packageVersion = await RequestPackageVersionAsync(packageName);
                Log.Debug($"Request package '{packageName}' version '{packageVersion}' success.");
                return packageVersion;
            }
            catch (Exception e)
            {
                Log.Error($"Update package '{packageName}' version failed: {e}");
                var result = await GameEntry.UI.ShowMessageBoxAsync($"请求获取资源包“{packageName}”版本失败，是否尝试重新请求",
                    UIMessageBoxType.Error,
                    UIMessageBoxButtons.YesNo);
                if (result == 0)
                {
                    if (retryCount >= GameEntry.Resource.FailedTryAgain)
                    {
                        await GameEntry.UI.ShowMessageBoxAsync($"已重试达到最大次数，游戏即将退出。", UIMessageBoxType.Fatal);
                        return null;
                    }

                    var packageVersion = await RequestPackageVersionWithRetryAsync(packageName, retryCount + 1);
                    if (!string.IsNullOrEmpty(packageVersion))
                    {
                        return packageVersion;
                    }
                }

                return null;
            }
        }

        private async UniTask<string> RequestPackageVersionAsync(string packageName)
        {
            var package = YooAssetsHelper.GetPackage(packageName);
            var operation = package.RequestPackageVersionAsync();
            await operation.ToUniTask();
            return operation.PackageVersion;
        }
    }
}

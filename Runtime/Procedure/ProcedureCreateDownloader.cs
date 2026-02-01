using System;
using Cysharp.Threading.Tasks;
using EasyGameFramework.Core.Fsm;
using EasyGameFramework.Core.Procedure;
using EasyGameFramework.Essentials;
using EasyGameFramework.Tasks;
using EasyGameFramework.YooAsset;
using UnityEngine;

namespace EasyGameFramework.Bootstrap
{
    public class ProcedureCreateDownloader : ProcedureBase
    {
        protected override Func<int, int, string> LoadingSpinnerDescriptionGetter => null;

        protected override async UniTask OnEnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            Log.Debug($"Create downloader for package '{Constant.Package.Main}'");

            var downloader = YooAssetsHelper.CreatePackageResourceDownloader(
                Constant.Package.Main,
                GameEntry.Resource.DownloadingMaxNum,
                GameEntry.Resource.FailedTryAgain);

            if (downloader.TotalDownloadCount == 0)
            {
                Log.Debug("Not found any download files !");
                ChangeState<ProcedureDownloadFiles>(procedureOwner);
            }
            else
            {
                Log.Debug($"Found total {downloader.TotalDownloadCount} files that need download ！");

                var size = downloader.TotalDownloadBytes / 1024f / 1024f;
                size = Mathf.Clamp(size, 0.01f, float.MaxValue);

                var index = await GameEntry.UI.ShowMessageBoxAsync(
                    $"找到更新补丁文件，数量：{downloader.TotalDownloadCount}，大小：{size:F2}MB。\n是否下载？",
                    UIMessageBoxType.Tip, UIMessageBoxButtons.YesNo);

                if (index == 0)
                {
                    ChangeState<ProcedureDownloadFiles>(procedureOwner);
                }
                else
                {
                    ChangeState<ProcedureEndGame>(procedureOwner);
                }
            }
        }
    }
}

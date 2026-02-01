using System;
using Cysharp.Threading.Tasks;
using EasyGameFramework.Core.Fsm;
using EasyGameFramework.Core.Procedure;
using EasyGameFramework.Essentials;
using EasyGameFramework.Tasks;
using EasyGameFramework.YooAsset;
using UnityEngine;
using YooAsset;

namespace EasyGameFramework.Bootstrap
{
    public class ProcedureDownloadFiles : ProcedureBase
    {
        private IFsm<IProcedureManager> _procedureOwner;
        private ResourceDownloaderOperation _downloader;

        private long _lastUpdateDownloadedSize;

        private ResourceDownloaderOperation Downloader => _downloader ??=
            YooAssetsHelper.GetPackageResourceDownloader(Constant.Package.Main);

        private long CurrentSpeedBytes
        {
            get
            {
                var sizeDiff = Downloader.CurrentDownloadBytes - _lastUpdateDownloadedSize;
                _lastUpdateDownloadedSize = Downloader.CurrentDownloadBytes;
                var speed = sizeDiff / (double)Time.deltaTime;
                return (long)speed;
            }
        }

        protected override bool EnableAutoUpdateLoadingUISpinnerBox => Downloader.TotalDownloadCount > 0;

        protected override UniTask OnEnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            if (Downloader.TotalDownloadCount == 0)
            {
                ChangeState<ProcedureDownloadOver>(procedureOwner);
                return UniTask.CompletedTask;
            }

            _procedureOwner = procedureOwner;

            Downloader.DownloadFinishCallback += OnDownloadFinish;
            Downloader.DownloadErrorCallback += OnDownloadError;
            Downloader.DownloadUpdateCallback += OnDownloadUpdate;

            Downloader.BeginDownload();
            return UniTask.CompletedTask;
        }

        private void OnDownloadFinish(DownloaderFinishData data)
        {
            ChangeState<ProcedureDownloadOver>(_procedureOwner);
        }

        protected override string GetLoadingSpinnerDescription(int phaseIndex, int phaseCount)
        {
            return $"下载资源包“{Downloader.PackageName}”......\n" +
                   $"进度：{BytesToMb(Downloader.CurrentDownloadBytes)}/{BytesToMb(Downloader.TotalDownloadCount)}mb\n" +
                   $"速度：{BytesToMb(CurrentSpeedBytes)}mb/s";

            string BytesToMb(long bytes)
            {
                var mb = Mathf.Clamp(bytes / 1024f / 1024f, 0.01f, float.MaxValue);
                return mb.ToString("F");
            }
        }

        private void OnDownloadUpdate(DownloadUpdateData data)
        {
        }

        private void OnDownloadError(DownloadErrorData data)
        {
            Log.Error($"Download files failed: {data.ErrorInfo}");
            GameEntry.UI.ShowMessageBoxAsync($"下载资源失败，是否尝试重新下载？",
                    UIMessageBoxType.Error,
                    UIMessageBoxButtons.YesNo)
                .ContinueWith(i =>
                {
                    if (i == 0)
                    {
                        ChangeState<ProcedureCreateDownloader>(_procedureOwner);
                    }
                    else
                    {
                        ChangeState<ProcedureEndGame>(_procedureOwner);
                    }
                })
                .Forget();
        }
    }
}

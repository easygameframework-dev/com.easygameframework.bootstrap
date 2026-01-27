using System;
using Cysharp.Threading.Tasks;
using EasyGameFramework.Core.Fsm;
using EasyGameFramework.Core.Procedure;
using EasyGameFramework.Essentials;
using EasyGameFramework.Tasks;

namespace EasyGameFramework.Bootstrap
{
    public abstract class ProcedureBase : EasyGameFramework.Core.Procedure.ProcedureBase
    {
        private static int s_loadingPhasesIndex = 0;

        protected virtual bool EnableAutoUpdateLoadingPhasesContext => true;
        protected virtual bool EnableAutoUpdateLoadingUISpinnerBox => true;

        protected virtual Func<int, int, string> LoadingSpinnerDescriptionGetter => GetLoadingSpinnerDescription;

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            EnterAsync(procedureOwner).Forget();
        }

        private async UniTask EnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            // ProcedureInitPackage => ProcedureUpdateVersion => ProcedureUpdateManifest => ProcedureCreateDownloader =>
            // ProcedureDownloadFiles => ProcedureDownloadOver => ProcedurePreload => ProcedureLoadAssembly => ProcedureStartGame
            var phaseCount = 9;
            var phaseIndex = s_loadingPhasesIndex;

            if (EnableAutoUpdateLoadingPhasesContext)
            {
                s_loadingPhasesIndex++;
            }

            if (EnableAutoUpdateLoadingUISpinnerBox && UISpinnerBox.LastSpinnerBox != null)
            {
                float percentage = phaseIndex / (float)phaseCount;
                // Ensure that the previous phase is update completely.
                await GameEntry.UI.UpdateSpinnerBoxAsync(percentage);

                if (LoadingSpinnerDescriptionGetter != null)
                {
                    // Update the description of the current phase.
                    GameEntry.UI.UpdateSpinnerBoxAsync(
                        () => LoadingSpinnerDescriptionGetter(phaseIndex, phaseCount),
                        percentage).Forget();
                }
            }

            await OnEnterAsync(procedureOwner);
        }

        protected virtual string GetLoadingSpinnerDescription(int phaseIndex, int phaseCount)
        {
            return null;
        }

        protected virtual UniTask OnEnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            return UniTask.CompletedTask;
        }
    }
}

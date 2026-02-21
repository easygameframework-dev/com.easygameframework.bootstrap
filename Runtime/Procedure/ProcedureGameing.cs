using Cysharp.Threading.Tasks;
using EasyGameFramework.Core.Fsm;
using EasyGameFramework.Core.Procedure;
using EasyGameFramework.Essentials;

namespace EasyGameFramework.Bootstrap
{
    public class ProcedureGameing : ProcedureBase
    {
        protected override bool EnableAutoUpdateLoadingPhasesContext => false;
        protected override bool EnableAutoUpdateLoadingUISpinnerBox => false;

        protected override async UniTask OnEnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            if (UISpinnerBox.LastSpinnerBox)
            {
                await GameEntry.UI.UpdateSpinnerBoxAsync("加载完成", 1);
                GameEntry.UI.EndSpinnerBox();
            }
        }
    }
}

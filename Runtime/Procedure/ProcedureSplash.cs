using Cysharp.Threading.Tasks;
using EasyGameFramework.Core.Fsm;
using EasyGameFramework.Core.Procedure;
using EasyGameFramework.Tasks;
using UnityEngine;

namespace EasyGameFramework.Bootstrap
{
    /// <summary>
    /// 流程 => 闪屏。
    /// </summary>
    public class ProcedureSplash : ProcedureBase
    {
        protected override bool EnableAutoUpdateLoadingPhasesContext => false;
        protected override bool EnableAutoUpdateLoadingUISpinnerBox => false;

        protected override async UniTask OnEnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            await GameEntry.UI.BeginSpinnerBoxAsync("开始加载流程", 0, 1f);

            ChangeState<ProcedureInitPackage>(procedureOwner);
        }
    }
}

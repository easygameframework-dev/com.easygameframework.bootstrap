using Cysharp.Threading.Tasks;
using EasyGameFramework.Core.Event;
using EasyGameFramework.Core.Fsm;
using EasyGameFramework.Core.Procedure;
using EasyGameFramework.Essentials;

namespace EasyGameFramework.Bootstrap
{
    public class ProcedureFatalError : ProcedureBase
    {
        protected virtual bool EnableAutoUpdateLoadingPhasesContext => false;
        protected virtual bool EnableAutoUpdateLoadingUISpinnerBox => false;

        private string _message;
        private IFsm<IProcedureManager> _procedureOwner;

        protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnInit(procedureOwner);
            _procedureOwner = procedureOwner;
            GameEntry.Event.Subscribe<FatalErrorEventArgs>(OnFatalError);
        }

        protected override async UniTask OnEnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            await GameEntry.UI.ShowMessageBoxAsync(
                string.IsNullOrEmpty(_message) ? "出现严重异常，游戏即将退出。" : _message,
                UIMessageBoxType.Fatal);
            ChangeState<ProcedureEndGame>(procedureOwner);
        }

        private void OnFatalError(object sender, FatalErrorEventArgs e)
        {
            _message = e.Message;
            ChangeState<ProcedureFatalError>(_procedureOwner);
        }
    }
}

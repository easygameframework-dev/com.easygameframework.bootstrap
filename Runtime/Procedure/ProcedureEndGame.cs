using EasyGameFramework.Core.Fsm;
using EasyGameFramework.Core.Procedure;
using UnityEngine;

namespace EasyGameFramework.Bootstrap
{
    public class ProcedureEndGame : ProcedureBase
    {
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}

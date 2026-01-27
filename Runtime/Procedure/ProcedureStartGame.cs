using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using EasyGameFramework.Core.Fsm;
using EasyGameFramework.Core.Procedure;
using EasyGameFramework.Essentials;
using EasyGameFramework.Tasks;
using UnityEngine;

namespace EasyGameFramework.Bootstrap
{
    public class ProcedureStartGame : ProcedureBase
    {
        private GameSceneLoadState _gameSceneLoadState;

        protected override async UniTask OnEnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            // await GameEntry.JsSystem.RunAsync();
            var startSceneAssetReference = GameConfigAsset.Instance.StartSceneAssetReference;
            try
            {
                await GameEntry.GameScene.LoadGameSceneAsync(
                    startSceneAssetReference.ToAssetAddress(),
                    OnLoadStateChanged);
            }
            catch (Exception e)
            {
                Log.Fatal($"Load start scene failed: {e}");
                ChangeState<ProcedureFatalError>(procedureOwner);
                return;
            }

            GameObject.Find("Main Camera").GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
            ChangeState<ProcedureGameing>(procedureOwner);
        }

        private void OnLoadStateChanged(GameSceneLoadState loadState)
        {
            _gameSceneLoadState = loadState;
        }

        protected override string GetLoadingSpinnerDescription(int phaseIndex, int phaseCount)
        {
            switch (_gameSceneLoadState)
            {
                case GameSceneLoadState.LoadingNewScene:
                    return "加载新场景......";
                case GameSceneLoadState.InitializingNewScene:
                    return "初始化新场景......";
                case GameSceneLoadState.UnloadingPreviousScene:
                    return "卸载上一个场景......";
                case GameSceneLoadState.Completed:
                    return "新场景准备完毕";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

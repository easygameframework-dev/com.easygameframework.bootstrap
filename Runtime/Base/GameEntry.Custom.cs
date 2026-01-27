using EasyGameFramework.Essentials;
using UnityEngine;

namespace EasyGameFramework.Bootstrap
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GameEntry : MonoBehaviour
    {
        public static GameSceneComponent GameScene { get; private set; }
        public static GameObjectPoolComponent GameObjectPool { get; private set; }
        // public static JsSystemComponent JsSystem { get; private set; }
        public static ContextComponent Context { get; private set; }

        private static void InitCustomComponents()
        {
            GameScene = EasyGameFramework.GameEntry.GetComponent<GameSceneComponent>();
            GameObjectPool = EasyGameFramework.GameEntry.GetComponent<GameObjectPoolComponent>();
            // JsSystem = EasyGameFramework.GameEntry.GetComponent<JsSystemComponent>();
            Context = EasyGameFramework.GameEntry.GetComponent<ContextComponent>();
        }
    }
}

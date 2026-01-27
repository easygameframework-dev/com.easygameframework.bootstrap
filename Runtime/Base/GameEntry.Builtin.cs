using UnityEngine;

namespace EasyGameFramework.Bootstrap
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GameEntry : MonoBehaviour
    {
        /// <summary>
        /// 获取游戏基础组件。
        /// </summary>
        public static BaseComponent Base
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取配置组件。
        /// </summary>
        public static ConfigComponent Config
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取数据结点组件。
        /// </summary>
        public static DataNodeComponent DataNode
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取数据表组件。
        /// </summary>
        public static DataTableComponent DataTable
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取调试组件。
        /// </summary>
        public static DebuggerComponent Debugger
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取下载组件。
        /// </summary>
        public static DownloadComponent Download
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取实体组件。
        /// </summary>
        public static EntityComponent Entity
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取事件组件。
        /// </summary>
        public static EventComponent Event
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取文件系统组件。
        /// </summary>
        public static FileSystemComponent FileSystem
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取有限状态机组件。
        /// </summary>
        public static FsmComponent Fsm
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取本地化组件。
        /// </summary>
        public static LocalizationComponent Localization
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取网络组件。
        /// </summary>
        public static NetworkComponent Network
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取对象池组件。
        /// </summary>
        public static ObjectPoolComponent ObjectPool
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取流程组件。
        /// </summary>
        public static ProcedureComponent Procedure
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取资源组件。
        /// </summary>
        public static ResourceComponent Resource
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取场景组件。
        /// </summary>
        public static SceneComponent Scene
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取配置组件。
        /// </summary>
        public static SettingComponent Setting
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取声音组件。
        /// </summary>
        public static SoundComponent Sound
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取界面组件。
        /// </summary>
        public static UIComponent UI
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取网络组件。
        /// </summary>
        public static WebRequestComponent WebRequest
        {
            get;
            private set;
        }

        private static void InitBuiltinComponents()
        {
            Base = EasyGameFramework.GameEntry.GetComponent<BaseComponent>();
            Config = EasyGameFramework.GameEntry.GetComponent<ConfigComponent>();
            DataNode = EasyGameFramework.GameEntry.GetComponent<DataNodeComponent>();
            DataTable = EasyGameFramework.GameEntry.GetComponent<DataTableComponent>();
            Debugger = EasyGameFramework.GameEntry.GetComponent<DebuggerComponent>();
            Download = EasyGameFramework.GameEntry.GetComponent<DownloadComponent>();
            Entity = EasyGameFramework.GameEntry.GetComponent<EntityComponent>();
            Event = EasyGameFramework.GameEntry.GetComponent<EventComponent>();
            FileSystem = EasyGameFramework.GameEntry.GetComponent<FileSystemComponent>();
            Fsm = EasyGameFramework.GameEntry.GetComponent<FsmComponent>();
            Localization = EasyGameFramework.GameEntry.GetComponent<LocalizationComponent>();
            Network = EasyGameFramework.GameEntry.GetComponent<NetworkComponent>();
            ObjectPool = EasyGameFramework.GameEntry.GetComponent<ObjectPoolComponent>();
            Procedure = EasyGameFramework.GameEntry.GetComponent<ProcedureComponent>();
            Resource = EasyGameFramework.GameEntry.GetComponent<ResourceComponent>();
            Scene = EasyGameFramework.GameEntry.GetComponent<SceneComponent>();
            Setting = EasyGameFramework.GameEntry.GetComponent<SettingComponent>();
            Sound = EasyGameFramework.GameEntry.GetComponent<SoundComponent>();
            UI = EasyGameFramework.GameEntry.GetComponent<UIComponent>();
            WebRequest = EasyGameFramework.GameEntry.GetComponent<WebRequestComponent>();
        }
    }
}

using System;
using System.Collections.Generic;
using EasyGameFramework.Essentials;
using EasyToolkit.Core.Patterns;
using EasyToolkit.Inspector.Attributes;
using UnityEngine;

namespace EasyGameFramework.Bootstrap
{
    public enum ConfigDataType
    {
        ProtobufJson,
        ProtobufBinary,
    }

    [EasyInspector]
    [ScriptableObjectSingletonConfiguration("Assets/Resources/Configs")]
    public class GameConfigAsset : ScriptableObjectSingleton<GameConfigAsset>
    {
        [Title("Assets")]
        [ListDrawerSettings(ShowIndexLabel = false)]
        [SerializeField] private List<string> _preloadAssetTags;

        [SerializeField] private AssetReference _startSceneAssetReference = new AssetReference(Constant.Package.Main, "Scene_User");

        [Title("Config")]
        [SerializeField] private ConfigDataType _configDataType = ConfigDataType.ProtobufJson;
        [SerializeField] private string _configPackageName = "Main";
        [SerializeField] private string _configAssetName = "Config_{0}";

        [Title("HybridCLR")]
        [SerializeField] private string _assemblyPackageName = "Main";
        [SerializeField] private string _assemblyAssetName = "DLL_{0}";

        [ListDrawerSettings(ShowIndexLabel = false)]
        [SerializeField] private List<string> _hotUpdateAssemblyNames = new List<string>();

        [ListDrawerSettings(ShowIndexLabel = false)]
        [SerializeField] private List<string> _aotMetaAssemblyNames = new List<string>();

        public IReadOnlyList<string> PreloadAssetTags => _preloadAssetTags;

        public ConfigDataType ConfigDataType => _configDataType;
        public string ConfigPackageName => _configPackageName;
        public string ConfigAssetName => _configAssetName;

        public string AssemblyPackageName => _assemblyPackageName;
        public string AssemblyAssetName => _assemblyAssetName;

        public IReadOnlyList<string> HotUpdateAssemblyNames => _hotUpdateAssemblyNames;
        public IReadOnlyList<string> AOTMetaAssemblyNames => _aotMetaAssemblyNames;

        public AssetReference StartSceneAssetReference => _startSceneAssetReference;
    }
}

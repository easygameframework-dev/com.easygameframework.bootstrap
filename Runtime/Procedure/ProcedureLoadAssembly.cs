using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using Cysharp.Threading.Tasks;
using EasyGameFramework.Core;
using EasyGameFramework.Core.Fsm;
using EasyGameFramework.Core.Procedure;
using EasyGameFramework.Core.Resource;
using EasyGameFramework.Tasks;
using PlayMode = EasyGameFramework.Core.Resource.PlayMode;

namespace EasyGameFramework.Bootstrap
{
    /// <summary>
    /// 流程加载器 - 代码初始化
    /// </summary>
    public class ProcedureLoadAssembly : ProcedureBase
    {
        private readonly List<Assembly> _hotUpdateAssemblies = new List<Assembly>();
        private bool _isRetrying = false;
        private IFsm<IProcedureManager> _procedureOwner;

        private bool _isEntryingAssemblies = false;
        private int _entryedAssemblyCount = 0;

        protected override async UniTask OnEnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            _procedureOwner = procedureOwner;

            if (GameEntry.Resource.PlayMode == PlayMode.EditorSimulateMode)
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var hotUpdateAssemblyName in GameConfigAsset.Instance.HotUpdateAssemblyNames)
                    {
                        if (hotUpdateAssemblyName == $"{assembly.GetName().Name}")
                        {
                            _hotUpdateAssemblies.Add(assembly);
                        }
                    }

                    if (_hotUpdateAssemblies.Count == GameConfigAsset.Instance.HotUpdateAssemblyNames.Count)
                    {
                        break;
                    }
                }
            }
            else
            {
                try
                {
                    var tasks = GameConfigAsset.Instance.HotUpdateAssemblyNames.Select(LoadAssemblyByNameAsync);
                    await UniTask.WhenAll(tasks);
                }
                catch (Exception e)
                {
                    Log.Error($"Load assemblies failed: {e}");
                    ChangeState<ProcedureFatalError>(procedureOwner);
                    return;
                }
            }

            Log.Info("Load assemblies complete.");

            _isEntryingAssemblies = true;
            try
            {
                await EntryAssembliesAsync();
            }
            catch (Exception e)
            {
                Log.Error($"Entry assemblies failed: {e}");
                ChangeState<ProcedureFatalError>(procedureOwner);
                return;
            }

            _isEntryingAssemblies = false;

            Log.Info("Entry assemblies complete.");

            ChangeState<ProcedureStartGame>(procedureOwner);
        }

        private async UniTask EntryAssembliesAsync()
        {
            var entries = _hotUpdateAssemblies.SelectMany(assembly => assembly.GetTypes())
                .SelectMany(type => type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                .Where(method => method.IsDefined(typeof(HotUpdateEntryAttribute)))
                .OrderByDescending(method => method.GetCustomAttribute<HotUpdateEntryAttribute>().Priority);

            foreach (var entry in entries)
            {
                var result = entry.Invoke(null, null);
                if (result is UniTask task)
                {
                    await task;
                }

                await UniTask.Delay(100);
                _entryedAssemblyCount++;
            }
        }

        protected override string GetLoadingSpinnerDescription(int phaseIndex, int phaseCount)
        {
            if (_isEntryingAssemblies)
            {
                return $"初始化程序集......\n{_entryedAssemblyCount}/{_hotUpdateAssemblies.Count}";
            }

            return $"加载程序集......\n{_hotUpdateAssemblies.Count}/{GameConfigAsset.Instance.HotUpdateAssemblyNames.Count}";
        }

        private async UniTask LoadAssemblyByNameAsync(string assemblyName)
        {
            var textAsset = await LoadAssemblyTextAssetByNameWithRetryAsync(assemblyName);
            _isRetrying = false;

            try
            {
                var assembly = Assembly.Load(textAsset.bytes);
                _hotUpdateAssemblies.Add(assembly);
                Log.Debug($"Assembly '{assembly.GetName().Name}' loaded");
            }
            catch (Exception e)
            {
                Log.Error($"Load assembly '{assemblyName}' failed: {e}");
                throw;
            }
        }

        private async UniTask<TextAsset> LoadAssemblyTextAssetByNameWithRetryAsync(string assemblyName,
            int retryCount = 0)
        {
            TextAsset textAsset;
            try
            {
                textAsset = await GameEntry.Resource.LoadAssetAsync<TextAsset>(new AssetAddress(
                    GameConfigAsset.Instance.AssemblyPackageName,
                    Utility.Text.Format(GameConfigAsset.Instance.AssemblyAssetName, assemblyName)));
            }
            catch (Exception e)
            {
                Log.Error($"Load assembly '{assemblyName}' text asset failed: {e}");

                // wait for another retry finish.
                if (retryCount == 0)
                {
                    while (_isRetrying)
                    {
                        await UniTask.Delay(500);
                    }
                }

                _isRetrying = true;
                if (retryCount >= GameEntry.Resource.FailedTryAgain)
                {
                    await GameEntry.UI.ShowMessageBoxAsync($"已重试达到最大次数。", UIMessageBoxType.Fatal);
                }
                else
                {
                    var index = await GameEntry.UI.ShowMessageBoxAsync($"加载程序集“{assemblyName}”资源失败，是否尝试重新加载？",
                        UIMessageBoxType.Error, UIMessageBoxButtons.YesNo);

                    if (index == 0)
                    {
                        textAsset = await LoadAssemblyTextAssetByNameWithRetryAsync(assemblyName, retryCount + 1);
                        if (textAsset != null)
                        {
                            return textAsset;
                        }
                    }
                }

                throw;
            }

            return textAsset;
        }

        // /// <summary>
        // /// 为Aot Assembly加载原始metadata， 这个代码放Aot或者热更新都行。
        // /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行。
        // /// </summary>
        // public void LoadMetadataForAOTAssembly()
        // {
        //     // 可以加载任意aot assembly的对应的dll。但要求dll必须与unity build过程中生成的裁剪后的dll一致，而不能直接使用原始dll。
        //     // 我们在BuildProcessor_xxx里添加了处理代码，这些裁剪后的dll在打包时自动被复制到 {项目目录}/HybridCLRData/AssembliesPostIl2CppStrip/{Target} 目录。
        //
        //     // 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
        //     // 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
        //     if (GameConfigAsset.Instance.AOTMetaAssemblyNames.Count == 0)
        //     {
        //         m_LoadMetadataAssemblyComplete = true;
        //         return;
        //     }
        //
        //     foreach (string aotDllName in GameConfigAsset.Instance.AOTMetaAssemblyNames)
        //     {
        //         var assetLocation = aotDllName;
        //         Log.Debug($"LoadMetadataAsset: [ {assetLocation} ]");
        //         m_LoadMetadataAssetCount++;
        //         // GameModule.Resource.LoadAsset<TextAsset>(assetLocation, LoadMetadataAssetSuccess);
        //     }
        //
        //     m_LoadMetadataAssemblyWait = true;
        // }
        //
        // /// <summary>
        // /// 加载元数据资源成功回调。
        // /// </summary>
        // /// <param name="textAsset">资源操作句柄。</param>
        // private void LoadMetadataAssetSuccess(TextAsset textAsset)
        // {
        //     m_LoadMetadataAssetCount--;
        //
        //     if (null == textAsset)
        //     {
        //         Log.Debug($"LoadMetadataAssetSuccess:Load Metadata failed.");
        //         return;
        //     }
        //
        //     string assetName = textAsset.name;
        //     Log.Debug($"LoadMetadataAssetSuccess, assetName: [ {assetName} ]");
        //
        //     try
        //     {
        //         byte[] dllBytes = textAsset.bytes;
        //         // 加载assembly对应的dll，会自动为它hook。一旦Aot泛型函数的native函数不存在，用解释器版本代码
        //         HomologousImageMode mode = HomologousImageMode.SuperSet;
        //         LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
        //         Log.Warning($"LoadMetadataForAOTAssembly:{assetName}. mode:{mode} ret:{err}");
        //     }
        //     catch (Exception e)
        //     {
        //         m_FailureMetadataAssetCount++;
        //         Log.Fatal(e.Message);
        //         throw;
        //     }
        //     finally
        //     {
        //         m_LoadMetadataAssemblyComplete = m_LoadMetadataAssemblyWait && 0 == m_LoadMetadataAssetCount;
        //     }
        // }
    }
}

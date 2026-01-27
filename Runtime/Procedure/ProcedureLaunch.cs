using System;
using EasyGameFramework.Core.Fsm;
using EasyGameFramework.Core.Localization;
using EasyGameFramework.Core.Procedure;
using EasyGameFramework.Essentials;
using UnityEngine;

namespace EasyGameFramework.Bootstrap
{
    /// <summary>
    /// 流程 => 启动器。
    /// </summary>
    public class ProcedureLaunch : ProcedureBase
    {
        protected override bool EnableAutoUpdateLoadingPhasesContext => false;
        protected override bool EnableAutoUpdateLoadingUISpinnerBox => false;

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);

            InitLanguageSettings();
            InitSoundSettings();
        }

        protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            ChangeState<ProcedureInitBuiltinPackage>(procedureOwner);
        }

        private void InitLanguageSettings()
        {
            bool isEditorMode = Application.platform == RuntimePlatform.WindowsEditor ||
                                Application.platform == RuntimePlatform.OSXEditor ||
                                Application.platform == RuntimePlatform.LinuxEditor;
            if (isEditorMode && GameEntry.Base.EditorLanguage != Language.Unspecified)
            {
                // 编辑器资源模式直接使用 Inspector 上设置的语言
                return;
            }

            Language language = GameEntry.Localization.Language;
            if (GameEntry.Setting.HasSetting(Constant.Setting.Language))
            {
                try
                {
                    string languageString = GameEntry.Setting.GetString(Constant.Setting.Language);
                    language = (Language)Enum.Parse(typeof(Language), languageString);
                }
                catch(Exception exception)
                {
                    Log.Error("Init language error, reason {0}",exception.ToString());
                }
            }

            if (language != Language.English
                && language != Language.ChineseSimplified
                && language != Language.ChineseTraditional
                && language != Language.Korean)
            {
                // 若是暂不支持的语言，则使用英语
                language = Language.English;

                GameEntry.Setting.SetString(Constant.Setting.Language, language.ToString());
                GameEntry.Setting.Save();
            }

            GameEntry.Localization.Language = language;
            Log.Debug("Init language settings complete, current language is '{0}'.", language.ToString());
        }

        private void InitSoundSettings()
        {
            GameEntry.Sound.Mute(Constant.SoundGroup.Music, GameEntry.Setting.GetBool(Constant.Setting.MusicMuted, false));
            GameEntry.Sound.SetVolume(Constant.SoundGroup.Music, GameEntry.Setting.GetFloat(Constant.Setting.MusicVolume, 0.5f));
            GameEntry.Sound.Mute(Constant.SoundGroup.Sound, GameEntry.Setting.GetBool(Constant.Setting.SoundMuted, false));
            GameEntry.Sound.SetVolume(Constant.SoundGroup.Sound, GameEntry.Setting.GetFloat(Constant.Setting.SoundVolume, 0.5f));
            GameEntry.Sound.Mute(Constant.SoundGroup.UISound, GameEntry.Setting.GetBool(Constant.Setting.UISoundMuted, false));
            GameEntry.Sound.SetVolume(Constant.SoundGroup.UISound, GameEntry.Setting.GetFloat(Constant.Setting.UISoundVolume, 0.5f));
            Log.Debug("Init sound settings complete.");
        }
    }
}

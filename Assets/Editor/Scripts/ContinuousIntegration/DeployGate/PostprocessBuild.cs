using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Build;
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif
using UnityEngine;
using UnityModule.Command.VCS;
using UnityModule.Settings;

namespace ContinuousIntegration
{
    // ReSharper disable once PartialTypeWithSinglePart
    [PublicAPI]
    public partial class DeployGate
    {
        private const int PostprocessBuildCallbackOrder = 200;

#if UNITY_2018_1_OR_NEWER
        public class PostprocessBuild : IPostprocessBuildWithReport
        {
#else
        public class PostprocessBuild : IPostprocessBuild
        {
#endif

            /// <summary>
            /// 環境変数キー: ユーザ
            /// </summary>
            private const string EnvironmentKeyBuildUser = "BUILD_USER";

            /// <summary>
            /// 環境変数キー: ブランチ
            /// </summary>
            private const string EnvironmentKeyBuildBranch = "BUILD_BRANCH";

            /// <summary>
            /// 環境変数キー: 環境
            /// </summary>
            private const string EnvironmentKeyBuildDevelopment = "BUILD_DEVELOPMENT";

            /// <summary>
            /// 環境変数キー: エディタバージョン
            /// </summary>
            private const string EnvironmentKeyBuildEditorVersion = "BUILD_EDITOR_VERSION";

            /// <summary>
            /// 環境変数: Android App Bundle を有効にするかどうか
            /// </summary>
            private const string EnvironmentVariableAndroidAppBundle = "BUILD_ANDROID_APP_BUNDLE";

            /// <summary>
            /// メッセージ接頭辞
            /// </summary>
            private static readonly Dictionary<string, string> MessagePrefixes = new Dictionary<string, string>()
            {
                {EnvironmentKeyBuildUser, "User"},
                {EnvironmentKeyBuildBranch, "Branch"},
                {EnvironmentKeyBuildDevelopment, "Environment"},
                {EnvironmentKeyBuildEditorVersion, "Unity"},
            };

            public int callbackOrder => PostprocessBuildCallbackOrder;

#if UNITY_2018_1_OR_NEWER
            public void OnPostprocessBuild(BuildReport report)
            {
                if (!DeployGateSetting.GetOrDefault().ShouldDeployToDeployGate)
                {
                    return;
                }

                // NOTE: Android App Bundle は DeployGate 側がサポートしてくれないため、出力の有無を問わず処理を行わない
                if (report.summary.platform == BuildTarget.Android && Environment.GetEnvironmentVariable(EnvironmentVariableAndroidAppBundle) == "true")
                {
                    return;
                }

                var archivePath = ResolveArchivePath(report.summary.platform, report.summary.outputPath);
                if (!File.Exists(archivePath))
                {
                    Debug.LogError($"\"{archivePath}\" にビルド済のアーカイブが見付かりませんでした。");
                    if (Application.isBatchMode)
                    {
                        // NOTE: throw Exceptionだとビルドが止まらないため、ExitCode返却しつつ終了
                        EditorApplication.Exit(1);
                    }
                    else
                    {
                        return;
                    }
                }

                Deploy(archivePath, GenerateMessage(report.summary.platform));
            }
#else
            public void OnPostprocessBuild(BuildTarget target, string path)
            {
                if (!DeployGateSetting.GetOrDefault().ShouldDeployToDeployGate)
                {
                    return;
                }
                Deploy(ResolveArchivePath(target, path), GenerateMessage(target));
            }
#endif

            /// <summary>
            /// ビルド済のアーカイブファイルのパスを解決
            /// </summary>
            /// <param name="target">出力先ターゲットプラットフォーム</param>
            /// <param name="path">出力先パス</param>
            /// <returns>解決済みのパス</returns>
            /// <exception cref="ArgumentException">ファイルが見付からなかった場合に throw</exception>
            private static string ResolveArchivePath(BuildTarget target, string path)
            {
                var archivePath = string.Empty;
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (target)
                {
                    case BuildTarget.iOS:
                        // XXX: 出力先は @umm/xcode_archiver に依存するので、書き換えを検討する
                        archivePath = $"{path}/export-ad-hoc/Unity-iPhone.ipa";
                        break;
                    case BuildTarget.Android:
                        archivePath = path;
                        break;
                }

                return archivePath;
            }

            /// <summary>
            /// メッセージを生成する
            /// </summary>
            /// <param name="target">出力先ターゲットプラットフォーム</param>
            /// <returns>メッセージ</returns>
            private static string GenerateMessage(BuildTarget target)
            {
                var message = string.Empty;
                message += GenerateBuildMessage(EnvironmentKeyBuildUser);
                message += GeneratePlatformMessage(target);
                message += GenerateAppVersionMessage();
                message += GenerateBuildMessage(EnvironmentKeyBuildBranch);
                message += GenerateCommitMessage();
                message += GenerateEnvironmentMessage();
                message += GenerateBuildMessage(EnvironmentKeyBuildEditorVersion);
                return message;
            }

            /// <summary>
            /// 指定されたビルドパラメータに該当するメッセージを生成して返却する
            /// </summary>
            /// <returns>メッセージ</returns>
            private static string GenerateBuildMessage(string buildParameter)
            {
                var value = Environment.GetEnvironmentVariable(buildParameter);
                if (string.IsNullOrEmpty(value))
                {
                    return string.Empty;
                }

                return $"{MessagePrefixes[buildParameter]}: {value}\n";
            }

            /// <summary>
            /// プラットフォームのメッセージを生成して返却する
            /// </summary>
            /// <returns>メッセージ</returns>
            private static string GeneratePlatformMessage(BuildTarget target)
            {
                return $"Platform: {(target == BuildTarget.iOS ? "iOS" : "Android")}\n";
            }

            /// <summary>
            /// アプリバージョンのメッセージを生成して返却する
            /// </summary>
            /// <returns>メッセージ</returns>
            private static string GenerateAppVersionMessage()
            {
                return $"Version: {Application.version}\n";
            }

            /// <summary>
            /// 環境のメッセージを生成して返却する
            /// </summary>
            /// <returns>メッセージ</returns>
            private static string GenerateEnvironmentMessage()
            {
                var value = Environment.GetEnvironmentVariable(EnvironmentKeyBuildDevelopment);
                if (string.IsNullOrEmpty(value))
                {
                    return string.Empty;
                }

                return $"{MessagePrefixes[EnvironmentKeyBuildDevelopment]}: {(value == "true" ? "development" : "production")}\n";
            }

            /// <summary>
            /// コミット番号のメッセージを生成して返却する
            /// </summary>
            /// <returns>メッセージ</returns>
            private static string GenerateCommitMessage()
            {
                return $"Commit: {Git.GetCurrentCommitHash()}\n";
            }
        }
    }
}
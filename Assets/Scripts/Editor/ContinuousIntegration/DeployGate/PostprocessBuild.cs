using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityModule.Settings;

namespace ContinuousIntegration {

    // ReSharper disable once PartialTypeWithSinglePart
    public partial class DeployGate {

        public const int POSTPROCESS_BUILD_CALLBACK_ORDER = 200;

        public class PostprocessBuild : IPostprocessBuild {

            /// <summary>
            /// 環境変数キー: ユーザ
            /// </summary>
            private const string ENVIRONMENT_KEY_BUILD_USER = "BUILD_USER";

            /// <summary>
            /// 環境変数キー: ブランチ
            /// </summary>
            private const string ENVIRONMENT_KEY_BUILD_BRANCH = "BUILD_BRANCH";

            /// <summary>
            /// 環境変数キー: 環境
            /// </summary>
            private const string ENVIRONMENT_KEY_BUILD_DEVELOPMENT = "BUILD_DEVELOPMENT";

            /// <summary>
            /// 環境変数キー: エディタバージョン
            /// </summary>
            private const string ENVIRONMENT_KEY_BUILD_EDITOR_VERSION = "BUILD_EDITOR_VERSION";

            /// <summary>
            /// メッセージ接頭辞
            /// </summary>
            private static readonly Dictionary<string, string> MESSAGE_PREFIXES = new Dictionary<string, string>() {
                { ENVIRONMENT_KEY_BUILD_USER,           "User" },
                { ENVIRONMENT_KEY_BUILD_BRANCH,         "Branch" },
                { ENVIRONMENT_KEY_BUILD_DEVELOPMENT,    "Environment" },
                { ENVIRONMENT_KEY_BUILD_EDITOR_VERSION, "Unity" },
            };

            public int callbackOrder {
                get {
                    return POSTPROCESS_BUILD_CALLBACK_ORDER;
                }
            }

            public void OnPostprocessBuild(BuildTarget target, string path) {
                Deploy(ResolveArchivePath(target, path), GenerateMessage(target));
            }

            /// <summary>
            /// ビルド済のアーカイブファイルのパスを解決
            /// </summary>
            /// <param name="target">出力先ターゲットプラットフォーム</param>
            /// <param name="path">出力先パス</param>
            /// <returns>解決済みのパス</returns>
            /// <exception cref="ArgumentException">ファイルが見付からなかった場合に throw</exception>
            private static string ResolveArchivePath(BuildTarget target, string path) {
                string archivePath = string.Empty;
                switch (target) {
                    case BuildTarget.iOS:
                        archivePath = string.Format("{0}/build/Unity-iPhone.ipa", path);
                        break;
                    case BuildTarget.Android:
                        archivePath = path;
                        break;
                }
                if (!File.Exists(archivePath)) {
                    throw new ArgumentException(string.Format("\"{0}\" にビルド済のアーカイブが見付かりませんでした。", archivePath));
                }
                return archivePath;
            }

            /// <summary>
            /// メッセージを生成する
            /// </summary>
            /// <param name="target">出力先ターゲットプラットフォーム</param>
            /// <returns>メッセージ</returns>
            private static string GenerateMessage(BuildTarget target) {
                string message = string.Empty;
                message += GenerateBuildMessage(ENVIRONMENT_KEY_BUILD_USER);
                message += GeneratePlatformMessage(target);
                message += GenerateAppVersionMessage();
                message += GenerateBuildMessage(ENVIRONMENT_KEY_BUILD_BRANCH);
                message += GenerateCommitMessage();
                message += GenerateEnvironmentMessage(ENVIRONMENT_KEY_BUILD_DEVELOPMENT);
                message += GenerateBuildMessage(ENVIRONMENT_KEY_BUILD_EDITOR_VERSION);
                return message;
            }

            /// <summary>
            /// 指定されたビルドパラメータに該当するメッセージを生成して返却する
            /// </summary>
            /// <returns>メッセージ</returns>
            private static string GenerateBuildMessage(string buildParameter) {
                string value = Environment.GetEnvironmentVariable(buildParameter);
                if (string.IsNullOrEmpty(value)) {
                    return string.Empty;
                }
                return string.Format("{0}: {1}\n", MESSAGE_PREFIXES[buildParameter], value);
            }

            /// <summary>
            /// プラットフォームのメッセージを生成して返却する
            /// </summary>
            /// <returns>メッセージ</returns>
            private static string GeneratePlatformMessage(BuildTarget target) {
                return string.Format("Platform: {0}\n", target == BuildTarget.iOS ? "iOS" : "Android");
            }

            /// <summary>
            /// アプリバージョンのメッセージを生成して返却する
            /// </summary>
            /// <returns>メッセージ</returns>
            private static string GenerateAppVersionMessage() {
                return string.Format("Version: {0}\n", Application.version);
            }

            /// <summary>
            /// 環境のメッセージを生成して返却する
            /// </summary>
            /// <returns>メッセージ</returns>
            private static string GenerateEnvironmentMessage(string buildParameter) {
                string value = Environment.GetEnvironmentVariable(buildParameter);
                if (string.IsNullOrEmpty(value)) {
                    return string.Empty;
                }
                return string.Format("{0}: {1}\n", MESSAGE_PREFIXES[buildParameter], value == "true" ? "development" : "production");
            }

            /// <summary>
            /// コミット番号のメッセージを生成して返却する
            /// </summary>
            /// <returns>メッセージ</returns>
            private static string GenerateCommitMessage() {
                return string.Format("Commit: {0}\n", GetCommit());
            }

            /// <summary>
            /// コミット番号を取得する
            /// </summary>
            /// <returns>メッセージ</returns>
            private static string GetCommit() {
                System.Diagnostics.Process process = new System.Diagnostics.Process {
                    StartInfo = {
                        FileName = "/usr/local/bin/git",
                        Arguments = "rev-parse HEAD",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                process.WaitForExit();
                string commit = process.StandardOutput.ReadToEnd().TrimEnd();
                process.Close();
                return commit;
            }

        }

    }

}
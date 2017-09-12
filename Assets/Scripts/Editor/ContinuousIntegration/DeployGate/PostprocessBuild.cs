using System;
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

            public int callbackOrder {
                get {
                    return POSTPROCESS_BUILD_CALLBACK_ORDER;
                }
            }

            public void OnPostprocessBuild(BuildTarget target, string path) {
                Deploy(ResolveArchivePath(target, path));
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

        }

    }

}
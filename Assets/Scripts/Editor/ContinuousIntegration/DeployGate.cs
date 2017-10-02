using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityModule.Settings;

namespace ContinuousIntegration {

    // ReSharper disable once PartialTypeWithSinglePart
    public partial class DeployGate {

        /// <summary>
        /// DeployGate に配信します
        /// </summary>
        /// <param name="archivePath">配信対象のアーカイブファイルのパス</param>
        /// <param name="message">アップロードするファイルの説明</param>
        public static void Deploy(string archivePath, string message = null) {
            System.Diagnostics.Process process = new System.Diagnostics.Process {
                StartInfo = {
                    FileName = ResolveCommandPath(),
                    Arguments = GenerateArguments(archivePath, message),
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();
            process.Close();
        }

        /// <summary>
        /// dg コマンドへのパスを解決
        /// </summary>
        /// <remarks>相対パスを絶対パスに変換します</remarks>
        /// <remarks>チルダをホームディレクトリのパスに変換します</remarks>
        /// <returns>解決済みのパス</returns>
        /// <exception cref="ArgumentException"></exception>
        private static string ResolveCommandPath() {
            string commandPath = EnvironmentSetting.Instance.Path.CommandDeployGate;
            if (string.IsNullOrEmpty(commandPath)) {
                throw new ArgumentException("DeployGate コマンド (dg) のパスが設定されていません。");
            }
            return Path.GetFullPath(commandPath.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.Personal)));
        }

        /// <summary>
        /// dg コマンドへ渡す引数を生成する
        /// </summary>
        /// <param name="archivePath">配信対象のアーカイブファイルのパス</param>
        /// <param name="message">アップロードするファイルの説明</param>
        /// <returns>dg コマンドへ渡す引数</returns>
        private static string GenerateArguments(string archivePath, string message) {
            string arguments = string.Format("deploy \"{0}\"", archivePath);
            if (!string.IsNullOrEmpty(message)) {
                arguments += string.Format(" -m \"{0}\"", message);
            }
            return arguments;
        }

    }

}

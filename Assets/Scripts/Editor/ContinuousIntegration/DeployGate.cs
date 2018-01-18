using System;
using System.Diagnostics;
using System.IO;
using UnityModule.Settings;

namespace ContinuousIntegration {

    // ReSharper disable once PartialTypeWithSinglePart
    public partial class DeployGate {

        /// <summary>
        /// 環境変数キー: ユーザ
        /// </summary>
        private const string ENVIRONMENT_KEY_COMMAND_DEPLOY_GATE = "COMMAND_DEPLOY_GATE";

        /// <summary>
        /// DeployGate に配信します
        /// </summary>
        /// <param name="archivePath">配信対象のアーカイブファイルのパス</param>
        /// <param name="message">アップロードするファイルの説明</param>
        public static void Deploy(string archivePath, string message = null) {
            Process process = new Process {
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
                commandPath = Environment.GetEnvironmentVariable(ENVIRONMENT_KEY_COMMAND_DEPLOY_GATE);
            }
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

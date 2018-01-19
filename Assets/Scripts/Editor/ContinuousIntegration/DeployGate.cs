using System.Diagnostics;
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
                    FileName = EnvironmentSetting.Instance.Path.CommandDeployGate,
                    Arguments = GenerateArguments(archivePath, message),
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();
            process.Close();
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

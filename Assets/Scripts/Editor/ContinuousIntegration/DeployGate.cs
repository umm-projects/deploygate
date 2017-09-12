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
        public static void Deploy(string archivePath) {
            Debug.Log(ResolveCommandPath());
            Debug.Log(archivePath);
            System.Diagnostics.Process process = new System.Diagnostics.Process {
                StartInfo = {
                    FileName = ResolveCommandPath(),
                    Arguments = string.Format("deploy \"{0}\"", archivePath),
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

    }

}
using System;
using UnityEngine;

namespace UnityModule.Settings {

    // ReSharper disable once PartialTypeWithSinglePart
    public partial class EnvironmentSetting {

        // ReSharper disable once PartialTypeWithSinglePart
        public partial class EnvironmentSetting_Path {

            /// <summary>
            /// デフォルトの dg コマンドパス
            /// </summary>
            private const string DEFAULT_COMMAND_PATH_DEPLOY_GATE = "/usr/local/bin/dg";

            /// <summary>
            /// dg コマンドへのパスを保存している環境変数のキー
            /// </summary>
            private const string ENVIRONMENT_KEY_COMMAND_DEPLOY_GATE = "COMMAND_DEPLOY_GATE";

            /// <summary>
            /// dg コマンドのパスの実体
            /// </summary>
            [SerializeField]
            private string commandDeployGate;

            /// <summary>
            /// dg コマンドのパス
            /// </summary>
            public string CommandDeployGate {
                get {
                    if (string.IsNullOrEmpty(this.commandDeployGate)) {
                        this.commandDeployGate = Environment.GetEnvironmentVariable(ENVIRONMENT_KEY_COMMAND_DEPLOY_GATE);
                    }
                    if (string.IsNullOrEmpty(this.commandDeployGate)) {
                        this.commandDeployGate = DEFAULT_COMMAND_PATH_DEPLOY_GATE;
                    }
                    return this.commandDeployGate;
                }
            }

        }

        /// <summary>
        /// DeployGate に deploy するかどうかの実体
        /// </summary>
        [SerializeField]
        private bool shouldDeployToDeployGate = true;

        /// <summary>
        /// DeployGate に deploy するかどうか
        /// </summary>
        public bool ShouldDeployToDeployGate {
            get {
                return this.shouldDeployToDeployGate;
            }
            set {
                this.shouldDeployToDeployGate = value;
            }
        }

    }

}

using UnityEngine;

namespace UnityModule.Settings {

    // ReSharper disable once PartialTypeWithSinglePart
    public partial class EnvironmentSetting {

        // ReSharper disable once PartialTypeWithSinglePart
        public partial class EnvironmentSetting_Path {

            private const string DEFAULT_DEPLOY_GATE_COMMAND_PATH = "";

            /// <summary>
            /// dg コマンドのパスの実体
            /// </summary>
            [SerializeField]
            private string commandDeployGate = DEFAULT_DEPLOY_GATE_COMMAND_PATH;

            /// <summary>
            /// dg コマンドのパス
            /// </summary>
            public string CommandDeployGate {
                get {
                    return this.commandDeployGate;
                }
                set {
                    this.commandDeployGate = value;
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

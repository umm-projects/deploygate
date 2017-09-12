using System;
using System.Collections;
using System.Collections.Generic;
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

    }

}

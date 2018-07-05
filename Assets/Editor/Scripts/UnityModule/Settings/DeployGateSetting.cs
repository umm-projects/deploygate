using System;
using UnityEngine;

namespace UnityModule.Settings
{
    public class DeployGateSetting : Setting<DeployGateSetting>, IEnvironmentSetting
    {
        /// <summary>
        /// デフォルトの dg コマンドパス
        /// </summary>
        private const string DefaultCommandPathDeployGate = "/usr/local/bin/dg";

        /// <summary>
        /// dg コマンドへのパスを保存している環境変数のキー
        /// </summary>
        private const string EnvironmentKeyCommandDeployGate = "COMMAND_DEPLOY_GATE";

        /// <summary>
        /// dg コマンドのパスの実体
        /// </summary>
        [SerializeField] private string commandDeployGate = (
            !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(EnvironmentKeyCommandDeployGate))
                ? Environment.GetEnvironmentVariable(EnvironmentKeyCommandDeployGate)
                : DefaultCommandPathDeployGate
        );

        /// <summary>
        /// dg コマンドのパス
        /// </summary>
        public string CommandDeployGate => commandDeployGate;

        /// <summary>
        /// DeployGate に deploy するかどうかの実体
        /// </summary>
        [SerializeField] private bool shouldDeployToDeployGate = true;

        /// <summary>
        /// DeployGate に deploy するかどうか
        /// </summary>
        public bool ShouldDeployToDeployGate
        {
            get { return shouldDeployToDeployGate; }
            set { shouldDeployToDeployGate = value; }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Settings/DeployGate Setting")]
        public static void CreateSettingAsset()
        {
            CreateAsset();
        }
#endif
    }
}
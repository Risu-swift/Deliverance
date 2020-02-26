using UnityEngine;
using Zenject;

namespace GFG
{
    [CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Installers/GameSettingsInstaller")]
    public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
    {
        public GameManager.Settings Settings;

        public override void InstallBindings()
        {
            Container.BindInstance(Settings);
        }
    }
}

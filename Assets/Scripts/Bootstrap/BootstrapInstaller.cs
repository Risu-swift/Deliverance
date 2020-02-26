using Zenject;

namespace GFG
{
    public class BootstrapInstaller : MonoInstaller<BootstrapInstaller>
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

			Container.BindInterfacesAndSelfTo<GameManager>().AsSingle();
        }
    }
}

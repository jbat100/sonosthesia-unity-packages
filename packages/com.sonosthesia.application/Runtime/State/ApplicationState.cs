using UniRx;

namespace Sonosthesia.Application
{
    public enum ApplicationStateSwitchSelector
    {
        None,
        ActiveUI
    }
    
    public class ApplicationState
    {
        public BoolReactiveProperty activeUI { get; } = new (true);

        public BoolReactiveProperty Select(ApplicationStateSwitchSelector selector)
        {
            return selector switch
            {
                ApplicationStateSwitchSelector.ActiveUI => activeUI,
                _ => null
            };
        }
    }
}
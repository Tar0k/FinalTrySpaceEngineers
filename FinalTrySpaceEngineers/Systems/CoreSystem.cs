namespace IngameScript
{
    public class CoreSystem
    {
        internal class CoreSystem
        {
            private Program _program;
            public readonly SafetySystem SafetySystem;
            private readonly LightSystem _lightSystem;
            private readonly SoundSystem _soundSystem;

            public CoreSystem(Program program)
            {
                _program = program;
                SafetySystem = new SafetySystem(_program, safetyGroupName: "Signal lamp", activateSafeZone: true);
                _soundSystem = new SoundSystem(_program);
                _lightSystem = new LightSystem(_program);
            }

            public void Monitoring()
            {
                SafetySystem.Monitoring();
            
                if (SafetySystem.AlarmStatus)
                {
                    _lightSystem.AlarmOn();
                    _soundSystem.AlarmOn();
                }
                else
                {
                    _lightSystem.AlarmOff();
                    _soundSystem.AlarmOff();
                }
            }
        
        }
    }
}
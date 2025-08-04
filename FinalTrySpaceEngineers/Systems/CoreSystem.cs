namespace IngameScript
{
    internal class CoreSystem
    {
        private readonly SafetySystem _safetySystem;
        private readonly LightSystem _lightSystem;
        private readonly SoundSystem _soundSystem;

        public CoreSystem(Program program)
        {
            _safetySystem = new SafetySystem(program, safetyGroupName: "Signal lamp", activateSafeZone: true);
            _soundSystem = new SoundSystem(program);
            _lightSystem = new LightSystem(program);
        }

        public void Monitoring()
        {
            _safetySystem.Monitoring();
            
            if (_safetySystem.AlarmStatus)
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
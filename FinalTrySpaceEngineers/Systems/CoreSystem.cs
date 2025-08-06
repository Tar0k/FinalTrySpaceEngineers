using System;

namespace IngameScript
{
    public class CoreSystem
    {
        public event Action UpdateSystems;
        public event Action<AlarmMessage> AlarmTriggered;

        private readonly LogSystem _logSystem;

        public CoreSystem(Program program)
        {
            _logSystem = new LogSystem(program, this);
            _logSystem.SystemAlarmTriggered += OnSystemAlarmTriggered;
        }

        public void Update()
        {
            UpdateSystems?.Invoke();
        }

        private void OnSystemAlarmTriggered(AlarmMessage message)
        {
            _logSystem.WriteText(message);
        }
        
    }
}
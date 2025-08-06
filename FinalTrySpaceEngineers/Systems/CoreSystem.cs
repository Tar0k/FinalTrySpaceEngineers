using System;
using System.Collections.Generic;

namespace IngameScript
{
    public class CoreSystem : BaseSystem
    {
        public event Action UpdateSystems;
        public event Action<AlarmMessage> AlarmTriggered;

        private readonly LogSystem _logSystem;
        private readonly LightSystem _lightSystem;

        private readonly Dictionary<string, Action> _availableCommands;

        public CoreSystem(Program program)
        {
            _logSystem = new LogSystem(program, this);
            _logSystem.SystemAlarmTriggered += OnSystemAlarmTriggered;
            
            _lightSystem = new LightSystem(program, this);
            _lightSystem.SystemAlarmTriggered += OnSystemAlarmTriggered;

            _availableCommands = new Dictionary<string, Action>
            {
                { "lightSystem TurnOn", _lightSystem.TurnOn },
                { "lightSystem TurnOff", _lightSystem.TurnOff },
                { "lightSystem SwitchLight", _lightSystem.SwitchLight }
            };
        }

        public override void Update()
        {
            UpdateSystems?.Invoke();
        }

        public void ExecuteCommand(string command)
        {
            Action availableCommand;
            if (_availableCommands.TryGetValue(command, out availableCommand))
                availableCommand.Invoke();
            else
                _logSystem.WriteText(new AlarmMessage
                {
                    AlarmCode = "COMMAND_INFO",
                    Message = $"Введена неизвестная команда {command}",
                    System = this
                });
        }

        private void OnSystemAlarmTriggered(AlarmMessage message)
        {
            _logSystem.WriteText(message);
        }
        
    }
}
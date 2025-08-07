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
                { "lightSystem TurnOn", () =>
                    {
                        _lightSystem.LightState = LightStates.On;
                    }
                },
                { "lightSystem TurnOff", () =>
                    {
                        _lightSystem.LightState = LightStates.Off;
                    }
                },
                { "lightSystem SwitchLight", _lightSystem.SwitchLight },
                { "lightSystem Default", () => 
                    {
                        _lightSystem.LightState = LightStates.Default;
                    }
                },
                {
                    "lightSystem AlarmOn", () =>
                    {
                        _lightSystem.LightState = LightStates.Alarm;
                    }
                },
                { "lightSystem SwitchAlarm", _lightSystem.SwitchAlarm }
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
                    AlarmCode = AlarmCodes.CommandInfo,
                    Message = $"Введена неизвестная команда {command}",
                    System = this,
                    Type = MessageType.Warning,
                    IsActive = true
                });
        }

        private void OnSystemAlarmTriggered(AlarmMessage message)
        {
            if (message.Type == MessageType.Error && message.IsActive)
            {
                AlarmTriggered?.Invoke(message);
            }
        }
        
    }
}
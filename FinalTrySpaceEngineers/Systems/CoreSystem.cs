using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    public class CoreSystem : BaseSystem
    {
        public event Action UpdateSystems;
        public event Action<AlarmMessage> AlarmTriggered;

        private readonly LogSystem _logSystem;
        private readonly LightSystem _lightSystem;
        private readonly IEnumerable<BaseSystem> _systems;

        public CoreSystem(Program program)
        {
            _logSystem = new LogSystem(program, this);
            _logSystem.SystemAlarmTriggered += OnSystemAlarmTriggered;
            
            _lightSystem = new LightSystem(program, this, _logSystem);

            _systems = new List<BaseSystem>
            {
                _logSystem, _lightSystem
            };
        }

        public override IEnumerable<string> GetCommands()
        {
            return Enumerable.Empty<string>();
        }

        public override void Update()
        {
            UpdateSystems?.Invoke();
        }

        public override void ExecuteCommand(string command)
        {
            // Проверки полученной команды на формат
            var cmd = command.Split(' ');
            if (cmd.Length != 2)
            {
                _logSystem?.WriteText(new AlarmMessage
                {
                    AlarmCode = AlarmCodes.CommandInfo,
                    Message = $"Получена команда в неверном формате: \"{command}\"",
                    System = this,
                    Type = MessageType.Warning,
                    IsActive = true
                });
                return;
            }

            if (!_systems.Select(s => s.SystemName).Contains(cmd[0]))
            {
                _logSystem?.WriteText(new AlarmMessage
                {
                    AlarmCode = AlarmCodes.CommandInfo,
                    Message = $"Получена команда для неизвестной системы: \"{command}\"",
                    System = this,
                    Type = MessageType.Warning,
                    IsActive = true
                });
                return;
            }
            
            // Вызываем выполнение команды для конкретной системы
            _systems.First(s => s.SystemName == cmd[0]).ExecuteCommand(cmd[1]);
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
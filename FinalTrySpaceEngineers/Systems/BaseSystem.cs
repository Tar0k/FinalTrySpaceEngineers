using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    public abstract class BaseSystem
    {
        protected Dictionary<string, Action> AvailableCommands;
        protected ILogger Logger;
        
        protected BaseSystem()
        {
            AvailableCommands = new Dictionary<string, Action>();
            SystemName = GetType().Name;
        }

        // Название системы для отображения в UI
        public string SystemName { get; set; }

        // Ссылочное название системы в CustomData для определения принадлежности
        protected string RefCustomData { get; set; } = nameof(ToString);
        // Текущий статус системы
        public SystemStates SystemState { get; private set; }

        public virtual void ExecuteCommand(string command)
        {
            // Проверки полученной команды на формат
            var cmd = command.Split(' ');
            if (cmd.Length != 2)
            {
                Logger?.WriteText(new AlarmMessage
                {
                    AlarmCode = AlarmCodes.CommandInfo,
                    Message = $"Получена команда в неверном формате: \"{command}\"",
                    System = this,
                    Type = MessageType.Warning,
                    IsActive = true
                });
                return;
            }

            if (cmd[0] != SystemName)
            {
                Logger?.WriteText(new AlarmMessage
                {
                    AlarmCode = AlarmCodes.CommandInfo,
                    Message = $"Получена команда от другой системы: \"{command}\"",
                    System = this,
                    Type = MessageType.Warning,
                    IsActive = true
                });
                return;
            }
            
            // Исполнение команды
            Action availableCommand;
            if (AvailableCommands.TryGetValue(cmd[1], out availableCommand))
            {
                availableCommand.Invoke();
                Logger?.WriteText(new AlarmMessage
                {
                    AlarmCode = AlarmCodes.CommandInfo,
                    Message = $"Выполнена команда: \"{command}\"",
                    System = this,
                    Type = MessageType.Info,
                    IsActive = true
                });
            }
            else
                Logger?.WriteText(new AlarmMessage
                {
                    AlarmCode = AlarmCodes.CommandInfo,
                    Message = $"Введена неизвестная команда {command}",
                    System = this,
                    Type = MessageType.Warning,
                    IsActive = true
                });
        }

        public virtual IEnumerable<string> GetCommands()
        {
            return AvailableCommands.Keys.Select(k => $"{SystemName} {k}");
        }
        
        // Метод обновления данных в системе.
        public abstract void Update();

        public override string ToString()
        {
            return SystemName;
        }
    }
}
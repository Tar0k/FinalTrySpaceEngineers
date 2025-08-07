using System.Collections.Generic;

namespace IngameScript
{
    public abstract class BaseSystem
    {
        protected BaseSystem()
        {
            SystemName = GetType().Name;
        }

        // Название системы для отображения в UI
        public string SystemName { get; set; }

        // Ссылочное название системы в CustomData для определения принадлежности
        protected string RefCustomData { get; set; } = nameof(ToString);
        // Текущий статус системы
        public SystemStates SystemState { get; private set; }

        public abstract void ExecuteCommand(string command);
        public abstract IEnumerable<string> GetCommands();
        
        // Метод обновления данных в системе.
        public abstract void Update();

        public override string ToString()
        {
            return SystemName;
        }
    }
}
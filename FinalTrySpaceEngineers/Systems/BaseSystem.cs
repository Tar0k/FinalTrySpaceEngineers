using System.Collections.Generic;

namespace IngameScript
{
    public abstract class BaseSystem
    {
        public BaseSystem()
        {
            SystemName = GetType().Name;
        }

        // Название системы для отображения в UI
        protected string SystemName { get; set; }

        // Ссылочное название системы в CustomData для определения принадлежности
        protected string RefCustomData { get; set; } = nameof(ToString);
        // Текущий статус системы
        public SystemStates SystemState { get; private set; }

        
        // Метод обновления данных в системе.
        public abstract void Update();

        public override string ToString()
        {
            return SystemName;
        }
    }
}
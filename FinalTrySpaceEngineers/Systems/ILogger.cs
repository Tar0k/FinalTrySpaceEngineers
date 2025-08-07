namespace IngameScript
{
    public interface ILogger
    {
        /// <summary>
        /// Записывает в журнал тревогу
        /// </summary>
        /// <param name="message">Сообщение о тревоге</param>
        /// <returns>Результат выполнения</returns>
        bool WriteText(AlarmMessage message);
        
        /// <summary>
        /// Записывает в журнал тревогу
        /// </summary>
        /// <param name="text">Текст сообщения</param>
        /// <param name="system">Система источник</param>
        /// <param name="isActive">Активна на текущий момент</param>
        /// <returns>Результат выполнения</returns>
        bool WriteText(string text, BaseSystem system, bool? isActive);
    }
}
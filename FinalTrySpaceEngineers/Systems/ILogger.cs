namespace IngameScript
{
    public interface ILogger
    {
        void WriteText(AlarmMessage message);
        void WriteText(string text, BaseSystem system, bool? isActive);
    }
}
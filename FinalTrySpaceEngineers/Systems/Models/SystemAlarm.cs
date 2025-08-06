namespace IngameScript
{
    public class SystemAlarm
    {
        public string AlarmCode { get; set; }
        public string Message { get; set; }
        public BaseSystem System { get; set; }
        public MessageType Type { get; set; }
    }
}
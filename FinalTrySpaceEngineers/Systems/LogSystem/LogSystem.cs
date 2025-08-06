using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public class LogSystem : BaseSystem
    {
        private readonly List<IMyTextPanel> _logPanels;
        private readonly Queue<LogMessage> _logMessages = new Queue<LogMessage>();
        private readonly int _maxMessages;

        private readonly List<SystemAlarm> _alarms = new List<SystemAlarm>();
        
        public event Action<AlarmMessage> SystemAlarmTriggered;
        
        
        public LogSystem(Program program, CoreSystem core, int maxMessages = 200)
        {
            var panels = new List<IMyTextPanel>();
            program.GridTerminalSystem.GetBlocksOfType(panels);
            _logPanels = panels.Where(p => p.CustomData.Contains(RefCustomData)).ToList();
            
            ConfigurePanels();
            _maxMessages = maxMessages;

            core.UpdateSystems += Update;
        }

        public void WriteText(AlarmMessage message)
        {
            WriteText(text: message.Message, system: message.System, isActive: message.IsActive);
        }

        private void WriteText(string text, BaseSystem system, bool? isActive)
        {
            if (isActive.HasValue)
            {
                if (isActive == true)
                {
                    _logMessages.Enqueue(new LogMessage
                    {
                        Message = text,
                        System = system.ToString(),
                        OccurrenceTime = DateTime.Now,
                        EndTime = null
                    });
                }
                else
                {
                    var loggedMessage =
                        _logMessages.FirstOrDefault(m => m.Message == text && m.System == system.ToString());
                    if (loggedMessage != null) loggedMessage.EndTime = DateTime.Now;
                }
            }
            else
            {
                _logMessages.Enqueue(new LogMessage
                {
                    Message = text,
                    System = system.ToString(),
                });
            }
            
            if (_logPanels.Count > _maxMessages) _logMessages.Dequeue();
        }

        protected override void Update()
        {
            CheckSystemState();
            
            foreach (var textPanel in _logPanels)
            {
                var str = new StringBuilder();
                str.AppendLine("ЖУРНАЛ СООБЩЕНИЙ");
                str.AppendLine("----------------");
                str.AppendLine(string.Join(Environment.NewLine, _logMessages));
                textPanel.WriteText(str.ToString());
            }
        }

        private void ConfigurePanels()
        {
            foreach (var logPanel in _logPanels)
            {
                logPanel.FontSize = 10;
                logPanel.TextPadding = 3;
            }
        }

        // Проверяет состояние системы и информирует подписчиков об изменении
        private void CheckSystemState()
        {
            // Сохраняем тревоги с предыдущего цикла (до обработки)
            var previousAlarms = new List<SystemAlarm>(_alarms);
            
            // Проверка и обновление списка текущих тревог
            CheckAvailablePanels();
            
            // Вызов событий для обновлений в списке тревог
            InvokeNewAlarms(previousAlarms);
        }

        private void CheckAvailablePanels()
        {
            if (_logPanels.Count <= 0 && !_alarms.Select(a => a.AlarmCode).Contains("NoPanels"))
                _alarms.Add(new SystemAlarm
                {
                    Message = $"Не найдено панелей с системой \"{RefCustomData}\" в CustomData",
                    AlarmCode = "NoPanels",
                    System = this,
                    Type = MessageType.Info
                });
            else if (_alarms.Select(a => a.AlarmCode).Contains("NoPanels"))
                _alarms.RemoveAll(a => a.AlarmCode == "NoPanels");
        }

        private void InvokeNewAlarms(List<SystemAlarm> previousAlarms)
        {
            // Извещаем о новых тревогах
            foreach (var currentAlarm in _alarms
                         .Where(currentAlarm=> !previousAlarms
                             .Select(prevAlarm => prevAlarm.AlarmCode)
                             .Contains(currentAlarm.AlarmCode)))
            {
                SystemAlarmTriggered?.Invoke(FormAlarmMessage(currentAlarm, true));
            }

            // Извещаем об ушедших тревогах
            foreach (var previousAlarm in previousAlarms
                         .Where(previousAlarm => !_alarms
                             .Select(currentAlarm => currentAlarm.AlarmCode)
                             .Contains(previousAlarm.AlarmCode)))
            {
                SystemAlarmTriggered?.Invoke(FormAlarmMessage(previousAlarm, false));
            }
        }

        private AlarmMessage FormAlarmMessage(SystemAlarm alarm, bool isActive)
        {
            switch (alarm.AlarmCode)
            {
                case "NoPanels":
                    return new AlarmMessage(alarm, isActive);
                default:
                    return new AlarmMessage
                    {
                        AlarmCode = alarm.AlarmCode,
                        Message = $"Неизвестная ошибка в системе \"{SystemName}\"",
                        Type = MessageType.Warning,
                        System = this,
                        IsActive = isActive,
                    };
            }
        }
    }
}
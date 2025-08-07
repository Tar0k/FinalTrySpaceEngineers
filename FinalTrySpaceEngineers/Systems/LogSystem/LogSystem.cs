using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandbox.ModAPI.Ingame;
using VRage.Game.GUI.TextPanel;

namespace IngameScript
{
    public class LogSystem : BaseSystem, ILogger, IDisposable
    {
        private readonly CoreSystem _coreSystem;
        
        private readonly List<IMyTextPanel> _logPanels;
        private readonly Queue<LogMessage> _logMessages = new Queue<LogMessage>();
        private readonly int _maxMessages;
        private readonly List<SystemAlarm> _alarms = new List<SystemAlarm>();
        private bool _firstRun = true;
        
        public event Action<AlarmMessage> SystemAlarmTriggered;
        
        
        public LogSystem(Program program, CoreSystem core, int maxMessages = 200)
        {
            RefCustomData = "LogSystem";
            _maxMessages = maxMessages;
            var panels = new List<IMyTextPanel>();
            program.GridTerminalSystem.GetBlocksOfType(panels);
            _logPanels = panels.Where(p => p.CustomData.Contains(RefCustomData)).ToList();
            
            ConfigurePanels(_logPanels);
            _coreSystem = core;
            _coreSystem.UpdateSystems += Update;
        }

        public void WriteText(AlarmMessage message)
        {
            WriteText(text: message.Message, system: message.System, isActive: message.IsActive);
        }

        public void WriteText(string text, BaseSystem system, bool? isActive)
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
                    OccurrenceTime = DateTime.Now,
                });
            }
            
            if (_logPanels.Count > _maxMessages) _logMessages.Dequeue();
        }

        public override void ExecuteCommand(string command) {}

        public override IEnumerable<string> GetCommands()
        {
            return Enumerable.Empty<string>();
        }

        public override void Update()
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

        // Проверяет состояние системы и информирует подписчиков об изменении
        private void CheckSystemState()
        {
            // Сохраняем тревоги с предыдущего цикла (до обработки)
            var previousAlarms = new List<SystemAlarm>(_alarms);
            
            // Проверка при первом запуске
            if (_firstRun) CheckFirstRun();
            // Проверка и обновление списка текущих тревог
            CheckAvailablePanels();
            
            // Вызов событий для обновлений в списке тревог
            InvokeNewAlarms(previousAlarms);
        }

        private void CheckFirstRun()
        {
            if (_firstRun && _logPanels.Count > 0)
            {
                SystemAlarmTriggered?.Invoke(new AlarmMessage
                {
                    AlarmCode = AlarmCodes.StartupInfo,
                    Message = $"Инициализировано {_logPanels.Count} панелей",
                    System = this,
                    Type = MessageType.Warning,
                    IsActive = true
                });
            }
            _firstRun = false;
        }

        private void CheckAvailablePanels()
        {
            if (_logPanels.Count <= 0 && !_alarms.Select(a => a.AlarmCode).Contains(AlarmCodes.InitCount))
                _alarms.Add(new SystemAlarm
                {
                    Message = $"Не найдено панелей с системой \"{RefCustomData}\" в CustomData",
                    AlarmCode = AlarmCodes.InitCount,
                    System = this,
                    Type = MessageType.Warning
                });
            else if (_alarms.Select(a => a.AlarmCode).Contains(AlarmCodes.InitCount))
                _alarms.RemoveAll(a => a.AlarmCode == AlarmCodes.InitCount);
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
                case AlarmCodes.Unknown:
                    return new AlarmMessage
                    {
                        AlarmCode = alarm.AlarmCode,
                        Message = $"Неизвестная ошибка в системе \"{SystemName}\"",
                        Type = MessageType.Warning,
                        System = this,
                        IsActive = isActive,
                    };
                case AlarmCodes.OnOffInfo:
                case AlarmCodes.StartupInfo:
                case AlarmCodes.InitCount:
                case AlarmCodes.CommandInfo:
                default:
                    return new AlarmMessage(alarm, isActive);
            }
        }
        
        // Настройка параметров панелей
        private static void ConfigurePanels(List<IMyTextPanel> logPanels)
        {
            foreach (var logPanel in logPanels)
            {
                logPanel.ContentType = ContentType.TEXT_AND_IMAGE;
                logPanel.FontSize = 0.8f;
                logPanel.TextPadding = 3;
            }
        }

        public void Dispose()
        {
            _coreSystem.UpdateSystems -= Update;
        }
    }
}
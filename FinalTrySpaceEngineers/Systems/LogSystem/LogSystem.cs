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

        public LogSystem(Program program, CoreSystem core, int maxMessages = 200)
        {
            var panels = new List<IMyTextPanel>();
            program.GridTerminalSystem.GetBlocksOfType(panels);
            _logPanels = panels.Where(p => p.CustomData.Contains(RefCustomData)).ToList();
            
            ConfigurePanels();
            _maxMessages = maxMessages;

            core.UpdateSystems += Update;
        }
        
        public void WriteText(string text, BaseSystem system, bool? occured)
        {
            if (occured.HasValue)
            {
                if (occured == true)
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
    }
}
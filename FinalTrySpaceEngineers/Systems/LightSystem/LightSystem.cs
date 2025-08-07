using System;
using System.Collections.Generic;
using System.Linq;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRageMath;

namespace IngameScript
{
    public class LightSystem : BaseSystem, IDisposable
    {
        private readonly CoreSystem _coreSystem;
        private readonly ILogger _logger;
        
        private readonly List<IMyInteriorLight> _lights = new List<IMyInteriorLight>();
        private bool _firstRun = true;
        private bool _lightOn = true;
        
        public LightSystem(Program program, CoreSystem core)
        {
            program.GridTerminalSystem.GetBlocksOfType(_lights);
            _coreSystem = core;
            _coreSystem.UpdateSystems += Update;
            _coreSystem.AlarmTriggered += SwitchAlarm;
        }
        
        public LightSystem(Program program, CoreSystem core, ILogger logger) : this(program, core)
        {
            _logger = logger;
        }
        
        
        public LightStates LightState
        {
            get
            {
                if (_lights.All(l => l.Enabled))
                {
                    return LightStates.On;
                }
                if (_lights.All(l => !l.Enabled))
                {
                    return LightStates.Off;
                }
                return LightStates.Mixed;
            }
            set
            {
                switch (value)
                {
                    case LightStates.On:
                        TurnOn();
                        break;
                    case LightStates.Alarm:
                        SwitchAlarm();
                        break;
                    case LightStates.Default:
                        Default();
                        break;
                    case LightStates.Off:
                    case LightStates.Mixed:
                    default:
                        TurnOff();
                        break;
                }
            }
        }

        public event Action<AlarmMessage> SystemAlarmTriggered;

        //Переключатель света
        public void SwitchLight()
        {
            switch (LightState)
            {
                case LightStates.On:
                    TurnOff();
                    break;
                case LightStates.Off:
                    TurnOn();
                    break;
                case LightStates.Default:
                    Default();
                    break;
                case LightStates.Mixed:
                case LightStates.Alarm:
                default:
                    TurnOff();
                    break;
            }
        }

        // Включить свет
        public void TurnOn()
        {
            foreach (var light in  _lights)
            {
                light.Enabled = light.Enabled == false;
            }
            
            _lightOn = true;
            
            _logger?.WriteText(new AlarmMessage
            {
                AlarmCode = AlarmCodes.OnOffInfo,
                Message = "Свет включен",
                System = this,
                Type = MessageType.Info,
                IsActive = true
            });
        }

        // Выключить свет
        public void TurnOff()
        {
            foreach (var light in  _lights)
            {
                light.Enabled = light.Enabled != true;
            }

            _lightOn = false;

            _logger?.WriteText(new AlarmMessage
            {
                AlarmCode = AlarmCodes.OnOffInfo,
                Message = "Свет выключен",
                System = this,
                Type = MessageType.Info,
                IsActive = true
            });

        }

        // Переключить тревогу без условий
        public void SwitchAlarm()
        {
            if (!_lightOn) return;
            if (LightState == LightStates.Alarm ) AlarmOff();
            else AlarmOn();
        }

        // Переключить тревогу по сообщению
        private void SwitchAlarm(AlarmMessage alarm)
        {
            if (alarm.IsActive)
            {
                AlarmOn();
            }

            else
            {
                AlarmOff();
            }
                
        }
        
        private void AlarmOn()
        {
            if (!_lightOn) return;
            foreach (var light in _lights)
            {
                light.Color = Color.Red;
                light.BlinkLength = 3;
            }
            LightState = LightStates.Alarm;

        }

        private void AlarmOff()
        {
            if (_lightOn)
            {
                Default();
            }
        }

        private void Default()
        {
            foreach (var light in _lights)
            {
                light.Color = Color.White;
                light.BlinkLength = 0;
            }
            LightState = LightStates.Default;
        }

        public override void Update()
        {
            if (_firstRun)
                CheckFirstRun();
            CheckAvailableLights();
            
        }
        
        private void CheckFirstRun()
        {
            if (_firstRun && _lights.Count > 0)
            {
                _logger?.WriteText(new AlarmMessage
                {
                    AlarmCode = AlarmCodes.StartupInfo,
                    Message = $"Инициализировано {_lights.Count} источников света",
                    System = this,
                    Type = MessageType.Warning,
                    IsActive = true
                });
            }
            _firstRun = false;
        }

        private void CheckAvailableLights()
        {
            if (_lights.Count <= 0)
            {
                _logger?.WriteText(new AlarmMessage
                {
                    AlarmCode = AlarmCodes.InitCount,
                    Message = $"Инициализировано {_lights.Count} источников света",
                    System = this,
                    Type = MessageType.Warning,
                    IsActive = true
                });
            }
        }

        public void Dispose()
        {
            _coreSystem.UpdateSystems -= Update;
        }
    }
}
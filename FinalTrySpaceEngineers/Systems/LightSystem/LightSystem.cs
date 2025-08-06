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
        
        private readonly List<IMyInteriorLight> _lights = new List<IMyInteriorLight>();
        private bool _firstRun = true;
        private LightStates LightState
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
                    case LightStates.Off:
                    case LightStates.Mixed:
                    default:
                        TurnOff();
                        break;
                }
            }
        }

        public event Action<AlarmMessage> SystemAlarmTriggered;

        public LightSystem(Program program, CoreSystem core)
        {
            program.GridTerminalSystem.GetBlocksOfType(_lights);
            _coreSystem = core;
            _coreSystem.UpdateSystems += Update;
            Default();
        }

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
                case LightStates.Mixed:
                default:
                    TurnOff();
                    break;
            }
        }

        public void TurnOn()
        {
            foreach (var light in  _lights)
            {
                light.Enabled = light.Enabled == false;
            }

            SystemAlarmTriggered?.Invoke(new AlarmMessage
            {
                AlarmCode = "ON/OFF INFO",
                Message = "Свет включен",
                System = this,
                Type = MessageType.Info
            });
        }

        public void TurnOff()
        {
            foreach (var light in  _lights)
            {
                light.Enabled = light.Enabled != true;
            }
            
            SystemAlarmTriggered?.Invoke(new AlarmMessage
            {
                AlarmCode = "ON/OFF INFO",
                Message = "Свет выключен",
                System = this,
                Type = MessageType.Info
            });
        }

        public void AlarmOn()
        {
            foreach (var light in _lights)
            {
                light.Color = Color.Red;
                light.BlinkLength = 3;
            }
        }

        public void AlarmOff() => Default();

        private void Default()
        {
            foreach (var light in _lights)
            {
                light.Color = Color.White;
                light.BlinkLength = 0;
            }
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
                SystemAlarmTriggered?.Invoke(new AlarmMessage
                {
                    AlarmCode = "STARTUP INFO",
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
                SystemAlarmTriggered?.Invoke(new AlarmMessage
                {
                    AlarmCode = "LIGHTS COUNT",
                    Message = "Не удалось найти IMyInteriorLight объектов",
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
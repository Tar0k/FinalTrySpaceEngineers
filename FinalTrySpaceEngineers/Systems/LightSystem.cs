using System.Collections.Generic;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRageMath;

namespace IngameScript
{
    internal class LightSystem
    {
        private readonly List<IMyInteriorLight> _lights = new List<IMyInteriorLight>();

        public LightSystem(Program program)
        {
            program.GridTerminalSystem.GetBlocksOfType(_lights);
            Default();
        }

        public void TurnOn()
        {
            foreach (var light in  _lights)
            {
                light.Enabled = light.Enabled == false;
            }
        }

        public void TurnOff()
        {
            foreach (var light in  _lights)
            {
                light.Enabled = light.Enabled != true;
            }
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

    }
}
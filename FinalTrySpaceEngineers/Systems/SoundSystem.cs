namespace IngameScript
{
        internal class SoundSystem
        {
            private readonly Program _program;
            private readonly List<IMySoundBlock> _soundBlocks = new List<IMySoundBlock>();

            public SoundSystem(Program program)
            {
                _program = program;
                _program.GridTerminalSystem.GetBlocksOfType(_soundBlocks);
                Default();
            }
        
            public void TurnOn()
            {
                foreach (var soundBlock in  _soundBlocks)
                {
                    soundBlock.Enabled = soundBlock.Enabled == false;
                }
            }
        
            public void TurnOff()
            {
                foreach (var soundBlock in  _soundBlocks)
                {
                    soundBlock.Enabled = soundBlock.Enabled != true;
                }
            }

            private void Default()
            {
                foreach (var soundBlock in _soundBlocks)
                {
                    soundBlock.SelectedSound = null;
                }
            }
        


            public void AlarmOn()
            {
                foreach (var soundBlock in _soundBlocks)
                {
                    soundBlock.SelectedSound = "SoundBlockAlert1";
                    soundBlock.Play();
                }
            }

            public void AlarmOff() => Stop();

            private void Stop()
            {
                foreach (var soundBlock in _soundBlocks)
                {
                    soundBlock.Stop();
                    soundBlock.SelectedSound = null;
                }
            }

        }
    }
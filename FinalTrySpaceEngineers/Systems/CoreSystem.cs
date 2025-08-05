using System;

namespace IngameScript
{
    public class CoreSystem
    {
        public event Action UpdateSystems;

        private LogSystem _logSystem;

        public CoreSystem(Program program)
        {
            _logSystem = new LogSystem(program, this);
        }

        public void Update()
        {
            UpdateSystems?.Invoke();
        }
        
    }
}
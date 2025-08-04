namespace IngameScript
{
    internal class SafetySystem
    {
        private readonly Program _program;
        private readonly List<IMyTerminalBlock> _safetyBlocks = new List<IMyTerminalBlock>();
        private readonly List<IMyLargeTurretBase> _turrets = new List<IMyLargeTurretBase>();
        private readonly List<IMySafeZoneBlock> _safeZoneBlocks = new List<IMySafeZoneBlock>();
        private readonly List<IMyDoor> _doors = new List<IMyDoor>();
        private readonly List<SafeDoor> _safeDoors = new List<SafeDoor>();
        private bool _alarmPrevState;
        private DateTime _lastAlarm = new DateTime();
        private bool ActivateSafeZone { get; }
        private bool _safeZoneActivated;


        public SafetySystem(Program program, string safetyGroupName = "", bool allTurrets = true, bool activateSafeZone = false)
        {
            _program = program;
            ActivateSafeZone = activateSafeZone;
            
            if (safetyGroupName == string.Empty) return;

            var safeGroup = _program.GridTerminalSystem.GetBlockGroupWithName(safetyGroupName);
            safeGroup.GetBlocks(_safetyBlocks);
            safeGroup.GetBlocksOfType(_safeZoneBlocks);
            safeGroup.GetBlocksOfType(_doors);

            foreach (var door in _doors)
            {
                _safeDoors.Add(new SafeDoor(door));
            }
            


            if (allTurrets) _program.GridTerminalSystem.GetBlocksOfType(_turrets); // Со всей базы
            else _turrets = _safetyBlocks.OfType<IMyLargeTurretBase>().ToList(); // Только из safety группы
            
            Monitoring();
        }
        
        public bool AlarmStatus { get; private set; }
        public bool TestAlarm { get; set; }
        
        
        /// <summary>
        /// Срабатывает только при срабатывании тревоги на один цикл.
        /// </summary>
        public bool AlarmTriggeredNow { get; private set; }
        
        
        /// <summary>
        /// Циклическое обновление статусов.
        /// </summary>
        public void Monitoring()
        {
            AlarmStatus = CheckTurrets() || TestAlarm;
            AlarmTriggeredNow = AlarmStatus & _alarmPrevState == false;


            if (AlarmTriggeredNow)
            {
                _lastAlarm = DateTime.Now;
                if (ActivateSafeZone) SafeZoneOn();
            }
            
            if (_alarmPrevState & AlarmStatus == false) _lastAlarm = DateTime.Now;
            
            if (!AlarmStatus & _safeZoneActivated & DateTime.Now > _lastAlarm.AddSeconds(30))  SafeZoneOff();

            foreach (var safeDoor in _safeDoors)
            {
                safeDoor.Monitoring();
            }
            

            _alarmPrevState = AlarmStatus;
        }

        private void SafeZoneOn()
        {
            foreach (var safeZoneBlock in _safeZoneBlocks.Where(safeZoneBlock => !safeZoneBlock.Enabled))
            {
                safeZoneBlock.Enabled = true;
                if (!safeZoneBlock.GetValue<bool>("SafeZoneCreate")) safeZoneBlock.SetValue("SafeZoneCreate", true);

            }
            _safeZoneActivated = true;
        }

        private void SafeZoneOff()
        {
            foreach (var safeZoneBlock in _safeZoneBlocks.Where(safeZoneBlock => safeZoneBlock.Enabled))
            {
                if (safeZoneBlock.GetValue<bool>("SafeZoneCreate")) safeZoneBlock.SetValue("SafeZoneCreate", false);
                safeZoneBlock.Enabled = false;
            }
            _safeZoneActivated = false;
        }
        
        /// <summary>
        /// Проверяет нацелились турели на какие-либо цели.
        /// </summary>
        /// <returns>Найдена цель.</returns>
        private bool CheckTurrets() => _turrets.Any(turret => turret.IsAimed || turret.HasTarget);
    }
}
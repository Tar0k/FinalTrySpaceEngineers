namespace IngameScript
{
    internal class SafeDoor
    {
        private readonly IMyDoor _door;
        public int _openDoorTimer;
        public SafeDoor(IMyDoor door)
        {
            _door = door;
            Monitoring();
        }

        public void Monitoring()
        {
            switch (_door.Status)
            {
                case DoorStatus.Open:
                    _openDoorTimer += 1;
                    break;
                case DoorStatus.Closed:
                    _openDoorTimer = 0;
                    break;
            }

            if (_openDoorTimer > 1)
            {
                _openDoorTimer = 0;
                _door.CloseDoor();
            }
        }
    }
}
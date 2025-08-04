using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    internal class SafeDoor
    {
        private readonly IMyDoor _door;
        private int _openDoorTimer;
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
                case DoorStatus.Opening:
                case DoorStatus.Closing:
                default:
                    break;
            }

            if (_openDoorTimer <= 1) return;
            _openDoorTimer = 0;
            _door.CloseDoor();
        }
    }
}
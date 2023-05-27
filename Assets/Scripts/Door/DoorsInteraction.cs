using Game;

namespace Door
{
    public static class DoorsInteraction
    {
        public static bool ChangeDoorState(MyDoorController door)
        {
            if (!GameInputs.Instance.Interact || door.AnimatorIsPlaying) return false;
            
            GameInputs.Instance.ResetInteractInput();
            door.SetDoorState(!door.isOpen.Value);
            return true;
        }
    }
}

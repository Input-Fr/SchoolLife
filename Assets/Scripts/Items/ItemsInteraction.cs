using Game;

namespace Items
{
    public static class ItemsInteraction
    {
        public static bool PickUp(Item item)
        {
            if (!GameInputs.Instance.Interact) return false;
            GameInputs.Instance.ResetInteractInput();
            
            return item.Pickup();
        }
    }
}

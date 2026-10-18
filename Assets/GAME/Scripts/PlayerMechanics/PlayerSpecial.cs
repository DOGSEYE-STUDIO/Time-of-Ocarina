using UnityEngine;

public class PlayerSpecial : MonoBehaviour
{
    public SpecialItem EquippedItem;
    private bool isSpecialButtonHeld;

    public void UseSpecial(Vector2 facingDirection)
    {
        if (EquippedItem == null) return;

        EquippedItem.SetButtonHeld(isSpecialButtonHeld);
        EquippedItem.Use(facingDirection);
    }

    public void EquipSpecial(SpecialItem newItem)
    {
        EquippedItem = newItem;
    }

    public void UnequipSpecial()
    {
        EquippedItem = null;
    }

    public bool HasSpecialEquipped()
    {
        return EquippedItem != null;
    }

    public void SetSpecialButtonHeld(bool held)
    {
        isSpecialButtonHeld = held;

        if (EquippedItem != null) EquippedItem.SetButtonHeld(held);
    }
}

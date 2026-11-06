using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hotbar : MonoBehaviour
{
    private const int SlotCount = 4;

    private APickable[] _consumables = new APickable[SlotCount - 1];

    public event Action OnChanged;

    public int Selected { get; private set; }

    public APickable GetConsumable(int slot)
    {
        return _consumables[slot - 1];
    }

    private void Update()
    {
        int previous = Selected;
        Keyboard keyboard = Keyboard.current;
        if (keyboard.digit1Key.wasPressedThisFrame) Selected = 0;
        if (keyboard.digit2Key.wasPressedThisFrame) Selected = 1;
        if (keyboard.digit3Key.wasPressedThisFrame) Selected = 2;
        if (keyboard.digit4Key.wasPressedThisFrame) Selected = 3;

        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll > 0) Selected = (Selected + SlotCount - 1) % SlotCount;
        if (scroll < 0) Selected = (Selected + 1) % SlotCount;

        if (Selected != previous) OnChanged();
    }

    public bool AddItem(APickable item)
    {
        for (int i = 0; i < _consumables.Length; i++)
        {
            if (_consumables[i] == null)
            {
                _consumables[i] = item;
                OnChanged();
                return true;
            }
        }

        return false;
    }

    public void UseSelected()
    {
        APickable item = _consumables[Selected - 1];
        if (item == null) return;

        APickable used = Instantiate(item, transform.position, Quaternion.identity);
        used.OnPickup();
        Destroy(used.gameObject);
        _consumables[Selected - 1] = null;
        OnChanged();
    }
}

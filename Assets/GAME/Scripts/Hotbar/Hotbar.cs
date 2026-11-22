using System;
using MelenitasDev.SoundsGood;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hotbar : MonoBehaviour
{
    private const int SlotCount = 4;

    private APickable[] _consumables = new APickable[SlotCount - 1];

    public event Action OnChanged;

    public int Selected { get; private set; }
    public bool IsOpen { get; private set; }

    public APickable GetConsumable(int slot)
    {
        return _consumables[slot - 1];
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;
        Gamepad mando = Gamepad.current;

        bool abrir = (keyboard != null && keyboard.tabKey.wasPressedThisFrame) || (mando != null && mando.buttonNorth.wasPressedThisFrame);

        if (abrir)
        {
            IsOpen = !IsOpen;
            if (!IsOpen) Selected = 0;
            new Sound(IsOpen ? SFX.HotbarAbrir : SFX.HotbarCerrar).SetOutput(Output.SFX).Play();
            Avisar();
        }

        if (!IsOpen) return;

        int previous = Selected;

        if (keyboard != null)
        {
            if (keyboard.digit1Key.wasPressedThisFrame) Selected = 0;
            if (keyboard.digit2Key.wasPressedThisFrame) Selected = 1;
            if (keyboard.digit3Key.wasPressedThisFrame) Selected = 2;
            if (keyboard.digit4Key.wasPressedThisFrame) Selected = 3;
        }

        bool anterior = (mouse != null && mouse.scroll.ReadValue().y > 0) || (mando != null && mando.leftShoulder.wasPressedThisFrame);
        bool siguiente = (mouse != null && mouse.scroll.ReadValue().y < 0) || (mando != null && mando.rightShoulder.wasPressedThisFrame);

        if (anterior) Selected = (Selected + SlotCount - 1) % SlotCount;
        if (siguiente) Selected = (Selected + 1) % SlotCount;

        if (Selected != previous) Avisar();
    }

    private void Avisar()
    {
        if (OnChanged != null) OnChanged();
    }

    public bool AddItem(APickable item)
    {
        for (int i = 0; i < _consumables.Length; i++)
        {
            if (_consumables[i] == null)
            {
                _consumables[i] = item;
                Avisar();
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
        Avisar();
    }
}

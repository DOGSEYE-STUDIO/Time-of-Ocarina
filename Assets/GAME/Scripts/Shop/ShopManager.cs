using MelenitasDev.SoundsGood;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private ShopItem[] _itemPrefabs;
    [SerializeField] private Transform[] _slots;
    [SerializeField] private SpriteRenderer _heldIcon;

    private ShopItem[] _items;

    public ShopItem Held { get; private set; }

    private void Start()
    {
        int count = Mathf.Min(_itemPrefabs.Length, _slots.Length);
        _items = new ShopItem[count];

        for (int i = 0; i < count; i++)
        {
            _items[i] = Instantiate(_itemPrefabs[i], _slots[i].position, Quaternion.identity);
            _items[i].OnTaken += Hold;
        }
    }

    private void OnDestroy()
    {
        if (_items == null) return;

        for (int i = 0; i < _items.Length; i++)
        {
            if (_items[i] != null) _items[i].OnTaken -= Hold;
        }
    }

    private void Hold(ShopItem item)
    {
        ReturnHeld();

        new Sound(SFX.ShopPick).SetOutput(Output.SFX).Play();
        Held = item;
        item.gameObject.SetActive(false);
        _heldIcon.sprite = item.Sprite;
        _heldIcon.enabled = true;
    }

    public void ReturnHeld()
    {
        if (Held == null) return;

        Held.gameObject.SetActive(true);
        Held = null;
        _heldIcon.enabled = false;
    }

    public bool Buy()
    {
        if (Held == null) return false;
        if (!PlayerInventory.Instance.HasEnoughCoins(Held.Price)) return false;

        if (!PlayerController.Instance.GetComponent<Hotbar>().AddItem(Held.Reward)) return false;

        PlayerInventory.Instance.RemoveCoins(Held.Price);

        Held = null;
        _heldIcon.enabled = false;
        return true;
    }
}

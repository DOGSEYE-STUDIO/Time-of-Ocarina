using System;
using TMPro;
using UnityEngine;

public class ShopItem : APlayerInteractable
{
    [SerializeField] private int _price = 10;
    [SerializeField] private APickable _reward;
    [SerializeField] private TMP_Text _priceText;

    public event Action<ShopItem> OnTaken;

    public int Price { get { return _price; } }
    public APickable Reward { get { return _reward; } }
    public Sprite Sprite { get { return GetComponent<SpriteRenderer>().sprite; } }

    public override string ActionText { get { return "Coger"; } }

    private void Start()
    {
        _priceText.text = _price.ToString();
    }

    public override void OnPlayerInteract()
    {
        if (OnTaken != null) OnTaken(this);
    }
}

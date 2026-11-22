using System;
using MelenitasDev.SoundsGood;
using UnityEngine;

public class Coin : APickable
{
    [SerializeField] private int _amount = 1;
    [SerializeField] private GameObject _destello;

    public override void OnPickup()
    {
        PlayerInventory.Instance.AddCoins(_amount);
        new Sound(SFX.coin).SetOutput(Output.SFX).Play();
        if (_destello != null) Instantiate(_destello, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag != "Player") return;
        
        OnPickup();
    }
}

using System;
using UnityEngine;

public class Coin : APickable
{
    [SerializeField] private int _amount = 1;

    public override void OnPickup()
    {
        PlayerInventory.Instance.AddCoins(_amount);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag != "Player") return;
        
        OnPickup();
    }
}

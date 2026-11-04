using UnityEngine;

public class HeartPickup : APickable
{
    [SerializeField] private int _amount = 2;

    public override void OnPickup()
    {
        PlayerController.Instance.GetComponent<PlayerHealth>().Heal(_amount);
    }
}

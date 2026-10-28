using UnityEngine;

public class Key : APickable
{
    [SerializeField] private bool isBossKey = false;

    public override void OnPickup()
    {
        if (!isBossKey)
        {
            PlayerInventory.Instance.AddKey();
        }
        else
        {
            PlayerInventory.Instance.AddBossKey();
        }
    }
}

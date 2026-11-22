using UnityEngine;

public class FrisbeePickup : APickable
{
    [SerializeField] private bool mejorado;

    public override void OnPickup()
    {
        FrisbeeItem frisbee = PlayerController.Instance.GetComponentInChildren<FrisbeeItem>(true);

        PlayerController.Instance.GetComponentInChildren<PlayerSpecial>().EquipSpecial(frisbee);
        if (mejorado) frisbee.Mejorar();
    }
}

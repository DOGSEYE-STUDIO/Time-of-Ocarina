using UnityEngine;

public class EnemyFrisbeeHit : AFrisbeeInteractable
{
    private EnemyBase _enemigo;

    private void Awake()
    {
        _enemigo = GetComponent<EnemyBase>();
    }

    public override void OnFrisbeeHit()
    {
        _enemigo.Aturdir();
    }
}

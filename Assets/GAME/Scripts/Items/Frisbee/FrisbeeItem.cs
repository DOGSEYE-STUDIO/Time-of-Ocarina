
using MelenitasDev.SoundsGood;
using UnityEngine;

public class FrisbeeItem : SpecialItem
{
    [SerializeField] private GameObject frisbeePrefab;
    [SerializeField] private GameObject frisbeeMejoradoPrefab;
    [SerializeField] private Transform spawnPoint;

    private bool isButtonHeld;
    [SerializeField] private bool mejorado;
    private GameObject activeFrisbee;

    public void Mejorar()
    {
        mejorado = true;
    }

    public override void Use(Vector2 facingDirection)
    {
        if (activeFrisbee != null) return;
        if (frisbeePrefab == null || spawnPoint == null) return;
        if (PlayerController.Instance == null) return;

        new Sound(SFX.FrisbeeLanzar).SetOutput(Output.SFX).SetRandomPitch().Play();

        activeFrisbee = Instantiate(mejorado ? frisbeeMejoradoPrefab : frisbeePrefab, spawnPoint.position, Quaternion.identity);

        FrisbeeProyectile projectile = activeFrisbee.GetComponent<FrisbeeProyectile>();

        if (projectile == null)
        {
            Destroy(activeFrisbee);
            activeFrisbee = null;
            return;
        }

        projectile.OnReturned += () => activeFrisbee = null;
        projectile.SetButtonHeld(isButtonHeld);
        projectile.Launch(facingDirection, PlayerController.Instance.transform);
    }

    public override void SetButtonHeld(bool held)
    {
        isButtonHeld = held;

        if (activeFrisbee == null) return;

        FrisbeeProyectile projectile = activeFrisbee.GetComponent<FrisbeeProyectile>();

        if (projectile != null) projectile.SetButtonHeld(held);
    }
}

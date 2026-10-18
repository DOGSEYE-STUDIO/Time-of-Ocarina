
using UnityEngine;

public class FrisbeeItem : SpecialItem
{
    [SerializeField] private GameObject frisbeePrefab;
    [SerializeField] private Transform spawnPoint;

    private bool isButtonHeld;
    private GameObject activeFrisbee;

    public override void Use(Vector2 facingDirection)
    {
        if (activeFrisbee != null) return;
        if (frisbeePrefab == null || spawnPoint == null) return;
        if (PlayerController.Instance == null) return;

        AudioSource audio = GetComponent<AudioSource>();
        audio.Play();

        activeFrisbee = Instantiate( frisbeePrefab, spawnPoint.position, Quaternion.identity);

        FrisbeeProyectile projectile = activeFrisbee.GetComponent<FrisbeeProyectile>();

        if (projectile == null)
        {
            Destroy(activeFrisbee);
            activeFrisbee = null;
            return;
        }

        projectile.OnReturned += () => activeFrisbee = null;
        projectile.SetButtonHeld(isButtonHeld);
        projectile.Launch( facingDirection, PlayerController.Instance.transform;
    }

    public override void SetButtonHeld(bool held)
    {
        isButtonHeld = held;

        if (activeFrisbee == null) return;

        FrisbeeProyectile projectile = activeFrisbee.GetComponent<FrisbeeProyectile>();

        if (projectile != null) projectile.SetButtonHeld(held);
    }
}

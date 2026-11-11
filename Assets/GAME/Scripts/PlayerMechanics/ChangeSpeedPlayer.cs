using UnityEngine;

public class ChangeSpeedPlayer : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 0.5f;

    private float previousSpeed;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        previousSpeed = PlayerController.Instance.GetPlayerSpeed();

        PlayerController.Instance.SetPlayerSpeed( previousSpeed * speedMultiplier);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController.Instance.SetPlayerSpeed(previousSpeed);
    }
}
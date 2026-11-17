using DG.Tweening;
using UnityEngine;

public class Pit : MonoBehaviour
{
    [SerializeField] private PitRespawn respawn;
    [SerializeField] private GameObject movePos;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        other.gameObject.transform.DOMove(movePos.transform.position, 0.3f);

        PlayerController.Instance.StopPlayer();
        PlayerController.Instance.PlayAnimation("Falling", () => Respawn(other));
    }

    private void Respawn(Collider2D other)
    {
        other.transform.position = respawn.respawnPosition; 
        PlayerController.Instance.ResumePlayer();
    }
}

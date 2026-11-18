using DG.Tweening;
using UnityEngine;

public class Pit : MonoBehaviour
{
    [SerializeField] private PitRespawn respawn;
    [SerializeField] private GameObject movePos;

    [SerializeField] private bool alignX = true;
    [SerializeField] private bool alignY = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (alignX) other.gameObject.transform.DOMoveX(movePos.transform.position.x, 0.3f);
        if (alignY) other.gameObject.transform.DOMoveY(movePos.transform.position.y, 0.3f);

        PlayerController.Instance.StopPlayer();
        PlayerController.Instance.PlayAnimation("Falling", () => Respawn(other));
    }

    private void Respawn(Collider2D other)
    {
        other.transform.position = respawn.respawnPosition; 
        PlayerController.Instance.ResumePlayer();
    }
}

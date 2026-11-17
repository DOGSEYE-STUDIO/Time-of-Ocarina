using DG.Tweening;
using UnityEngine;

public class Pit : MonoBehaviour
{
    [SerializeField] private PitRespawn respawn;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerController.Instance.StopPlayer();

        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(0.5f);

        //Animación

        seq.AppendCallback(() => other.transform.position = respawn.respawnPosition);
        seq.AppendCallback(Respawn);
    }

    private void Respawn()
    {
        PlayerController.Instance.ResumePlayer();
    }
}

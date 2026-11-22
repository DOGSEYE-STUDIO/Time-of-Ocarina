using DG.Tweening;
using MelenitasDev.SoundsGood;
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

        new Sound(SFX.Caida).SetOutput(Output.SFX).Play();
        PlayerController.Instance.StopPlayer();
        PlayerController.Instance.PlayAnimation("Falling", () => Respawn(other));
    }

    private void Respawn(Collider2D other)
    {
        other.transform.position = respawn.respawnPosition;
        PlayerController.Instance.ResumePlayer();

        SpriteRenderer sprite = other.GetComponentInChildren<SpriteRenderer>();
        sprite.DOKill(true);
        sprite.DOFade(0.2f, 0.08f).SetLoops(6, LoopType.Yoyo);
    }
}

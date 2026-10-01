using DG.Tweening;
using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    private Sequence hitboxSequence;

    public float hitboxOffset = 0.3f;

    public bool DebugMode;
    public Transform debugSprite;


    public void Start()
    {
        debugSprite.gameObject.SetActive(DebugMode);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ASwordInteractable interactable = other.GetComponent<ASwordInteractable>();

        if (interactable != null)
        {
            interactable.OnSwordHit();
        }
    }

    public void HitboxMove(float duration)
    {
        hitboxSequence?.Kill();
        hitboxSequence = DOTween.Sequence();

        hitboxSequence.Append(transform.DOLocalMoveX(-hitboxOffset, 0));
        hitboxSequence.Append(transform.DOLocalMoveX(hitboxOffset, duration));
    }
}

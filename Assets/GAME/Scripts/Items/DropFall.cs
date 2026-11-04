using DG.Tweening;
using UnityEngine;

public class DropFall : MonoBehaviour
{
    [SerializeField] private Transform _sprite;
    [SerializeField] private float _jumpHeight = 1f;
    [SerializeField] private float _jumpTime = 0.25f;

    public void Play()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.enabled = false;

        float h = _jumpHeight;
        float t = _jumpTime;

        DOTween.Sequence()
            .Append(_sprite.DOLocalMoveY(h, t).SetEase(Ease.OutQuad))
            .Append(_sprite.DOLocalMoveY(0f, t).SetEase(Ease.InQuad))
            .Append(_sprite.DOLocalMoveY(h * 0.35f, t * 0.6f).SetEase(Ease.OutQuad))
            .Append(_sprite.DOLocalMoveY(0f, t * 0.6f).SetEase(Ease.InQuad))
            .Append(_sprite.DOLocalMoveY(h * 0.15f, t * 0.4f).SetEase(Ease.OutQuad))
            .Append(_sprite.DOLocalMoveY(0f, t * 0.4f).SetEase(Ease.InQuad))
            .AppendCallback(() => col.enabled = true)
            .SetLink(gameObject);
    }
}

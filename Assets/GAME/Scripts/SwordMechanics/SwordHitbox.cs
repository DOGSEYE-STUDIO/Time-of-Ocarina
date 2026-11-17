using DG.Tweening;
using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    private Sequence hitboxSequence;

    [SerializeField] private float forwardOffset = 1f;
    [SerializeField] private float hitboxOffset = 0.5f;

    public bool DebugMode;
    public Transform debugSprite;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Awake()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    private void Start()
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

    public void HitboxMove(float duration, Vector2 direction)
    {
        hitboxSequence?.Kill();

        direction.Normalize();

        Vector2 perpendicular = new Vector2(-direction.y, direction.x);

        Vector3 centerPosition = initialPosition + (Vector3)(direction * forwardOffset);

        Vector3 startPosition = centerPosition - (Vector3)(perpendicular * hitboxOffset);
        Vector3 endPosition = centerPosition + (Vector3)(perpendicular * hitboxOffset);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        transform.localPosition = startPosition;

        hitboxSequence = DOTween.Sequence();

        hitboxSequence.Append(
            transform.DOLocalMove(endPosition, duration).SetEase(Ease.Linear)
        );

        hitboxSequence.OnComplete(() =>
        {
            transform.localPosition = initialPosition;
            transform.localRotation = initialRotation;
        });
    }

    private void OnDisable()
    {
        hitboxSequence?.Kill();
    }
}
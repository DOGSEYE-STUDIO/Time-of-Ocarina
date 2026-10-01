using DG.Tweening;
using System.Globalization;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject sword;
    [SerializeField] private SwordHitbox hitbox;

    private float attackDuration = 0.25f;

    private Sequence attackSequence;
    private void Start()
    {
        sword.SetActive(false);
    }

    public void Attack(Vector2 attackDirection)
    {
        sword.SetActive(true);

        //Temporal hasta tener la animación
        float angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
        sword.transform.localRotation = Quaternion.Euler(0, 0, angle + 90f);

        attackSequence?.Kill();

        attackSequence = DOTween.Sequence();
        attackSequence.AppendCallback(() =>
        {
            PlayerController.Instance.StopPlayerMovement();
            hitbox.HitboxMove(attackDuration);
        });
        attackSequence.Append(sword.transform.DOLocalMove(attackDirection, 0));
        attackSequence.AppendInterval(attackDuration);
        attackSequence.AppendCallback(() => 
        {
            sword.SetActive(false); 
            PlayerController.Instance.ResumePlayerMovement();
        });
    }
}

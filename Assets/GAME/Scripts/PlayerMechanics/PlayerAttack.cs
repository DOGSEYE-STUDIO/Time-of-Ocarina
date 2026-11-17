using DG.Tweening;
using MelenitasDev.SoundsGood;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject sword;
    [SerializeField] private SwordHitbox hitbox;

    private float attackDuration = 0.21f;

    private void Start()
    {
        sword.SetActive(false);
    }

    public void Attack(Vector2 attackDirection)
    {
        new Sound(SFX.SwordSwing).SetOutput(Output.SFX).Play();

        sword.SetActive(true);

        PlayerController.Instance.PlayAnimation("Attack_" + GetDirectionName(PlayerController.Instance.GetFacingDirection()), () => {
            PlayerController.Instance.ResumePlayerMovement();
            sword.SetActive(false);
            });
        PlayerController.Instance.StopPlayerMovement();
        hitbox.HitboxMove(attackDuration, attackDirection);
    }

    private string GetDirectionName(Vector2 facing)
    {
        if (facing == Vector2.up) return "Up";
        if (facing == Vector2.down) return "Down";
        if (facing == Vector2.left) return "Left";

        return "Right";
    }
}

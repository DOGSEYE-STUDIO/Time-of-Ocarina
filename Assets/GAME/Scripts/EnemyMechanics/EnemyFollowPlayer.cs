using UnityEngine;

public class EnemyFollowPlayer : EnemyBase
{
    [SerializeField] private float _detectionRange = 4f;
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _stunDuration = 0.3f;
    private float _stunTimer;

    private void FixedUpdate()
    {
        // Aturdido por el knockback
        if (_stunTimer > 0)
        {
            _stunTimer -= Time.fixedDeltaTime;
            return;
        }

        Vector2 playerPos = PlayerController.Instance.transform.position;
        float distance = Vector2.Distance(transform.position, playerPos);

        if (distance < _detectionRange)
        {
            // Dirección desde el enemigo hacia el jugador
            Vector2 direction = (playerPos - (Vector2)transform.position).normalized;
            _rb.linearVelocity = direction * _moveSpeed;
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
        }
    }

    public override void OnSwordHit()
    {
        base.OnSwordHit(); // vida, knockback, feedbacks y sonido de EnemyBase
        _stunTimer = _stunDuration;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }
}

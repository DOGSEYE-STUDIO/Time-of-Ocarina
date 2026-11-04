using MelenitasDev.SoundsGood;
using MoreMountains.Feedbacks;
using UnityEngine;

[RequireComponent(typeof(ItemDrop))]
public class EnemyBase : ASwordInteractable
{
    [SerializeField] private int _health = 100;
    [SerializeField] private int _damagePerHit = 10;
    [SerializeField] private float _knockbackForce = 5f;
    [SerializeField] private int _contactDamage = 1;
    protected Rigidbody2D _rb;

    //Feedbacks
    [SerializeField] private MMF_Player particlesFeedback;  
    [SerializeField] private MMF_Player cameraFeedback;
    [SerializeField] private SoundEmitter _sfxPlayer;
    
    
    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public override void OnSwordHit()
    {
        // Vida menos
        _health -= _damagePerHit;

        if (_health <= 0)
        {
            Die();
        }

        // Knockback: dirección desde el jugador hacia el enemigo
        Vector2 direction = (transform.position - PlayerController.Instance.transform.position).normalized;
        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(direction * _knockbackForce, ForceMode2D.Impulse);

        //Feedback camara y particulas
        particlesFeedback.PlayFeedbacks();
        cameraFeedback.PlayFeedbacks();

        // Sonido
        _sfxPlayer.Play();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();

        if (player != null) player.TakeDamage(_contactDamage, transform.position);
    }

    public void Die()
    {
        // Muere
        GetComponent<ItemDrop>().Drop();
        Destroy(gameObject);
    }
    
    
}

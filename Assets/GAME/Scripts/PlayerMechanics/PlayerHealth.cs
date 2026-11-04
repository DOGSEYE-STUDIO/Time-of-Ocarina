using DG.Tweening;
using MelenitasDev.SoundsGood;
using MoreMountains.Feedbacks;
using UnityEngine;

[DefaultExecutionOrder(100)]
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 6;
    [SerializeField] private float _invulnerableTime = 1f;
    [SerializeField] private float _knockbackSpeed = 10f;
    [SerializeField] private float _knockbackTime = 0.12f;
    [SerializeField] private MMF_Player _hitFeedback;

    private Rigidbody2D _rb;
    private Vector2 _knockbackDirection;
    private float _knockbackTimer;
    private float _invulnerableUntil;

    public int CurrentHealth { get; private set; }
    public int MaxHealth { get { return _maxHealth; } }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        CurrentHealth = _maxHealth;
    }

    private void FixedUpdate()
    {
        if (_knockbackTimer <= 0) return;

        _knockbackTimer -= Time.fixedDeltaTime;
        _rb.MovePosition(_rb.position + _knockbackDirection * _knockbackSpeed * Time.fixedDeltaTime);
    }

    public void TakeDamage(int damage, Vector2 sourcePosition)
    {
        if (CurrentHealth <= 0 || Time.time < _invulnerableUntil) return;

        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            GameOverMenu.Instance.Show();
            return;
        }

        _invulnerableUntil = Time.time + _invulnerableTime;

        if (!DOTween.IsTweening(_rb))
        {
            _knockbackDirection = (_rb.position - sourcePosition).normalized;
            _knockbackTimer = _knockbackTime;
        }

        new Sound(SFX.hit).SetOutput(Output.SFX).Play();
        if (_hitFeedback != null) _hitFeedback.PlayFeedbacks();
    }

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, _maxHealth);
    }
}

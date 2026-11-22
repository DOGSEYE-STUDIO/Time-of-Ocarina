using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour
{
    [SerializeField] private Image[] _hearts;
    [SerializeField] private Sprite[] _idleHeartFrames;
    [SerializeField] private Sprite[] _deadHeartFrames;
    [SerializeField] private Sprite _emptyHeart;
    [SerializeField] private float _heartFps = 12f;
    [SerializeField] private TMP_Text _coinsText;
    [SerializeField] private TMP_Text _keysText;
    [SerializeField] private GameObject _bossKeyIcon;
    [SerializeField] private TMP_Text _actionText;
    [SerializeField] private Image _actionIcon;
    [SerializeField] private Sprite _attackIcon;
    [SerializeField] private Sprite _interactIcon;

    private PlayerHealth _health;
    private Hotbar _hotbar;
    private bool[] _wasFull;
    private float[] _deadTime;
    private int _monedasAntes;

    private void Start()
    {
        _health = PlayerController.Instance.GetComponent<PlayerHealth>();
        _hotbar = PlayerController.Instance.GetComponent<Hotbar>();

        _wasFull = new bool[_hearts.Length];
        _deadTime = new float[_hearts.Length];
        for (int i = 0; i < _hearts.Length; i++)
        {
            _wasFull[i] = i < _health.CurrentHealth;
            _deadTime[i] = -1f;
        }

        _monedasAntes = PlayerInventory.Instance.Coins;
    }

    private void Update()
    {
        for (int i = 0; i < _hearts.Length; i++)
        {
            bool full = i < _health.CurrentHealth;

            if (full)
            {
                if (!_wasFull[i]) Rebotar(_hearts[i].transform);
                _deadTime[i] = -1f;
                _hearts[i].sprite = _idleHeartFrames[(int)(Time.time * _heartFps) % _idleHeartFrames.Length];
            }
            else
            {
                if (_wasFull[i]) _deadTime[i] = 0f;

                int frame = (int)(_deadTime[i] * _heartFps);
                if (_deadTime[i] >= 0f && frame < _deadHeartFrames.Length)
                {
                    _hearts[i].sprite = _deadHeartFrames[frame];
                    _deadTime[i] += Time.deltaTime;
                }
                else
                {
                    _deadTime[i] = -1f;
                    _hearts[i].sprite = _emptyHeart;
                }
            }

            _wasFull[i] = full;
        }

        if (PlayerInventory.Instance.Coins != _monedasAntes)
        {
            _monedasAntes = PlayerInventory.Instance.Coins;
            Rebotar(_coinsText.transform);
        }

        _coinsText.text =PlayerInventory.Instance.Coins.ToString();
        _keysText.text = PlayerInventory.Instance.Keys.ToString();
        _bossKeyIcon.SetActive(PlayerInventory.Instance.HasBossKey);

        APlayerInteractable interactable = PlayerController.Instance.GetCurrentInteractable();

        if (interactable == null || interactable.ActionText == "")
        {
            _actionText.text = _hotbar.Selected == 0 ? "Atacar" : "Usar";
            _actionIcon.sprite = _attackIcon;
        }
        else
        {
            _actionText.text = interactable.ActionText;
            _actionIcon.sprite = _interactIcon;
        }
    }

    private void Rebotar(Transform objeto)
    {
        objeto.DOKill(true);
        objeto.DOPunchScale(Vector3.one * 0.3f, 0.25f, 8, 0f);
    }
}

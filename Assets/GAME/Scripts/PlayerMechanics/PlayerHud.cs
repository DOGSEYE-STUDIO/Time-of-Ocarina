using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour
{
    [SerializeField] private Image[] _hearts;
    [SerializeField] private Sprite _fullHeart;
    [SerializeField] private Sprite _emptyHeart;
    [SerializeField] private TMP_Text _coinsText;
    [SerializeField] private TMP_Text _keysText;
    [SerializeField] private GameObject _bossKeyIcon;
    [SerializeField] private TMP_Text _actionText;
    [SerializeField] private Image _actionIcon;
    [SerializeField] private Sprite _attackIcon;
    [SerializeField] private Sprite _interactIcon;

    private PlayerHealth _health;

    private void Start()
    {
        _health = PlayerController.Instance.GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        for (int i = 0; i < _hearts.Length; i++)
        {
            _hearts[i].sprite = i < _health.CurrentHealth ? _fullHeart : _emptyHeart;
        }

        _coinsText.text = PlayerInventory.Instance.Coins.ToString();
        _keysText.text = PlayerInventory.Instance.Keys.ToString();
        _bossKeyIcon.SetActive(PlayerInventory.Instance.HasBossKey);

        APlayerInteractable interactable = PlayerController.Instance.GetCurrentInteractable();

        if (interactable == null || interactable.ActionText == "")
        {
            _actionText.text = "Atacar";
            _actionIcon.sprite = _attackIcon;
        }
        else
        {
            _actionText.text = interactable.ActionText;
            _actionIcon.sprite = _interactIcon;
        }
    }
}

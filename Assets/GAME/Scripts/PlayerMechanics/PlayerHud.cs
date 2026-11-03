using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour
{
    [SerializeField] private Image[] _hearts;
    [SerializeField] private Sprite _fullHeart;
    [SerializeField] private Sprite _emptyHeart;
    [SerializeField] private TMP_Text _coinsText;

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
    }
}

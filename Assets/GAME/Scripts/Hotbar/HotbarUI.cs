using UnityEngine;
using UnityEngine.UI;

public class HotbarUI : MonoBehaviour
{
    [SerializeField] private Image[] _icons;
    [SerializeField] private Sprite _swordIcon;
    [SerializeField] private RectTransform _selector;

    private Hotbar _hotbar;

    private void Start()
    {
        _hotbar = PlayerController.Instance.GetComponent<Hotbar>();
        _hotbar.OnChanged += Refresh;
        Refresh();
    }

    private void OnDestroy()
    {
        if (_hotbar != null) _hotbar.OnChanged -= Refresh;
    }

    private void Refresh()
    {
        _icons[0].sprite = _swordIcon;

        for (int i = 1; i < _icons.Length; i++)
        {
            APickable item = _hotbar.GetConsumable(i);
            _icons[i].enabled = item != null;
            if (item != null) _icons[i].sprite = item.GetComponentInChildren<SpriteRenderer>().sprite;
        }

        _selector.position = _icons[_hotbar.Selected].transform.position;
    }
}

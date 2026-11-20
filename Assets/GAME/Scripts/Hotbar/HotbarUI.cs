using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HotbarUI : MonoBehaviour
{
    [SerializeField] private Image[] _icons;
    [SerializeField] private Sprite _swordIcon;
    [SerializeField] private RectTransform _selector;
    [SerializeField] private Sprite _casillaNormal;
    [SerializeField] private Sprite _casillaSeleccionada;
    [SerializeField] private float _escalaSeleccionada = 1.2f;

    private Hotbar _hotbar;
    private RectTransform _rect;
    private float _shownY;
    private float _hiddenY;

    private void Start()
    {
        _rect = (RectTransform)transform;
        _shownY = _rect.anchoredPosition.y;
        _hiddenY = -_rect.rect.height;
        _rect.anchoredPosition = new Vector2(_rect.anchoredPosition.x, _hiddenY);

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

        for (int i = 0; i < _icons.Length; i++)
        {
            bool seleccionada = i == _hotbar.Selected;
            Transform casilla = _icons[i].transform.parent;
            casilla.GetComponent<Image>().sprite = seleccionada ? _casillaSeleccionada : _casillaNormal;
            casilla.localScale = Vector3.one * (seleccionada ? _escalaSeleccionada : 1f);
        }

        _selector.position = _icons[_hotbar.Selected].transform.position;

        _rect.DOKill();
        _rect.DOAnchorPosY(_hotbar.IsOpen ? _shownY : _hiddenY, 0.3f).SetEase(Ease.OutCubic);
    }
}

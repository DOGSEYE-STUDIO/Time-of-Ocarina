using DG.Tweening;
using MelenitasDev.SoundsGood;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, ISubmitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private bool _sonidoArpegio;
    [SerializeField] private GameObject _cursor;
    [SerializeField] private RectTransform _texto;
    [SerializeField] private float _desplazamiento = 8f;

    private void OnEnable()
    {
        if (_cursor == null) return;

        bool seleccionado = EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject;
        _cursor.SetActive(seleccionado);
        _texto.anchoredPosition = new Vector2(seleccionado ? _desplazamiento : 0f, 0f);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (eventData is AxisEventData) SonarMover();

        if (_cursor == null) return;

        _cursor.SetActive(true);
        _cursor.transform.DOKill();
        _cursor.transform.localScale = Vector3.one * 0.6f;
        _cursor.transform.DOScale(1f, 0.15f).SetEase(Ease.OutBack).SetUpdate(true).SetLink(gameObject);

        _texto.DOKill();
        _texto.DOAnchorPosX(_desplazamiento, 0.12f).SetUpdate(true).SetLink(gameObject);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (_cursor == null) return;

        _cursor.SetActive(false);

        _texto.DOKill();
        _texto.DOAnchorPosX(0f, 0.12f).SetUpdate(true).SetLink(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject) return;

        EventSystem.current.SetSelectedGameObject(gameObject);
        SonarMover();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        PlayConfirm();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayConfirm();
    }

    private void SonarMover()
    {
        new Sound(SFX.MenuMove).SetOutput(Output.SFX).Play();
    }

    private void PlayConfirm()
    {
        if (GetComponent<Slider>() != null) return;

        if (_sonidoArpegio) new Sound(SFX.MenuStart).SetOutput(Output.SFX).Play();
        else new Sound(SFX.MenuConfirm).SetOutput(Output.SFX).Play();

        transform.DOKill(true);
        transform.DOPunchScale(Vector3.one * -0.08f, 0.2f, 8, 0f).SetUpdate(true).SetLink(gameObject);
    }
}

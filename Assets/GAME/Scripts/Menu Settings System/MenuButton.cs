using MelenitasDev.SoundsGood;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, ISubmitHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject) return;

        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        PlayConfirm();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayConfirm();
    }

    private void PlayConfirm()
    {
        if (GetComponent<Slider>() != null) return;

        new Sound(SFX.MenuConfirm).SetOutput(Output.SFX).Play();
    }
}

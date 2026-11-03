using MelenitasDev.SoundsGood;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IMoveHandler, ISubmitHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject) return;

        EventSystem.current.SetSelectedGameObject(gameObject);
        new Sound(SFX.hit).SetVolume(0.5f).SetOutput(Output.SFX).Play();
    }

    public void OnMove(AxisEventData eventData)
    {
        new Sound(SFX.hit).SetVolume(0.5f).SetOutput(Output.SFX).Play();
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

        new Sound(SFX.coin).SetOutput(Output.SFX).Play();
    }
}

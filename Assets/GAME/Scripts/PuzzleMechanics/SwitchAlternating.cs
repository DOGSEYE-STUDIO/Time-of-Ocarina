
using System.Collections;
using UnityEngine;

public class SwitchAlternating : ASwitch
{
    [SerializeField] private Animator animator;
    [SerializeField] private Animator animatorTop;

    private void Start()
    {
        OnStateChanged += UpdateAnimation;
        UpdateAnimation(IsOn);
    }

    private void UpdateAnimation(bool isOn)
    {
        animator.SetBool("azul", isOn);
        animatorTop.SetBool("azul", isOn);

        CanToggle = false;
        StartCoroutine(WaitForAnimation());
    }

    private IEnumerator WaitForAnimation()
    {
        yield return null;

        while (!IsIdle(animator) || !IsIdle(animatorTop))
        {
            yield return null;
        }

        FinishToggle();
    }

    private bool IsIdle(Animator targetAnimator)
    {
        if (targetAnimator.IsInTransition(0))
            return false;

        AnimatorStateInfo state =
            targetAnimator.GetCurrentAnimatorStateInfo(0);

        return state.IsName("switch_rojo") ||
               state.IsName("switch_azul");
    }
}

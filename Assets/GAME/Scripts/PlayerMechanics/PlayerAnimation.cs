using System;
using UnityEngine;
using DG.Tweening;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private bool isPlayingAction;
    private Sequence actionSequence;

    private string currentAnimation;

    public bool IsPlayingAction => isPlayingAction;

    public void UpdateLocomotion(bool isMoving, bool isPushing, Vector2 facing)
    {
        if (isPlayingAction) return;

        string direction = GetDirectionName(facing);
        string animation = GetDirectionName(facing);

        if (isPushing)
            animation = isMoving ? "Push_" : "PushIdle_";
        else
            animation = isMoving ? "Walk_" : "Idle_";

        PlayLocomotionAnimation(animation + direction);
    }

    private void PlayLocomotionAnimation(string animationName)
    {
        if (currentAnimation == animationName) return;

        currentAnimation = animationName;

        animator.Play(animationName, 0, 0f);
    }

    public void PlayAnimation(string animationName, Action onFinished = null, bool updateLocomotion = true)
    {
        if (actionSequence != null)
            actionSequence.Kill();

        isPlayingAction = true;
        currentAnimation = animationName;

        animator.Play(animationName, 0, 0f);

        float duration = GetAnimationDuration(animationName);

        actionSequence = DOTween.Sequence();

        actionSequence.AppendInterval(duration / animator.speed);

        actionSequence.OnComplete(() =>
        {
            actionSequence = null;

            if (updateLocomotion)
            {
                isPlayingAction = false;
                currentAnimation = "";

                UpdateLocomotion(false, false, PlayerController.Instance.GetFacingDirection());
            }

            onFinished?.Invoke();
        });
    }

    private float GetAnimationDuration(string animationName)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name == animationName)
                return clip.length;
        }

        return 0f;
    }

    private string GetDirectionName(Vector2 facing)
    {
        if (facing == Vector2.up) return "Up";
        if (facing == Vector2.down) return "Down";
        if (facing == Vector2.left) return "Left";

        return "Right";
    }

    public void ResumeLocomotion()
    {
        isPlayingAction = false;
        currentAnimation = "";

        UpdateLocomotion(false, false, PlayerController.Instance.GetFacingDirection());
    }

    private void OnDestroy()
    {
        actionSequence?.Kill();
    }
}

using System;
using UnityEngine;
using DG.Tweening;

public class FrisbeeProyectile : MonoBehaviour
{
    public event Action OnReturned;

    private float speed = 15f;
    private float outboundDistance = 5f;
    private float pauseDuration = 0.5f;

    private float catchDistance = 0.15f;

    private Transform player;
    private Sequence outboundSequence;

    private bool isWaitingToReturn;
    private bool isReturning;
    private bool hasReturned;
    private bool isButtonHeld;

    private float pauseStartedAt;

    public void Launch(Vector2 direction, Transform playerTransform)
    {
        player = playerTransform;

        if (direction.sqrMagnitude < 0.001f) direction = Vector2.down;

        direction.Normalize();

        isWaitingToReturn = false;
        isReturning = false;
        hasReturned = false;

        float outboundDuration = outboundDistance / speed;

        Vector3 targetPosition = transform.position + (Vector3)(direction * outboundDistance);

        outboundSequence?.Kill();
        outboundSequence = DOTween.Sequence();

        outboundSequence.Append(transform.DOMove(targetPosition, outboundDuration) .SetEase(Ease.Linear) );
        outboundSequence.OnComplete(() =>
        {
            pauseStartedAt = Time.time;
            isWaitingToReturn = true;
        });
    }

    public void SetButtonHeld(bool held) { isButtonHeld = held; }

    private void Update()
    {
        if (hasReturned) return;

        if (isWaitingToReturn)
        {
            float elapsed = Time.time - pauseStartedAt;

            if (elapsed >= pauseDuration && !isButtonHeld)
            {
                isWaitingToReturn = false;
                isReturning = true;
            }

            return;
        }

        if (!isReturning) return;

        if (player == null)
        {
            FinishReturn();
            return;
        }

        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = player.position;
        targetPosition.z = currentPosition.z;

        float distance = Vector2.Distance(currentPosition, targetPosition);

        if (distance <= catchDistance)
        {
            FinishReturn();
            return;
        }

        transform.position = Vector3.MoveTowards(currentPosition, targetPosition, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, player.position) <= catchDistance)
        {
            FinishReturn();
        }
    }

    private void FinishReturn()
    {
        if (hasReturned) return;

        hasReturned = true;
        isWaitingToReturn = false;
        isReturning = false;

        outboundSequence?.Kill();

        OnReturned?.Invoke();
        OnReturned = null;

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        outboundSequence?.Kill();
    }
}

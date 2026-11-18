using DG.Tweening;
using UnityEngine;

public class Poof : MonoBehaviour
{
    void Start()
    {
        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(1);
        seq.AppendCallback(() => Destroy(gameObject));
    }
}

using MelenitasDev.SoundsGood;
using MoreMountains.Feedbacks;
using UnityEngine;

public class EnemyFrisbeeHit : AFrisbeeInteractable
{
    [SerializeField] private GameObject _onda;
    [SerializeField] private MMF_Player _feedbackImpacto;

    private EnemyBase _enemigo;

    private void Awake()
    {
        _enemigo = GetComponent<EnemyBase>();
    }

    public override void OnFrisbeeHit()
    {
        _enemigo.Aturdir();
        new Sound(SFX.AturdirTambor).SetOutput(Output.SFX).SetRandomPitch().Play();

        if (_onda != null) Instantiate(_onda, transform.position, Quaternion.identity);
        if (_feedbackImpacto != null) _feedbackImpacto.PlayFeedbacks();
    }
}

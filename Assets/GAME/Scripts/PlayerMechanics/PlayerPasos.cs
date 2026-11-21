using MelenitasDev.SoundsGood;
using UnityEngine;

public class PlayerPasos : MonoBehaviour
{
    [SerializeField] private float _tiempoEntrePasos = 0.3f;
    [SerializeField, Range(0f, 1f)] private float _volumen = 0.35f;

    private PlayerMovement _movimiento;
    private float _tiempo;

    private void Awake()
    {
        _movimiento = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (!_movimiento.IsMoving)
        {
            _tiempo = _tiempoEntrePasos;
            return;
        }

        _tiempo += Time.deltaTime;

        if (_tiempo < _tiempoEntrePasos) return;

        _tiempo = 0f;
        new Sound(SFX.Pasos).SetOutput(Output.SFX).SetVolume(_volumen).SetRandomPitch().Play();
    }
}

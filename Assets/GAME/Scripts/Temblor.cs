using Unity.Cinemachine;
using UnityEngine;

public static class Temblor
{
    public static void Camara(Vector3 posicion, float fuerza)
    {
        CinemachineImpulseDefinition impulso = new CinemachineImpulseDefinition();
        impulso.ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Bump;
        impulso.ImpulseType = CinemachineImpulseDefinition.ImpulseTypes.Uniform;
        impulso.ImpulseDuration = 0.15f;
        impulso.CreateEvent(posicion, Random.insideUnitCircle.normalized * fuerza);
    }
}

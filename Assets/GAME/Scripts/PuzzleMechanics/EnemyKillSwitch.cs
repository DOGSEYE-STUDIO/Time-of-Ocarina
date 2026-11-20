using UnityEngine;

public class EnemyKillSwitch : ASwitch
{
    // Comprobar que todos los enemigos de una lista están muertos y activar el switch cuando eso

    private void Start()
    {
        Toggle();
    }
}

using DG.Tweening;
using MelenitasDev.SoundsGood;
using UnityEngine;

public class SwitchSpawn : MonoBehaviour
{
    [SerializeField] private ASwitch targetSwitch;

    public GameObject[] objectsToSpawn;
    public GameObject[] objectsToDespawn;
    public GameObject poof;
    [SerializeField] private float spawnDelay = 0.1f;

    private BoxCollider2D objectCollider;

    Sequence seq;

    void Start()
    {
        targetSwitch.OnStateChanged += SpawnObjects;

        objectCollider = GetComponent<BoxCollider2D>();
        if (objectCollider != null) objectCollider.enabled = false;

        foreach (var obj in objectsToSpawn) obj.SetActive(false);
        foreach (var obj in objectsToDespawn) obj.SetActive(true);
    }

    private void SpawnObjects(bool isOn)
    {
        if (isOn && targetSwitch is EnemyKillSwitch sala && !sala.Completado) return;
        if (isOn && objectsToSpawn.Length > 0) new Sound(SFX.Acierto).SetOutput(Output.SFX).Play();
        foreach (var obj in objectsToDespawn) obj.SetActive(!isOn);
        seq?.Kill();
        seq = DOTween.Sequence();
        foreach (var obj in objectsToSpawn)
        {
            seq.AppendCallback(() => {
                obj.SetActive(isOn);
                Instantiate(poof, obj.transform.position, obj.transform.rotation);
            });
            if (isOn) seq.AppendInterval(spawnDelay);
        }
        seq.OnComplete(() => { if (objectCollider != null) objectCollider.enabled = isOn; });
    }
}

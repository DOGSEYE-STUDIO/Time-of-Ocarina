using UnityEngine;

public class StairSpawn : MonoBehaviour
{
    [SerializeField] private ASwitch targetSwitch;

    public GameObject topRow;
    public GameObject bottomRow;
    public GameObject pit;

    private BoxCollider2D collider;

    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        collider.enabled = false;

        topRow.SetActive(false);
        bottomRow.SetActive(false);

        targetSwitch.OnStateChanged += SpawnStairs;
    }

    private void SpawnStairs(bool isOn)
    {
        topRow.SetActive(isOn);
        bottomRow.SetActive(isOn);
        pit.SetActive(!isOn);

        collider.enabled = isOn;
    }
}

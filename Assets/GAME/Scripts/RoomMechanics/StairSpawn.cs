using UnityEngine;

public class StairSpawn : MonoBehaviour
{
    [SerializeField] private ASwitch targetSwitch;

    public GameObject topRow;
    public GameObject bottomRow;
    public GameObject pit;

    private BoxCollider2D stairCollider;

    void Start()
    {
        stairCollider = GetComponent<BoxCollider2D>();
        stairCollider.enabled = false;

        topRow.SetActive(false);
        bottomRow.SetActive(false);

        targetSwitch.OnStateChanged += SpawnStairs;
    }

    private void SpawnStairs(bool isOn)
    {
        topRow.SetActive(isOn);
        bottomRow.SetActive(isOn);
        pit.SetActive(!isOn);

        stairCollider.enabled = isOn;
    }
}

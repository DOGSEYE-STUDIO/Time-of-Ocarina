using UnityEngine;

public class PitRespawn : MonoBehaviour
{
    public Vector3 respawnPosition;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

       respawnPosition = other.gameObject.transform.position;
        Debug.Log(respawnPosition);
    }
}

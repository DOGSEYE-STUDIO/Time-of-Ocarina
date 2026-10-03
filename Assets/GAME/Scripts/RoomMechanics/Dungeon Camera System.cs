using Unity.Cinemachine;
using UnityEngine;

public class DungeonCameraSystem : MonoBehaviour
{
    [SerializeField] private Transform camTarget;
    [SerializeField] private CinemachineCamera roomCam;   // cámara propia de esta sala

    private BoxCollider2D colliderSala;
    private Transform player;   // null = el jugador no está en esta sala

    private void Start()
    {
        colliderSala = GetComponent<BoxCollider2D>();
        roomCam.enabled = false;   // empieza apagada, manda la del jugador
    }

    private void LateUpdate()
    {
        if (player == null) return;   // el jugador no está en esta sala

        Vector2 limits = GetLimits();
        Vector3 centro = colliderSala.bounds.center;

        Vector3 pos = player.position;
        pos.x = Mathf.Clamp(pos.x, centro.x - limits.x, centro.x + limits.x);
        pos.y = Mathf.Clamp(pos.y, centro.y - limits.y, centro.y + limits.y);

        camTarget.position = pos;
    }

    // Cuánto se puede mover el centro de la cámara desde el centro de la sala
    private Vector2 GetLimits()
    {
        Bounds sala = colliderSala.bounds;

        float camHalfHeight = Camera.main.orthographicSize;
        float camHalfWidth = camHalfHeight * Camera.main.aspect;

        float limitX = Mathf.Max(0, sala.extents.x - camHalfWidth);
        float limitY = Mathf.Max(0, sala.extents.y - camHalfHeight);

        return new Vector2(limitX, limitY);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        player = other.transform;
        camTarget.position = player.position;   // el target empieza donde está el jugador
        roomCam.enabled = true;                  // el Brain hace el blend hacia esta cámara
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        roomCam.enabled = false;   // el Brain vuelve a la cámara que quede activa
        player = null;
    }

    // Dibuja en la Scene la zona por la que se puede mover el centro de la cámara
    private void OnDrawGizmos()
    {
        if (colliderSala == null) colliderSala = GetComponent<BoxCollider2D>();
        if (colliderSala == null || Camera.main == null) return;

        Vector2 limits = GetLimits();

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(colliderSala.bounds.center, new Vector3(limits.x * 2, limits.y * 2, 0));
    }
}
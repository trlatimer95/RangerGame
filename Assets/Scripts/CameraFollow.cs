using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject player;

    private Vector3 offset;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        offset = transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        transform.position = new Vector3(player.transform.position.x + offset.x, transform.position.y, player.transform.position.z + offset.z);
    }
}

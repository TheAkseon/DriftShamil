using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    private Rigidbody playerRB;
    public Vector3 Offset;
    public float speed;

    private void Start()
    {
        playerRB = player.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 playerForward = (playerRB.velocity + player.transform.forward).normalized;
        transform.position = Vector3.Lerp(transform.position,
            player.position + player.transform.TransformVector(Offset)
            + playerForward * (-5f),
            speed * Time.deltaTime);
        transform.LookAt(player);
    }
}

using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Smoothing")]
    [SerializeField] private float damping = 0.3f;

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        FindPlayer();
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            FindPlayer();
            return;
        }

        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(
            transform.position, targetPosition, ref velocity, damping);
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
            transform.position = target.position + offset;
        }
    }
}
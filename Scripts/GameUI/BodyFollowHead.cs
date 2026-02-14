using UnityEngine;

public class BodyFollowHead : MonoBehaviour
{
    [Header("Assign the XR Main Camera (Head)")]
    public Transform head;

    [Header("Body settings")]
    public float groundY = 0f;        // set to your floor height
    public float followSpeed = 20f;   // higher = snappier
    public bool rotateWithHead = true;

    void LateUpdate()
    {
        if (!head) return;

        // Follow head position on X/Z only (stay on ground)
        Vector3 target = transform.position;
        target.x = head.position.x;
        target.z = head.position.z;
        target.y = groundY;

        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * followSpeed);

        // Optional: rotate body to face same direction as head (yaw only)
        if (rotateWithHead)
        {
            Vector3 e = transform.eulerAngles;
            e.y = head.eulerAngles.y;
            transform.eulerAngles = e;
        }
    }
}

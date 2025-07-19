using Unity.VisualScripting;
using UnityEngine;

public class SpiderController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1.5f;
    public float stopDistance = 0.05f;
    private Vector3? movementTarget;

    [Header("BodyRays")]
    public GameObject[] bodyRays;
    public float bodyHeightOffset = 0.5f;
    public float lerpSpeed = 5f;
    public LayerMask groundLayer;

    [Header("Leg Group")]
    public SpiderLegController[] legsInZigzagOrder;

    private int currentLegIndex = 0;
    private float stepCooldown = 0.02f;
    private float lastStepTime = 0f;
    

    void Update()
    {
        HandleMovement();
        
        if (Time.time - lastStepTime >= stepCooldown)
        {
            for (int i = 0; i < legsInZigzagOrder.Length; i++)
            {
                int legIndex = (currentLegIndex + i) % legsInZigzagOrder.Length;
                var legController = legsInZigzagOrder[legIndex];
                if (legController.ShouldStep())
                {
                    legController.TryMove();
                    currentLegIndex = (legIndex + 1) % legsInZigzagOrder.Length;
                    lastStepTime = Time.time;
                    break;
                }
            }
        }
        UpdateBodyPose();
    }


    void UpdateBodyPose()
    {
        if (legsInZigzagOrder.Length < 3) return;

        int totalPoints = legsInZigzagOrder.Length + bodyRays.Length;
        Vector3[] points = new Vector3[totalPoints];
        Vector3[] normals = new Vector3[totalPoints];

        for (int i = 0; i < legsInZigzagOrder.Length; i++)
        {
            points[i] = legsInZigzagOrder[i].ikTarget.position;
            normals[i] = Vector3.up;
        }

        for (int i = 0; i < bodyRays.Length; i++)
        {
            Vector3 origin = bodyRays[i].transform.position + Vector3.up * 1f;
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 50f, groundLayer))
            {
                points[legsInZigzagOrder.Length + i] = hit.point;
                normals[legsInZigzagOrder.Length + i] = hit.normal;
            }
            else
            {
                points[legsInZigzagOrder.Length + i] = origin - Vector3.up * 0.1f;
                normals[legsInZigzagOrder.Length + i] = Vector3.up;
            }
        }

        Vector3 center = Vector3.zero;
        foreach (var p in points)
            center += p;
        center /= points.Length;

        Vector3 averageNormal = Vector3.zero;
        foreach (var n in normals)
            averageNormal += n;
        averageNormal.Normalize();

        Vector3 targetPosition = new Vector3(transform.position.x, center.y + bodyHeightOffset, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * lerpSpeed);

        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, averageNormal) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * lerpSpeed);
    }

    void HandleMovement()
    {
        if (movementTarget.HasValue)
        {
            Vector3 target = movementTarget.Value;
            Vector3 direction = target - transform.position;
            direction.y = 0f;

            if (direction.magnitude > stopDistance)
            {
                transform.position += direction.normalized * moveSpeed * Time.deltaTime;
                
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(direction),
                    Time.deltaTime * 5f
                );
                
            }
            else
            {
                movementTarget = null;
            }
        }
    }

    public void MoveTo(Vector3 position)
    {
        movementTarget = position;
    }
}
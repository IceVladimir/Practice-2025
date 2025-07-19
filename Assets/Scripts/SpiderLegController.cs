using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpiderLegController : MonoBehaviour
{
    [Header("Leg IK")]
    public Transform ikTarget;
    public Transform raycastOrigin;
    public Transform spiderBody;

    [Header("Leg Settings")]
    public float stepDistance = 0.3f;
    public float stepHeight = 0.2f;
    public float stepDuration = 0.2f;
    public LayerMask groundLayer;



    private Coroutine currentStepCoroutine = null;
    private bool isStepping = false;
    private float rayLength = 50f;
    private float maxDistance_mupliplier = 1.5f;

    public void TryMove()
    {

        if (isStepping)
        {
            if (Physics.Raycast(raycastOrigin.position, Vector3.down, out RaycastHit hit, rayLength, groundLayer))
            {
                float currentDistance = Vector3.Distance(ikTarget.position, hit.point);
                
                if (currentDistance > stepDistance * maxDistance_mupliplier) 
                {
                    if (currentStepCoroutine != null)
                        StopCoroutine(currentStepCoroutine);

                    currentStepCoroutine = StartCoroutine(MoveLeg(hit.point));
                    return;
                }
            }
            return;
        }

        if (Physics.Raycast(raycastOrigin.position, Vector3.down, out RaycastHit hit2, rayLength, groundLayer))
        {
            float currentDistance = Vector3.Distance(ikTarget.position, hit2.point);
            if (currentDistance > stepDistance)
            {
                currentStepCoroutine = StartCoroutine(MoveLeg(hit2.point));
            }
        }
    }

    public bool ShouldStep()
    {
        Vector3 origin = raycastOrigin.position;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayLength, groundLayer))
        {
            Vector3 ikTarget_Horizontal = new Vector3(ikTarget.position.x, 0, ikTarget.position.z);
            Vector3 rayHit_Horizontal = new Vector3(hit.point.x, 0, hit.point.z);

            float dist = Vector3.Distance(ikTarget_Horizontal, rayHit_Horizontal);
            return dist > stepDistance && !isStepping;
        }
        return false;
    }

    private IEnumerator MoveLeg(Vector3 target)
    {
        isStepping = true;
        Vector3 start = ikTarget.position;
        float elapsed = 0f;

        while (elapsed < stepDuration)
        {
            float t = elapsed / stepDuration;
            float height = Mathf.Sin(t * Mathf.PI) * stepHeight;
            ikTarget.position = Vector3.Lerp(start, target, t) + Vector3.up * height;

            elapsed += Time.deltaTime;
            yield return null;
        }

        ikTarget.position = target;
        isStepping = false;
        currentStepCoroutine = null;
    }
}
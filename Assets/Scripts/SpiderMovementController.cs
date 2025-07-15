using UnityEngine;

public class SpiderMovementController : MonoBehaviour
{
    [Header("Settings")]
    public LayerMask groundLayer;
    public SpiderController spider;

    void Update()
    {
        HandleClick();
    }

    void HandleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
            {
                spider.MoveTo(hit.point);
            }
        }
    }
}
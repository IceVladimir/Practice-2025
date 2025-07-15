using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float boostMultiplier = 3f;
    public float mouseSensitivity = 1f;
    public float maxSpeed = 100f;

    private float currentSpeed;
    private Vector2 rotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        currentSpeed = moveSpeed;
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            HandleMouseLook();
            HandleMovement();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotation.x += mouseX;
        rotation.y -= mouseY;
        rotation.y = Mathf.Clamp(rotation.y, -90f, 90f);

        transform.rotation = Quaternion.Euler(rotation.y, rotation.x, 0f);
    }

    void HandleMovement()
    {
        Vector3 input = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            Input.GetKey(KeyCode.E) ? 1f : Input.GetKey(KeyCode.Q) ? -1f : 0f,
            Input.GetAxisRaw("Vertical")
        ).normalized;

        transform.position += transform.TransformDirection(input) * currentSpeed * Time.deltaTime;
    }
}
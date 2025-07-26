using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed at which the camera moves
    public float zoomSpeed = 2f; // Speed at which the camera zooms
    public float minZoom = 5f;   // Minimum zoom level
    public float maxZoom = 20f;  // Maximum zoom level

    private Camera cam;
    private Thing follow;

    void Start()
    {
        cam = GetComponent<Camera>(); // Get the Camera component attached to this GameObject
    }

    public void LookAt(Vector2 position)
    {
        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }

    public void Follow(Thing thing)
    {
        this.follow = thing;
    }

    public void ClearFollow()
    {
        this.follow = null;
    }

    void Update()
    {
        // if( Find.DialogManager.AnyDialogPausesGame )
        //     return;

        if( follow != null )
            HandleFollow();
        else
            HandlePlayerInput();
    }

    void HandleFollow()
    {
        LookAt(follow.position);
    }

    void HandlePlayerInput()
    {
        // Camera movement using WASD or arrow keys
        var horizontalInput = Input.GetAxis("Horizontal");
        var verticalInput = Input.GetAxis("Vertical");
        var move = new Vector3(horizontalInput, verticalInput, 0) * moveSpeed * Time.deltaTime;
        
        transform.position += move;

        // Camera zooming using mouse scroll wheel
        var scrollInput = Input.GetAxis("Mouse ScrollWheel");
        var newSize = cam.orthographicSize - scrollInput * zoomSpeed;

        cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
    }
}
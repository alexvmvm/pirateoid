using UnityEngine;

public enum CameraMode
{
    Perspective,
    Orthographic
}

public class CameraController : MonoBehaviour
{
    //Const
    private const float PerspectiveLookDistance = 10;
    private const float PerspectiveYOffset = -2;

    //Config
    public float moveSpeed = 5f; // Speed at which the camera moves
    public float zoomSpeed = 2f; // Speed at which the camera zooms
    public float minZoom = 5f;   // Minimum zoom level
    public float maxZoom = 20f;  // Maximum zoom level

    //Working vars
    private Camera cam;
    private Thing follow;
    private CameraMode cameraMode = CameraMode.Orthographic;

    //Props
    public CameraMode Mode
    {
        get => cameraMode;
        set 
        {
            if( cameraMode != value )
            {
                if( value == CameraMode.Perspective )
                    SetPerspective();
                else
                    SetOrthographic();

                cameraMode = value;
            }
        }
    }

    void Awake()
    {
        cam = GetComponent<Camera>(); // Get the Camera component attached to this GameObject
    }

    public void LookAt(Vector2 position)
    {
        if( Mode == CameraMode.Orthographic )
            transform.position = new Vector3(position.x, position.y, transform.position.z);
        else
            transform.position = new Vector3(position.x, position.y + PerspectiveYOffset,  -PerspectiveLookDistance);
    }

    public void Follow(Thing thing)
    {
        this.follow = thing;
    }

    public void ClearFollow()
    {
        this.follow = null;
    }

    private void SetPerspective()
    {
        cam.orthographic = false;
        cam.transform.rotation = Quaternion.Euler(-15, 0, 0);
    }

    private void SetOrthographic()
    {
        cam.orthographic = true;
        cam.transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        // if( Find.DialogManager.AnyDialogPausesGame )
        //     return;

        if( DebugSettings.cameraPerspective && Mode != CameraMode.Perspective )
            Mode = CameraMode.Perspective;
        else if( DebugSettings.cameraOrthographic && Mode != CameraMode.Orthographic )
            Mode = CameraMode.Orthographic;
    

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
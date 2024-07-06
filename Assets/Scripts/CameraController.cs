using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    // Inspector variables
    [SerializeField]
    private GameObject centerObject;
    [SerializeField]
    float distance;
    [SerializeField]
    float offset;
    [SerializeField]
    float lerpSpeed;
    [SerializeField]
    float speed;

    public InputManager inputManager;

    private InputAction move;
    private Rigidbody centerRB;
    private Vector3 velocity;

    void Awake() {
        // Initiate input system
        inputManager = new InputManager();
    }

    void Start() {
        // Initialize variables
        centerRB = centerObject.GetComponent<Rigidbody>();
    }

    // Input system
    void OnEnable() {
        move = inputManager.Main.Move;
        move.Enable();
    }
    void OnDisable() {
        move.Disable();
    }


    void Update() {
        // Camera Movement
        Vector2 moveVector = move.ReadValue<Vector2>();
        
        velocity = new Vector3(moveVector.x * speed, 0, moveVector.y * speed);
        // Fix Camera movement angle for isometric
        velocity = Quaternion.AngleAxis(45, Vector3.up) * velocity;
        centerRB.velocity = velocity;

    }

    void FixedUpdate() {
        lerpCam();
    }

    void lerpCam() {
        // Using the 45 45 logic for camera position to get the isometric view
        transform.position = Vector3.Lerp(
            transform.position, 
            centerObject.transform.position + new Vector3(-distance+offset, distance-0.2f, -distance),
            lerpSpeed*Time.deltaTime
        );
    }
}

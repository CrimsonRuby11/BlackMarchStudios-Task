using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
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
        inputManager = new InputManager();
    }

    void Start() {
        
        centerRB = centerObject.GetComponent<Rigidbody>();
    }

    void OnEnable() {
        move = inputManager.Main.Move;
        move.Enable();
    }

    void OnDisable() {
        move.Disable();
    }

    void Update() {
        Vector2 moveVector = move.ReadValue<Vector2>();
        
        velocity = new Vector3(moveVector.x * speed, 0, moveVector.y * speed);
        velocity = Quaternion.AngleAxis(45, Vector3.up) * velocity;
        centerRB.velocity = velocity;
    }

    void FixedUpdate() {
        lerpCam();
    }

    void lerpCam() {
        transform.position = Vector3.Lerp(
            transform.position, 
            centerObject.transform.position + new Vector3(-distance+offset, distance-0.2f, -distance),
            lerpSpeed*Time.deltaTime
        );
    }
}

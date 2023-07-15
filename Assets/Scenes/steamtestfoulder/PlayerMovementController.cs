using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerMovementController : NetworkBehaviour
{
    public float Speed = 01f;
    public GameObject PlayerModel;
    public GameObject cube;

    [Header("Base setup")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    [SerializeField]
    private float cameraYOffset = 0.4f;
    public Camera playerCamera;




    private void Start() {
        PlayerModel.SetActive(false);
        characterController = transform.GetComponent<CharacterController>();
    }

    private void Update() {

        if(SceneManager.GetActiveScene().name == "Game") {
        
            if(PlayerModel.activeSelf == false) {
                SetPosition();
                PlayerModel.SetActive(true); 
                cube.SetActive(true);

                if (isLocalPlayer) {
                    Camera.main.gameObject.SetActive(false);
                    playerCamera.gameObject.SetActive(true);
                    playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
                } else {
                    Destroy(playerCamera);
                }
            }

            if (hasAuthority) {
                Movement();
            }
        }
    }


    public void SetPosition() {
        transform.position = new Vector3(Random.Range(-5, 5), 0.8f, Random.Range(-15, 7));
    }

    public void Movement() {
        bool isRunning = false;

        // Press Left Shift to run
        isRunning = Input.GetKey(KeyCode.LeftShift);

        // We are grounded, so recalculate move direction based on axis
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded) {
            moveDirection.y = jumpSpeed;
        } else {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded) {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove && playerCamera != null) {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}

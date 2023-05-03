using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : PlayerComponent
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform handsTransform;
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float moveSmoothTime = 0;
    [SerializeField] private float lookSpeed;
    [SerializeField] private float lookXLimit;
    private Vector3 moveVelocity = Vector3.zero;
    private float moveX = 0;
    private float moveY = 0;
    private float moveXSmoothVelocity = 0;
    private float moveYSmoothVelocity = 0;
    private float rotationX = 0;
    private bool controllerWasGrounded = true;
    public Transform HandsTransform => handsTransform;

    public override void PlayerUpdate()
    {
        if (!isLocalPlayer || !manager.Get<PhaseManager>().HasStarted)
        {
            return;
        }

        UpdateMovementVectors();
        UpdateMovement();
    }

    private void UpdateMovementVectors()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float lastMoveX = moveX;
        float lastMoveY = moveY;
        moveX = (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical");
        moveY = (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal");
        moveX = Mathf.SmoothDamp(lastMoveX, moveX, ref moveXSmoothVelocity, moveSmoothTime);
        moveY = Mathf.SmoothDamp(lastMoveY, moveY, ref moveYSmoothVelocity, moveSmoothTime);
    }
    private void UpdateMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Stop movement when cursor is being used
        if (manager.Get<CursorManager>().IsCursorVisable)
        {
            moveX = 0;
            moveY = 0;
        }

        float moveVelocityY = moveVelocity.y;
        moveVelocity = (forward * moveX) + (right * moveY);

        // Allow jumps only when cursor is hidden and is grounded
        if (!manager.Get<CursorManager>().IsCursorVisable && controller.isGrounded && Input.GetButtonDown("Jump"))
            moveVelocity.y = jumpSpeed;
        else // Keep old y velocity
            moveVelocity.y = moveVelocityY;

        // Apply gravity when not on ground
        if (!controller.isGrounded)
            moveVelocity.y -= gravity * Time.deltaTime;

        // Apply move velocity to controller
        Vector3 moveDelta = moveVelocity * Time.deltaTime;
        controller.Move(moveDelta);

        // If was grounded last update and isn't grounded now and falling down, cancel velocity
        if (controllerWasGrounded && !controller.isGrounded && moveVelocity.y < 0)
            moveVelocity.y = 0;

        // Only allow looking when cursor is hidden
        if (!manager.Get<CursorManager>().IsCursorVisable)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

            Quaternion rotation = Quaternion.Euler(rotationX, 0, 0);
            Camera.main.transform.localRotation = rotation;
            handsTransform.transform.localRotation = rotation;

            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        controllerWasGrounded = controller.isGrounded;
    }
}

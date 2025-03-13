using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float runSpeed;

    [SerializeField]
    private float crouchSpeed;

    private float applySpeed;

    [SerializeField]
    private float jumpForce;
    
    

    //상태변수
    private bool isRun = false;
    private bool isCrouch = false;
    private bool isGround = false;

    [SerializeField]
    private float crouchPosY;
    private float originPosY;
    private float applyCrouchPosY;


    //땅착지여부
    private CapsuleCollider capsuleCollider;

    //민감도
    [SerializeField]
    private float lookSensitivity;

    //카메라 제한
    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX = 0;

    //컴포넌트
    [SerializeField]
    private Camera theCamera;

    private Rigidbody myRigid;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        applySpeed = walkSpeed;
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;

    }

    // Update is called once per frame
    void Update() {
        IsGround();
        TryJump();
        TryRun();
        TryCrouch();

        Move();
        CameraRotation();
        CharacterRotation();
    }

    private void TryCrouch()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl)){
            Crouch();
        }
    }

    private void Crouch()
    {
        isCrouch = !isCrouch;
        if(isCrouch){
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else{
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }

        StartCoroutine(CrouchCoroutine());
    }

    IEnumerator CrouchCoroutine(){//같이 실행하고싶은 로직
        float _posY = theCamera.transform.localPosition.y;
        int count =0;
        while (_posY != applyCrouchPosY)
        {
            ++count;
            _posY = Mathf.Lerp(_posY,applyCrouchPosY,0.1f);
            theCamera.transform.localPosition = new Vector3(0f,_posY,0f);
            if(count > 15)
                break;

            yield return null;

        }
        theCamera.transform.localPosition = new Vector3(0f,applyCrouchPosY,0f);
    }

    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position,Vector3.down,capsuleCollider.bounds.extents.y + 0.1f);
    }

    private void TryJump() {
        if(Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
        }
    }

    private void Jump(){
        if(isCrouch)//앉은상태시 점프하면 앉기풀림.
            Crouch();
        myRigid.linearVelocity = transform.up *jumpForce;
    }


    private void TryRun() {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            Running();
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            RunningCancel();
        }

    }

    private void Running()
    {
        if(isCrouch)//앉은상태시 점프하면 앉기풀림.
            Crouch();
        isRun = true;
        applySpeed = runSpeed;

    }

    private void RunningCancel()
    {
        isRun = false;
        applySpeed = walkSpeed;
    }

    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal +_moveVertical).normalized * applySpeed;
        

        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX,-cameraRotationLimit,cameraRotationLimit);
        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX,0f,0f);

    }

    private void CharacterRotation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f,_yRotation,0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));

    }
}

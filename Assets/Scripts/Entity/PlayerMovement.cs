using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public PlayerCharacter playerCharacter;
    [HideInInspector] public float verticalMoveSpeed; //앞뒤 움직임 속도
    [HideInInspector] public float horizontalMoveSpeed;//양옆 움직임 속도
    [HideInInspector] public float sprintSpeed;//달리기 속도 곱

    [Header("Mouse Settings")]
    [HideInInspector] public float xMouseSensitivity = 1f; //좌우 마우스 움직임 속도
    [HideInInspector] public float yMouseSensitivity = 1f; //상하 마우스 움직임 속도

    [Header("Energy Settings")]
    float maxEnergy = 100f;
    public Slider energySlider; // 기력을 표시할 UI 슬라이더

    [Header("UI Elements")]
    [HideInInspector] public RectTransform uiElement; // 이동할 UI 요소의 RectTransform
    [HideInInspector] private RectTransform parentRectTransform;
    private Rigidbody playerRigidbody;
    private PlayerInput playerInput;
    private Animator playerAnimator; //플레이어 캐릭터 애니메이터
    float startEnergy;
    float energy;
    private bool isRunning;
    private Vector2 moveInput;

    [Header("Camera")]
    [HideInInspector] public CinemachineVirtualCamera virtualCamera;  // 시네머신 가상 카메라
    private CinemachineComposer cinemachineComposer;  // CinemachineComposer
    private PlayerShooter playerShooter; // 총구 위치를 가져오기 위한 컴포넌트
    private Vector3 previousUIPosition; // uiElement의 이전 위치를 저장
    private float xRotation = 0f; // x축 회전 누적 값 (카메라 pitch)
    private Camera mainCamera;
    private Vector3 lookAtPoint;

    [Header("Sound")]
    [SerializeField] private AudioClip footstepClip; // 발소리 클립립
    [SerializeField] private float footstepRadius = 10f; // 소리가 들리는 반경
    private float footstepTimer = 0f;
    [SerializeField] private float footstepInterval = 0.5f; // 한 걸음마다 소리 간격(초), 걷기/뛰기 속도에 따라 조정
    private AudioSource audioSource; // 오디오 소스 컴포넌트트
    private bool isSprintingLocked = false; // sprint 잠금 상태
    //private bool canRun = true;
    private bool prevSprintButton = false;

    private void OnEnable()
    {
        startEnergy = maxEnergy;
        energy = startEnergy;

        energySlider.maxValue = startEnergy;
        energySlider.value = energy;

        verticalMoveSpeed = playerCharacter.frontBackMoveSpeed;
        horizontalMoveSpeed = playerCharacter.leftRightMoveSpeed;
        sprintSpeed = playerCharacter.sprintPlusSpeed;
    }

    // Start is called before the first frame update
    private void Start()
    {
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        //사용할 컴포넌트 가져오기
        playerRigidbody = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<Animator>();
        uiElement = GameObject.Find("Crosshair").GetComponent<RectTransform>();
        parentRectTransform = uiElement.parent.GetComponent<RectTransform>();

        // 시네머신 가상 카메라 설정
        virtualCamera = GameObject.Find("TPS Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = transform;
        virtualCamera.LookAt = transform;
        cinemachineComposer = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineComposer>();

        playerShooter = GetComponent<PlayerShooter>();

        xMouseSensitivity = 1f;
        yMouseSensitivity = 1f;
        // 초기 UI 위치 저장
        previousUIPosition = uiElement.localPosition;

        mainCamera = Camera.main;
        lookAtPoint = Vector3.zero;

        audioSource = GetComponent<AudioSource>();

    }

    private void Update()
    {
        moveInput = new Vector2(playerInput.horizontalMove, playerInput.verticalMove);

        // sprint 키를 뗐을 때만 sprint 잠금 해제
        if (!playerInput.sprintButton)
        {
            isSprintingLocked = false;
        }

        isRunning = playerInput.sprintButton && !isSprintingLocked;
    
        playerAnimator.SetFloat("MoveX", moveInput.x);
        playerAnimator.SetFloat("MoveY", moveInput.y);
        playerAnimator.SetBool("isRunning", isRunning);
        
        EnergyControl();
        Rotation();
        MoveUIElement();
        MovePlayer();

        // 발소리 타이머
        if (moveInput.magnitude > 0.1f) // 움직이고 있을 때만
        {
            footstepTimer += Time.deltaTime;
            float interval = isRunning ? footstepInterval * 0.6f : footstepInterval; // 달리기는 더 짧게
            if (footstepTimer >= interval)
            {
                FootStepSound();
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = footstepInterval; // 멈추면 타이머 초기화
        }

        prevSprintButton = playerInput.sprintButton;
    }

    // Update is called once per frame
    // private void FixedUpdate()
    // {
    //     MovePlayer();
    // }


    //움직임 메서드
    private void MovePlayer()
    {
        float forwardSpeed = isRunning && energy > 0 ? sprintSpeed * verticalMoveSpeed : verticalMoveSpeed;
        float strafeSpeed = isRunning && energy > 0 ? sprintSpeed * horizontalMoveSpeed : horizontalMoveSpeed;

        Vector3 forwardMovement = transform.forward * moveInput.y * forwardSpeed;
        Vector3 strafeMovement = transform.right * moveInput.x * strafeSpeed;

        Vector3 moveVelocity = forwardMovement + strafeMovement;

        if (moveVelocity.sqrMagnitude > 0.01f)
        {
           //playerRigidbody.AddForce(moveVelocity, ForceMode.VelocityChange);
                        // y축 속도(중력 등)는 보존
            Vector3 velocity = moveVelocity;
            velocity.y = playerRigidbody.velocity.y;
            playerRigidbody.velocity = velocity;
        }
        else
        {
            // 입력이 없으면 x, z 속도만 0으로
            Vector3 velocity = playerRigidbody.velocity;
            velocity.x = 0f;
            velocity.z = 0f;
            playerRigidbody.velocity = velocity;
        }
    }



    private void Rotation()
    {
        // UI 요소의 현재 위치
        Vector3 currentUIPosition = uiElement.localPosition;

        Vector2 parentSize = parentRectTransform.rect.size;  // 부모 캔버스 크기
        Vector2 elementSize = uiElement.rect.size;  // UI 요소의 크기

        // 위치 변화량 계산
        float deltaY = currentUIPosition.y - previousUIPosition.y; // 상하 변화량

        // y축 회전 (플레이어 본체 회전)
        float yRotation = playerInput.xMouseMove * xMouseSensitivity;

        transform.Rotate(0f, yRotation, 0f);


        // x축 회전 (카메라 pitch 적용)
        float pitch = deltaY;
        xRotation -= pitch; // 위로 움직이면 각도 감소, 아래로 움직이면 각도 증가

        // 카메라의 x축 회전 적용
        // Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);


        // Cinemachine의 m_ScreenY 값을 uiElement의 y 위치에 따라 제한된 범위로 설정
        float mappedValue = Mathf.InverseLerp(-parentSize.y / 5f + elementSize.y / 2, parentSize.y / 2 - elementSize.y / 2, currentUIPosition.y);  // Y값을 0에서 1 사이로 변환
        mappedValue = Mathf.Clamp(mappedValue, 0.50f, 0.7f);  // 0.75 ~ 1 사이로 제한
        cinemachineComposer.m_ScreenY = mappedValue;

        // 이전 UI 위치 업데이트
        previousUIPosition = currentUIPosition;
    }


    private void MoveUIElement()
    {
        // 마우스 이동 변화량 가져오기
        float moveY = playerInput.yMouseMove;

        // 감도 값을 곱하여 UI 이동 반영
        moveY *= yMouseSensitivity;

        // UI 요소의 현재 위치에 이동량 반영
        Vector3 currentPosition = uiElement.localPosition;

        currentPosition.y += moveY;


        Vector2 parentSize = parentRectTransform.rect.size;  // 부모 캔버스 크기
        Vector2 elementSize = uiElement.rect.size;  // UI 요소의 크기

        // 이동 범위 제한 (UI 요소가 부모 캔버스를 벗어나지 않도록)
        currentPosition.y = Mathf.Clamp(currentPosition.y, -parentSize.y / 5f + elementSize.y / 2, parentSize.y / 2 - elementSize.y / 2);

        // 새로운 위치로 UI 요소 이동
        uiElement.localPosition = currentPosition;

    }

    //조준점의 로컬 좌표를 월드 좌표로 변환 - 크로스 헤어의 타겟팅을 구현
    public RaycastHit? LocalPosToWorldRaycast()
    {
        Ray ray = mainCamera.ScreenPointToRay(uiElement.position);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Debug.DrawRay(ray.origin, ray.direction * hitInfo.distance, Color.red, 1f);
            return hitInfo;
        }
        return null; // 충돌이 없을 경우 null 반환
    }

    private void EnergyControl()
    {
        if (playerInput.sprintButton && energy > 0)
        {
            energy -= 50f * Time.deltaTime;
        }
        else if (energy < startEnergy)
        {
            energy += 100f * Time.deltaTime;
        }
        energySlider.value = energy;

        // 에너지 0이면 강제로 sprint 잠금
        if (energy <= 0)
        {
            isSprintingLocked = true;
        }
    }

    void FootStepSound()
    {
        if (audioSource != null && footstepClip != null)
        {
            audioSource.PlayOneShot(footstepClip);
        }
    }
}

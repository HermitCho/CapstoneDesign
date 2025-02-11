using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector] public float verticalMoveSpeed = 5f;//앞뒤 움직임 속도
    [HideInInspector] public float horizontalMoveSpeed = 2.5f;//양옆 움직임 속도
    [HideInInspector] public float xMouseSensitivity = 1f; //좌우 마우스 움직임 속도
    [HideInInspector] public float yMouseSensitivity = 1f; //상하 마우스 움직임 속도
    [HideInInspector] public float sprintSpeed = 3f;//달리기 속도
    public RectTransform uiElement; // 이동할 UI 요소의 RectTransform
    private RectTransform parentRectTransform;

    private Rigidbody playerRigidbody;
    private PlayerInput playerInput;
    private Animator playerAnimator; //플레이어 캐릭터 애니메이터

    public CinemachineVirtualCamera virtualCamera;  // 시네머신 가상 카메라
    private CinemachineComposer cinemachineComposer;  // CinemachineComposer
    private PlayerShooter playerShooter; // 총구 위치를 가져오기 위한 컴포넌트

    private Vector3 previousUIPosition; // uiElement의 이전 위치를 저장
    private float xRotation = 0f; // x축 회전 누적 값 (카메라 pitch)

    private Camera mainCamera;
    private Vector3 lookAtPoint;

    public Slider energySlider; // 기력을 표시할 UI 슬라이더

    float maxEnergy = 100f;
    float startEnergy;
    float energy;

    private void OnEnable()
    {
        startEnergy = maxEnergy;
        energy = startEnergy;

        energySlider.maxValue = startEnergy;
        energySlider.value = energy;
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
        parentRectTransform = uiElement.parent.GetComponent<RectTransform>();
        cinemachineComposer = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineComposer>();
        playerShooter = GetComponent<PlayerShooter>();

        xMouseSensitivity = 5f;
        yMouseSensitivity = 10f;
        // 초기 UI 위치 저장
        previousUIPosition = uiElement.localPosition;

        mainCamera = Camera.main;
        lookAtPoint = Vector3.zero;

    }

    private void Update()
    {
        Vector3 firePos = LocalPosToWorldDirection();
        EnergyControl();
        /////////////Debug.Log(lookAtPoint);

        // Debug.Log(playerInput.xMouseMove);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {

        if (playerInput.verticalMove == 0f)
        {
            playerAnimator.SetBool("IsSideMove", true);
        }
        else
        {
            playerAnimator.SetBool("IsSideMove", false);
            playerAnimator.SetFloat("Move", playerInput.verticalMove * (playerInput.sprintButton + 1));
        }
        playerAnimator.SetFloat("SideMove", playerInput.horizontalMove);


        MoveUIElement();
        Rotation();
        Move();






    }


    //움직임 메서드
    private void Move()
    {
        float totalVerticalMoveSpeed = verticalMoveSpeed + (sprintSpeed * Mathf.Abs(playerInput.sprintButton));

        //상대적으로 이동할 거리 계산
        if (energy <= 6f)
        {
            totalVerticalMoveSpeed = verticalMoveSpeed;
        }

        Vector3 moveDistance = ((playerInput.verticalMove * transform.forward * totalVerticalMoveSpeed)
                                + (playerInput.horizontalMove * transform.right * horizontalMoveSpeed)) * Time.deltaTime;

        //리지드바디를 이용해 플레이어 위치 변경
        playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);
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

    //조준점의 로컬 좌표를 월드 좌표로 변환 - Gun 컴포넌트에서 총 쏠 위치 구할때 사용
    public Vector3 LocalPosToWorldDirection()
    {
        Ray ray = mainCamera.ScreenPointToRay(uiElement.position);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            lookAtPoint = hitInfo.point; // 마우스가 가리키는 월드 좌표

        }
        Vector3 direction = (lookAtPoint - playerShooter.gun.fireTransform.transform.position).normalized; // 바라볼 방향 계산
        return direction;
    }

    private void EnergyControl()
    {
        if (Mathf.Abs(playerInput.sprintButton) >= 0.2f && energy > 0)
        {
            energy -= 100f * Time.deltaTime;
        }
        else if (energy < startEnergy)
        {
            energy += 100f * Time.deltaTime;
        }
        energySlider.value = energy;
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector] public float verticalMoveSpeed = 5f;//�յ� ������ �ӵ�
    [HideInInspector] public float horizontalMoveSpeed = 2.5f;//�翷 ������ �ӵ�
    [HideInInspector] public float xMouseSensitivity = 1f; //�¿� ���콺 ������ �ӵ�
    [HideInInspector] public float yMouseSensitivity = 1f; //���� ���콺 ������ �ӵ�
    [HideInInspector] public float sprintSpeed = 3f;//�޸��� �ӵ�
    public RectTransform uiElement; // �̵��� UI ����� RectTransform
    private RectTransform parentRectTransform;

    private Rigidbody playerRigidbody;
    private PlayerInput playerInput;
    private Animator playerAnimator; //�÷��̾� ĳ���� �ִϸ�����

    public CinemachineVirtualCamera virtualCamera;  // �ó׸ӽ� ���� ī�޶�
    private CinemachineComposer cinemachineComposer;  // CinemachineComposer
    private PlayerShooter playerShooter; // �ѱ� ��ġ�� �������� ���� ������Ʈ

    private Vector3 previousUIPosition; // uiElement�� ���� ��ġ�� ����
    private float xRotation = 0f; // x�� ȸ�� ���� �� (ī�޶� pitch)

    private Camera mainCamera;
    private Vector3 lookAtPoint;

    public Slider energySlider; // ����� ǥ���� UI �����̴�

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

        //����� ������Ʈ ��������
        playerRigidbody = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<Animator>();
        parentRectTransform = uiElement.parent.GetComponent<RectTransform>();
        cinemachineComposer = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineComposer>();
        playerShooter = GetComponent<PlayerShooter>();

        xMouseSensitivity = 5f;
        yMouseSensitivity = 10f;
        // �ʱ� UI ��ġ ����
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


    //������ �޼���
    private void Move()
    {
        float totalVerticalMoveSpeed = verticalMoveSpeed + (sprintSpeed * Mathf.Abs(playerInput.sprintButton));

        //��������� �̵��� �Ÿ� ���
        if (energy <= 6f)
        {
            totalVerticalMoveSpeed = verticalMoveSpeed;
        }

        Vector3 moveDistance = ((playerInput.verticalMove * transform.forward * totalVerticalMoveSpeed)
                                + (playerInput.horizontalMove * transform.right * horizontalMoveSpeed)) * Time.deltaTime;

        //������ٵ� �̿��� �÷��̾� ��ġ ����
        playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);
    }



    private void Rotation()
    {
        // UI ����� ���� ��ġ
        Vector3 currentUIPosition = uiElement.localPosition;

        Vector2 parentSize = parentRectTransform.rect.size;  // �θ� ĵ���� ũ��
        Vector2 elementSize = uiElement.rect.size;  // UI ����� ũ��

        // ��ġ ��ȭ�� ���
        float deltaY = currentUIPosition.y - previousUIPosition.y; // ���� ��ȭ��

        // y�� ȸ�� (�÷��̾� ��ü ȸ��)
        float yRotation = playerInput.xMouseMove * xMouseSensitivity;

        transform.Rotate(0f, yRotation, 0f);


        // x�� ȸ�� (ī�޶� pitch ����)
        float pitch = deltaY;
        xRotation -= pitch; // ���� �����̸� ���� ����, �Ʒ��� �����̸� ���� ����

        // ī�޶��� x�� ȸ�� ����
        // Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);


        // Cinemachine�� m_ScreenY ���� uiElement�� y ��ġ�� ���� ���ѵ� ������ ����
        float mappedValue = Mathf.InverseLerp(-parentSize.y / 5f + elementSize.y / 2, parentSize.y / 2 - elementSize.y / 2, currentUIPosition.y);  // Y���� 0���� 1 ���̷� ��ȯ
        mappedValue = Mathf.Clamp(mappedValue, 0.50f, 0.7f);  // 0.75 ~ 1 ���̷� ����
        cinemachineComposer.m_ScreenY = mappedValue;

        // ���� UI ��ġ ������Ʈ
        previousUIPosition = currentUIPosition;


    }




    private void MoveUIElement()
    {
        // ���콺 �̵� ��ȭ�� ��������
        float moveY = playerInput.yMouseMove;

        // ���� ���� ���Ͽ� UI �̵� �ݿ�

        moveY *= yMouseSensitivity;



        // UI ����� ���� ��ġ�� �̵��� �ݿ�
        Vector3 currentPosition = uiElement.localPosition;

        currentPosition.y += moveY;



        Vector2 parentSize = parentRectTransform.rect.size;  // �θ� ĵ���� ũ��
        Vector2 elementSize = uiElement.rect.size;  // UI ����� ũ��

        // �̵� ���� ���� (UI ��Ұ� �θ� ĵ������ ����� �ʵ���)
        currentPosition.y = Mathf.Clamp(currentPosition.y, -parentSize.y / 5f + elementSize.y / 2, parentSize.y / 2 - elementSize.y / 2);

        // ���ο� ��ġ�� UI ��� �̵�
        uiElement.localPosition = currentPosition;

    }

    //�������� ���� ��ǥ�� ���� ��ǥ�� ��ȯ - Gun ������Ʈ���� �� �� ��ġ ���Ҷ� ���
    public Vector3 LocalPosToWorldDirection()
    {
        Ray ray = mainCamera.ScreenPointToRay(uiElement.position);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            lookAtPoint = hitInfo.point; // ���콺�� ����Ű�� ���� ��ǥ

        }
        Vector3 direction = (lookAtPoint - playerShooter.gun.fireTransform.transform.position).normalized; // �ٶ� ���� ���
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
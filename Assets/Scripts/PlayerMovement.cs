using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cinemachine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    // [HideInInspector] public float verticalMoveSpeed = 5f;//�յ� ������ �ӵ�
    // [HideInInspector] public float horizontalMoveSpeed = 2.5f;//�翷 ������ �ӵ�
    [HideInInspector] public float xMouseSensitivity = 1f; //�¿� ���콺 ������ �ӵ�
    [HideInInspector] public float yMouseSensitivity = 1f; //���� ���콺 ������ �ӵ�
    public RectTransform uiElement; // �̵��� UI ����� RectTransform
    private RectTransform parentRectTransform;

    private Rigidbody playerRigidbody;
    private PlayerInput playerInput;
    private PlayerHealth playerHealth;
    public CinemachineVirtualCamera virtualCamera;  // �ó׸ӽ� ���� ī�޶�
    private CinemachineComposer cinemachineComposer;  // CinemachineComposer

    private Vector3 previousUIPosition; // uiElement�� ���� ��ġ�� ����
    private float xRotation = 0f; // x�� ȸ�� ���� �� (ī�޶� pitch)

    // Start is called before the first frame update
    private void Start()
    {
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        //����� ������Ʈ ��������
        playerRigidbody = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        playerHealth = GetComponent<PlayerHealth>();
        parentRectTransform = uiElement.parent.GetComponent<RectTransform>();
        cinemachineComposer = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineComposer>();

        xMouseSensitivity = 13f;
        yMouseSensitivity = 40f;
        // �ʱ� UI ��ġ ����
        previousUIPosition = uiElement.localPosition;

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        MoveUIElement();
        Rotation();
        Move();
    }


    //������ �޼���
    private void Move()
    {
        //��������� �̵��� �Ÿ� ���
        Vector3 moveDistance = ((playerInput.verticalMove * transform.forward * playerHealth.moveSpeed) + (playerInput.horizontalMove * transform.right * playerHealth.moveSpeed)) * Time.deltaTime;

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
        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);


        // Cinemachine�� m_ScreenY ���� uiElement�� y ��ġ�� ���� ���ѵ� ������ ����
        float mappedValue = Mathf.InverseLerp(-parentSize.y / 10f + elementSize.y / 2, parentSize.y / 2 - elementSize.y / 2, currentUIPosition.y);  // Y���� 0���� 1 ���̷� ��ȯ
        mappedValue = Mathf.Clamp(mappedValue, 0.50f, 0.9f);  // 0.75 ~ 1 ���̷� ����
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
        currentPosition.y = Mathf.Clamp(currentPosition.y, -parentSize.y / 10f + elementSize.y / 2, parentSize.y / 2 - elementSize.y / 2);

        // ���ο� ��ġ�� UI ��� �̵�
        uiElement.localPosition = currentPosition;

    }

}

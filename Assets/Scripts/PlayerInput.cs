using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class PlayerInput : MonoBehaviour
{
    [HideInInspector] public string verticalMoveAxisName = "Vertical";//�յ� �̵� ����Ű �Է� ��ư �̸�
    [HideInInspector] public string horizontalMoveAxisName = "Horizontal";//�� �̵� ����Ű �Է� ��ư �̸�
    [HideInInspector] public string xMouseMoveAxisName = "Mouse X";//x�� ���콺 �Է� ���� �̸�
    [HideInInspector] public string yMouseMoveAxisName = "Mouse Y";//y�� ���콺 �Է� ���� �̸�
    [HideInInspector] public string sprintButtonName = "Sprint";//�޸��� �Է� ��ư �̸�
    [HideInInspector] public string crouchButtonName = "Crouch";//�ɱ� �Է� ��ư �̸�
    [HideInInspector] public string fireButtonName = "Fire1";//�߻� �Է� ��ư �̸�
    [HideInInspector] public string reloadButtonName = "Reload";//������ �Է� ��ư �̸�
    [HideInInspector] public string skill_1_ButtonName = "Skill 1";//1�� ��ų �Է� ��ư �̸�
    [HideInInspector] public string skill_2_ButtonName = "Skill 2";//2�� ��ų �Է� ��ư �̸�

    //[HideInInspector] public string skill_3_ButtonName = "Skill3";//3�� ��ų �Է� ��ư �̸�

    public Vector3 mousePoint { get; private set; }
    public float verticalMove { get; private set; }
    public float horizontalMove { get; private set; }
    public float xMouseMove { get; private set; }
    public float yMouseMove { get; private set; }
    public bool sprintButton { get; private set; }
    public bool crouchButton { get; private set; }
    public bool fireButton { get; private set; }
    public bool reloadButton { get; private set; }
    public bool skill_1_Button { get; private set; }
    public bool skill_2_Button { get; private set; }
    //public bool skill_3_Button { get; private set; }




    // Update is called once per frame
    void Update()
    {
        mousePoint = Input.mousePosition;
        verticalMove = Input.GetAxis(verticalMoveAxisName);
        horizontalMove = Input.GetAxis(horizontalMoveAxisName);
        xMouseMove = Input.GetAxis(xMouseMoveAxisName);
        yMouseMove = Input.GetAxis(yMouseMoveAxisName);
        sprintButton = Input.GetButton(sprintButtonName);
        crouchButton = Input.GetButton(crouchButtonName);
        fireButton = Input.GetButton(fireButtonName);
        reloadButton = Input.GetButtonDown(reloadButtonName);
        skill_1_Button = Input.GetButtonDown(skill_1_ButtonName);
        skill_2_Button = Input.GetButtonDown(skill_2_ButtonName);
        //skill_3_Button = Input.GetButtonDown(skill_3_ButtonName);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class PlayerInput : MonoBehaviour
{
    [HideInInspector] public string verticalMoveAxisName = "Vertical";//앞뒤 이동 방향키 입력 버튼 이름
    [HideInInspector] public string horizontalMoveAxisName = "Horizontal";//옆 이동 방향키 입력 버튼 이름
    [HideInInspector] public string xMouseMoveAxisName = "Mouse X";//x축 마우스 입력 방향 이름
    [HideInInspector] public string yMouseMoveAxisName = "Mouse Y";//y축 마우스 입력 방향 이름
    [HideInInspector] public string sprintButtonName = "Sprint";//달리기 입력 버튼 이름
    [HideInInspector] public string crouchButtonName = "Crouch";//앉기 입력 버튼 이름
    [HideInInspector] public string fireButtonName = "Fire1";//발사 입력 버튼 이름
    [HideInInspector] public string reloadButtonName = "Reload";//재장전 입력 버튼 이름
    /*    [HideInInspector] public string skill_1_ButtonName = "Skill1";//1번 스킬 입력 버튼 이름 
        [HideInInspector] public string skill_2_ButtonName = "Skill2";//2번 스킬 입력 버튼 이름 
        [HideInInspector] public string skill_3_ButtonName = "Skill3";//3번 스킬 입력 버튼 이름*/

    public Vector3 mousePoint { get; private set; }
    public float verticalMove { get; private set; }
    public float horizontalMove { get; private set; }
    public float xMouseMove { get; private set; }
    public float yMouseMove { get; private set; }
    public bool sprintButton { get; private set; }
    public bool crouchButton { get; private set; }
    public bool fireButton { get; private set; }
    public bool reloadButton { get; private set; }
    /*    public bool skill_1_Button { get; private set; }
        public bool skill_2_Button { get; private set; }
        public bool skill_3_Button { get; private set; }*/




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
        /*        skill_1_Button = Input.GetButtonDown(skill_1_ButtonName);
                skill_2_Button = Input.GetButtonDown(skill_2_ButtonName);
                skill_3_Button = Input.GetButtonDown(skill_3_ButtonName);*/

    }
}

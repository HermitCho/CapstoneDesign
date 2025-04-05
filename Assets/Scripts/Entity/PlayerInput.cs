using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Assertions.Comparers;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    // Input Actions 객체
    [HideInInspector] public PlayerActions inputActions;
    [HideInInspector] public InputActionAsset ActionAsset => inputActions.asset;

    // PlayerInput 프로퍼티
    public Vector3 mousePoint { get; private set; }
    public float verticalMove { get; private set; }
    public float horizontalMove { get; private set; }
    public float xMouseMove { get; private set; }
    public float yMouseMove { get; private set; }
    public float sprintButton { get; private set; }
    public bool crouchButton { get; private set; }
    public bool fireButton { get; private set; }
    public bool reloadButton { get; private set; }
    public bool skill_1_Button { get; private set; }
    public bool skill_2_Button { get; private set; }
    public bool handleGunButton { get; private set; }

    // PlayerInput 초기화
    private void Awake()
    {
        inputActions = new PlayerActions();

        // Bind input actions
        BindInputActions();
    }

    // PlayerInputActions 바인딩
    private void BindInputActions()
    {
        // 이동 입력 처리
        inputActions.PlayerAction.Move.performed += ctx =>
        {
            Vector2 move = ctx.ReadValue<Vector2>();
            verticalMove = move.y;
            horizontalMove = move.x;
        };
        inputActions.PlayerAction.Move.canceled += ctx =>
        {
            verticalMove = 0;
            horizontalMove = 0;
        };

        // 마우스 움직임 처리
        inputActions.PlayerAction.Look.performed += ctx =>
        {
            Vector2 look = ctx.ReadValue<Vector2>();
            xMouseMove = look.x;
            yMouseMove = look.y;
        };
        inputActions.PlayerAction.Look.canceled += ctx =>
        {
            xMouseMove = 0;
            yMouseMove = 0;
        };

        // Sprint 입력 처리
        inputActions.PlayerAction.Sprint.performed += ctx => sprintButton = 1;
        inputActions.PlayerAction.Sprint.canceled += ctx => sprintButton = 0;

        // Crouch 입력 처리
        inputActions.PlayerAction.Crouch.started += ctx => crouchButton = true;
        inputActions.PlayerAction.Crouch.canceled += ctx => crouchButton = false;

        // Fire 입력 처리
        inputActions.PlayerAction.Fire.started += ctx => fireButton = true;
        inputActions.PlayerAction.Fire.canceled += ctx => fireButton = false;

        // Reload 입력 처리 (버튼을 누를 때만 true, 떼면 false)
        inputActions.PlayerAction.Reload.started += ctx => reloadButton = true;  // 버튼 눌렀을 때 true
        inputActions.PlayerAction.Reload.canceled += ctx => reloadButton = false; // 버튼 뗐을 때 false


        // Skill1 입력 처리 (버튼을 누를 때만 true, 떼면 false)
        inputActions.PlayerAction.Skill1.started += ctx => skill_1_Button = true;  // 버튼 눌렀을 때 true
        inputActions.PlayerAction.Skill1.canceled += ctx => skill_1_Button = false; // 버튼 뗐을 때 false

        // Skill2 입력 처리 (버튼을 누를 때만 true, 떼면 false)
        inputActions.PlayerAction.Skill2.started += ctx => skill_2_Button = true;  // 버튼 눌렀을 때 true
        inputActions.PlayerAction.Skill2.canceled += ctx => skill_2_Button = false; // 버튼 뗐을 때 false

        // HandleGun 입력 처리 (버튼을 누를 때만 true, 떼면 false)
        inputActions.PlayerAction.HandleGun.started += ctx => handleGunButton = true;  // 버튼 눌렀을 때 true
        inputActions.PlayerAction.HandleGun.canceled += ctx => handleGunButton = false; // 버튼 뗐을 때 false
    }

    private void OnEnable()
    {
        inputActions.PlayerAction.Enable();
    }

    private void OnDisable()
    {
        inputActions.PlayerAction.Disable();
    }

    private void Update()
    {
        mousePoint = Mouse.current.position.ReadValue();
    }
}
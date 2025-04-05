using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq;


public class KeyRebind : MonoBehaviour
{
    public PlayerInput playerInput;               // PlayerInput 컴포넌트 참조
    public GameObject rebindPanel;                // "바꿀 키를 입력하시오" UI Panel

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private string actionToRebind;




    private void Awake()
    {
        rebindPanel.SetActive(false);
    }



    public void OnRebindReloadKey()
    {
        KeyRebinding("Reload");
    }

    public void OnRebindCrouchKey()
    {
        KeyRebinding("Crouch");
    }

    public void OnRebindSprintKey()
    {
        KeyRebinding("Sprint");
    }

    public void OnRebindSkill1Key()
    {
        KeyRebinding("Skill1");
    }

    public void OnRebindSkill2Key()
    {
        KeyRebinding("Skill2");
    }

    public void OnRebindHandleGunKey()
    {
        KeyRebinding("HandleGun");
    }

    public void OnRebindUpKey()
    {
        KeyRebinding("Move","up");
    }
    public void OnRebindDownKey()
    {
        KeyRebinding("Move", "down");
    }

    public void OnRebindLeftKey()
    {
        KeyRebinding("Move", "left");
    }

    public void OnRebindRightKey()
    {
        KeyRebinding("Move", "right");
    }

    // 버튼에서 action 이름을 넘겨받아 키 리바인딩 시작  (단일 바인드용)
    public void KeyRebinding(string actionName)
    {
        actionToRebind = actionName;

        InputAction action = playerInput.inputActions.FindAction(actionName);
        if (action == null)
        {
            Debug.LogError("해당 InputAction을 찾을 수 없습니다: " + actionName);
            return;
        }

        rebindPanel.SetActive(true);

        playerInput.inputActions.PlayerAction.Disable(); // 입력 비활성화

        rebindingOperation = action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation =>
            {
                action.ApplyBindingOverride(operation.selectedControl.path);
                operation.Dispose();
                rebindPanel.SetActive(false);
                playerInput.inputActions.PlayerAction.Enable(); // 다시 입력 활성화
            })
            .Start();
    }

    // 버튼에서 action 이름을 넘겨받아 키 리바인딩 시작 (Composite 바인드용)
    public void KeyRebinding(string actionName, string bindingName)
    {


        actionToRebind = actionName;

        InputAction action = playerInput.ActionAsset.FindAction(actionName);

        if (action == null)
        {
            Debug.LogError("해당 InputAction을 찾을 수 없습니다: " + actionName);
            return;
        }

        // composite 내 특정 바인딩 찾기 ( "Up", "Down" 등)
        int bindingIndex = action.bindings.ToList().FindIndex(b => b.name == bindingName && b.isPartOfComposite);
        if (bindingIndex == -1)
        {
            Debug.LogError($"'{bindingName}' 바인딩을 찾을 수 없습니다. Action: {actionName}");
            return;
        }

        rebindPanel.SetActive(true);

        playerInput.inputActions.PlayerAction.Disable();

        rebindingOperation = action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation =>
            {
                action.ApplyBindingOverride(bindingIndex, operation.selectedControl.path);
                operation.Dispose();
                rebindPanel.SetActive(false);
                playerInput.inputActions.PlayerAction.Enable();
            })
            .Start();
    }

    public void CancelRebinding()
    {
        if (rebindingOperation != null)
        {
            rebindingOperation.Cancel();
            rebindingOperation.Dispose();
        }

        rebindPanel.SetActive(false);
        playerInput.inputActions.PlayerAction.Enable();
    }








}

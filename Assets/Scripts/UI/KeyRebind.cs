using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq;


public class KeyRebind : MonoBehaviour
{
    public PlayerInput playerInput;               // PlayerInput ������Ʈ ����
    public GameObject rebindPanel;                // "�ٲ� Ű�� �Է��Ͻÿ�" UI Panel

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

    // ��ư���� action �̸��� �Ѱܹ޾� Ű �����ε� ����  (���� ���ε��)
    public void KeyRebinding(string actionName)
    {
        actionToRebind = actionName;

        InputAction action = playerInput.inputActions.FindAction(actionName);
        if (action == null)
        {
            Debug.LogError("�ش� InputAction�� ã�� �� �����ϴ�: " + actionName);
            return;
        }

        rebindPanel.SetActive(true);

        playerInput.inputActions.PlayerAction.Disable(); // �Է� ��Ȱ��ȭ

        rebindingOperation = action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation =>
            {
                action.ApplyBindingOverride(operation.selectedControl.path);
                operation.Dispose();
                rebindPanel.SetActive(false);
                playerInput.inputActions.PlayerAction.Enable(); // �ٽ� �Է� Ȱ��ȭ
            })
            .Start();
    }

    // ��ư���� action �̸��� �Ѱܹ޾� Ű �����ε� ���� (Composite ���ε��)
    public void KeyRebinding(string actionName, string bindingName)
    {


        actionToRebind = actionName;

        InputAction action = playerInput.ActionAsset.FindAction(actionName);

        if (action == null)
        {
            Debug.LogError("�ش� InputAction�� ã�� �� �����ϴ�: " + actionName);
            return;
        }

        // composite �� Ư�� ���ε� ã�� ( "Up", "Down" ��)
        int bindingIndex = action.bindings.ToList().FindIndex(b => b.name == bindingName && b.isPartOfComposite);
        if (bindingIndex == -1)
        {
            Debug.LogError($"'{bindingName}' ���ε��� ã�� �� �����ϴ�. Action: {actionName}");
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

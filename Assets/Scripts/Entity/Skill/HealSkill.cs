using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealSkill : Skill
{
    float coolTime = 5f; // ȸ�� ��ų ��Ÿ��
    private int count = 3; // ȸ�� ��ų ����
    int recoverHealth = 10; // ȸ�� ��ų�� ȸ���� ü��
    PlayerMovement playerMovement; // ȸ�� ��ų�� ���� ĳ������ �÷��̾� �����Ʈ ������Ʈ
    PlayerInput playerInput; // ȸ�� ��ų�� ���� ĳ������ �÷��̾� ��ǲ ������Ʈ
    PlayerHealth playerHealth; // ȸ�� ��ų�� ���� ĳ������ �÷��̾� �ｺ ������Ʈ

    public override void OnEnable()
    {
        base.OnEnable();
        maxCoolDown = coolTime;
        maxSkillCount = count;
        currentSkillCount = maxSkillCount;

        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    public override void inputSkillKey()
    {
        base.inputSkillKey();
        UIManager.Instance.SelectGunORSkillUI(2); // �ΰ��� UI�� ����ź ������ ǥ��, ��ų 2�� Ű�� �������� 2 ����

        count -= 1;
        invokeSkill();
    }

    public override void invokeSkill()
    {
        base.invokeSkill();
        Debug.Log("ȸ�� ��ų ���");
        RaycastHit? hitInfo = playerMovement.LocalPosToWorldRaycast();
        Debug.Log(hitInfo);
        if (hitInfo.Value.collider.tag == "Team")
        {
            PlayerHealth teamPlayerHealth = hitInfo.Value.collider.GetComponent<PlayerHealth>();
            if (teamPlayerHealth != null)
            {
                // playerHealth ������Ʈ�� �����ϸ� ����� �� ����
                teamPlayerHealth.RestoreHealth(10); // ü�� 10 ȸ��
            }
        }
        else
        {
            playerHealth.RestoreHealth(recoverHealth); // ü�� 10 ȸ��
        }
    }

    private void Update()
    {
        skillCountCheck();

        if (playerInput.skill_2_Button && currentCoolDown <= 0)
        {
            inputSkillKey();
        }
    }
}

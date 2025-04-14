using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealSkill : Skill
{
    float coolTime = 5f; // 회복 스킬 쿨타임
    private int count = 3; // 회복 스킬 개수
    int recoverHealth = 10; // 회복 스킬로 회복할 체력
    PlayerMovement playerMovement; // 회복 스킬을 가진 캐릭터의 플레이어 무브먼트 컴포넌트
    PlayerInput playerInput; // 회복 스킬을 가진 캐릭터의 플레이어 인풋 컴포넌트
    PlayerHealth playerHealth; // 회복 스킬을 가진 캐릭터의 플레이어 헬스 컴포넌트

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
        UIManager.Instance.SelectGunORSkillUI(2); // 인게임 UI에 수류탄 아이콘 표시, 스킬 2번 키를 눌렀으니 2 전송

        count -= 1;
        invokeSkill();
    }

    public override void invokeSkill()
    {
        base.invokeSkill();
        Debug.Log("회복 스킬 사용");
        RaycastHit? hitInfo = playerMovement.LocalPosToWorldRaycast();
        Debug.Log(hitInfo);
        if (hitInfo.Value.collider.tag == "Team")
        {
            PlayerHealth teamPlayerHealth = hitInfo.Value.collider.GetComponent<PlayerHealth>();
            if (teamPlayerHealth != null)
            {
                // playerHealth 컴포넌트가 존재하면 사용할 수 있음
                teamPlayerHealth.RestoreHealth(10); // 체력 10 회복
            }
        }
        else
        {
            playerHealth.RestoreHealth(recoverHealth); // 체력 10 회복
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

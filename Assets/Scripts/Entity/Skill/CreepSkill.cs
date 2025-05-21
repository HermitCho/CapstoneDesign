using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepSkill : Skill
{
    public CreepSkill()
    {
        skillType = SkillType.cooldown;
    }

    private PlayerMovement playerMovement;
    private PlayerInput playerInput;
    float coolTime = 10f; // 스킬 쿨타임
    void OnEnable()
    {
        maxCoolDown = coolTime;
        playerMovement = GetComponent<PlayerMovement>();
        playerInput = GetComponent<PlayerInput>();
    }

    public override void invokeSkill()
    {
        base.invokeSkill();
        if (playerMovement != null)
            playerMovement.creeper = true; // 스킬 사용 시 creeper 활성화
        StartCoroutine(EndCreepAfterDelay(5f));
    }

    private IEnumerator EndCreepAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EndCreep();
    }
    public void EndCreep()
    {
        if (playerMovement != null)
            playerMovement.creeper = false; // 스킬 종료 시 creeper 비활성화
    }

    void Update()
    {
        skillCoolDownCheck();

        if (playerInput.skill_1_Button && checkSkill == true)
        {
            invokeSkill();
        }
    }
}

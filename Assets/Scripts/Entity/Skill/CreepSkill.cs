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
    float skill_duration = 7f; //스킬 지속시간
    [SerializeField] GameObject skillEffect; // 스킬 이펙트


    void Start()
    {
        maxCoolDown = coolTime;
        playerMovement = GetComponent<PlayerMovement>();
        playerInput = GetComponent<PlayerInput>();
        audioSource = GetComponent<AudioSource>();
    }

    public override void inputSkillKey()
    {
        invokeSkill();
    }

    public override void invokeSkill()
    {
        base.invokeSkill();
        if (playerMovement != null)
        {
            playerMovement.creeper = true; // 스킬 사용 시 creeper 활성화
            GameObject effect = Instantiate(skillEffect, transform.position, transform.rotation);
            Destroy(effect, 2f); // 2초 후에 이펙트 제거
            audioSource.PlayOneShot(skillSound);
            StartCoroutine(EndCreepAfterDelay(skill_duration));
        }
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

        if (playerInput.skill_2_Button && checkSkill == true)
        {
            inputSkillKey();
        }
    }
}

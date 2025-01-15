using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeSkill : Skill
{
    private int damage = 90;
    private float explosionCount = 5f;
    private float coolTime = 3f;
    private int count = 2;
    private PlayerInput playerInput;

    // 수류탄 스킬 초기화
    public override void OnEnable()
    {
        base.OnEnable();
        maxCoolTime = coolTime;
        maxSkillCount = count;
        currentSkillCount = maxSkillCount;
        playerInput = GetComponent<PlayerInput>();
    }

    public override void inputSkillKey()
    {
        base.inputSkillKey();
    }

    public override void invokeSkill()
    {
        base.invokeSkill();
        Debug.Log("수류탄 투척!");
    }

    // Update is called once per frame
    void Update()
    {
        countCoolTime();

        if (playerInput.skill_1_Button)
        {
            inputSkillKey();
        }

    }
}

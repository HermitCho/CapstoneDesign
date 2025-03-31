using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �̸��� ���ﱤ �ѱ� ���� �ӵ�, ������ �ӵ��� ������ �ϴ� ��ų��.
/// </summary>
public class WarmongerSkill : Skill
{
    float coolTime = 10f; // ���ﱤ ��Ÿ��
    float currentCoolTime = 0; // ���ﱤ ��ų ��Ÿ��
    int count = 99; // ���ﱤ ��ų ���� -> ��Ÿ�Ӹ� �Ǹ� �������� �� �� �ֵ��� �� ���� 99�� ����
    PlayerInput playerInput; // ���ﱤ ��ų�� ���� ĳ������ Ű��ǲ ������Ʈ
    PlayerMovement playerMovement;
    GunData gunData;

    public Gun gun;

    bool onSkill = false; // ���ﱤ ��ų ��� ������ Ȯ��
    float skillDuration = 5f; // ���ﱤ ��ų ���� �ð�
    float nowSkillDuration = 0f; // ���ﱤ ��ų ���� ���� �ð�

    ParticleSystem particleSystem;

    // ���ﱤ ��ų �ʱ�ȭ
    public override void OnEnable()
    {
        base.OnEnable();
        maxCoolTime = coolTime;
        maxSkillCount = count;
        currentSkillCount = maxSkillCount;
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        gunData = gun.gunData;
        particleSystem = GetComponent<ParticleSystem>();
        particleSystem.time = skillDuration;
    }

    // ��ų Ű �Է� ��
    public override void inputSkillKey()
    {
        base.inputSkillKey();
        if (checkSkill == true)
        {
            invokeSkill();
        }
    }

    // ��ų�� ���������� ���Ǵ� �Լ�
    public override void invokeSkill()
    {
        base.invokeSkill();
        Debug.Log("���ﱤ ��ų ���!");
        onSkill = true;
        gunData.reloadTime = gunData.reloadTime / 2;
        playerMovement.verticalMoveSpeed = playerMovement.verticalMoveSpeed * 2;
        playerMovement.horizontalMoveSpeed = playerMovement.horizontalMoveSpeed * 2;
        playerMovement.sprintSpeed = playerMovement.sprintSpeed * 2;
        particleSystem.Play();
    }

    //��ų ��Ÿ�� ������ ��ų ���� �ð� ���� + Ű �Է� �ν�
    void Update()
    {
        countCoolTime();
        SkillDurating();

        if (currentCoolTime >= 0f && playerInput.skill_1_Button)
        {
            inputSkillKey();
        }
    }
    
    //��ų ���� ���� �Լ�
    void SkillDurating()
    {
        if (nowSkillDuration < skillDuration && onSkill)
        {
            nowSkillDuration += Time.deltaTime;
        }
        else if(nowSkillDuration >= skillDuration)
        {
            Debug.Log("���ﱤ ��ų ����!");
            gunData.reloadTime = gunData.reloadTime * 2;
            playerMovement.verticalMoveSpeed = playerMovement.verticalMoveSpeed / 2;
            playerMovement.horizontalMoveSpeed = playerMovement.horizontalMoveSpeed / 2;
            playerMovement.sprintSpeed = playerMovement.sprintSpeed / 2;

            nowSkillDuration = 0f;
            onSkill = false;
        }
    }
}

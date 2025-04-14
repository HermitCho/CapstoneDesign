using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �̸��� ���ﱤ �ѱ� ���� �ӵ�, ������ �ӵ��� ������ �ϴ� ��ų��.
/// </summary>
public class WarmongerSkill : Skill
{

    public WarmongerSkill()
    {
        skillType = SkillType.instantCooldown;
    }

    //��ų ���� ����
    float coolTime = 10f; // ���ﱤ ��ų ��� �� ��Ÿ��
    float currentCoolDown = 0; // ���ﱤ ��ų ���� ��Ÿ��

    bool onSkill = false; // ���ﱤ ��ų ��� ������ Ȯ��
    float skillDuration = 5f; // ���ﱤ ��ų ���� �ð�
    float nowSkillDuration = 0f; // ���ﱤ ��ų ���� ���� �ð�

    ParticleSystem particleSystem; // ���ﱤ ��ų ��� ��ƼŬ


    // ��ų ���� ����
    PlayerInput playerInput; // ĳ������ Ű��ǲ ������Ʈ
    PlayerMovement playerMovement; // ĳ���� ������ ������Ʈ

    [SerializeField] Gun gun; // ��� ĳ������ �ѱ�
    GunData gunData; // ���ﱤ ��ų�� ���� ĳ���� �ѱ� ������



    // ���ﱤ ��ų �ʱ�ȭ
    public override void OnEnable()
    {
        base.OnEnable();
        maxCoolDown = coolTime; // �ִ� ��Ÿ�� ����


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

        UIManager.Instance.CoolDownButtonInput(2); // ������ ������Ʈ

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
        skillCoolDownCheck();
        SkillDurating();

        if (currentCoolDown >= 0f && playerInput.skill_2_Button)
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

            Debug.Log(gunData.reloadTime);
            Debug.Log(playerMovement.verticalMoveSpeed);
            Debug.Log(playerMovement.horizontalMoveSpeed);
            Debug.Log(playerMovement.sprintSpeed);

            nowSkillDuration = 0f;
            onSkill = false;
        }
    }
}

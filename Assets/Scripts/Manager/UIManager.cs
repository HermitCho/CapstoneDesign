using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEditor.Experimental.GraphView;
using VFolders.Libs;

public class UIManager : Singleton<UIManager>
{
    Skill[] skills; // ��ų �����͵�
    Gun gun; // ĳ������ �� ������

    [SerializeField] private Image gunImage;  // �� ������
    [SerializeField] private TextMeshProUGUI ammoText; // ��ź/��ź ����

    Skill skill1; // ��ų1 ������
    [SerializeField] private Image skillImage_1; // ��ų ������1 ��ġ
    [SerializeField] private TextMeshProUGUI skill_CountOrCooldown_1; // ��ų ������1�� ���� or ��Ÿ��

    Skill skill2; // ��ų2 ������
    [SerializeField] private Image skillImage_2; // ��ų ������2 ��ġ
    [SerializeField] private TextMeshProUGUI skill_CountOrCooldown_2; // ��ų ������1�� ���� or ��Ÿ��

    PlayerCharacter playerCharacter; // ĳ������ �÷��̾�ĳ���� ������Ʈ
    GameObject player; // ĳ������ �÷��̾� ������Ʈ

    void Start()
    {
        GetDataForUI();
    }

    // ���� ���� �� UI ������ ���� ������ ��������
    void GetDataForUI()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerCharacter = player.GetComponent<PlayerHealth>().playerCharacter;
        gun = player.GetComponentInChildren<Gun>();

        skills = player.GetComponents<Skill>();

        foreach (Skill skill in skills)
        {
            if (skill is GrenadeSkill)
            {
                skill1 = skill;
                skillImage_1.DOFade(0.5f, 0.5f);
            }
            else if (skill is SmokeSkill)
            {
                skill1 = skill;
                skillImage_1.DOFade(0.5f, 0.5f);
            }

            else if (skill is WarmongerSkill)
            {
                skill2 = skill;
            }
            else if (skill is FlashbangSkill)
            {
                skill2 = skill;
                skillImage_1.DOFade(0.5f, 0.5f);
            }
            else if (skill is HealSkill) skill2 = skill;
        }
        SetUI();
    }

    // ���� ���� �� ������ �����͸� ���� ĳ������ UI ����
    void SetUI()
    {
        gunImage.sprite = playerCharacter.gunIcon;
        skillImage_1.sprite = playerCharacter.skillIcon_1;
        skillImage_2.sprite = playerCharacter.skillIcon_2;
        skillImage_1.preserveAspect = true;
        skillImage_2.preserveAspect = true;

        Image image1 = skillImage_1.transform.GetChild(0).GetComponent<Image>();
        Image image2 = skillImage_2.transform.GetChild(0).GetComponent<Image>();


        if (skill1.skillType == Skill.SkillType.cooldown || skill1.skillType == Skill.SkillType.instantCooldown)
        {
            image1.sprite = skillImage_1.sprite;
            image1.DOFade(0.5f, 0.5f);
            image1.preserveAspect = true;
        }
        else
        {
            image1.Destroy();
        }

        if (skill2.skillType == Skill.SkillType.cooldown || skill2.skillType == Skill.SkillType.instantCooldown)
        {
            image2.sprite = skillImage_2.sprite;
            image2.DOFade(0.5f, 0.5f);
            image2.preserveAspect = true;
        }
        else
        {
            image1.Destroy();
        }

        checkSkillType();
    }

    // UI ������Ʈ
    void Update()
    {
        // ���� �Ѿ� UI ǥ��
        ammoText.text = gun.magAmmo + " / " + gun.gunData.magCapacity;
        checkSkillType();
    }

    // ��ų Ÿ�Կ� ���� UI ����
    void checkSkillType()
    {
        UpdateSkillUIString(skill1, skill_CountOrCooldown_1);
        UpdateSkillUIString(skill2, skill_CountOrCooldown_2);
    }
    //checkSkillType()���� ��ų Ÿ�Կ� ���� UI ������Ʈ
    void UpdateSkillUIString(Skill skill, TextMeshProUGUI text)
    {
        if (skill.skillType == Skill.SkillType.instantCount)
        {
            text.text = skill.currentSkillCount.ToString();
        }
        else if (skill.skillType == Skill.SkillType.instantCooldown)
        {
            text.text = Math.Truncate(skill.currentCoolDown).ToString();
        }
        else if (skill.skillType == Skill.SkillType.count)
        {
            text.text = skill.currentSkillCount.ToString();
        }
        else if (skill.skillType == Skill.SkillType.cooldown)
        {
            text.text = Math.Truncate(skill.currentCoolDown).ToString();
        }
    }

    // ��/��ų ���� �� ������ ���� ����
    public void SelectGunORSkillUI(int iconNum)
    {
        switch (iconNum)
        {
            case 0: // �� ���õ�
                gunImage.DOFade(1f, 0.5f);
                UIImageFade(skill1, skillImage_1, false);
                UIImageFade(skill2, skillImage_2, false);
                break;
            case 1: // ��ų1 ���õ�
                gunImage.DOFade(0.5f, 0.5f);
                UIImageFade(skill1, skillImage_1, true);
                UIImageFade(skill2, skillImage_2, false);
                break;
            case 2: // ��ų2 ���õ�
                gunImage.DOFade(0.5f, 0.5f);
                UIImageFade(skill1, skillImage_1, false);
                UIImageFade(skill2, skillImage_2, true);
                break;
        }
    }

    // UI ������ ���� ���� ����
    void UIImageFade(Skill skill, Image image, bool isSelected)
    {
        if (skill.skillType == Skill.SkillType.count || skill.skillType == Skill.SkillType.cooldown)
        {
            float targetAlpha = isSelected ? 1f : 0.5f;
            image.DOFade(targetAlpha, 0.5f);
        }
    }

    // ��Ÿ�� ��ų ��� �� �ش� ��ų ��Ÿ�� ����
    public void CoolDownButtonInput(int iconNum)
    {
        switch (iconNum)
        {
            case 1:
                skillImage_1.fillAmount = 0f;
                skillImage_1.gameObject.GetComponent<Icon>().cooldownTime = skill1.maxCoolDown;
                skillImage_1.gameObject.GetComponent<Icon>().cooldownCheck = true;
                break;
            case 2:
                skillImage_2.fillAmount = 0f;
                skillImage_2.gameObject.GetComponent<Icon>().cooldownTime = skill2.maxCoolDown;
                skillImage_2.gameObject.GetComponent<Icon>().cooldownCheck = true;
                break;
        }
    }
}
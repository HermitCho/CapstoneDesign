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
    Skill[] skills; // 스킬 데이터들
    Gun gun; // 캐릭터의 총 데이터

    [SerializeField] private Image gunImage;  // 총 아이콘
    [SerializeField] private TextMeshProUGUI ammoText; // 장탄/잔탄 개수

    Skill skill1; // 스킬1 데이터
    [SerializeField] private Image skillImage_1; // 스킬 아이콘1 위치
    [SerializeField] private TextMeshProUGUI skill_CountOrCooldown_1; // 스킬 아이콘1의 개수 or 쿨타임

    Skill skill2; // 스킬2 데이터
    [SerializeField] private Image skillImage_2; // 스킬 아이콘2 위치
    [SerializeField] private TextMeshProUGUI skill_CountOrCooldown_2; // 스킬 아이콘1의 개수 or 쿨타임

    PlayerCharacter playerCharacter; // 캐릭터의 플레이어캐릭터 컴포넌트
    GameObject player; // 캐릭터의 플레이어 오브젝트

    void Start()
    {
        GetDataForUI();
    }

    // 게임 시작 전 UI 설정을 위한 데이터 가져오기
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

    // 게임 시작 전 가져온 데이터를 통해 캐릭터의 UI 설정
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

    // UI 업데이트
    void Update()
    {
        // 남은 총알 UI 표시
        ammoText.text = gun.magAmmo + " / " + gun.gunData.magCapacity;
        checkSkillType();
    }

    // 스킬 타입에 따라 UI 설정
    void checkSkillType()
    {
        UpdateSkillUIString(skill1, skill_CountOrCooldown_1);
        UpdateSkillUIString(skill2, skill_CountOrCooldown_2);
    }
    //checkSkillType()에서 스킬 타입에 따라 UI 업데이트
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

    // 총/스킬 선택 시 아이콘 투명도 변경
    public void SelectGunORSkillUI(int iconNum)
    {
        switch (iconNum)
        {
            case 0: // 총 선택됨
                gunImage.DOFade(1f, 0.5f);
                UIImageFade(skill1, skillImage_1, false);
                UIImageFade(skill2, skillImage_2, false);
                break;
            case 1: // 스킬1 선택됨
                gunImage.DOFade(0.5f, 0.5f);
                UIImageFade(skill1, skillImage_1, true);
                UIImageFade(skill2, skillImage_2, false);
                break;
            case 2: // 스킬2 선택됨
                gunImage.DOFade(0.5f, 0.5f);
                UIImageFade(skill1, skillImage_1, false);
                UIImageFade(skill2, skillImage_2, true);
                break;
        }
    }

    // UI 아이콘 투명도 변경 종류
    void UIImageFade(Skill skill, Image image, bool isSelected)
    {
        if (skill.skillType == Skill.SkillType.count || skill.skillType == Skill.SkillType.cooldown)
        {
            float targetAlpha = isSelected ? 1f : 0.5f;
            image.DOFade(targetAlpha, 0.5f);
        }
    }

    // 쿨타임 스킬 사용 시 해당 스킬 쿨타임 제어
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
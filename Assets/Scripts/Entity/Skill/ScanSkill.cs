using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanSkill : Skill
{
    public ScanSkill()
    {
        skillType = SkillType.cooldown;
    }
    private PlayerInput playerInput;
    private Animator animator;
    float coolTime = 10f; // 스킬 쿨타임
    void OnEnable()
    {
        maxCoolDown = coolTime;
        playerInput = GetComponent<PlayerInput>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    public override void inputSkillKey()
    {
        invokeSkill();
    }

    public override void invokeSkill()
    {
        base.invokeSkill();
        animator.SetTrigger("ScanUse");
        audioSource.PlayOneShot(skillSound);
        StartCoroutine(DetectAndHighlightEnemies());
    }

    IEnumerator DetectAndHighlightEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<Outline> changedOutlines = new List<Outline>();

        foreach (GameObject enemy in enemies)
        {
            Debug.Log(enemy.name);
            var outline = enemy.GetComponent<Outline>();
            if (outline == null)
                outline = enemy.AddComponent<Outline>();
            outline.enabled = true;
            outline.OutlineColor = Color.yellow;
            outline.OutlineWidth = 8f;
            changedOutlines.Add(outline);
        }

        yield return new WaitForSeconds(5f); // 5초간 하이라이트

        foreach (Outline outline in changedOutlines)
        {
            if (outline != null)
                outline.enabled = false;
        }
    }

    void Update()
    {
        skillCoolDownCheck();

        if (playerInput.skill_2_Button && checkSkill == true)
        {
            invokeSkill();
        }
    }
}
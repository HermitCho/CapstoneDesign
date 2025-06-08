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
        List<Renderer> changedRenderers = new List<Renderer>();
        List<Outline> changedOutlines = new List<Outline>();

        foreach (GameObject enemy in enemies)
        {
            Debug.Log(enemy.name);
            Renderer rend = enemy.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = Color.yellow; // 하이라이트 색상
                changedRenderers.Add(rend);
            }

            var outline = enemy.GetComponent<Outline>();
            if (outline == null)
                outline = enemy.AddComponent<Outline>();
            outline.enabled = true;
            outline.OutlineColor = Color.yellow;
            outline.OutlineWidth = 8f;
            changedOutlines.Add(outline);
        }

        yield return new WaitForSeconds(5f); // 5초간 하이라이트

        foreach (Renderer rend in changedRenderers)
        {
            if (rend != null)
                rend.material.color = Color.white; // 원래 색상(필요시 원래 색상 저장/복구)
        }

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

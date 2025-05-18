using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObstacleSkill : Skill
{
    public SpawnObstacleSkill()
    {
        skillType = SkillType.countCooldown;
    }

    [SerializeField] GameObject obstacle; // 실제로 설치될 장애물 프리팹
    [SerializeField] GameObject obstaclePreviewPrefab; // 투명한 미리보기(프리뷰) 장애물 프리팹
    GameObject currentPreview; // 현재 씬에 존재하는 프리뷰 오브젝트 인스턴스
    bool isPlacing = false; // 현재 장애물 설치 모드 여부
    bool isPreview = false; // 프리뷰 활성화 여부

    int count = 2; // 스킬 최대 사용 횟수
    float coolTime = 10f; // 스킬 쿨타임
    PlayerInput playerInput; // 플레이어 입력 컴포넌트 참조

    HandlingWeapon handlingWeapon; // 손에 든 무기에 관한 컴포넌트

    // 스킬이 활성화될 때 호출됨 (초기화)
    public override void OnEnable()
    {
        base.OnEnable();
        maxSkillCount = count; //최대 사용 횟수
        currentSkillCount = maxSkillCount;
        maxCoolDown = coolTime; //쿨타임
        playerInput = GetComponent<PlayerInput>();
        handlingWeapon = GetComponent<HandlingWeapon>();
    }

    // 스킬키 입력 감지 및 프리뷰 생성
    public override void inputSkillKey()
    {
        base.inputSkillKey();
        if (playerInput.skill_2_Button && checkSkill == true)
        {
            UIManager.Instance.SelectGunORSkillUI(2);
            if (isPlacing == false)
            {
                handlingWeapon.showGun = false;
                handlingWeapon.controlPlayerShooter(false);
                isPreview = true;
                if (currentPreview == null)
                {
                    currentPreview = Instantiate(obstaclePreviewPrefab);
                    SetPreviewTransparency(currentPreview, 0.5f); // 투명도 적용
                }
                isPlacing = true;
            }
        }
    }

    // 실제 스킬 발동(장애물 설치) 처리
    public override void invokeSkill()
    {
        base.invokeSkill();
        UIManager.Instance.CoolDownButtonInput(2); // 아이콘 업데이트
        SpawnObstacle();
    }

    // 매 프레임마다 호출됨
    void Update()
    {
        skillbothCheck();

        inputSkillKey();
        if (isPreview && obstaclePreviewPrefab != null)
        {
            UpdatePreviewPosition();
            if (Input.GetMouseButtonDown(0)) // 마우스 클릭 시 설치
            {
                invokeSkill();
            }
            else if (playerInput.handleGunButton || playerInput.skill_1_Button) // 취소
            {
                CancelPlacing();
            }
        }
    }

    // 마우스 위치에 따라 프리뷰 오브젝트 위치 및 회전 갱신
    void UpdatePreviewPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Debug.Log($"Raycast hit: {Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Ground"))}");
        Debug.Log($"Current Preview exists: {currentPreview != null}");
        if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Ground")))
        {
            if (currentPreview != null)
            {
                currentPreview.transform.position = hit.point + new Vector3(0, 0.65f, 0);
                float yRot = Camera.main.transform.eulerAngles.y;
                currentPreview.transform.rotation = Quaternion.Euler(0, yRot, 0);
            }
        }
    }

    // 실제 장애물 설치 및 프리뷰 제거
    void SpawnObstacle()
    {
        Instantiate(obstacle, currentPreview.transform.position, currentPreview.transform.rotation);
        Destroy(currentPreview);
        currentPreview = null;
        isPreview = false;
        isPlacing = false;
    }

    // 설치 취소 및 프리뷰 제거
    void CancelPlacing()
    {
        Destroy(currentPreview);
        currentPreview = null;
        isPreview = false;
        isPlacing = false;
    }

    // 프리뷰 오브젝트의 머티리얼 투명도 설정
    void SetPreviewTransparency(GameObject preview, float alpha)
    {
        Renderer[] renderers = preview.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material mat in renderer.materials)
            {
                Color color = mat.color;
                color.a = alpha;
                mat.color = color;
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
            }
        }
    }
}

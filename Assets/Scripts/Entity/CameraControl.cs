using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraControl : MonoBehaviour
{
    public Transform player; //플레이어 캐릭터
    public LayerMask obstacleLayer;//투명화 시킬 장애물 레이어
    public LayerMask roofLayer; // 완전 투명화 시킬 Roof 레이어

    private List<Renderer> previousObstacles = new List<Renderer>(); // 이전 프레임에서 투명했던 오브젝트
    private List<Renderer> previousRoofs = new List<Renderer>(); // 이전 프레임에서 투명했던 Roof

    // Start is called before the first frame update
    void Start()
    {
        InitializeTransparency(); // 모든 Obstacle & Roof 물체 초기화
    }

    void Update()
    {
        HandleObstacleTransparency(); // 장애물 투명화
        HandleRoofTransparency(); // Roof 투명화
    }

    /// <summary>
    /// 씬 시작 시 모든 Obstacle 및 Roof 물체를 초기화
    /// </summary>
    private void InitializeTransparency()
    {
        // 모든 Obstacle & Roof 오브젝트 찾기
        Collider[] allObstacles = FindObjectsOfType<Collider>();

        foreach (var collider in allObstacles)
        {
            Renderer renderer = collider.GetComponent<Renderer>();
            if (renderer != null)
            {
                int objLayer = collider.gameObject.layer;

                // 장애물(Opaque) 초기화
                if (((1 << objLayer) & obstacleLayer) != 0)
                {
                    SetOpaque(renderer);
                }
                // Roof(Opaque) 초기화
                else if (((1 << objLayer) & roofLayer) != 0)
                {
                    SetOpaque(renderer);
                }
            }
        }
    }

    /// <summary>
    /// 카메라에서 플레이어로 가는 길목의 장애물(Obstacle만)을 투명하게 처리
    /// </summary>
    private void HandleObstacleTransparency()
    {
        // 이전 프레임에서 투명했던 장애물 복원
        foreach (Renderer renderer in previousObstacles)
        {
            if (renderer != null)
            {
                SetOpaque(renderer);
            }
        }
        previousObstacles.Clear();

        // 카메라에서 플레이어로 Ray
        Vector3 direction = player.position - transform.position;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction.normalized, direction.magnitude, obstacleLayer);

        foreach (RaycastHit hit in hits)
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer != null)
            {
                SetTransparent(renderer);
                previousObstacles.Add(renderer);
            }
        }
    }

    /// <summary>
    /// 플레이어 기준 위 방향(Vector3.up)으로 Ray를 쏴서 Roof 물체를 감지하고 투명화
    /// </summary>
    private void HandleRoofTransparency()
    {
        // 이전 프레임에서 투명했던 Roof 물체 원상복구
        foreach (Renderer renderer in previousRoofs)
        {
            if (renderer != null)
            {
                SetOpaque(renderer);
            }
        }
        previousRoofs.Clear();

        // 플레이어 위치에서 위 방향으로 Ray 발사하여 Roof 감지
        RaycastHit hit;
        if (Physics.Raycast(player.position, Vector3.up, out hit, Mathf.Infinity, roofLayer))
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer != null)
            {
                SetFullyTransparent(renderer);
                previousRoofs.Add(renderer);
            }
        }
    }

    // 장애물 투명화 (일반)
    void SetTransparent(Renderer renderer)
    {
        foreach (Material mat in renderer.materials)
        {
            Color color = mat.color;
            color.a = 0.5f;
            mat.color = color;
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.renderQueue = 3000;
        }
    }

    // Roof 완전 투명화
    void SetFullyTransparent(Renderer renderer)
    {
        foreach (Material mat in renderer.materials)
        {
            Color color = mat.color;
            color.a = 0f;
            mat.color = color;
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.renderQueue = 3000;
        }
    }

    // 원상복구 (불투명)
    void SetOpaque(Renderer renderer)
    {
        foreach (Material mat in renderer.materials)
        {
            Color color = mat.color;
            color.a = 1f;
            mat.color = color;
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            mat.SetInt("_ZWrite", 1);
            mat.renderQueue = 2000;
        }
    }
}
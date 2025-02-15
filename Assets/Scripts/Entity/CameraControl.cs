using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform player; //플레이어 캐릭터
    public LayerMask obstacleLayer;//투명화 시킬 장애물 레이어

    private List<Renderer> previousObstacles = new List<Renderer>(); // 이전 프레임에서 투명했던 오브젝트
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        HandleTransparency();
    }


    private void HandleTransparency()
    {
        // 이전 프레임에서 투명했던 오브젝트 복원
        foreach (Renderer renderer in previousObstacles)
        {
            if (renderer != null)
            {
                SetOpaque(renderer);
            }
        }
        previousObstacles.Clear();

        //카메라에서 캐릭터로 ray 
        Vector3 direction = player.position - transform.position;
        direction.y = 2.0f;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction.normalized, direction.magnitude, obstacleLayer);

        foreach (RaycastHit hit in hits)
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if(renderer != null)
            {
                SetTransparent(renderer);
                previousObstacles.Add(renderer);
            }
        }

    }

    //투명도 조절 메서드
    void SetTransparent(Renderer renderer)
    {
        foreach (Material mat in renderer.materials)
        {
            Color color = mat.color;
            color.a = 0.5f; // 투명도 조절
            mat.color = color;
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.renderQueue = 3000;
        }
    }

    //투명도 복원 메서드
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

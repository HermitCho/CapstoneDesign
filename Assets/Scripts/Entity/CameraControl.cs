using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform player; //�÷��̾� ĳ����
    public LayerMask obstacleLayer;//����ȭ ��ų ��ֹ� ���̾�

    private List<Renderer> previousObstacles = new List<Renderer>(); // ���� �����ӿ��� �����ߴ� ������Ʈ
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
        // ���� �����ӿ��� �����ߴ� ������Ʈ ����
        foreach (Renderer renderer in previousObstacles)
        {
            if (renderer != null)
            {
                SetOpaque(renderer);
            }
        }
        previousObstacles.Clear();

        //ī�޶󿡼� ĳ���ͷ� ray 
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

    //���� ���� �޼���
    void SetTransparent(Renderer renderer)
    {
        foreach (Material mat in renderer.materials)
        {
            Color color = mat.color;
            color.a = 0.5f; // ���� ����
            mat.color = color;
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.renderQueue = 3000;
        }
    }

    //���� ���� �޼���
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

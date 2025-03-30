using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraControl : MonoBehaviour
{
    public Transform player; //�÷��̾� ĳ����
    public LayerMask obstacleLayer;//����ȭ ��ų ��ֹ� ���̾�
    public LayerMask roofLayer; // ���� ����ȭ ��ų Roof ���̾�

    private List<Renderer> previousObstacles = new List<Renderer>(); // ���� �����ӿ��� �����ߴ� ������Ʈ
    private List<Renderer> previousRoofs = new List<Renderer>(); // ���� �����ӿ��� �����ߴ� Roof

    // Start is called before the first frame update
    void Start()
    {
        InitializeTransparency(); // ��� Obstacle & Roof ��ü �ʱ�ȭ
    }

    void Update()
    {
        HandleObstacleTransparency(); // ��ֹ� ����ȭ
        HandleRoofTransparency(); // Roof ����ȭ
    }

    /// <summary>
    /// �� ���� �� ��� Obstacle �� Roof ��ü�� �ʱ�ȭ
    /// </summary>
    private void InitializeTransparency()
    {
        // ��� Obstacle & Roof ������Ʈ ã��
        Collider[] allObstacles = FindObjectsOfType<Collider>();

        foreach (var collider in allObstacles)
        {
            Renderer renderer = collider.GetComponent<Renderer>();
            if (renderer != null)
            {
                int objLayer = collider.gameObject.layer;

                // ��ֹ�(Opaque) �ʱ�ȭ
                if (((1 << objLayer) & obstacleLayer) != 0)
                {
                    SetOpaque(renderer);
                }
                // Roof(Opaque) �ʱ�ȭ
                else if (((1 << objLayer) & roofLayer) != 0)
                {
                    SetOpaque(renderer);
                }
            }
        }
    }

    /// <summary>
    /// ī�޶󿡼� �÷��̾�� ���� ����� ��ֹ�(Obstacle��)�� �����ϰ� ó��
    /// </summary>
    private void HandleObstacleTransparency()
    {
        // ���� �����ӿ��� �����ߴ� ��ֹ� ����
        foreach (Renderer renderer in previousObstacles)
        {
            if (renderer != null)
            {
                SetOpaque(renderer);
            }
        }
        previousObstacles.Clear();

        // ī�޶󿡼� �÷��̾�� Ray
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
    /// �÷��̾� ���� �� ����(Vector3.up)���� Ray�� ���� Roof ��ü�� �����ϰ� ����ȭ
    /// </summary>
    private void HandleRoofTransparency()
    {
        // ���� �����ӿ��� �����ߴ� Roof ��ü ���󺹱�
        foreach (Renderer renderer in previousRoofs)
        {
            if (renderer != null)
            {
                SetOpaque(renderer);
            }
        }
        previousRoofs.Clear();

        // �÷��̾� ��ġ���� �� �������� Ray �߻��Ͽ� Roof ����
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

    // ��ֹ� ����ȭ (�Ϲ�)
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

    // Roof ���� ����ȭ
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

    // ���󺹱� (������)
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
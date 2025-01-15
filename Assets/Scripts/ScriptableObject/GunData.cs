
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/GunData", fileName = "Gun Data")]
public class GunData : ScriptableObject
{
    [Header ("�ѱ� �߻� �Ҹ�")]
    [Tooltip ("�ѱ� �߻� �Ҹ� ���� Ŭ�� ���� - SoundClip")]
    public AudioClip shotClip; //�߻� �Ҹ�

    [Header("�ѱ� ������ �Ҹ�")]
    [Tooltip("�ѱ� ������ �Ҹ� ���� Ŭ�� ���� - SoundClip")]
    public AudioClip reloadClip; //������ �Ҹ�

    [Header("�ѱ� ������ ��ġ")]
    [Tooltip("�ѱ� ������ ��ġ ���� - Float")]
    public float damage = 1; //���ݷ�

    [Header("�ѱ� źâ �뷮")]
    [Tooltip("�ѱ� źâ �뷮 ��ġ ���� - Inteager")]
    public int magCapacity = 30; //źâ �뷮

    [Header("ź�� �߻� ����")]
    [Tooltip("ź�� �߻� ���� ���� - Float")]
    public float timeBetFire = 0.1f; //ź�� �߻� ����

    [Header("ź�� ������ �ð�")]
    [Tooltip("ź�� ������ �ð� ���� - Float")]
    public float reloadTime = 2.0f; //������ �ҿ� �ð� 


    [Header("�� ��Ÿ�")]
    [Tooltip("�� ��Ÿ� ���� - Float")]
    public float fireDistance = 50f; //�� ��Ÿ�
}

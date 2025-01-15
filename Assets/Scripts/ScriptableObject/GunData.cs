
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/GunData", fileName = "Gun Data")]
public class GunData : ScriptableObject
{
    [Header ("총기 발사 소리")]
    [Tooltip ("총기 발사 소리 사운드 클립 삽입 - SoundClip")]
    public AudioClip shotClip; //발사 소리

    [Header("총기 재장전 소리")]
    [Tooltip("총기 재장전 소리 사운드 클립 삽입 - SoundClip")]
    public AudioClip reloadClip; //재장전 소리

    [Header("총기 데미지 수치")]
    [Tooltip("총기 데미지 수치 설정 - Float")]
    public float damage = 1; //공격력

    [Header("총기 탄창 용량")]
    [Tooltip("총기 탄창 용량 수치 설정 - Inteager")]
    public int magCapacity = 30; //탄창 용량

    [Header("탄알 발사 간격")]
    [Tooltip("탄알 발사 간격 설정 - Float")]
    public float timeBetFire = 0.1f; //탄알 발사 간격

    [Header("탄알 재장전 시간")]
    [Tooltip("탄알 재장전 시간 설정 - Float")]
    public float reloadTime = 2.0f; //재장전 소요 시간 


    [Header("총 사거리")]
    [Tooltip("총 사거리 설정 - Float")]
    public float fireDistance = 50f; //총 사거리
}

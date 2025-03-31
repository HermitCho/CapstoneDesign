using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Scriptable/PlayerCharacter", fileName = "PlayerCharacter")]
public class PlayerCharacter : ScriptableObject
{
    public AudioClip deathClip; // 사망 소리
    public AudioClip hitClip; // 피격 소리
    public float maxHealth; //최대 체력
    public float maxShield; // 최대 추가 방어막
    public float frontBackMoveSpeed; // 앞뒤 기본 이동 속도
    public float leftRightMoveSpeed; // 좌우 기본 이동 속도
    public float sprintPlusSpeed; // 달리기 추가 속도
}

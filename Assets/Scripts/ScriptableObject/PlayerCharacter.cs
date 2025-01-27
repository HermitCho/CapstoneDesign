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
    public float defaultMoveSpeed; // 기본 이동 속도
}

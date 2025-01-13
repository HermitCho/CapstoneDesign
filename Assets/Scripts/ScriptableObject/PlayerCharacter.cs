using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Scriptable/PlayerCharacter", fileName = "PlayerCharacter")]
public class PlayerCharacter : ScriptableObject
{
    public AudioClip deathClip; // 사망 소리
    public AudioClip hitClip; // 피격 소리
    public float maxHealth; //최대 체력
    public float currentHealth; // 현재 체력
    public float defaultMoveSpeed; // 기본 이동 속도
    public float currentMoveSpeed; // 현재 이동 속도
}

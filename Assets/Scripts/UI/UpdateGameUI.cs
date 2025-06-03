using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

//포톤2 이용하여 플레이어 초상화 동기화
public class UpdateGameUI : MonoBehaviourPunCallbacks
{
    [SerializeField] private Image[] playerImages; // 플레이어 이미지 UI 배열
    [SerializeField] private Sprite[] playerSprites; // 플레이어 스프라이트 배열
    [SerializeField] private Sprite deadSprite; // 사망 시 표시할 스프라이트

    private Dictionary<int, int> playerNumberMap = new Dictionary<int, int>(); // 플레이어 ID와 이미지 번호 매핑
    private bool isInitialized = false;

    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            InitPlayerUI();
        }
    }

    void Update()
    {
        if (!isInitialized && PhotonNetwork.IsConnected)
        {
            InitPlayerUI();
        }
    }

    //포톤 서버에 들어온 순서에 따라 플레이어 초상화 Image 부여
    void InitPlayerUI()
    { 
        if (playerNumberMap.Count >= 4) return; // 이미 4명이 할당되었다면 리턴

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        int myNumber = playerCount;

        // 현재 플레이어의 번호를 저장
        if (!playerNumberMap.ContainsKey(PhotonNetwork.LocalPlayer.ActorNumber))
        {
            playerNumberMap[PhotonNetwork.LocalPlayer.ActorNumber] = myNumber;
            photonView.RPC("SyncPlayerNumber", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, myNumber);
        }

        // 이미지 할당
        if (myNumber <= 4 && myNumber > 0)
        {
            playerImages[myNumber - 1].sprite = playerSprites[myNumber - 1];
        }

        isInitialized = true;
    }

    [PunRPC]
    void SyncPlayerNumber(int playerId, int number)
    {
        if (!playerNumberMap.ContainsKey(playerId))
        {
            playerNumberMap[playerId] = number;
        }
    }

    void UpdatePlayerUI()
    {
        // 현재 플레이어의 LivingEntity 컴포넌트 가져오기
        LivingEntity livingEntity = GetComponent<LivingEntity>();
        if (livingEntity == null) return;

        // 플레이어가 죽었다면 이미지 변경
        if (livingEntity.dead)
        {
            int myNumber = playerNumberMap[PhotonNetwork.LocalPlayer.ActorNumber];
            if (myNumber <= 4 && myNumber > 0)
            {
                playerImages[myNumber - 1].sprite = deadSprite;
                photonView.RPC("SyncDeadStatus", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }
    }

    [PunRPC]
    void SyncDeadStatus(int playerId)
    {
        if (playerNumberMap.ContainsKey(playerId))
        {
            int playerNumber = playerNumberMap[playerId];
            if (playerNumber <= 4 && playerNumber > 0)
            {
                playerImages[playerNumber - 1].sprite = deadSprite;
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 새로 들어온 플레이어에게 현재 플레이어 번호 정보 전송
            foreach (var kvp in playerNumberMap)
            {
                photonView.RPC("SyncPlayerNumber", newPlayer, kvp.Key, kvp.Value);
            }
        }
    }
}

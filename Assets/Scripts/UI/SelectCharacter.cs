using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;
using UnityEngine.InputSystem;
using Cinemachine;
using Michsky.UI.Heat;

public class SelectCharacter : MonoBehaviour
{
    [Header("할당할 캐릭터 프리펩")]
    public GameObject[] characterPrefabs;

    [Header("패널 관리 - 할당 X")]
    [SerializeField] private PanelManager panelManager;

    [Header("캐릭터 선택 시간 텍스트")]
    public TMP_Text selectTimeText;

    [Header("캐릭터 선택 시간")]
    [SerializeField]
    private float selectTime = 10f;

    [Header("카메라 설정 - 할당 X")]
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineComposer cinemachineComposer;

    private int selectedCharacterIndex = -1;
    private bool isSelectionActive = false;
    private LoadSettings loadSettings;
    private GameObject selectedCharacter;
    private PlayerMovement playerMovement;

    private CameraControl cameraControl;

    void Awake()
    {
        cameraControl = FindObjectOfType<CameraControl>();

        // LoadSettings 컴포넌트 찾기
        loadSettings = FindObjectOfType<LoadSettings>();
        if (loadSettings == null)
        {
            Debug.LogError("LoadSettings를 찾을 수 없습니다!");
        }

        // PanelManager 찾기
        if (panelManager == null)
        {
            panelManager = FindObjectOfType<PanelManager>();
            if (panelManager == null)
            {
                Debug.LogError("PanelManager를 찾을 수 없습니다!");
            }
        }

        // 시네머신 가상 카메라 찾기
        if (virtualCamera == null)
        {
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            if (virtualCamera == null)
            {
                Debug.LogError("CinemachineVirtualCamera를 찾을 수 없습니다!");
            }
            else
            {
                // CinemachineComposer 컴포넌트 가져오기
                cinemachineComposer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
                if (cinemachineComposer == null)
                {
                    Debug.LogError("CinemachineComposer를 찾을 수 없습니다!");
                }
            }
        }

        // 선택 패널로 전환
        if (panelManager != null)
        {
            panelManager.OpenPanelByIndex(3); // 캐릭터 선택 패널로 전환
        }

        // 마우스 커서 표시 및 활성화
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 선택 시간 시작
        StartCoroutine(SelectTime());
    }

    private IEnumerator SelectTime()
    {
        isSelectionActive = true;
        float remainingTime = selectTime;

        while (remainingTime > 0 && isSelectionActive)
        {
            // 남은 시간 UI 업데이트
            selectTimeText.text = $"남은 시간: {Mathf.CeilToInt(remainingTime)} 초";
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        // 시간 종료 처리
        EndSelectTime();
    }

    public void OnClickSelectButton(int index)
    {
        if (!isSelectionActive) return;

        selectedCharacterIndex = index;
        Debug.Log($"캐릭터 {index + 1}번 선택됨");
    }

    public void SettingCharacter()
    {
        if (selectedCharacterIndex == -1) return;

        // 이전에 생성된 캐릭터가 있다면 제거
        if (selectedCharacter != null)
        {
            Destroy(selectedCharacter);
        }

        // 선택된 캐릭터 생성
        selectedCharacter = Instantiate(characterPrefabs[selectedCharacterIndex], Vector3.zero, Quaternion.identity);

       


        // PlayerMovement 컴포넌트 가져오기
        playerMovement = selectedCharacter.GetComponent<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement 컴포넌트를 찾을 수 없습니다!");
        }

        // AudioSource 컴포넌트 가져오기
        AudioSource playerAudioSource = selectedCharacter.GetComponent<AudioSource>();
        if (playerAudioSource == null)
        {
            Debug.LogError("AudioSource 컴포넌트를 찾을 수 없습니다!");
        }

        // 총 오디오 소스 찾기
        AudioSource gunAudioSource = null;
        Transform[] allChildren = selectedCharacter.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.CompareTag("Gun"))
            {
                gunAudioSource = child.GetComponent<AudioSource>();
                break;
            }
        }

        // 저장된 설정 불러오기
        if (loadSettings != null)
        {
            string savePath = System.IO.Path.Combine(Application.persistentDataPath, "keybinds_settings.json");
            if (System.IO.File.Exists(savePath))
            {
                try
                {
                    string jsonData = System.IO.File.ReadAllText(savePath);
                    var data = JsonUtility.FromJson<LoadSettings.SettingsData>(jsonData);

                    // 감도 설정 적용
                    if (playerMovement != null)
                    {
                        playerMovement.xMouseSensitivity = data.xSensitivity;
                        playerMovement.yMouseSensitivity = data.ySensitivity;
                        Debug.Log($"감도 설정 적용: X={data.xSensitivity}, Y={data.ySensitivity}");
                    }

                    // 소리 설정 적용
                    if (playerAudioSource != null)
                    {
                        playerAudioSource.volume = data.soundVolume;
                        Debug.Log($"플레이어 소리 설정 적용: {data.soundVolume}");
                    }
                    if (gunAudioSource != null)
                    {
                        gunAudioSource.volume = data.soundVolume;
                        Debug.Log($"총 소리 설정 적용: {data.soundVolume}");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"설정 파일을 로드하는 중 오류가 발생했습니다: {e.Message}");
                }
            }
        }

        // 카메라 설정
        if (virtualCamera != null && cinemachineComposer != null)
        {
            // Follow와 LookAt 설정
            virtualCamera.Follow = selectedCharacter.transform;
            virtualCamera.LookAt = selectedCharacter.transform;

            // PlayerMovement에 CinemachineComposer 설정
            if (playerMovement != null)
            {
                playerMovement.SetCinemachineComposer(cinemachineComposer);
            }

            Debug.Log("카메라 설정 완료");
        }
        else
        {
            Debug.LogError("카메라 컴포넌트를 찾을 수 없습니다!");
        }
        // PlayerInput 컴포넌트 가져오기
        PlayerInput playerInput = selectedCharacter.GetComponent<PlayerInput>();
        if (playerInput != null && loadSettings != null)
        {
            // LoadSettings에 PlayerInput 설정
            loadSettings.SetPlayerInput(playerInput);
            
            // 저장된 설정을 플레이어에 적용
            loadSettings.LoadSettingsToPlayer();
            Debug.Log("PlayerInput 설정 완료");
        }
        

        UIManager.Instance.GetDataForUI();
        
        Debug.Log($"캐릭터 설정 완료: {selectedCharacter.name}");
    }

    public void EndSelectTime()
    {
        isSelectionActive = false;
        selectTimeText.text = "시간 종료";

        // 선택된 캐릭터가 없으면 첫 번째 캐릭터 선택
        if (selectedCharacterIndex == -1)
        {
            selectedCharacterIndex = 0;
        }

        // 캐릭터 생성 및 설정
        SettingCharacter();
        cameraControl.SetPlayer(selectedCharacter.transform);
        // 게임 플레이 패널로 전환
        if (panelManager != null)
        {
            panelManager.OpenPanelByIndex(0); // 게임 플레이 패널로 전환
        }

        // 마우스 커서 숨기기 및 중앙 고정 (게임 시작 시에만)
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Debug.Log("캐릭터 선택 시간 종료 및 설정 완료");
    }
}

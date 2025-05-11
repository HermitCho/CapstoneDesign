using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class LobbyButton : MonoBehaviour
{
    [Header("Character Selection")]
    public GameObject[] characters;       // SampleScene에 미리 배치된 4개의 캐릭터
    public Image[] characterImages;       // 캐릭터 이미지 UI (Alpha 조정용)
    public TMP_Text timerText;           // 남은 시간 표시 UI
    public GameObject[] otherUIElements;  // 게임 중 활성화될 다른 UI (비활성화할 UI)
    public Canvas selectionCanvas;        // 캐릭터 선택 UI 캔버스

    private int selectedCharacterIndex = -1; // 선택된 캐릭터 인덱스 (-1은 선택되지 않음)
    private float selectionTime = 10f;       // 선택 가능 시간 (초)
    private bool isSelectionActive = false;  // 선택 가능 상태 확인
    private CameraControl cameraControl;     // 카메라 컨트롤 참조
    private KeyRebindManager keyRebindManager; // 키 리바인드 매니저 참조

    private void Awake()
    {
        // CameraControl 컴포넌트 찾기
        cameraControl = FindObjectOfType<CameraControl>();
        if (cameraControl == null)
        {
            Debug.LogError("CameraControl을 찾을 수 없습니다!");
        }

        // KeyRebindManager 컴포넌트 찾기
        keyRebindManager = FindObjectOfType<KeyRebindManager>();
        if (keyRebindManager == null)
        {
            Debug.LogError("KeyRebindManager를 찾을 수 없습니다!");
        }

        // 모든 캐릭터 비활성화 (초기 상태)
        foreach (GameObject character in characters)
        {
            character.SetActive(false);
        }

        // 다른 UI 요소 비활성화
        foreach (GameObject ui in otherUIElements)
        {
            ui.SetActive(false);
        }

        // 선택 시간 시작
        StartCoroutine(CharacterSelectionTimer());
    }

    // 캐릭터 선택 버튼 클릭 시 호출할 함수
    public void SelectCharacter(int index)
    {
        if (!isSelectionActive) return; // 선택 시간이 지나면 선택 불가

        // 선택된 캐릭터 이미지 알파 값 조정
        for (int i = 0; i < characterImages.Length; i++)
        {
            Color color = characterImages[i].color;
            color.a = (i == index) ? 1.0f : 0.5f; // 선택된 캐릭터는 불투명, 나머지는 반투명
            characterImages[i].color = color;
        }

        // 선택된 캐릭터 인덱스 저장
        selectedCharacterIndex = index;
    }

    // 선택 타이머
    private IEnumerator CharacterSelectionTimer()
    {
        isSelectionActive = true;
        float remainingTime = selectionTime;

        while (remainingTime > 0)
        {
            // 남은 시간 UI 업데이트
            timerText.text = $"남은 시간: {Mathf.CeilToInt(remainingTime)} 초";
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        // 시간 종료 처리
        isSelectionActive = false;
        timerText.text = "시간 종료";

        // 캐릭터 자동 선택 (선택된 캐릭터 없으면 첫 번째로)
        if (selectedCharacterIndex == -1)
        {
            selectedCharacterIndex = 0;
        }

        // 모든 캐릭터 비활성화 후 선택된 캐릭터만 활성화
        foreach (GameObject character in characters)
        {
            character.SetActive(false);
        }

        // 선택된 캐릭터 활성화
        characters[selectedCharacterIndex].SetActive(true);

        // CameraControl에 선택된 플레이어 설정
        if (cameraControl != null)
        {
            cameraControl.SetPlayer(characters[selectedCharacterIndex].transform);
        }

        // KeyRebindManager에 선택된 플레이어의 PlayerInput 설정
        if (keyRebindManager != null)
        {
            PlayerInput playerInput = characters[selectedCharacterIndex].GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                keyRebindManager.SetPlayerInput(playerInput);
            }
            else
            {
                Debug.LogError("선택된 캐릭터에 PlayerInput 컴포넌트가 없습니다!");
            }
        }

        // 게임 UI 활성화
        foreach (GameObject ui in otherUIElements)
        {
            ui.SetActive(true);
        }

        // 선택 UI 캔버스 비활성화
        if (selectionCanvas != null)
        {
            selectionCanvas.gameObject.SetActive(false);
        }

        Debug.Log("게임 시작: " + characters[selectedCharacterIndex].name);
    }

    // 게임 시작 버튼 클릭 시 호출할 함수 (선택 시간 이전 강제 시작 가능)
    public void StartGame()
    {
        if (!isSelectionActive)
        {
            Debug.LogWarning("게임이 이미 시작되었습니다.");
            return;
        }

        // 선택 시간 종료
        isSelectionActive = false;

        // 모든 캐릭터 비활성화 후 선택된 캐릭터만 활성화
        foreach (GameObject character in characters)
        {
            character.SetActive(false);
        }

        // 선택된 캐릭터 활성화
        if (selectedCharacterIndex == -1)
        {
            selectedCharacterIndex = 0; // 선택된 캐릭터가 없으면 첫 번째 자동 선택
        }

        characters[selectedCharacterIndex].SetActive(true);

        // CameraControl에 선택된 플레이어 설정
        if (cameraControl != null)
        {
            cameraControl.SetPlayer(characters[selectedCharacterIndex].transform);
        }

        // KeyRebindManager에 선택된 플레이어의 PlayerInput 설정
        if (keyRebindManager != null)
        {
            PlayerInput playerInput = characters[selectedCharacterIndex].GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                keyRebindManager.SetPlayerInput(playerInput);
            }
            else
            {
                Debug.LogError("선택된 캐릭터에 PlayerInput 컴포넌트가 없습니다!");
            }
        }

        // 게임 UI 활성화
        foreach (GameObject ui in otherUIElements)
        {
            ui.SetActive(true);
        }

        // 선택 UI 캔버스 비활성화
        if (selectionCanvas != null)
        {
            selectionCanvas.gameObject.SetActive(false);
        }

        // 게임 시작 로직 추가 (게임 매니저 호출 등)
        Debug.Log("게임 시작: " + characters[selectedCharacterIndex].name);
    }
}
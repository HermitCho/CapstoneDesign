using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    private SelectPlayerButton selectPlayerButton;
    private float gameTime = 20f; // 4분 = 240초
    private bool isGameRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        // 씬이 로드될 때마다 호출되는 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // 씬이 언로드될 때 이벤트 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SampleScene")
        {
            // SampleScene이 로드되면 LobbyButton 찾기
            selectPlayerButton = FindObjectOfType<SelectPlayerButton>();
            if (selectPlayerButton == null)
            {
                Debug.LogError("SampleScene에서 LobbyButton을 찾을 수 없습니다!");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameRunning)
        {
            gameTime -= Time.deltaTime;
            if (gameTime <= 0)
            {
                RestartGame();
            }
        }
    }

    public void OnGameFindButtonClicked()
    {
        // TODO: PUN2 연결 및 게임 찾기 로직 구현
        // 현재는 바로 SampleScene으로 전환
        SceneManager.LoadScene("SampleScene");
        // 씬이 로드된 후 StartGame 호출을 위해 코루틴 사용
        StartCoroutine(StartGameAfterSceneLoad());
    }

    private IEnumerator StartGameAfterSceneLoad()
    {
        // 씬이 완전히 로드될 때까지 대기
        yield return new WaitForSeconds(0.1f);
        StartGame();
    }

    public void RestartGame()
    {
        isGameRunning = false;
        gameTime = 20f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // 씬이 로드된 후 StartGame 호출을 위해 코루틴 사용
        StartCoroutine(StartGameAfterSceneLoad());
    }

    public void StartGame()
    {
        if (selectPlayerButton != null)
        {
            // LobbyButton의 캐릭터 선택 초기화
            selectPlayerButton.InitializeCharacterSelection();
            
            // 게임 시작 및 타이머 시작
            isGameRunning = true;
            gameTime = 20f;
        }
        else
        {
            Debug.LogError("selectPlayerButton을 찾을 수 없어 게임을 시작할 수 없습니다!");
        }
    }

    public void EndGame()
    {
        isGameRunning = false;
    }

    public void AttackerWin()
    {
        EndGame();
        // TODO: 공격자 승리 UI 표시
        
    }

    public void DefenderWin()
    {
        EndGame();
        // TODO: 방어자 승리 UI 표시
    }
}

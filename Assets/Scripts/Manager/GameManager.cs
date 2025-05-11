using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGameFindButtonClicked()
    {
        // TODO: PUN2 연결 및 게임 찾기 로직 구현
        // 현재는 바로 SampleScene으로 전환
        SceneManager.LoadScene("SampleScene");
    }
}

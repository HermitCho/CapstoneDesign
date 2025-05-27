using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;

public class LoadSettings : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerMovement playerMovement;
    private AudioSource playerAudioSource;
    private AudioSource gunAudioSource;
    private string savePath => Path.Combine(Application.persistentDataPath, "keybinds_settings.json");

    [System.Serializable]
    public class SettingsData
    {
        public string rebindsJson;
        public float xSensitivity;
        public float ySensitivity;
        public float soundVolume;
    }

    void Awake()
    {
        Debug.Log($"설정 파일 경로: {savePath}");
    }

    public void SetPlayerInput(PlayerInput newPlayerInput)
    {
        if (newPlayerInput == null)
        {
            Debug.LogError("LoadSettings: PlayerInput이 null입니다!");
            return;
        }

        playerInput = newPlayerInput;
        
        // 선택된 플레이어의 컴포넌트들 가져오기
        playerMovement = playerInput.GetComponent<PlayerMovement>();
        playerAudioSource = playerInput.GetComponent<AudioSource>();
        
        // 총 오디오 소스는 자식의 자식 오브젝트(Gun Pivot)에 있으므로 찾기
        Transform[] allChildren = playerInput.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.CompareTag("Gun"))
            {
                gunAudioSource = child.GetComponent<AudioSource>();
                break;
            }
        }

        if (playerMovement == null)
        {
            Debug.LogError("선택된 캐릭터에 PlayerMovement 컴포넌트가 없습니다!");
            return;
        }
        if (playerAudioSource == null)
        {
            Debug.LogError("선택된 캐릭터에 AudioSource 컴포넌트가 없습니다!");
            return;
        }
        if (gunAudioSource == null)
        {
            Debug.LogWarning("총 오디오 소스를 찾을 수 없습니다. 총 사운드가 재생되지 않을 수 있습니다.");
        }

        // 저장된 설정 불러오기
        LoadSettingsToPlayer();
    }

    // UI에서 설정을 JSON 파일로 저장
    public void SaveSettingsToJson(float xSensitivity, float ySensitivity, float soundVolume)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));

            // 현재 키 바인딩 상태 저장
            string rebindsJson = "";
            
            // playerInput이 있으면 현재 키 바인딩 상태 저장
            if (playerInput != null && playerInput.inputActions != null)
            {
                rebindsJson = playerInput.inputActions.SaveBindingOverridesAsJson();
                Debug.Log($"새로운 키 바인딩 JSON 저장: {rebindsJson}");
            }
            else
            {
                // 기존 설정 파일에서 rebindsJson 값을 유지
                if (File.Exists(savePath))
                {
                    string existingJson = File.ReadAllText(savePath);
                    var existingData = JsonUtility.FromJson<SettingsData>(existingJson);
                    rebindsJson = existingData.rebindsJson;
                    Debug.Log($"기존 키 바인딩 JSON 유지: {rebindsJson}");
                }
            }

            SettingsData data = new SettingsData
            {
                rebindsJson = rebindsJson,
                xSensitivity = xSensitivity,
                ySensitivity = ySensitivity,
                soundVolume = soundVolume
            };

            string jsonData = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, jsonData);
            Debug.Log($"설정이 저장되었습니다. 경로: {savePath}\n저장된 값: X감도={xSensitivity}, Y감도={ySensitivity}, 소리={soundVolume}");

            // 현재 선택된 플레이어가 있다면 설정 즉시 적용
            if (playerMovement != null)
            {
                ApplySettingsToPlayer(xSensitivity, ySensitivity, soundVolume);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"설정 저장 중 오류 발생: {e.Message}");
        }
    }

    // JSON 파일에서 설정을 로드하여 UI에 적용
    public void LoadSettingsToUI(KeyRebindManager keyRebindManager)
    {
        if (File.Exists(savePath))
        {
            try
            {
                string jsonData = File.ReadAllText(savePath);
                var data = JsonUtility.FromJson<SettingsData>(jsonData);

                // UI 요소들 업데이트
                if (keyRebindManager.xSensitivitySlider != null) keyRebindManager.xSensitivitySlider.value = data.xSensitivity;
                if (keyRebindManager.ySensitivitySlider != null) keyRebindManager.ySensitivitySlider.value = data.ySensitivity;
                if (keyRebindManager.soundSlider != null) keyRebindManager.soundSlider.value = data.soundVolume;
                if (keyRebindManager.xSensitivityField != null) keyRebindManager.xSensitivityField.text = data.xSensitivity.ToString("F2");
                if (keyRebindManager.ySensitivityField != null) keyRebindManager.ySensitivityField.text = data.ySensitivity.ToString("F2");
                if (keyRebindManager.soundField != null) keyRebindManager.soundField.text = data.soundVolume.ToString("F2");

                // 키 바인딩 UI 업데이트
                if (playerInput != null && !string.IsNullOrEmpty(data.rebindsJson))
                {
                    playerInput.inputActions.LoadBindingOverridesFromJson(data.rebindsJson);
                    keyRebindManager.UpdateAllUI();
                }

                Debug.Log($"UI에 설정을 로드했습니다. X감도={data.xSensitivity}, Y감도={data.ySensitivity}, 소리={data.soundVolume}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"설정 파일을 로드하는 중 오류가 발생했습니다: {e.Message}");
                SetDefaultSettingsToUI(keyRebindManager);
            }
        }
        else
        {
            SetDefaultSettingsToUI(keyRebindManager);
        }
    }

    // JSON 파일에서 설정을 로드하여 플레이어에 적용
    public void LoadSettingsToPlayer()
    {
        if (File.Exists(savePath))
        {
            try
            {
                string jsonData = File.ReadAllText(savePath);
                var data = JsonUtility.FromJson<SettingsData>(jsonData);

                // 키 바인딩 설정 로드
                if (playerInput != null && !string.IsNullOrEmpty(data.rebindsJson))
                {
                    playerInput.inputActions.LoadBindingOverridesFromJson(data.rebindsJson);
                    Debug.Log($"키 바인딩 로드됨: {data.rebindsJson}");
                }
                else
                {
                    // 키 바인딩이 없으면 기본값으로 초기화
                    if (playerInput != null)
                    {
                        playerInput.inputActions.RemoveAllBindingOverrides();
                        Debug.Log("키 바인딩이 기본값으로 초기화됨");
                    }
                }

                ApplySettingsToPlayer(data.xSensitivity, data.ySensitivity, data.soundVolume);
                Debug.Log($"플레이어에 설정을 로드했습니다. X감도={data.xSensitivity}, Y감도={data.ySensitivity}, 소리={data.soundVolume}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"설정 파일을 로드하는 중 오류가 발생했습니다: {e.Message}");
                SetDefaultSettingsToPlayer();
            }
        }
        else
        {
            SetDefaultSettingsToPlayer();
        }
    }

    // 설정을 플레이어에 적용
    private void ApplySettingsToPlayer(float xSensitivity, float ySensitivity, float soundVolume)
    {
        if (playerMovement != null)
        {
            playerMovement.xMouseSensitivity = xSensitivity;
            playerMovement.yMouseSensitivity = ySensitivity;
        }

        if (playerAudioSource != null)
        {
            playerAudioSource.volume = soundVolume;
        }
        if (gunAudioSource != null)
        {
            gunAudioSource.volume = soundVolume;
        }
    }

    // UI에 기본 설정 적용
    private void SetDefaultSettingsToUI(KeyRebindManager keyRebindManager)
    {
        float defaultX = 1f;
        float defaultY = 1f;
        float defaultVolume = 0.2f;

        if (keyRebindManager.xSensitivitySlider != null) keyRebindManager.xSensitivitySlider.value = defaultX;
        if (keyRebindManager.ySensitivitySlider != null) keyRebindManager.ySensitivitySlider.value = defaultY;
        if (keyRebindManager.soundSlider != null) keyRebindManager.soundSlider.value = defaultVolume;
        if (keyRebindManager.xSensitivityField != null) keyRebindManager.xSensitivityField.text = defaultX.ToString("F2");
        if (keyRebindManager.ySensitivityField != null) keyRebindManager.ySensitivityField.text = defaultY.ToString("F2");
        if (keyRebindManager.soundField != null) keyRebindManager.soundField.text = defaultVolume.ToString("F2");

        Debug.Log("UI에 기본 설정이 적용되었습니다.");
    }

    // 플레이어에 기본 설정 적용
    private void SetDefaultSettingsToPlayer()
    {
        float defaultX = 1f;
        float defaultY = 1f;
        float defaultVolume = 0.2f;

        ApplySettingsToPlayer(defaultX, defaultY, defaultVolume);
        Debug.Log("플레이어에 기본 설정이 적용되었습니다.");
    }

    public string GetCurrentKeyBindings()
    {
        if (playerInput != null && playerInput.inputActions != null)
        {
            string rebindsJson = playerInput.inputActions.SaveBindingOverridesAsJson();
            Debug.Log($"현재 키 바인딩 상태: {rebindsJson}");
            return rebindsJson;
        }
        return "";
    }

    // Start is called before the first frame update
    void Start()
    {
        // 현재 활성화된 플레이어의 PlayerInput 찾기
        PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
        foreach (PlayerInput input in playerInputs)
        {
            if (input.gameObject.activeInHierarchy)
            {
                playerInput = input;
                break;
            }
        }

        // 저장된 설정 불러오기
        if (playerInput != null)
        {
            LoadSettingsToPlayer();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

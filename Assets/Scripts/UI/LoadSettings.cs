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
            }
            else
            {
                // 기존 설정 파일에서 rebindsJson 값을 유지
                if (File.Exists(savePath))
                {
                    string existingJson = File.ReadAllText(savePath);
                    var existingData = JsonUtility.FromJson<SettingsData>(existingJson);
                    rebindsJson = existingData.rebindsJson;
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
            Debug.Log($"설정이 저장되었습니다. 경로: {savePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"설정 저장 중 오류 발생: {e.Message}");
        }
    }

    // UI에 기본 설정 적용
    private void SetDefaultSettingsToUI(KeyRebindManager keyRebindManager)
    {
        float defaultX = 1f;
        float defaultY = 1f;
        float defaultVolume = 0.2f;

        keyRebindManager.xSensitivitySlider.mainSlider.value = defaultX;
        keyRebindManager.ySensitivitySlider.mainSlider.value = defaultY;
        keyRebindManager.soundSlider.mainSlider.value = defaultVolume;
        keyRebindManager.xSensitivityField.text = defaultX.ToString("F2");
        keyRebindManager.ySensitivityField.text = defaultY.ToString("F2");
        keyRebindManager.soundField.text = defaultVolume.ToString("F2");

        Debug.Log("UI에 기본 설정이 적용되었습니다.");
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
                keyRebindManager.xSensitivitySlider.mainSlider.value = data.xSensitivity;
                keyRebindManager.ySensitivitySlider.mainSlider.value = data.ySensitivity;
                keyRebindManager.soundSlider.mainSlider.value = data.soundVolume;
                keyRebindManager.xSensitivityField.text = data.xSensitivity.ToString("F2");
                keyRebindManager.ySensitivityField.text = data.ySensitivity.ToString("F2");
                keyRebindManager.soundField.text = data.soundVolume.ToString("F2");

                // 키 바인딩 UI 업데이트
                if (playerInput != null && !string.IsNullOrEmpty(data.rebindsJson))
                {
                    playerInput.inputActions.LoadBindingOverridesFromJson(data.rebindsJson);
                }

                Debug.Log($"UI에 설정을 로드했습니다.");
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
                }

                // 감도 및 소리 설정 적용
                if (playerMovement != null)
                {
                    playerMovement.xMouseSensitivity = data.xSensitivity;
                    playerMovement.yMouseSensitivity = data.ySensitivity;
                }

                if (playerAudioSource != null)
                {
                    playerAudioSource.volume = (data.soundVolume * 0.06f);
                }

                if (gunAudioSource != null)
                {
                    gunAudioSource.volume = (data.soundVolume * 0.06f);
                }

                Debug.Log($"플레이어에 설정을 적용했습니다.");
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

    // 플레이어에 기본 설정 적용
    private void SetDefaultSettingsToPlayer()
    {
        float defaultX = 1f;
        float defaultY = 1f;
        float defaultVolume = 0.2f;

        if (playerMovement != null)
        {
            playerMovement.xMouseSensitivity = defaultX;
            playerMovement.yMouseSensitivity = defaultY;
        }

        if (playerAudioSource != null)
        {
            playerAudioSource.volume = defaultVolume;
        }
        if (gunAudioSource != null)
        {
            gunAudioSource.volume = defaultVolume;
        }

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

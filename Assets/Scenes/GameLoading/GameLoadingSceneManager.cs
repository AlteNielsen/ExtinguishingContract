using UnityEngine;

public class GameLoadingSceneManager : MonoBehaviour
{
    void Awake()
    {
        SaveDataManager.Instance.GameLoadingSceneSaveDataInitialize();
    }
}

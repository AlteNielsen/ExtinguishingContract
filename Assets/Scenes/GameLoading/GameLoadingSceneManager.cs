using UnityEngine;

public class GameLoadingSceneManager : MonoBehaviour
{
    void Awake()
    {
        SaveDataInitialize();
        SetupBurningSituation();
        SetupBlockIndicator();
    }

    private void SaveDataInitialize()
    {
        SaveDataManager.Instance.GameLoadingSceneSaveDataInitialize();
    }

    private void SetupBurningSituation()
    {
        float[] pools = new float[MapDataBase.Datas.Length];
        bool[] data = CulculateLibrary.GameSituationSetup();
        for (int i = 0; i < pools.Length; i++)
        {
            if (data[i])
            {
                pools[i] = 1;
            }
        }
        SaveDataManager.Instance.SetData((int)SaveDataManager.SaveDataChunk.BurningSituation, pools);
    }

    private void SetupBlockIndicator()
    {
        int EIndicatorNum = 6;
        float[] pools = new float[EIndicatorNum * Config.Data.IndicatorMaxLv];
        bool[] data = CulculateLibrary.IndicatorUpdate();
        for (int i = 0; i < pools.Length; i++)
        {
            if (data[i])
            {
                pools[i] = 1;
            }
        }
        SaveDataManager.Instance.SetData((int)SaveDataManager.SaveDataChunk.BlockIndicator, pools);
    }
}

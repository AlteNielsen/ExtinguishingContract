using UnityEngine;

public class GameLoadingSceneManager : MonoBehaviour
{
    void Awake()
    {
        SaveDataManager.Instance.GameLoadingSceneSaveDataInitialize();
        float[] pools = new float[MapDataBase.Datas.Length];
        bool[] situationData = CulculateLibrary.GameSituationSetup();
        for(int i = 0; i < pools.Length; i++)
        {
            if (situationData[i])
            {
                pools[i] = 1;
            }
        }
        SaveDataManager.Instance.SetData((int)SaveDataManager.SaveDataChunk.BurningSituation, pools);

        int EIndicatorNum = 6;
        float[] indicatorPools = new float[EIndicatorNum * Config.Data.IndicatorMaxLv];
        bool[] indicatorData = CulculateLibrary.IndicatorUpdate();
        for (int i = 0; i < indicatorPools.Length; i++)
        {
            if (indicatorData[i])
            {
                indicatorPools[i] = 1;
            }
        }
        SaveDataManager.Instance.SetData((int)SaveDataManager.SaveDataChunk.BlockIndicator, indicatorPools);
    }
}

using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public class GameLoadingSceneManager : MonoBehaviour
{
    [SerializeField] private UIDocument document;

    async void Awake()
    {
        SaveDataInitialize();
        SetupBurningSituation();
        SetupBlockIndicator();
        GameLoadingSceneView();
        await Task.Delay(3000);
        GameSceneManager.ToHome();
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

    private void GameLoadingSceneView()
    {
        document.rootVisualElement.Q<Label>().text = TextDataBase.GetTexts(TextDataBase.TextDictionary.GameLoading)[0];
    }
}

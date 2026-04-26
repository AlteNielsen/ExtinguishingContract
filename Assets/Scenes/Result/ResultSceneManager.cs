using UnityEngine;
using UnityEngine.UIElements;
using System;

public class ResultSceneManager : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    [SerializeField] private VisualTreeAsset unitPlate;
    private ResultSceneView sceneView;
    private float[] blockSituations;
    void Awake()
    {
        ExtinguishingContract.DevelopOnlyGameSetup();
        UnitPlateSetup();
        var (isSuccess, isExtinguish, other) = IsStageSuccess();
        sceneView = new ResultSceneView(document, isExtinguish);
        BurningSituationSetup(isExtinguish);
        OtherProgressExtinguish(other);
        BlockBurn();
        sceneView.DisplayBlockSituation(blockSituations.AsSpan());
        sceneView.DisplayProgress(other);
        UpdateResultStats(isSuccess, isExtinguish, other);
        UpdateTurnStats();
        UpdateSituation(other);
        SaveDataManager.Instance.ResultSceneSaveDataInitialize();
        ResultSceneCotroller();
    }

    private void ResultSceneCotroller()
    {
        Button nextButton = document.rootVisualElement.Q<Button>("NextButton");
        nextButton.clicked += NextScene;
    }

    private void UnitPlateSetup()
    {
        ScrollView scroll = document.rootVisualElement.Q<ScrollView>("UnitScrollView");
        scroll.contentContainer.Clear();
        for(int i = 0; i < UnitDataBase.Datas.Length; i++)
        {
            VisualElement plate = unitPlate.Instantiate();
            scroll.contentContainer.Add(plate);
        }
    }

    private (bool isSuccess, bool isExtinguish, float other) IsStageSuccess()
    {
        bool isSuccess = true;
        ReadOnlySpan<float> result = SaveDataManager.Instance.Access<FireMapChunk>((int)SaveDataManager.SaveDataChunk.FireMap).data.Span;
        for(int i = 0; i < result.Length; i++)
        {
            if(result[i] > 0.5f)
            {
                isSuccess = false;
            }
        }
        
        bool isExtinguish = false;
        float other = 0;
        if(isSuccess)
        {
            float chance = CulculateLibrary.CulculateChance();
            float dice = UnityEngine.Random.Range(0f, 1f);
            if(chance > 1f)
            {
                other = chance - 1f;
            }

            if(dice < chance)
            {
                isExtinguish = true;
                other += chance - dice;
            }
        }
        return (isSuccess, isExtinguish, other);
    }

    private void BurningSituationSetup(bool isExtinguish)
    {
        blockSituations = new float[MapDataBase.Datas.Length];
        SaveDataManager.Instance.GetData((int)SaveDataManager.SaveDataChunk.BurningSituation, blockSituations);
        for(int i = 0; i < blockSituations.Length; i++)
        {
            if(blockSituations[i] < -0.5f)
            {
                blockSituations[i] += 1f;
            }
        }
        if(isExtinguish)
        {
            int map = (int)SaveDataManager.Instance.Access<MapSelectChunk>(((int)SaveDataManager.SaveDataChunk.MapSelect)).data.Span[0];
            blockSituations[map] = -2;
        }
    }

    private void OtherProgressExtinguish(float chance)
    {
        if (chance <= 0) return;
        int extCount = (int)(chance + SaveDataManager.Instance.Access<OtherProgressChunk>((int)SaveDataManager.SaveDataChunk.OtherProgress).data.Span[0]);
        int burningCounter = 0;
        for(int i = 0; i < blockSituations.Length; i++)
        {
            if (blockSituations[i] > 0.5f)
            {
                burningCounter++;
            }
        }
        if (extCount >= burningCounter)
        {
            Array.Fill(blockSituations, 0);
            return;
        }
        bool[] extResult = CulculateLibrary.nCrProcess(burningCounter, extCount);
        int counter = 0;
        for(int i = 0; i < blockSituations.Length; i++)
        {
            if (blockSituations[i] > 0.5f)
            {
                if (extResult[counter])
                {
                    blockSituations[i] = -2;
                }
                counter++;
            }
        }
    }

    private void BlockBurn()
    {
        int burningCount = (int)CulculateLibrary.IndicatorBaseValues(SaveDataManager.Instance.Access<NowIDChunk>((int)SaveDataManager.SaveDataChunk.NowID).data.Span)[8];
        float[] copy = new float[blockSituations.Length];
        Array.Copy(blockSituations, copy, blockSituations.Length);
        while (burningCount > 0)
        {
            for (int i = 0; i < blockSituations.Length; i++)
            {
                int beforeIndex = i - 1;
                int afterIndex = i + 1;
                if (beforeIndex < 0)
                {
                    beforeIndex = blockSituations.Length - 1;
                }
                if (afterIndex > blockSituations.Length - 1)
                {
                    afterIndex = 0;
                }

                if (copy[i] > 0.5f)
                {
                    if (blockSituations[beforeIndex] > -0.5f)
                    {
                        blockSituations[beforeIndex] = 1;
                    }
                    if (blockSituations[afterIndex] > -0.5f)
                    {
                        blockSituations[afterIndex] = 1;
                    }
                }
            }
            burningCount--;
        }
    }

    private void UpdateSituation(float other)
    {
        SaveDataManager.Instance.SetData((int)SaveDataManager.SaveDataChunk.BurningSituation, blockSituations);
        SaveDataManager.Instance.SetData((int)SaveDataManager.SaveDataChunk.BlockIndicator, CulculateLibrary.SwitchBoolToFloat(CulculateLibrary.IndicatorUpdate()));
        float value = other + SaveDataManager.Instance.Access<OtherProgressChunk>((int)SaveDataManager.SaveDataChunk.OtherProgress).data.Span[0];
        while (value > 1f)
        {
            value -= 1f;
        }
        SaveDataManager.Instance.SetData((int)SaveDataManager.SaveDataChunk.OtherProgress, new float[] { value });
    }

    private void UpdateResultStats(bool isSuccess, bool isExtinguish, float other)
    {
        float[] datas = new float[SaveDataManager.Instance.Access<ResultStatsChunk>((int)SaveDataManager.SaveDataChunk.ResultStats).data.Length];
        SaveDataManager.Instance.GetData((int)SaveDataManager.SaveDataChunk.ResultStats, datas);
        datas[0]++;
        if(isSuccess)
        {
            float all = datas[2] * datas[1];
            all += CulculateLibrary.CulculateChance(); ;
            datas[1]++;
            datas[2] = all / datas[1];
        }
        if(isExtinguish)
        {
            datas[3]++;
            datas[4] += other;
        }

        ReadOnlySpan<float> indicators = SaveDataManager.Instance.Access<IndicatorSelectChunk>((int)SaveDataManager.SaveDataChunk.IndicatorSelect).data.Span;
        for(int i = 0; i < ExtinguishingContract.EIndicatorNum; i++)
        {
            for(int j = 0; j < Config.Data.IndicatorMaxLv; j++)
            {
                if (indicators[i * Config.Data.IndicatorMaxLv + j] > 0.5f)
                {
                    datas[5 + i]++;
                }
            }
        }
        SaveDataManager.Instance.SetData((int)SaveDataManager.SaveDataChunk.ResultStats, datas);
    }

    private void UpdateTurnStats()
    {
        float[] datas = new float[SaveDataManager.Instance.Access<TurnStatsChunk>((int)SaveDataManager.SaveDataChunk.TurnStats).data.Length];
        SaveDataManager.Instance.GetData((int)SaveDataManager.SaveDataChunk.TurnStats, datas);
        int counter = 0;
        for(int i = 0; i < blockSituations.Length; i++)
        {
            if (blockSituations[i] > 0.5f)
            {
                counter++;
            }
        }
        for(int i = 0; i < datas.Length; i++)
        {
            if(datas[i] == 0)
            {
                if(i == 0)
                {
                    datas[0] = (int)CulculateLibrary.IndicatorBaseValues(SaveDataManager.Instance.Access<NowIDChunk>((int)SaveDataManager.SaveDataChunk.NowID).data.Span)[7];
                    datas[1] = counter;
                }
                else
                {
                    datas[i] = counter;
                }
                break;
            }
        }
        SaveDataManager.Instance.SetData((int)SaveDataManager.SaveDataChunk.TurnStats, datas);
    }

    private void NextScene()
    {
        bool isFinish = true;
        int counter = 0;
        for(int i = 0; i < blockSituations.Length; i++)
        {
            if (blockSituations[i] > 0.5f)
            {
                isFinish = false;
                counter++;
            }
        }
        if(isFinish || counter == blockSituations.Length)
        {
            GameSceneManager.ToEnding();
        }
        else
        {
            GameSceneManager.ToHome();
        }
    }
}

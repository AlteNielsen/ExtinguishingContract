using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class HomeSceneView
{
    private UIDocument document;
    private List<VisualElement> blockButtonBGs;
    private List<Label> mapInfo;
    private List<VisualElement> indicatorButtonBGs;
    private List<VisualElement> waterGauge;
    private VisualElement indicatorPanel;
    private VisualElement mapPanel;
    private VisualElement mapButtonTex;

    private bool centerColomnCondition;//false: indicator true: map

    public HomeSceneView(UIDocument doc)
    {
        document = doc;
        blockButtonBGs = document.rootVisualElement.Query<VisualElement>("BlockSelectorBG").ToList();
        mapInfo = document.rootVisualElement.Query<Label>("MapInfo").ToList();
        indicatorButtonBGs = document.rootVisualElement.Query<VisualElement>("IndicatorButtonBG").ToList();
        waterGauge = document.rootVisualElement.Query<VisualElement>("WaterGauge").ToList();
        indicatorPanel = document.rootVisualElement.Q<VisualElement>("IndicatorPanel");
        mapPanel = document.rootVisualElement.Q<VisualElement>("MapPanel");
        mapButtonTex = document.rootVisualElement.Q<VisualElement>("MapButtonTex");
        WriteText();
        SetupValues();
        mapInfo[2].text = "" + CulculateLibrary.FloatToPercent(Config.Data.InitialChance) + "%";
        OtherBlockProgressDisplay(Config.Data.InitialChance - 1);
    }

    private void WriteText()
    {
        WriteBlockSelector();
        WriteSceneText();
    }

    private void WriteSceneText()
    {
        List<Label> labels = document.rootVisualElement.Query<Label>("Text").ToList();
        for (int i = 0; i < labels.Count; i++)
        {
            labels[i].text = TextDataBase.GetTexts(TextDataBase.TextDictionary.Home)[i];
        }
    }

    private void WriteBlockSelector()
    {
        List<Label> labels = document.rootVisualElement.Query<Label>("BlockSelectorText").ToList();
        for(int i = 0; i < labels.Count; i++)
        {
            labels[i].text = WordDataBase.Word(WordDataBase.WordSelector.MapTitle)[i];
        }
    }

    private void SetupValues()
    {
        SetupBlockSelectorDisplay();
        SetupContractMemoDisplay();
        SetupIndicatorsDisplay();
    }

    private void SetupContractMemoDisplay()
    {
        Label id = document.rootVisualElement.Q<Label>("ID");
        ReadOnlyMemory<float> now = SaveDataManager.Instance.Access<NowIDChunk>((int)SaveDataManager.SaveDataChunk.NowID).data;
        id.text = "" + now.Span[0] + now.Span[1] + now.Span[2] + "-" + now.Span[3] + now.Span[4] + now.Span[5] + "-" + now.Span[6] + now.Span[7] + now.Span[8];

        Label grade = document.rootVisualElement.Q<Label>("Grade");
        grade.text = "" + CulculateLibrary.ContractGrade(now.Span);
    }

    private void SetupBlockSelectorDisplay()
    {
        for(int i = 0; i < blockButtonBGs.Count; i++)
        {
            if(SaveDataManager.Instance.Access<BurningSituationChunk>(((int)SaveDataManager.SaveDataChunk.BurningSituation)).data.Span[i] > 0.5f)
            {
                blockButtonBGs[i].RemoveFromClassList("bg-darkgray");
                blockButtonBGs[i].AddToClassList("bg-red");
                blockButtonBGs[i].AddToClassList("semi-transparent");
            }
        }
    }

    private void SetupIndicatorsDisplay()
    {
        List<Label> chances = document.rootVisualElement.Query<Label>("IndicatorChance").ToList();
        List<Label> names = document.rootVisualElement.Query<Label>("IndicatorName").ToList();
        List<Label> types = document.rootVisualElement.Query<Label>("IndicatorType").ToList();
        List<Label> values = document.rootVisualElement.Query<Label>("IndicatorValue").ToList();
        ReadOnlySpan<float> indicators = SaveDataManager.Instance.Access<BlockIndicatorChunk>(((int)SaveDataManager.SaveDataChunk.BlockIndicator)).data.Span;
        float[] baseValue = CulculateLibrary.IndicatorBaseValues(SaveDataManager.Instance.Access<NowIDChunk>((int)SaveDataManager.SaveDataChunk.NowID).data.Span);
        int buttonCounter = 0;
        for (int i = 0; i < indicators.Length; i++)
        {
            if (indicators[i] > 0.5f)
            {
                int type = i / Config.Data.IndicatorMaxLv;
                int level = i % Config.Data.IndicatorMaxLv + 1;
                
                chances[buttonCounter].text = "+" + CulculateLibrary.FloatToPercent(Config.Data.IndicatorBaseChance * level) + "%";
                names[buttonCounter].text = WordDataBase.Word(WordDataBase.WordSelector.EIndicatorTitle)[type];
                types[buttonCounter].text = WordDataBase.Word(WordDataBase.WordSelector.EIndicatorEffect)[type];
                if (baseValue[type] > 0)
                {
                    values[buttonCounter].text = "+" + (baseValue[type] * level);
                }
                else
                {
                    values[buttonCounter].text = "" + (baseValue[type] * level);
                }
                buttonCounter++;
            }
        }
    }

    public void BlockSelect(int index)
    {
        for(int i = 0; i < blockButtonBGs.Count; i++)
        {
            blockButtonBGs[i].RemoveFromClassList("block-button-selected");
        }
        blockButtonBGs[index].AddToClassList("block-button-selected");

        mapInfo[0].text = WordDataBase.Word(WordDataBase.WordSelector.MapTitle)[index];
        if(SaveDataManager.Instance.Access<BurningSituationChunk>(((int)SaveDataManager.SaveDataChunk.BurningSituation)).data.Span[index] > 0.5f)
        {
            mapInfo[1].text = TextDataBase.GetTexts(TextDataBase.TextDictionary.Home)[8];
            mapInfo[1].RemoveFromClassList("color-white");
            mapInfo[1].AddToClassList("color-red");
        }
        else
        {
            mapInfo[1].text = TextDataBase.GetTexts(TextDataBase.TextDictionary.Home)[9];
            mapInfo[1].RemoveFromClassList("color-red");
            mapInfo[1].AddToClassList("color-white");
        }
        mapInfo[3].text = WordDataBase.Word(WordDataBase.WordSelector.MapTitle)[index] + " - " + WordDataBase.Word(WordDataBase.WordSelector.MapName)[index];
    }

    public void IndicatorSelect(int index, Span<bool> indicatorCondition)
    {
        if (indicatorCondition[index])
        {
            indicatorButtonBGs[index].RemoveFromClassList("bg-darkgray");
            indicatorButtonBGs[index].AddToClassList("indicator-button-selected");
        }
        else
        {
            indicatorButtonBGs[index].RemoveFromClassList("indicator-button-selected");
            indicatorButtonBGs[index].AddToClassList("bg-darkgray");
        }
        float chance = CulculateChance(indicatorCondition);
        if (chance > 1)
        {
            mapInfo[2].text = "" + 100 + "%";
        }
        else
        {
            mapInfo[2].text = "" + CulculateLibrary.FloatToPercent(chance) + "%";
        }
        OtherBlockProgressDisplay(chance - 1);
    }

    private float CulculateChance(Span<bool> indicatorCondition)
    {
        float result = 0;
        ReadOnlySpan<float> indicators = SaveDataManager.Instance.Access<BlockIndicatorChunk>(((int)SaveDataManager.SaveDataChunk.BlockIndicator)).data.Span;
        int counter = 0;
        for (int i = 0; i < indicators.Length; i++)
        {
            if (indicators[i] > 0.5f)
            {
                if (indicatorCondition[counter])
                {
                    int level = i % Config.Data.IndicatorMaxLv + 1;
                    result += Config.Data.IndicatorBaseChance * level;
                }
                counter++;
            }
        }
        result += Config.Data.InitialChance;
        return result;
    }

    private void OtherBlockProgressDisplay(float value)
    {
        float other = SaveDataManager.Instance.Access<OtherProgressChunk>((int)SaveDataManager.SaveDataChunk.OtherProgress).data.Span[0];
        waterGauge[2].style.height = new StyleLength(Length.Percent(CulculateLibrary.FloatToPercent(other)));
        if(value <= 0)
        {
            mapInfo[4].text = "" + 0 + "%";
            waterGauge[0].style.height = new StyleLength(Length.Percent(CulculateLibrary.FloatToPercent(1 - other)));
            waterGauge[1].style.height = new StyleLength(Length.Percent(0));
        }
        else
        {
            if(other + value > 1)
            {
                waterGauge[0].style.height = new StyleLength(Length.Percent(0));
                waterGauge[1].style.height = new StyleLength(Length.Percent(CulculateLibrary.FloatToPercent(1 - other)));
            }
            else
            {
                waterGauge[0].style.height = new StyleLength(Length.Percent(CulculateLibrary.FloatToPercent(1 - other - value)));
                waterGauge[1].style.height = new StyleLength(Length.Percent(CulculateLibrary.FloatToPercent(value)));
            }
            mapInfo[4].text = "+" + CulculateLibrary.FloatToPercent(value) + "%";
        }
    }

    public void MapDisplay()
    {
        centerColomnCondition = !centerColomnCondition;
        if(centerColomnCondition)
        {
            mapPanel.RemoveFromClassList("non-display");
            indicatorPanel.AddToClassList("non-display");
            mapButtonTex.RemoveFromClassList("map-button-tex-off");
            mapButtonTex.AddToClassList("map-button-tex-on");
        }
        else
        {
            mapPanel.AddToClassList("non-display");
            indicatorPanel.RemoveFromClassList("non-display");
            mapButtonTex.RemoveFromClassList("map-button-tex-on");
            mapButtonTex.AddToClassList("map-button-tex-off");
        }
    }
}

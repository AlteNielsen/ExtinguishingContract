using System.Collections.Generic;
using UnityEngine.UIElements;

public class HomeSceneView
{
    private UIDocument document;

    public HomeSceneView(UIDocument doc)
    {
        document = doc;
        WriteText();
        SetupBlockSelectorDisplay();
    }

    private void WriteText()
    {
        WriteBlockSelector();
    }

    private void WriteBlockSelector()
    {
        List<Label> labels = document.rootVisualElement.Query<Label>("BlockSelectorText").ToList();
        for(int i = 0; i < labels.Count; i++)
        {
            labels[i].text = WordDataBase.Word(WordDataBase.WordSelector.MapTitle)[i];
        }
    }

    private void SetupBlockSelectorDisplay()
    {
        List<VisualElement> ves = document.rootVisualElement.Query<VisualElement>("BlockSelectorBG").ToList();
        for(int i = 0; i < ves.Count; i++)
        {
            if(SaveDataManager.Instance.Access<BurningSituationChunk>(((int)SaveDataManager.SaveDataChunk.BurningSituation)).data.Span[i] > 0.5f)
            {
                ves[i].RemoveFromClassList("bg-darkgray");
                ves[i].AddToClassList("bg-red");
                ves[i].AddToClassList("semi-transparent");
            }
        }
    }
}

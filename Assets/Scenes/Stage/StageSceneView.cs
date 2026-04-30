using System.Collections.Generic;
using UnityEngine.UIElements;

public class StageSceneView
{
    private UIDocument document;

    public StageSceneView(UIDocument doc)
    {
        document = doc;
    }
}

public class StageBoardView
{
    private List<VisualElement> gridPanels;

    public StageBoardView(UIDocument document, VisualTreeAsset temp)
    {
        VisualElement board = document.rootVisualElement.Q<VisualElement>("Board");
        int selected = (int)SaveDataManager.Instance.Access<MapSelectChunk>((int)SaveDataManager.SaveDataChunk.MapSelect).data.Span[0];
        board.contentContainer.Clear();
        for (int i = 0; i < MapDataBase.Datas[selected].Data.layout.Length; i++)
        {
            VisualElement ve = temp.Instantiate();
            board.contentContainer.Add(ve);
            gridPanels[i] = ve;
        }
        MapSetup(selected);
    }

    private void MapSetup(int mapNum)
    {
        MapLayout layout = MapDataBase.Datas[mapNum].Data;
        for (int i = 0; i < layout.layout.Length; i++)
        {
            if (layout.layout[i] == 1)
            {
                gridPanels[i].AddToClassList("non-display");
            }
        }
    }
}

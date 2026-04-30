using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StageSceneView
{
    private UIDocument document;
    private List<VisualElement> gridPanels;

    public StageSceneView(UIDocument doc)
    {
        document = doc;
        gridPanels = document.rootVisualElement.Query<VisualElement>("GridPanel").ToList();
        SetupMap();
    }

    private void SetupMap()
    {
        int selected = (int)SaveDataManager.Instance.Access<MapSelectChunk>((int)SaveDataManager.SaveDataChunk.MapSelect).data.Span[0];
        MapLayout layout = MapDataBase.Datas[selected].Data;
        for(int i = 0; i < layout.layout.Length; i++)
        {
            if (layout.layout[i] == 1)
            {
                gridPanels[i].AddToClassList("non-display");
            }
        }
    }
}

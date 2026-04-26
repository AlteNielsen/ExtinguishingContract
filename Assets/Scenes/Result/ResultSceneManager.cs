using UnityEngine;
using UnityEngine.UIElements;
using System;

public class ResultSceneManager : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    [SerializeField] private VisualTreeAsset unitPlate;
    private ResultSceneView sceneView;
    void Awake()
    {
        ExtinguishingContract.DevelopOnlyGameSetup();
        UnitPlateSetup();
        sceneView = new ResultSceneView(document, IsStageSuccess());
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

    private bool IsStageSuccess()
    {
        ReadOnlySpan<float> result = SaveDataManager.Instance.Access<FireMapChunk>((int)SaveDataManager.SaveDataChunk.FireMap).data.Span;
        for(int i = 0; i < result.Length; i++)
        {
            if(result[i] > 0.5f)
            {
                return false;
            }
        }
        return true;
    }
}

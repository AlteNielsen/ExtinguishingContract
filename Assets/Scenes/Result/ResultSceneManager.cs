using UnityEngine;
using UnityEngine.UIElements;

public class ResultSceneManager : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    [SerializeField] private VisualTreeAsset unitPlate;
    private ResultSceneView sceneView;
    void Awake()
    {
        ExtinguishingContract.DevelopOnlyGameSetup();
        UnitPlateSetup();
        sceneView = new ResultSceneView(document);
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
}

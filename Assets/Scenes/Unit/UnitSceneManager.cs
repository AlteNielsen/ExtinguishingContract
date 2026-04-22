using UnityEngine;
using UnityEngine.UIElements;

public class UnitSceneManager : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    [SerializeField] private VisualTreeAsset unitCard;
    private UnitSceneView sceneView;

    void Awake()
    {
        ExtinguishingContract.DevelopOnlyGameSetup();
        ScrollViewSetup();
        sceneView = new UnitSceneView(document);
    }

    private void ScrollViewSetup()
    {
        ScrollView unitScrollView = document.rootVisualElement.Q<ScrollView>("UnitScrollView");
        unitScrollView.contentContainer.Clear();
        for(int i = 0; i < UnitDataBase.Datas.Length; i++)
        {
            VisualElement card = unitCard.Instantiate();
            unitScrollView.contentContainer.Add(card);
        }
    }
}

using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class HomeSceneManager : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    private HomeSceneView sceneView;
    private int blockSelector;

    void Awake()
    {
        ExtinguishingContract.DevelopOnlyGameSetup();
        sceneView = new HomeSceneView(document);
        HomeSceneController();
        BlockSelect(0);
    }

    private void HomeSceneController()
    {
        List<Button> blockButtons = document.rootVisualElement.Query<Button>("BlockSelectorButton").ToList();
        for(int i = 0; i < blockButtons.Count; i++)
        {
            int j = i;
            blockButtons[i].clicked += () => BlockSelect(j);
        }
    }

    private void BlockSelect(int index)
    {
        blockSelector = index;
        sceneView.BlockSelect(blockSelector);
    }
}

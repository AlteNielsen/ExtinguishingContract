using UnityEngine;
using UnityEngine.UIElements;

public class StageSceneManager : MonoBehaviour
{
    private const int tileNum = 96;
    [SerializeField] private UIDocument document;
    [SerializeField] private VisualTreeAsset tile;

    void Awake()
    {
        BoardSetup();
    }

    private void BoardSetup()
    {
        VisualElement board = document.rootVisualElement.Q<VisualElement>("Board");
        board.contentContainer.Clear();
        for(int i = 0; i < tileNum; i++)
        {
            VisualElement ve = tile.Instantiate();
            board.contentContainer.Add(ve);
        }
    }
}

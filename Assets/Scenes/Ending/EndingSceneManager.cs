using UnityEngine;
using UnityEngine.UIElements;

public class EndingSceneManager : MonoBehaviour
{
    [SerializeField] private UIDocument mainDocument;
    [SerializeField] private UIDocument goodDocument;
    [SerializeField] private UIDocument badDocument;
    private EndingSceneView sceneView;

    void Awake()
    {
        ExtinguishingContract.DevelopOnlyGameSetup();
        sceneView = new EndingSceneView(mainDocument, goodDocument, badDocument);
        EndingSceneController();
    }

    private void EndingSceneController()
    {
        Button goodNext = goodDocument.rootVisualElement.Q<Button>("NextButton");
        goodNext.clicked += sceneView.SwitchMainScreen;
        Button badNext = badDocument.rootVisualElement.Q<Button>("NextButton");
        badNext.clicked += sceneView.SwitchMainScreen;
    }
}

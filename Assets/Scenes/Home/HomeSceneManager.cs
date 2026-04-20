using UnityEngine;
using UnityEngine.UIElements;

public class HomeSceneManager : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    private HomeSceneView sceneView;

    void Awake()
    {
        ExtinguishingContract.DevelopOnlyGameSetup();
        sceneView = new HomeSceneView(document);
    }

    
}

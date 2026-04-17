using UnityEngine;
using UnityEngine.UIElements;

public class ContractSceneManager : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    private ContractSceneView sceneView;

    void Awake()
    {
        sceneView = new ContractSceneView(document);
    }
}

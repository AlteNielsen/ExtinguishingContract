using System;
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
        UpdateMaxID();
        SaveDataManager.Instance.EndingSceneSaveDataInitialize();
    }

    private void EndingSceneController()
    {
        Button goodNext = goodDocument.rootVisualElement.Q<Button>("NextButton");
        goodNext.clicked += sceneView.SwitchMainScreen;
        Button badNext = badDocument.rootVisualElement.Q<Button>("NextButton");
        badNext.clicked += sceneView.SwitchMainScreen;
    }

    private void UpdateMaxID()
    {
        ReadOnlySpan<float> now = SaveDataManager.Instance.Access<NowIDChunk>((int)SaveDataManager.SaveDataChunk.NowID).data.Span;
        int nowGrade = CulculateLibrary.ContractGrade(now);
        ReadOnlySpan<float> max = SaveDataManager.Instance.Access<MaxIDChunk>((int)SaveDataManager.SaveDataChunk.MaxID).data.Span;
        int maxGrade = CulculateLibrary.ContractGrade(max);
        if(nowGrade >= maxGrade)
        {
            SaveDataManager.Instance.SetData((int)SaveDataManager.SaveDataChunk.MaxID, now.ToArray());
        }
    }
}

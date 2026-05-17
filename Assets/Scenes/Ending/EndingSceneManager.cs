using System;
using UnityEngine;
using UnityEngine.UIElements;

public class EndingSceneManager : MonoBehaviour
{
    [SerializeField] private UIDocument mainDocument;
    [SerializeField] private UIDocument goodDocument;
    [SerializeField] private UIDocument badDocument;
    private EndingSceneView sceneView;

    private VisualElement mainFader;
    private VisualElement goodFader;
    private VisualElement badFader;

    void Awake()
    {
        ExtinguishingContract.DevelopOnlyGameSetup();
        bool isGoodEnding = CulculateLibrary.IsGoodEnding();
        sceneView = new EndingSceneView(mainDocument, goodDocument, badDocument, isGoodEnding);
        EndingSceneController();
        UpdateMaxID(isGoodEnding);
        SaveDataManager.Instance.EndingSceneSaveDataInitialize();

        mainFader = mainDocument.rootVisualElement.Q<VisualElement>("Fader");
        goodFader = goodDocument.rootVisualElement.Q<VisualElement>("Fader");
        badFader = badDocument.rootVisualElement.Q<VisualElement>("Fader");
        if(isGoodEnding)
        {
            CulculateLibrary.SceneFadeIn(goodFader);
        }
        else
        {
            CulculateLibrary.SceneFadeIn(badFader);
        }
        SEManager.PlayEndSE();
    }

    private void EndingSceneController()
    {
        Button goodNext = goodDocument.rootVisualElement.Q<Button>("NextButton");
        goodNext.clicked += () =>
        {
            SEManager.PlaySelectSE();
            CulculateLibrary.SceneFadeOut(goodFader, SwitchMainScreen);
        };
        Button badNext = badDocument.rootVisualElement.Q<Button>("NextButton");
        badNext.clicked += () =>
        {
            SEManager.PlaySelectSE();
            CulculateLibrary.SceneFadeOut(badFader, SwitchMainScreen);
        };
        Button backButton = mainDocument.rootVisualElement.Q<Button>("BackButton");
        backButton.clicked += () =>
        {
            SEManager.PlayDecideSE();
            CulculateLibrary.SceneFadeOut(mainFader, GameSceneManager.ToClear);
        };
    }

    private void SwitchMainScreen()
    {
        sceneView.SwitchMainScreen();
        mainFader.RemoveFromClassList("fader-active");
    }

    private void UpdateMaxID(bool isGoodEnding)
    {
        if(!isGoodEnding)
        {
            return;
        }
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

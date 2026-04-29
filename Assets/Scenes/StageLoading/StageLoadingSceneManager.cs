using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class StageLoadingSceneManager : MonoBehaviour
{
    [SerializeField] private UIDocument document;

    async void Awake()
    {
        ExtinguishingContract.DevelopOnlyGameSetup();
        StageLoadingSceneView();
        await Task.Delay(5000);
        SaveDataManager.Instance.StageLoadingSceneSaveDataInitialize();
        GameSceneManager.ToStage();
    }

    private void StageLoadingSceneView()
    {
        Label title = document.rootVisualElement.Q<Label>("Title");
        int mapSelect = (int)SaveDataManager.Instance.Access<MapSelectChunk>((int)SaveDataManager.SaveDataChunk.MapSelect).data.Span[0];
        title.text = WordDataBase.Word(WordDataBase.WordSelector.MapTitle)[mapSelect] + " - " +WordDataBase.Word(WordDataBase.WordSelector.MapName)[mapSelect];

        Label topic = document.rootVisualElement.Q<Label>("Topic");
        int tableNum = TextDataBase.GetTexts(TextDataBase.TextDictionary.StageLoading).Length;
        int selector = UnityEngine.Random.Range(0, tableNum);
        topic.text = TextDataBase.GetTexts(TextDataBase.TextDictionary.StageLoading)[selector];
    }
}

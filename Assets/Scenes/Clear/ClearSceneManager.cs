using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class ClearSceneManager : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    async void Awake()
    {
        ExtinguishingContract.DevelopOnlyGameSetup();
        document.rootVisualElement.Q<Label>("Text").text = TextDataBase.GetTexts(TextDataBase.TextDictionary.Clear)[0];
        VisualElement fader = document.rootVisualElement.Q<VisualElement>("Fader");
        CulculateLibrary.SceneFadeIn(fader);
        await Task.Delay(3000);
        CulculateLibrary.SceneFadeOut(fader, GameSceneManager.ToTitle);
    }
}

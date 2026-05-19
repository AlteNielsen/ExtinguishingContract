using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class CircleSceneManager : MonoBehaviour
{
    [SerializeField] UIDocument document;

    async void Awake()
    {
        VisualElement fader = document.rootVisualElement.Q<VisualElement>("Fader");
        CulculateLibrary.SceneFadeIn(fader);
        await Task.Delay(3000);
        CulculateLibrary.SceneFadeOut(fader, GameSceneManager.ToTitle);
    }
}

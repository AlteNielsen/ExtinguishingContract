using System.Threading.Tasks;
using UnityEngine;

public class ClearSceneManager : MonoBehaviour
{
    async void Awake()
    {
        await Task.Delay(3000);
        GameSceneManager.ToTitle();
    }
}

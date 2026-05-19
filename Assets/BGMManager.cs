using System;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }
    private AudioSource audio;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audio = GetComponent<AudioSource>();
            ReadOnlySpan<float> datas = SaveDataManager.Instance.Access<SettingChunk>((int)SaveDataManager.SaveDataChunk.Setting).data.Span;
            SetVolume(datas[0] * datas[1]);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetVolume(float volume)
    {
        audio.volume = volume;
    }
}

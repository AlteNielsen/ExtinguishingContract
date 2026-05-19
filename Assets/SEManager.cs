using UnityEngine;

public class SEManager : MonoBehaviour
{
    public static SEManager Instance { get; private set; }
    private AudioSource audioSource;

    [SerializeField] private AudioClip selectSE;
    [SerializeField] private AudioClip decideSE;
    [SerializeField] private AudioClip cancelSE;
    [SerializeField] private AudioClip loadingSE;
    [SerializeField] private AudioClip alert;
    [SerializeField] private AudioClip ending;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public static void PlaySelectSE()
    {
        Instance.audioSource.PlayOneShot(Instance.selectSE);
    }

    public static void PlayDecideSE()
    {
        Instance.audioSource.PlayOneShot(Instance.decideSE);
    }

    public static void PlayCancelSE()
    {
        Instance.audioSource.PlayOneShot(Instance.cancelSE);
    }

    public static void PlayLoadingSE()
    {
        Instance.audioSource.PlayOneShot(Instance.loadingSE);
    }

    public static void PlayAlertSE()
    {
        Instance.audioSource.PlayOneShot(Instance.alert);
    }

    public static void PlayEndSE()
    {
        Instance.audioSource.PlayOneShot(Instance.ending);
    }
}

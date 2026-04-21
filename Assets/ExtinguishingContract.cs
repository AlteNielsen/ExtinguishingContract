using UnityEngine;
using UnityEngine.SceneManagement;

public static class ExtinguishingContract
{
    public const int EIndicatorNum = 6;
    public const int CIndicatorNum = 9;
    public const int CIndicatorMaxLv = 10;
    public const int IndicatorChoicesNum = 9;

    public static void GameSetup()
    {
        DataBaseSetup();
    }

    private static void DataBaseSetup()
    {
        Config.ConfigSetup();
        ContractIndicatorDataBase.CIndicatorSetup();
        ExtinguishingIndicatorDataBase.EIndicatorSetup();
        UnitDataBase.UnitDataSetup();
        MapDataBase.MapDataSetup();

        WordDataBase.Setup();
        TextDataBase.Setup();

        TextureDataBase.Setup();

        new SaveDataManager();

        int lang = SaveDataManager.Instance.Access<SettingChunk>((int)SaveDataManager.SaveDataChunk.Setting).GetLang();

        WordDataBase.Load(lang);
        TextDataBase.Load(lang);

        TextureDataBase.Load();
        SaveDataManager.Instance.Save();
    }

    public static void DevelopOnlyGameSetup()
    {
        if (SaveDataManager.Instance == null)
        {
            GameSetup();
        }
    }
}

public static class GameSceneManager
{
    public static void ToTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public static void ToContract()
    {
        SceneManager.LoadScene("ContractScene");
    }

    public static void ToHome()
    {
        SceneManager.LoadScene("HomeScene");
    }

    public static void ToHelp()
    {
        SceneManager.LoadScene("HelpScene");
    }

    public static void QuitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public static void ToGameLoading()
    {
        SceneManager.LoadScene("GameLoadingScene");
    }
}

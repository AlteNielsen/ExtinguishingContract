using UnityEngine;
using UnityEngine.SceneManagement;

public static class ExtinguishingContract
{
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
}

public static class GameSceneManager
{
    public static void TitleToContract()
    {
        SceneManager.LoadScene("ContractScene");
    }

    public static void TitleToHome()
    {
        SceneManager.LoadScene("HomeScene");
    }

    public static void TitleToHelp()
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
}

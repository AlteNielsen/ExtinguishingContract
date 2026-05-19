using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ExtinguishingContract
{
    public const int EIndicatorNum = 6;
    public const int CIndicatorNum = 9;
    public const int CIndicatorMaxLv = 10;
    public const int IndicatorChoicesNum = 9;
    public const int ResolutionNum = 6;

    public static Vector2Int GetResolution(int index)
    {
        switch (index)
        {
            case 0:
                return new Vector2Int(1280, 720);
            case 1:
                return new Vector2Int(1920, 1080);
            case 2:
                return new Vector2Int(2560, 1440);
            case 3:
                return new Vector2Int(3840, 2160);
            case 4:
                return new Vector2Int(3440, 1440);
            case 5:
                return new Vector2Int(2160, 1440);
        }
        return new Vector2Int(1920, 1080);
    }

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
    }

    public static void DevelopOnlyGameSetup()
    {
        if (SaveDataManager.Instance == null)
        {
            GameSetup();
        }
    }

    public static void ReloadLang()
    {
        int lang = SaveDataManager.Instance.Access<SettingChunk>((int)SaveDataManager.SaveDataChunk.Setting).GetLang();
        WordDataBase.Load(lang);
        TextDataBase.Load(lang);
    }
}

public static class GameSceneManager
{
    private static GameScenes beforeScene;
    public static bool helpIsSetting;

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

    public static void ToHelp(GameScenes yourScene, bool isSetting)
    {
        beforeScene = yourScene;
        helpIsSetting = isSetting;
        SceneManager.LoadScene("HelpScene");
    }

    public static void BackFromHelp()
    {
        switch(beforeScene)
        {
            case GameScenes.Title:
                ToTitle();
                break;
            case GameScenes.Stage:
                ToStage();
                break;
        }
    }

    public static void ReloadHelp()
    {
        helpIsSetting = true;
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

    public static void ToUnit()
    {
        SceneManager.LoadScene("UnitScene");
    }

    public static void ToEnding()
    {
        SceneManager.LoadScene("EndingScene");
    }

    public static void ToClear()
    {
        SceneManager.LoadScene("ClearScene");
    }

    public static void ToStage()
    {
        SceneManager.LoadScene("StageScene");
    }

    public static void ToResult()
    {
        SceneManager.LoadScene("ResultScene");
    }

    public static void ToStageLoading()
    {
        SceneManager.LoadScene("StageLoadingScene");
    }
}

public enum GameScenes
{
    Title,
    Stage
}

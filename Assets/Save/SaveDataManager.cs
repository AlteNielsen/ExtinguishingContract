using Ray.Data;

public class SaveDataManager : RayAbstractDataManager
{
    public static SaveDataManager Instance {  get; private set; }

    public SaveDataManager()
    {
        base.Setup();
        if(Instance == null)
        {
            Instance = this;
        }
    }

    protected override void ChunkRegistry()
    {
        Register<NowIDChunk>();
        Register<MaxIDChunk>();
        Register<SettingChunk>();
        Register<BurningSituationChunk>();
        Register<BlockIndicatorChunk>();
        Register<OtherProgressChunk>();
        Register<MapSelectChunk>();
        Register<IndicatorSelectChunk>();
        Register<UnitSelectChunk>();
        Register<UnitLevelChunk>();
        Register<FireMapChunk>();
        Register<UnitMapChunk>();
        Register<UnitFacingChunk>();
        Register<ResultStatsChunk>();
        Register<TurnStatsChunk>();
    }

    public enum SaveDataChunk
    {
        NowID,
        MaxID,
        Setting,
        BurningSituation,
        BlockIndicator,
        OtherProgress,
        MapSelect,
        IndicatorSelect,
        UnitSelect,
        UnitLevel,
        FireMap,
        UnitMap,
        UnitFacing,
        ResultStats,
        TurnStats
    }

    public void TitleSceneSaveDataInitialize()
    {
        Initialize((int)SaveDataChunk.FireMap);
        Initialize((int)SaveDataChunk.UnitMap);
        Initialize((int)SaveDataChunk.UnitFacing);
    }

    public void GameLoadingSceneSaveDataInitialize()
    {
        Initialize((int)SaveDataChunk.BurningSituation);
        Initialize((int)SaveDataChunk.BlockIndicator);
        Initialize((int)SaveDataChunk.OtherProgress);
        Initialize((int)SaveDataChunk.MapSelect);
        Initialize((int)SaveDataChunk.IndicatorSelect);
        Initialize((int)SaveDataChunk.UnitSelect);
        Initialize((int)SaveDataChunk.UnitLevel);
        Initialize((int)SaveDataChunk.FireMap);
        Initialize((int)SaveDataChunk.UnitMap);
        Initialize((int)SaveDataChunk.UnitFacing);
        Initialize((int)SaveDataChunk.ResultStats);
        Initialize((int)SaveDataChunk.TurnStats);
    }

    public void ResultSceneSaveDataInitialize()
    {
        Initialize((int)SaveDataChunk.MapSelect);
        Initialize((int)SaveDataChunk.IndicatorSelect);
        Initialize((int)SaveDataChunk.UnitSelect);
        Initialize((int)SaveDataChunk.UnitLevel);
        Initialize((int)SaveDataChunk.FireMap);
        Initialize((int)SaveDataChunk.UnitMap);
        Initialize((int)SaveDataChunk.UnitFacing);
    }

    public void EndingSceneSaveDataInitialize()
    {
        Initialize((int)SaveDataChunk.NowID);
        Initialize((int)SaveDataChunk.BurningSituation);
        Initialize((int)SaveDataChunk.BlockIndicator);
        Initialize((int)SaveDataChunk.OtherProgress);
        Initialize((int)SaveDataChunk.MapSelect);
        Initialize((int)SaveDataChunk.IndicatorSelect);
        Initialize((int)SaveDataChunk.UnitSelect);
        Initialize((int)SaveDataChunk.UnitLevel);
        Initialize((int)SaveDataChunk.FireMap);
        Initialize((int)SaveDataChunk.UnitMap);
        Initialize((int)SaveDataChunk.UnitFacing);
        Initialize((int)SaveDataChunk.ResultStats);
        Initialize((int)SaveDataChunk.TurnStats);
    }

    public void StageLoadingSceneSaveDataInitialize()
    {
        Initialize((int)SaveDataChunk.FireMap);
        Initialize((int)SaveDataChunk.UnitMap);
        Initialize((int)SaveDataChunk.UnitFacing);
    }
}

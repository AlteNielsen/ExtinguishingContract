using System;
using UnityEngine;
using UnityEngine.UIElements;

public class StageSceneManager : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    [SerializeField] private VisualTreeAsset tile;
    private StageSceneView sceneView; 
    private StageBoardView boardView;
    private int spreadSpeed;
    private int width;
    private int height;
    private bool[] fireMap;
    private bool[] mapLayout;
    private int[] unitMap;
    private int[] unitFacing;

    void Awake()
    {
        ExtinguishingContract.DevelopOnlyGameSetup();
        boardView = new StageBoardView(document, tile);
        sceneView = new StageSceneView(document);
        var (speed, start) = GetParameter();
        spreadSpeed = speed;
        DataSetup();
        StartProcess(start);
    }

    private (int speed, int start) GetParameter()
    {
        float[] data = CulculateLibrary.IndicatorBaseValues(SaveDataManager.Instance.Access<NowIDChunk>((int)SaveDataManager.SaveDataChunk.NowID).data.Span);
        ReadOnlySpan<float> indicators = SaveDataManager.Instance.Access<IndicatorSelectChunk>((int)SaveDataManager.SaveDataChunk.IndicatorSelect).data.Span;
        int speed = 0;
        for(int i = 0; i < Config.Data.IndicatorMaxLv; i++)
        {
            if(indicators[Config.Data.IndicatorMaxLv * 4 + i] > 0.5f)
            {
                speed += (int)(data[4] * (i + 1));
            }
        }
        int start = 0;
        for (int i = 0; i < Config.Data.IndicatorMaxLv; i++)
        {
            if (indicators[Config.Data.IndicatorMaxLv * 5 + i] > 0.5f)
            {
                speed += (int)(data[5] * (i + 1));
            }
        }
        return (speed, start);
    }

    private void DataSetup()
    {
        int selected = (int)SaveDataManager.Instance.Access<MapSelectChunk>((int)SaveDataManager.SaveDataChunk.MapSelect).data.Span[0];
        MapLayout datas = MapDataBase.Datas[selected].Data;
        width = datas.width;
        height = datas.height;
        fireMap = new bool[datas.layout.Length];
        CulculateLibrary.SwitchFloatToBool(fireMap, SaveDataManager.Instance.Access<FireMapChunk>((int)SaveDataManager.SaveDataChunk.FireMap).data.Span);
        mapLayout = new bool[datas.layout.Length];
        for(int i = 0; i < mapLayout.Length; i++)
        {
            if (datas.layout[i] == 1)
            {
                mapLayout[i] = true;
            }
            else
            {
                mapLayout[i] = false;
            }
        }
        unitMap = new int[datas.layout.Length];
        CulculateLibrary.SwitchFloatToInt(unitMap, SaveDataManager.Instance.Access<UnitMapChunk>((int)SaveDataManager.SaveDataChunk.UnitMap).data.Span);
        unitFacing = new int[height];
        CulculateLibrary.SwitchFloatToInt(unitFacing, SaveDataManager.Instance.Access<UnitFacingChunk>((int)SaveDataManager.SaveDataChunk.UnitFacing).data.Span);
    }

    private void StartProcess(int start)
    {
        bool isStart = false;
        for(int i = 0; i < fireMap.Length; i++)
        {
            if (fireMap[i])
            {
                isStart = true;
                break;
            }
        }

        if(isStart)
        {
            return;
        }
        else
        {
            int selected = (int)SaveDataManager.Instance.Access<MapSelectChunk>((int)SaveDataManager.SaveDataChunk.MapSelect).data.Span[0];
            MapLayout datas = MapDataBase.Datas[selected].Data;
            fireMap[datas.fire_point] = true;
            for(int i = 0; i < start; i++)
            {
                FireSpread();
            }
        }
        boardView.DisplayBurning(fireMap);
    }

    private void FireSpread()
    {
        for (int j = 0; j < spreadSpeed; j++)
        {
            Span<bool> original = stackalloc bool[fireMap.Length];
            fireMap.CopyTo(original);
            for (int i = 0; i < fireMap.Length; i++)
            {
                if (original[i])
                {
                    int posX = i % width;
                    int posY = i / width;
                    if (0 < posX)
                    {
                        fireMap[i - 1] = true;
                    }
                    if ((posX + 1) < width)
                    {
                        fireMap[i + 1] = true;
                    }
                    if (0 < posY)
                    {
                        fireMap[i - width] = true;
                    }
                    if ((posY + 1) < height)
                    {
                        fireMap[i + width] = true;
                    }
                }
            }
            for (int i = 0; i < fireMap.Length; ++i)
            {
                if (mapLayout[i])
                {
                    fireMap[i] = false;
                }
            }
        }
    }
}

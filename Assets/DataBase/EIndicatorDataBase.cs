using System;
using System.IO;
using UnityEngine;
using Ray.FileIO;

public static class ExtinguishingIndicatorDataBase
{
    public static ExtinguishingIndicatorData Data {  get; private set; }

    private static readonly string filePath = Path.Combine("Contents", "Master", "Indicator", "extinguishing_indicator.json");

    public static void EIndicatorSetup()
    {
        Data = RayFileLoader.LoadJSON<ExtinguishingIndicatorData>(filePath);
        Data.Setup();
    }
}

[Serializable]
public class ExtinguishingIndicatorData
{
    [SerializeField] private EIndicator deployment;
    [SerializeField] private EIndicator unit_base_pressure;
    [SerializeField] private EIndicator lv_pressure_ratio;
    [SerializeField] private EIndicator usable_pressure;
    [SerializeField] private EIndicator spread_speed;
    [SerializeField] private EIndicator start_turn;

    public EIndicator Deployment { get; private set; }
    public EIndicator UnitBasePressure { get; private set; }
    public EIndicator LvPressureRatio {  get; private set; }
    public EIndicator UsablePressure {  get; private set; }
    public EIndicator SpreadSpeed { get; private set; }
    public EIndicator StartTurn { get; private set; }

    public void Setup()
    {
        Deployment = deployment;
        UnitBasePressure = unit_base_pressure;
        LvPressureRatio = lv_pressure_ratio;
        UsablePressure = usable_pressure;
        SpreadSpeed = spread_speed;
        StartTurn = start_turn;
    }
}

[Serializable]
public class EIndicator
{
    public string icon_path;
    public int base_value;
    public int lv_increase;
}
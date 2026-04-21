using UnityEngine;
using System;
using System.IO;
using Ray.FileIO;

public static class Config
{
    public static ConfigData Data {  get; private set; }

    private static readonly string filePath = Path.Combine("Contents", "Master", "Config", "config.json");
    public static void ConfigSetup()
    {
        Data = RayFileLoader.LoadJSON<ConfigData>(filePath);
        Data.Setup();
    }
}

[Serializable]
public class ConfigData
{
    [SerializeField] private int unit_base_pressure;
    [SerializeField] private float unit_lv_increase_ratio;
    [SerializeField] private int usable_pressure;
    [SerializeField] private int spread_speed;

    [SerializeField] private float initial_chance;
    [SerializeField] private int start_burning_block;
    [SerializeField] private int burning_speed;

    [SerializeField] private int unit_max_lv;
    [SerializeField] private int indicator_max_lv;
    [SerializeField] private float indicator_base_chance;


    public int UnitBasePressure { get; private set; }
    public float UnitLvIncreaseRatio { get; private set; }
    public int UsablePressure {  get; private set; }
    public int SpreadSpeed {  get; private set; }

    public float InitialChance {  get; private set; }
    public int StartBurningBlock { get; private set; }
    public int BurningSpeed { get; private set; }

    public int UnitMaxLv {  get; private set; }
    public int IndicatorMaxLv { get; private set; }
    public float IndicatorBaseChance { get; private set; }

    public void Setup()
    {
        UnitBasePressure = unit_base_pressure;
        UnitLvIncreaseRatio = unit_lv_increase_ratio;
        UsablePressure = usable_pressure;
        SpreadSpeed = spread_speed;

        InitialChance = initial_chance;
        StartBurningBlock = start_burning_block;
        BurningSpeed = burning_speed;

        UnitMaxLv = unit_max_lv;
        IndicatorMaxLv = indicator_max_lv;
        IndicatorBaseChance = indicator_base_chance;
    }
}

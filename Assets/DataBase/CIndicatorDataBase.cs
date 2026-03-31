using UnityEngine;
using System;
using System.IO;
using Ray.FileIO;

public static class ContractIndicatorDataBase
{
    public static ContractIndicatorData Data {  get; private set; }

    private static readonly string filePath = Path.Combine("Contents", "Master", "Indicator", "contract_indicator.json");

    public static void CIndicatorSetup()
    {
        Data = RayFileLoader.LoadJSON<ContractIndicatorData>(filePath);
        Data.Setup();
    }
}

[Serializable]
public class ContractIndicatorData
{
    [SerializeField] private CIndicator deployment;
    [SerializeField] private CIndicator unit_base_pressure;
    [SerializeField] private CIndicator lv_pressure_ratio;
    [SerializeField] private CIndicator usable_pressure;
    [SerializeField] private CIndicator spread_speed;
    [SerializeField] private CIndicator start_turn;

    [SerializeField] private CIndicator initial_chance;
    [SerializeField] private CIndicator start_burning_block;
    [SerializeField] private CIndicator burning_speed;


    public CIndicator Deployment {  get; private set; }
    public CIndicator UnitBasePressure { get; private set; }
    public CIndicator LvPressureRatio { get; private set; }
    public CIndicator UsablePressure {  get; private set; }
    public CIndicator SpreadSpeed { get; private set; }
    public CIndicator StartTurn { get; private set; }

    public CIndicator InitialChance {  get; private set; }
    public CIndicator StartBurningBlock {  get; private set; }
    public CIndicator BurningSpeed { get; private set; }

    public void Setup()
    {
        Deployment = deployment;
        UnitBasePressure = unit_base_pressure;
        LvPressureRatio = lv_pressure_ratio;
        UsablePressure = usable_pressure;
        SpreadSpeed = spread_speed;
        StartTurn = start_turn;
        InitialChance = initial_chance;
        StartBurningBlock = start_burning_block;
        BurningSpeed = burning_speed;

    }
}

[Serializable]
public class CIndicator
{
    public int start_ratio;
    public int final_ratio;
}

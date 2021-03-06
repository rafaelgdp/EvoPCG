using Godot;
using System;

public class GenerationResult {
    public int TestId;
    public int BestFitness;
    public int GenerationNumber;
    public float FivePercentBestFitnessAvg;
    public bool IsBestPlayable;
    public float FitnessAverage;
    public float FitnessMedian;
    public int PopulationSize;
    public int GenChunkSize;
    public float MutationRate;
    public float Elitism;
    public double ExecutionTimeMs;

    public static string[] GetHeaderStringArray() {
        string[] csvHeaderArray = {
            "TestId",
            "BestFitness",
            "GenerationNumber",
            "FivePercentBestFitnessAvg",
            "IsBestPlayable",
            "FitnessAverage",
            "FitnessMedian",
            "PopulationSize",
            "GenChunkSize",
            "MutationRate",
            "Elitism",
            "ExecutionTimeMs"
        };
        return csvHeaderArray;
    }
    public string[] GetCsvStringArray() {
        string[] csvStringArray = {
            $"{TestId}",
            $"{BestFitness}",
            $"{GenerationNumber}",
            $"{FivePercentBestFitnessAvg}",
            $"{IsBestPlayable}",
            $"{FitnessAverage}",
            $"{FitnessMedian}",
            $"{PopulationSize}",
            $"{GenChunkSize}",
            $"{MutationRate}",
            $"{Elitism}",
            $"{ExecutionTimeMs}"
        };
        return csvStringArray;
    }
}

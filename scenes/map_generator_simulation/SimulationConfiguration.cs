using Godot;
using System;
using System.Collections.Generic;

public class SimulationConfiguration
{
    int TestId;
    int PopulationSize;
    float MutationRate;
    int MaxIterations;
    int RefChunkSize;
    int GenChunkSize;
    float Elitism;
    public GAMapGenerator Generator;
    public TestResult Result;


    public TestConfiguration(
        int populationSize,
        float mutationRate,
        float elitism,
        int maxIterations,
        int genChunkSize,
        int refChunkSize)
    {

        this.TestId = 1;
        this.PopulationSize = populationSize;
        this.MutationRate = mutationRate;
        this.MaxIterations = maxIterations;
        this.RefChunkSize = refChunkSize;
        this.GenChunkSize = genChunkSize;
        this.Elitism = elitism;
        this.Generator = new GAMapGenerator(5, genChunkSize, 12, 0, populationSize, mutationRate, elitism, 10, false);
    }

    public void StartTest(RTLLog log = null)
    {
        File testData = new File();
        string fileName = "simulation.csv";
        if (testData.FileExists(fileName))
        {
            testData.Open(fileName, File.ModeFlags.ReadWrite);
            testData.GetCsvLine();
            while (!testData.EofReached())
            {
                Int32.TryParse(testData.GetCsvLine()[0], out int tId);
                if (this.TestId <= tId)
                {
                    this.TestId++;
                }
            }
        }
        else
        {
            testData.Open(fileName, File.ModeFlags.Write);
            testData.StoreCsvLine(GenerationResult.GetHeaderStringArray());
        }
        if (log != null)
        {
            log.LogLine($"Initiating simulation id {this.TestId} with config: popSize {this.PopulationSize}, mutRate {MutationRate}, elitism {Elitism}, maxIterations {MaxIterations}.");
        }
        // First generation of this test batch
        // Start timer to track ellapsed time per generation
        var watch = System.Diagnostics.Stopwatch.StartNew();
        double duration = 0;
        // the code that you want to measure comes here
        int genNumber = 0;
        List<MapIndividual> population = Generator.EvolveTestChunkOnRight();
        MapIndividual bestIndividual = population[population.Count - 1];
        GenerationResult currentGenerationResult = new GenerationResult
        {
            TestId = this.TestId,
            BestFitness = bestIndividual.Fitness,
            GenerationNumber = genNumber,
            FivePercentBestFitnessAvg = getFivePercentAvg(population),
            IsBestPlayable = bestIndividual.IsPlayable,
            FitnessAverage = getFitnessAverage(population),
            FitnessMedian = getFitnessMedian(population),
            PopulationSize = this.PopulationSize,
            GenChunkSize = this.GenChunkSize,
            MutationRate = this.MutationRate,
            Elitism = this.Elitism,
            BestGenome = bestIndividual.toString(),
        };
        watch.Stop();
        currentGenerationResult.ExecutionTimeMs = watch.ElapsedMilliseconds;
        testData.StoreCsvLine(currentGenerationResult.GetCsvStringArray());

        for (genNumber = genNumber + 1; genNumber <= MaxIterations; genNumber++)
        {
            watch = System.Diagnostics.Stopwatch.StartNew();
            population = Generator.EvolveTestChunkOnRight(population);
            bestIndividual = population[population.Count - 1];
            currentGenerationResult = new GenerationResult
            {
                TestId = this.TestId,
                BestFitness = bestIndividual.Fitness,
                GenerationNumber = genNumber,
                FivePercentBestFitnessAvg = getFivePercentAvg(population),
                IsBestPlayable = bestIndividual.IsPlayable,
                FitnessAverage = getFitnessAverage(population),
                FitnessMedian = getFitnessMedian(population),
                PopulationSize = this.PopulationSize,
                GenChunkSize = this.GenChunkSize,
                MutationRate = this.MutationRate,
                Elitism = this.Elitism,
            };
            watch.Stop();
            currentGenerationResult.ExecutionTimeMs = watch.ElapsedMilliseconds;
            duration += watch.ElapsedMilliseconds;
            testData.StoreCsvLine(currentGenerationResult.GetCsvStringArray());
        }
        // Close file at the end of this test
        testData.Close();
        TimeSpan t = TimeSpan.FromMilliseconds(duration);
        string durationString = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                                        t.Hours, t.Minutes, t.Seconds, t.Milliseconds);
        if (log != null) log.LogLine($"Done with test {this.TestId} in {durationString}!");
    }

    private float getFitnessAverage(List<MapIndividual> population)
    {
        float fitAvg = 0F;
        for (int i = 0; i < population.Count; i++)
        {
            fitAvg += population[i].Fitness;
        }
        fitAvg = fitAvg / population.Count;
        return fitAvg;
    }

    private float getFitnessMedian(List<MapIndividual> population)
    {
        int count = population.Count;
        int mid = population.Count / 2;
        if (count % 2 == 0)
        {
            return (population[mid - 1].Fitness + population[mid].Fitness) / 2F;
        }
        return population[mid].Fitness;
    }

    float getFivePercentAvg(List<MapIndividual> population)
    {
        float fivePerBestAvg = 0F;
        int fivePercentCount = (int)Mathf.Ceil((population.Count / 100F) * 5F);
        int lowerIndex = Mathf.Max(population.Count - fivePercentCount, 0);
        for (int i = population.Count - 1; i >= lowerIndex; i--)
        {
            fivePerBestAvg += population[i].Fitness;
        }
        fivePerBestAvg = fivePerBestAvg / fivePercentCount;
        return fivePerBestAvg;
    }

}
using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;


public class MapGeneratorSimulator : Node
{
    // int[] populationSizes = {10, 25, 50, 75, 100, 150, 200, 250};
    int[] populationSizes = { 10 };
    float[] mutationRates = { 0.005F, 0.01F, 0.025F, 0.05F, 0.1F };
    float[] elitisms = { 0.05F, 0.1F, 0.2F };
    int[] maxIterations = { 1000 };

    RTLLog logLabel;

    List<TestConfiguration> configurations = new List<TestConfiguration>();
    public override void _Ready()
    {
        logLabel = GetNode("CanvasLayer/RTLLog") as RTLLog;
        logLabel.LogLine("Prepping configs...");
        foreach (var populationSize in populationSizes)
        {
            foreach (var mutationRate in mutationRates)
            {
                foreach (var elitism in elitisms)
                {
                    foreach (var maxIteration in maxIterations)
                    {
                        GD.Print($"{populationSize}, {mutationRate}, {maxIteration}");
                        var config = new TestConfiguration(
                                            populationSize, // int popupationSize,
                                            mutationRate, // float mutationRate,
                                            elitism, // float elitism,
                                            maxIteration, // int maxIterations,
                                            100, // int genChunkSize,
                                            5 // int refChunkSize
                                        );
                        configurations.Add(config);
                    }
                }
            }
        }
        logLabel.LogLine("Configs ready.");
    }

    void _on_AnalysisScene_ready()
    {
        Task.Run(() => runTests());
    }

    void runTests()
    {
        foreach (var configuration in configurations)
        {
            configuration.StartTest(logLabel);
        }
        logLabel.LogLine("Done!");
    }

}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Vision;

public class Program
{
    static readonly string inputDataDirectoryPath = Path.Combine(Environment.CurrentDirectory, "..", "pieces");
    static MLContext mlContext = new MLContext(seed: 1);
    private static TextWriter outBack;
    private static TextWriter errBack;

    public class ModelInput
    {
        public string Label { get; set; }
        public string ImageSource { get; set; }
    }

    public class ModelOutput
    {
        public String PredictedLabel { get; set; }
    }

    static (ITransformer mlModel, IReadOnlyList<TrainCatalogBase.CrossValidationResult<MulticlassClassificationMetrics>> evaluation) TrainModel(ImageClassificationTrainer.Architecture architecture, int epoch)
    {
        // To suppress errors from the TensorFlow library, set $env:TF_CPP_MIN_LOG_LEVEL = 2
        var inputs = new List<ModelInput>();
        foreach (var subDir in Directory.GetDirectories(inputDataDirectoryPath))
        {
            foreach (var file in Directory.GetFiles(subDir))
            {
                inputs.Add(new ModelInput() { Label = subDir.Split("\\").Last(), ImageSource = file });
            }
        }
        var trainingDataView = mlContext.Data.LoadFromEnumerable<ModelInput>(inputs);
        var dataProcessPipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", "Label")
                                    .Append(mlContext.Transforms.LoadRawImageBytes("ImageSource_featurized", null, "ImageSource"))
                                    .Append(mlContext.Transforms.CopyColumns("Features", "ImageSource_featurized"));
        var trainer = mlContext.MulticlassClassification.Trainers.ImageClassification(
                                        new ImageClassificationTrainer.Options() 
                                            {
                                                Arch = architecture,
                                                Epoch = epoch,
                                                FeatureColumnName = "Features",
                                                LabelColumnName = "Label", 
                                            })
                                    .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"));
        IEstimator<ITransformer> trainingPipeline = dataProcessPipeline.Append(trainer);
        var mlModel = trainingPipeline.Fit(trainingDataView);
        var evaluation = mlContext.MulticlassClassification.CrossValidate(trainingDataView, trainingPipeline, numberOfFolds: 5, labelColumnName: "Label");
        return (mlModel, evaluation);
    }

    static ModelOutput Classify(PredictionEngine<ModelInput, ModelOutput> predEngine, string filePath)
    {
        ModelInput input = new ModelInput() { ImageSource = filePath };
        return predEngine.Predict(input);
    }

    static void CalculateAndPrintAverageMetrics(IEnumerable<TrainCatalogBase.CrossValidationResult<MulticlassClassificationMetrics>> crossValResults)
    {
        var metricsInMultipleFolds = crossValResults.Select(r => r.Metrics);

        var retVal = new Dictionary<string, (double Avg, double StdDev)>();

        retVal["MicroAccuracy"] = CalculateAverageMetrics(metricsInMultipleFolds.Select(m => m.MicroAccuracy));
        retVal["MacroAccuracy"] = CalculateAverageMetrics(metricsInMultipleFolds.Select(m => m.MacroAccuracy));
        retVal["LogLoss"] = CalculateAverageMetrics(metricsInMultipleFolds.Select(m => m.LogLoss));
        retVal["LogLossReduction"] = CalculateAverageMetrics(metricsInMultipleFolds.Select(m => m.LogLossReduction));

        Console.WriteLine($"Avg. MicroAccuracy (Std. Dev):    {retVal["MicroAccuracy"].Avg:0.###} ({retVal["MicroAccuracy"].StdDev:#.###})");
        Console.WriteLine($"Avg. MacroAccuracy (Std. Dev):    {retVal["MacroAccuracy"].Avg:0.###} ({retVal["MacroAccuracy"].StdDev:#.###})");
        Console.WriteLine($"Avg. LogLoss (Std. Dev):          {retVal["LogLoss"].Avg:#.###} ({retVal["LogLoss"].StdDev:#.###})");
        Console.WriteLine($"Avg. LogLossReduction (Std. Dev): {retVal["LogLossReduction"].Avg:#.###} ({retVal["LogLossReduction"].StdDev:#.###})");
    }

    static (double, double) CalculateAverageMetrics(IEnumerable<double> metricValues)
    {
        return (metricValues.Average(), CalculateStandardDeviation(metricValues));
    }

    static double CalculateStandardDeviation(IEnumerable<double> values)
    {
        double average = values.Average();
        double sumOfSquaresOfDifferences = values.Select(val => (val - average) * (val - average)).Sum();
        double standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / (values.Count() - 1));
        return standardDeviation;
    }

    static void TestClassifier(ITransformer model)
    {
        var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(model);

        var result = Classify(predEngine, Path.Combine(Environment.CurrentDirectory, "Black.jpg"));
        Console.WriteLine($"Testing with black piece. Prediction: {result.PredictedLabel}.");
        result = Classify(predEngine, Path.Combine(Environment.CurrentDirectory, "Blue.jpg"));
        Console.WriteLine($"Testing with blue piece. Prediction: {result.PredictedLabel}.");
        result = Classify(predEngine, Path.Combine(Environment.CurrentDirectory, "Green.jpg"));
        Console.WriteLine($"Testing with green piece. Prediction: {result.PredictedLabel}.");
        result = Classify(predEngine, Path.Combine(Environment.CurrentDirectory,  "Yellow.jpg"));
        Console.WriteLine($"Testing with yellow piece. Prediction: {result.PredictedLabel}.");
    }

    static void Main()
    {
        var architectures = new []{ ImageClassificationTrainer.Architecture.InceptionV3, ImageClassificationTrainer.Architecture.MobilenetV2, ImageClassificationTrainer.Architecture.ResnetV2101, ImageClassificationTrainer.Architecture.ResnetV250 };
        var epochs = new[] { 50, 100, 200, 400 };

        var results = new Dictionary<(ImageClassificationTrainer.Architecture arch, int epoch), (ITransformer model, IReadOnlyList<TrainCatalogBase.CrossValidationResult<MulticlassClassificationMetrics>> metrics)>();

        foreach(var arch in architectures)
        {
            foreach(var epoch in epochs)
            {
                Console.WriteLine($"Using architecture {arch}, epochs {epoch}.");
                StopAllOutput();
                results[(arch, epoch)] = TrainModel(arch, epoch);
                RestoreAllOutput();
                CalculateAndPrintAverageMetrics(results[(arch, epoch)].metrics);
                TestClassifier(results[(arch, epoch)].model);
            }
        }
    }
    static void StopAllOutput()
    {
        outBack = Console.Out;
        Console.SetOut(TextWriter.Null);
        errBack = Console.Error;
        Console.SetError(TextWriter.Null);
    }

    static void RestoreAllOutput()
    {
        Console.SetOut(outBack);
        Console.SetError(errBack);
    }
}


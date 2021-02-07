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
    static readonly string outputModelFilePath = Path.Combine(Environment.CurrentDirectory, "model.zip");
    static MLContext mlContext = new MLContext(seed: 1);
    static ITransformer mlModel;

    public class ModelInput
    {
        public string Label { get; set; }
        public string ImageSource { get; set; }
    }

    public class ModelOutput
    {
        public String PredictedLabel { get; set; }
    }

    static void TrainModel(ImageClassificationTrainer.Architecture architecture)
    {
        // To suppress errors from the TensorFlow library, set $env:TF_CPP_MIN_LOG_LEVEL = 2

        // Create the input dataset
        var inputs = new List<ModelInput>();
        foreach (var subDir in Directory.GetDirectories(inputDataDirectoryPath))
        {
            foreach (var file in Directory.GetFiles(subDir))
            {
                inputs.Add(new ModelInput() { Label = subDir.Split("\\").Last(), ImageSource = file });
            }
        }
        var trainingDataView = mlContext.Data.LoadFromEnumerable<ModelInput>(inputs);
        // Create training pipeline
        var dataProcessPipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", "Label")
                                    .Append(mlContext.Transforms.LoadRawImageBytes("ImageSource_featurized", null, "ImageSource"))
                                    .Append(mlContext.Transforms.CopyColumns("Features", "ImageSource_featurized"));
        var trainer = mlContext.MulticlassClassification.Trainers.ImageClassification(
                                        new ImageClassificationTrainer.Options() 
                                            {
                                                Arch = architecture,
                                                LabelColumnName = "Label", 
                                                FeatureColumnName = "Features",
                                            })
                                    .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"));
        IEstimator<ITransformer> trainingPipeline = dataProcessPipeline.Append(trainer);
        // Create the model
        mlModel = trainingPipeline.Fit(trainingDataView);
        Evaluate(mlContext, trainingDataView, trainingPipeline);
    }

    static ModelOutput Classify(string filePath)
    {
        // Create input to classify
        ModelInput input = new ModelInput() { ImageSource = filePath };
        // Load model and predict
        var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
        return predEngine.Predict(input);
    }

    static void Evaluate(MLContext mlContext, IDataView trainingDataView, IEstimator<ITransformer> trainingPipeline)
    {
        Console.WriteLine("=============== Cross-validating to get model's accuracy metrics ===============");
        var crossValidationResults = mlContext.MulticlassClassification.CrossValidate(trainingDataView, trainingPipeline, numberOfFolds: 5, labelColumnName: "Label");
        PrintMulticlassClassificationFoldsAverageMetrics(crossValidationResults);
    }

    static void PrintMulticlassClassificationFoldsAverageMetrics(IEnumerable<TrainCatalogBase.CrossValidationResult<MulticlassClassificationMetrics>> crossValResults)
    {
        var metricsInMultipleFolds = crossValResults.Select(r => r.Metrics);

        var microAccuracyValues = metricsInMultipleFolds.Select(m => m.MicroAccuracy);
        var microAccuracyAverage = microAccuracyValues.Average();
        var microAccuraciesStdDeviation = CalculateStandardDeviation(microAccuracyValues);
        var microAccuraciesConfidenceInterval95 = CalculateConfidenceInterval95(microAccuracyValues);

        var macroAccuracyValues = metricsInMultipleFolds.Select(m => m.MacroAccuracy);
        var macroAccuracyAverage = macroAccuracyValues.Average();
        var macroAccuraciesStdDeviation = CalculateStandardDeviation(macroAccuracyValues);
        var macroAccuraciesConfidenceInterval95 = CalculateConfidenceInterval95(macroAccuracyValues);

        var logLossValues = metricsInMultipleFolds.Select(m => m.LogLoss);
        var logLossAverage = logLossValues.Average();
        var logLossStdDeviation = CalculateStandardDeviation(logLossValues);
        var logLossConfidenceInterval95 = CalculateConfidenceInterval95(logLossValues);

        var logLossReductionValues = metricsInMultipleFolds.Select(m => m.LogLossReduction);
        var logLossReductionAverage = logLossReductionValues.Average();
        var logLossReductionStdDeviation = CalculateStandardDeviation(logLossReductionValues);
        var logLossReductionConfidenceInterval95 = CalculateConfidenceInterval95(logLossReductionValues);

        Console.WriteLine($"*************************************************************************************************************");
        Console.WriteLine($"*       Metrics for Multi-class Classification model      ");
        Console.WriteLine($"*------------------------------------------------------------------------------------------------------------");
        Console.WriteLine($"*       Average MicroAccuracy:    {microAccuracyAverage:0.###}  - Standard deviation: ({microAccuraciesStdDeviation:#.###})  - Confidence Interval 95%: ({microAccuraciesConfidenceInterval95:#.###})");
        Console.WriteLine($"*       Average MacroAccuracy:    {macroAccuracyAverage:0.###}  - Standard deviation: ({macroAccuraciesStdDeviation:#.###})  - Confidence Interval 95%: ({macroAccuraciesConfidenceInterval95:#.###})");
        Console.WriteLine($"*       Average LogLoss:          {logLossAverage:#.###}  - Standard deviation: ({logLossStdDeviation:#.###})  - Confidence Interval 95%: ({logLossConfidenceInterval95:#.###})");
        Console.WriteLine($"*       Average LogLossReduction: {logLossReductionAverage:#.###}  - Standard deviation: ({logLossReductionStdDeviation:#.###})  - Confidence Interval 95%: ({logLossReductionConfidenceInterval95:#.###})");
        Console.WriteLine($"*************************************************************************************************************");
    }

    static double CalculateStandardDeviation(IEnumerable<double> values)
    {
        double average = values.Average();
        double sumOfSquaresOfDifferences = values.Select(val => (val - average) * (val - average)).Sum();
        double standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / (values.Count() - 1));
        return standardDeviation;
    }

    static double CalculateConfidenceInterval95(IEnumerable<double> values)
    {
        double confidenceInterval95 = 1.96 * CalculateStandardDeviation(values) / Math.Sqrt((values.Count() - 1));
        return confidenceInterval95;
    }

    static void TestClassifier()
    {
        var result = Classify(Path.Combine(Environment.CurrentDirectory, "Black.jpg"));
        Console.WriteLine($"Testing with black piece. Prediction: {result.PredictedLabel}.");
        result = Classify(Path.Combine(Environment.CurrentDirectory, "Blue.jpg"));
        Console.WriteLine($"Testing with blue piece. Prediction: {result.PredictedLabel}.");
        result = Classify(Path.Combine(Environment.CurrentDirectory, "Green.jpg"));
        Console.WriteLine($"Testing with green piece. Prediction: {result.PredictedLabel}.");
        result = Classify(Path.Combine(Environment.CurrentDirectory,  "Yellow.jpg"));
        Console.WriteLine($"Testing with yellow piece. Prediction: {result.PredictedLabel}.");
    }

    static void Main()
    {
        var architecture = ImageClassificationTrainer.Architecture.InceptionV3;
        Console.WriteLine($"Using algorithm {architecture}");
        TrainModel(architecture);
        TestClassifier();

        architecture = ImageClassificationTrainer.Architecture.MobilenetV2;
        Console.WriteLine($"Using algorithm {architecture}");
        TrainModel(architecture);
        TestClassifier();

        architecture = ImageClassificationTrainer.Architecture.ResnetV2101;
        Console.WriteLine($"Using algorithm {architecture}");
        TrainModel(architecture);
        TestClassifier();

        architecture = ImageClassificationTrainer.Architecture.ResnetV250;
        Console.WriteLine($"Using algorithm {architecture}");
        TrainModel(architecture);
        TestClassifier();
    }
}


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

    static void TrainModel()
    {
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
        var trainer = mlContext.MulticlassClassification.Trainers.ImageClassification(new ImageClassificationTrainer.Options() { LabelColumnName = "Label", FeatureColumnName = "Features" })
                                    .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"));
        IEstimator<ITransformer> trainingPipeline = dataProcessPipeline.Append(trainer);
        // Create the model
        mlModel = trainingPipeline.Fit(trainingDataView);
    }

    static ModelOutput Classify(string filePath)
    {
        // Create input to classify
        ModelInput input = new ModelInput() { ImageSource = filePath };
        // Load model and predict
        var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
        return predEngine.Predict(input);
    }

    static void Main()
    {
        TrainModel();

        var result = Classify(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Black.jpg");
        Console.WriteLine($"Testing with black piece. Prediction: {result.PredictedLabel}.");
        result = Classify(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Blue.jpg");
        Console.WriteLine($"Testing with blue piece. Prediction: {result.PredictedLabel}.");
        result = Classify(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Green.jpg");
        Console.WriteLine($"Testing with green piece. Prediction: {result.PredictedLabel}.");
        result = Classify(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Yellow.jpg");
        Console.WriteLine($"Testing with yellow piece. Prediction: {result.PredictedLabel}.");
    }
}


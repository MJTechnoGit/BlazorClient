using Microsoft.Extensions.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using BlazorClient.Utilities;
using System.Drawing;

namespace BlazorClient.Data
{
    public class ImageClassificationService
    {
        private readonly string[] _labels;
        private readonly PredictionEnginePool<ImageInputData, ImageLabelPredictions> _predictionEnginePool;

        public ImageClassificationService(PredictionEnginePool<ImageInputData, ImageLabelPredictions> predictionEnginePool)
        {
            _predictionEnginePool = predictionEnginePool;

            //Read the labels from the text file available in the output bin folder.
            string labelsFileLocation = PathUtilities.GetPathFromBinFolder(
                Path.Combine("TFInceptionModel", "imagenet_comp_graph_label_strings.txt"));
            _labels = System.IO.File.ReadAllLines(labelsFileLocation);
        }


        public ImageClassificationResult Classify(MemoryStream image)
        {
            //Convert image to bitmap and load into an ImageInputData instance.
            Bitmap bitmapImage = (Bitmap)Image.FromStream(image);
            ImageInputData imageInputData = new ImageInputData { Image = bitmapImage };

            // Run the model.
            var imageLabelPredictions = _predictionEnginePool.Predict(imageInputData);

            // Find the label with the highest probability
            // and return the ImageClassficationResult instance
            float[] probabilities = imageLabelPredictions.PredictedLabels;
            var maxProbability = probabilities.Max();
            var maxProbabilityIndex = probabilities.AsSpan().IndexOf(maxProbability);

            return new ImageClassificationResult()
            {
                Label = _labels[maxProbabilityIndex],
                Probability = maxProbability
            };
        }

    }
}

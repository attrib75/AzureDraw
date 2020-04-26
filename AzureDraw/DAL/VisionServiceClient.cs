using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AzureDraw.DAL
{
    public class VisionServiceClient
    {
        private string _apiKey;
        private string _apiEndpoint;
        private Guid _projectId;
        private string _modelName;

        public VisionServiceClient(string apiKey, string apiEndpoint, Guid projectId, string modelName)
        {
            _apiKey = apiKey;
            _apiEndpoint = apiEndpoint;
            _projectId = projectId;
            _modelName = modelName;
        }

        public async Task<ImageCreateSummary> UploadTrainingImage(MemoryStream memoryStream, string tagName)
        {
            CustomVisionTrainingClient trainingApi = new CustomVisionTrainingClient()
            {
                ApiKey = _apiKey,
                Endpoint = _apiEndpoint
            };
            var tags = await trainingApi.GetTagsAsync(_projectId);
            foreach (var tag in tags)
            {
                if (tag.Name.Equals(tagName, StringComparison.InvariantCultureIgnoreCase))
                {
                    var res = trainingApi.CreateImagesFromData(_projectId, memoryStream, new List<Guid>() { tag.Id });
                    return res;
                }
            }
            return null;
        }

        public async Task<Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models.ImagePrediction>
            PredictImage(MemoryStream memoryStream)
        {
            CustomVisionPredictionClient endpoint = new CustomVisionPredictionClient()
            {
                ApiKey = _apiKey,
                Endpoint = _apiEndpoint
            };
            var result = await endpoint.ClassifyImageAsync(_projectId, _modelName, memoryStream);

            // Loop over each prediction and write out the results
            foreach (var c in result.Predictions)
            {
                Console.WriteLine($"\t{c.TagName}: {c.Probability:P1}");
            }

            return result;
        }
    }
}
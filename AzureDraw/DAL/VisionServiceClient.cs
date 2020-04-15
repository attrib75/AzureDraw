using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;

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

        public async Task<bool> UploadTrainingImage(MemoryStream memoryStream, string tagName)
        {
            CustomVisionTrainingClient trainingApi = new CustomVisionTrainingClient()
            {
                ApiKey = _apiKey,
                Endpoint = _apiEndpoint
            };
            var tags = await trainingApi.GetTagsAsync(_projectId);
            foreach (var tag in tags)
            {
                if (tag.Description.Equals(tagName, StringComparison.InvariantCultureIgnoreCase))
                {
                    trainingApi.CreateImagesFromData(_projectId, memoryStream, new List<Guid>() { tag.Id });
                    return true;
                }
            }
            return false;
        }
    }
}
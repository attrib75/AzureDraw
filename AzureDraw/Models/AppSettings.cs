using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureDraw.Models
{
    public class AppSettings
    {
        public string ImageService_ApiKey { get; set; }
        public string ImageService_ApiEndpoint { get; set; }
        public Guid ImageService_ProjectId { get; set; }
        public string ImageService_ModelName { get; set; }

        public string SpeechService_ApiKey { get; set; }
        public string SpeechServiceRegion { get; set; }
    }
}
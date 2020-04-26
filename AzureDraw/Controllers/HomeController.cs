using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AzureDraw.Models;
using System.Net;
using AzureDraw.DAL;
using Microsoft.Extensions.Options;
using System.IO;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
using Microsoft.CognitiveServices.Speech;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CognitiveServices.Speech.Audio;

namespace AzureDraw.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<HomeController> _logger;
        protected AppSettings AppSettings { get; private set; }

        public HomeController(ILogger<HomeController> logger, IOptions<AppSettings> appSettings,
             IWebHostEnvironment env)
        {
            _logger = logger;
            AppSettings = appSettings?.Value;
            _env = env;
            CheckAudioFiles();
        }

        private void CheckAudioFiles()
        {
            var outputAudioPath = GetAudioSavePath();
            var allFiles = Directory.GetFiles(outputAudioPath);
            if (!allFiles.Contains("airplane.wav"))
            {
                GenerateAllSpeakResults();
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }

        private async Task CreateAndSaveAudioAsync(string textToSay, string filePathToSave, string fileName)
        {
            var config = SpeechConfig.FromSubscription(AppSettings.SpeechService_ApiKey, AppSettings.SpeechServiceRegion);
            using var audioConfig = AudioConfig.FromWavFileOutput($"{filePathToSave}/{fileName}.wav");
            using var synthesizer = new SpeechSynthesizer(config, audioConfig);
            await synthesizer.SpeakTextAsync(textToSay);
        }

        private string GetAudioSavePath()
        {
            string webRootPath = _env.WebRootPath;

            var filePath = $"{webRootPath}/audio";
            return filePath;
        }

        public async Task GenerateAllSpeakResults()
        {
            await CreateAndSaveAudioAsync($"I know, its an airplane!", GetAudioSavePath(), "airplane");
            await CreateAndSaveAudioAsync($"I know, its a baseball!", GetAudioSavePath(), "baseball");
            await CreateAndSaveAudioAsync($"I know, its an alarm clock!", GetAudioSavePath(), "alarmclock");
            await CreateAndSaveAudioAsync($"Sorry.  I don't know what this is", GetAudioSavePath(), "dontknow");
        }

        [HttpPost]
        public async Task<JsonResult> Predict(string imageData)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(imageData))
                {
                    var imageClient = new VisionServiceClient(AppSettings.ImageService_ApiKey, AppSettings.ImageService_ApiEndpoint, AppSettings.ImageService_ProjectId, AppSettings.ImageService_ModelName);
                    var imgd = imageData.Replace("data:image/png;base64,", string.Empty);
                    byte[] bytes = Convert.FromBase64String(imgd);
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        var predictionResult = await imageClient.PredictImage(ms);
                        //Select into DTO so each probability can be rounded
                        var newResults = predictionResult.Predictions.Select(n =>
                        new AzureDraw.DTO.PredictionModel { TagName = n.TagName, Probability = n.Probability });
                        foreach (var item in newResults)
                        {
                            item.Probability = Math.Round(item.Probability, 6);
                        }
                        var topResult = newResults.OrderByDescending(p => Math.Round(p.Probability, 6)).First();

                        return Json(topResult);
                    }
                }
                else
                {
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json("Error");
            }
        }

        public async Task<JsonResult> UploadTrainingImage(string imageData, string tagName)
        {
            try
            {
                //todo:  uncomment this if you want to upload training images
                //await UploadTrainingImageAsync(imageData, tagName);
                return Json("Ok");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                throw;
            }
        }

        private async Task<ImageCreateSummary> UploadTrainingImageAsync(string imageData, string tagName)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(imageData))
                {
                    if (tagName == "Alarm Clock")
                    {
                        tagName = "alarm-clock";
                    }
                    var imageClient = new VisionServiceClient(AppSettings.ImageService_ApiKey, AppSettings.ImageService_ApiEndpoint, AppSettings.ImageService_ProjectId, AppSettings.ImageService_ModelName);
                    var imgd = imageData.Replace("data:image/png;base64,", string.Empty);
                    byte[] bytes = Convert.FromBase64String(imgd);
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        var predictionResult = await imageClient.UploadTrainingImage(ms, tagName);
                        return predictionResult;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult QuickDrawViewer()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
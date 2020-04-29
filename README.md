# AzureDraw
This is the Azure/Cognitive Services version of [Google Quick Draw](https://quickdraw.withgoogle.com/#).   [Check out the Live Demo Here!](https://guessyourdoodle.azurewebsites.net/) I saw this and was like, yeah we Azure fans can build this but using  [Microsoft Cognitive Services](https://azure.microsoft.com/en-us/services/cognitive-services/).  See [this tweet ](https://twitter.com/PatrickGoode/status/1248373682163994626).  We can't be outdone can we?  Join the cause! Help me build this. 

##### Dependencies
This project uses Microsoft's [Custom Vision Service](https://www.customvision.ai/https://www.customvision.ai/) as the AI engine.  I started by importing *some* of the Google Quick Draw image data and training on it.  Once I built this app, I use the images users draw when running the app to additionaly train the model.

Also, to be feature parity complete with Quick Draw, I added speech to text so the app speaks its predictions, using Microsoft's [Text to Speech](https://azure.microsoft.com/en-us/services/cognitive-services/text-to-speech/) service!  


Each service requires its own provisioning and keys, so to use this app you'll need to provision your own services and host it yourself.  It is an ASP.NET core app, and uses strongly typed app settings parsed from the appsettings.json file.  To set the config for this app, construct your appsettings.json file like this:

    "AppSettings": {
    "ImageService_ApiKey": "YOUR_CUSTOM_VISION_KEY",
    "ImageService_ApiEndpoint": "YOUR_CUSTOM_VISION_ENDPOINT",
    "ImageService_ProjectId": "YOUR_CUSTOM_VISION_PROJECT_ID",
    "ImageService_ModelName": "YOUR_CUSTOM_VISION_ITERATION",
    "SpeechService_ApiKey": "YOUR_SPEECH_SERVICE_API_KEY",
    "SpeechServiceRegion": "YOUR_SPEECH_SERVICE_REGION"
     }

After that, you should be all set.  This was fun to conceptualize, then bring to reality.  The cool part for me is that the whole stack from the app platform itself all the way to the AI is all Microsoft.  Yay! Feel free to discuss an idea or even send a PR if you want to contribute.  


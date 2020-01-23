# blazor-ml

Example application that shows how to integrate a Blazor server-side application with ML.NET

- Users can upload images using the Blazor application, which are classified into one of 1000 labels using the ML.NET model.
- The ML.NET model simply loads a pre-trained TensorFlow Google's inception model (available [here](https://github.com/dotnet/machinelearning-samples/tree/master/samples/csharp/getting-started/DeepLearning_ImageClassification_TensorFlow/ImageClassification/assets/inputs/inception))


## Installation
Make sure you have the .NET Core SDK 3.1 installed (download [here](https://dotnet.microsoft.com/download)) and clone this repo.

You will see a solution with 2 projects:

- The `ModelBuilder` is used to generate the ML.NET model, which is then saved to a ZIP file for the Blazor app to classify uploaded images
  ```bash
  cd ModelBuilder
  dotnet build
  dotnet run
  # ML.NET model's zip saved to PredictionModel.zip
  ```
- The `BlazorClient` provides the server-side Blazor app. Navigate the the *Identify Image* page (or `/identify`) to upload an image (using the [BlazorInputFile](http://blog.stevensanderson.com/2019/09/13/blazor-inputfile/) component), which will be classified using the previously saved ML.NET model
  ```bash
    cd BlazorClient
    dotnet build
    dotnet run
    # navigate to https://localhost:5001/identify
  ```

There are sample images in the `SampleImages` folder (see [wikimedia.md](./SampleImages/wikimedia.md) for attribution) which you can use to test the application

### Mac/Linux users
The code relies on System.Drawing in order to convert an image into a bitmap. While System.Drawing [is now part of .NET Core](https://www.hanselman.com/blog/HowDoYouUseSystemDrawingInNETCore.aspx) you will probably need to install its GDI+ dependencies:
```bash
# Linux
sudo apt install libc6-dev
sudo apt install libgdiplus
# Mac
brew install mono-libgdiplus
```

Feel free to try and use alternatives to System.Drawing such as [ImageSharp](https://github.com/SixLabors/ImageSharp).

## Reference

- https://github.com/dotnet/machinelearning-samples/tree/master/samples/csharp/getting-started/DeepLearning_ImageClassification_TensorFlow
- https://devblogs.microsoft.com/cesardelatorre/run-with-ml-net-c-code-a-tensorflow-model-exported-from-azure-cognitive-services-custom-vision/
- https://medium.com/machinelearningadvantage/detect-any-object-in-an-image-using-c-and-ml-net-machine-learning-50e606b821a3


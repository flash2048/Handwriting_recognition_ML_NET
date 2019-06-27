using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using MulticlassClassificationML.Model.DataModels;

namespace MulticlassClassification.Controllers
{
    public class HomeController : Controller
    {
        private const int SizeOfImage = 32;
        private const int SizeOfArea = 4;
        private const string ModelName = "MLModel.zip";

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(string imgBase64)
        {
            if (string.IsNullOrEmpty(imgBase64))
            {
                return BadRequest(new { prediction = "-", dataset = string.Empty });
            }
            MLContext mlContext = new MLContext();
            ITransformer mlModel = mlContext.Model.Load(ModelName, out var modelInputSchema);
            var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);

            var datasetValue = GetDatasetValuesFromImage(imgBase64);

            var input = new ModelInput();
            input.PixelValues = datasetValue.ToArray();
            ModelOutput result = predEngine.Predict(input);

            return Ok(new { prediction = result.Prediction, dataset = string.Join(",", datasetValue) });
        }

        private List<float> GetDatasetValuesFromImage(string base64Image)
        {
            base64Image = base64Image.Replace("data:image/png;base64,", "");
            var imageBytes = Convert.FromBase64String(base64Image).ToArray();

            Image image;

            using (var stream = new MemoryStream(imageBytes))
            {
                image = Image.FromStream(stream);
            }

            var res = new Bitmap(SizeOfImage, SizeOfImage);
            using (var g = Graphics.FromImage(res))
            {
                g.Clear(Color.White);
                g.DrawImage(image, 0, 0, SizeOfImage, SizeOfImage);
            }

            var datasetValue = new List<float>();

            for (int i = 0; i < SizeOfImage; i += SizeOfArea)
            {
                for (int j = 0; j < SizeOfImage; j += SizeOfArea)
                {
                    var sum = 0;
                    for (int k = i; k < i + SizeOfArea; k++)
                    {
                        for (int l = j; l < j + SizeOfArea; l++)
                        {
                            sum += res.GetPixel(l, k).Name == "ffffffff" ? 0 : 1;
                        }
                    }
                    datasetValue.Add(sum);
                }
            }

            return datasetValue;
        }
    }
}

using DirectShowLib;
using Microsoft.ML;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebcamObjectDetection.DataStructures;
using WebcamObjectDetection.YoloParser;
using System.Linq;
using Microsoft.ML.Data;
using System.Drawing.Drawing2D;
using Point = System.Drawing.Point;

namespace WebcamObjectDetection
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string assetsRelativePath = @"../../../assets";
        bool isCameraRunning = false;
        VideoCapture capture;
        MLContext mlContext = new MLContext();
        ITransformer model;
        YoloOutputParser parser;

        Mat frame;
        Bitmap imageAlternate;
        Bitmap image;
        bool isUsingImageAlternate = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            var videoDevices = new List<DsDevice>(DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice));
            foreach (var device in videoDevices)
            {
                deviceDropdown.Items.Add(device.Name);
            }
            if(videoDevices != null && videoDevices.Count > 0)
            {
                deviceDropdown.SelectedIndex = 0;
            }

            var assetsPath = GetAbsolutePath(assetsRelativePath);
            var modelFilePath = Path.Combine(assetsPath, "Model", "TinyYolo2_model.onnx");
            model = LoadModel(modelFilePath);
            parser = new YoloOutputParser();
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            if (!isCameraRunning)
            {
                startBtn.Text = "Stop";

                StartCamera();

                recordingTimer.Enabled = true;
                recordingTimer.Start();
            }
            else
            {
                StopCamera();

                startBtn.Text = "Start";
            }
        }

        private void StartCamera()
        {
            DisposeCameraResources();

            isCameraRunning = true;

            startBtn.Text = "Stop";

            int deviceIndex = deviceDropdown.SelectedIndex;
            capture = new VideoCapture(deviceIndex);
            try
            {
                capture.Open(deviceIndex);
                fpsLabel.Text = capture.Fps.ToString();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void StopCamera()
        {
            isCameraRunning = false;

            startBtn.Text = "Start";

            recordingTimer.Stop();
            recordingTimer.Enabled = false;
            cameraBox.Image = null;

            DisposeCaptureResources();
        }

        private void DisposeCameraResources()
        {
            if (frame != null)
            {
                frame.Dispose();
            }

            if (image != null)
            {
                image.Dispose();
            }

            if (imageAlternate != null)
            {
                imageAlternate.Dispose();
            }
        }

        private void DisposeCaptureResources()
        {
            if (capture != null)
            {
                capture.Release();
                capture.Dispose();
            }
        }

        private void recordingTimer_Tick(object sender, EventArgs e)
        {
            if (capture.IsOpened())
            {
                try
                {
                    frame = new Mat();
                    capture.Read(frame);
                    if (frame != null)
                    {
                        if (imageAlternate == null)
                        {
                            isUsingImageAlternate = true;
                            imageAlternate = BitmapConverter.ToBitmap(frame);
                        }
                        else if (image == null)
                        {
                            isUsingImageAlternate = false;
                            image = BitmapConverter.ToBitmap(frame);
                        }

                        Bitmap imageToProcess = isUsingImageAlternate ? imageAlternate : image;

                        // Load Data
                        List<ImageNetData> images = new List<ImageNetData> {
                            new ImageNetData { ImagePath = imageToProcess }
                        };
                        IDataView imageDataView = mlContext.Data.LoadFromEnumerable(images);

                        IEnumerable<float[]> probabilities = PredictDataUsingModel(imageDataView, model);

                        var boundingBoxes =
                            probabilities
                            .Select(probability => parser.ParseOutputs(probability))
                            .Select(boxes => parser.FilterBoundingBoxes(boxes, 5, .5F));

                        Image imageToShow = null;
                        // Draw bounding boxes for detected objects in each of the images
                        for (var i = 0; i < images.Count(); i++)
                        {
                            IList<YoloBoundingBox> detectedObjects = boundingBoxes.ElementAt(i);

                            imageToShow = DrawBoundingBox(imageToProcess, detectedObjects);
                        }
                        cameraBox.Image = imageToShow ?? imageToProcess;
                    }
                }
                catch (Exception ex)
                {
                    StopCamera();
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    if (frame != null)
                    {
                        frame.Dispose();
                    }

                    if (isUsingImageAlternate && image != null)
                    {
                        image.Dispose();
                        image = null;
                    }
                    else if (!isUsingImageAlternate && imageAlternate != null)
                    {
                        imageAlternate.Dispose();
                        imageAlternate = null;
                    }
                }
            }
            else
            {
                StopCamera();
                MessageBox.Show("Can't open camera, this may be a settings or permissions issue.");
            }
        }

        private ITransformer LoadModel(string modelLocation)
        {
            // Create IDataView from empty list to obtain input data schema
            var data = mlContext.Data.LoadFromEnumerable(new List<ImageNetData>());

            // Define scoring pipeline
            var pipeline = mlContext.Transforms.ResizeImages(outputColumnName: "image", imageWidth: 416, imageHeight: 416, inputColumnName: "image")
                            .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "image"))
                            .Append(mlContext.Transforms.ApplyOnnxModel(modelFile: modelLocation, outputColumnNames: new[] { "grid" }, inputColumnNames: new[] { "image" }));

            // Fit scoring pipeline
            var model = pipeline.Fit(data);

            return model;
        }

        private IEnumerable<float[]> PredictDataUsingModel(IDataView testData, ITransformer model)
        {
            IDataView scoredData = model.Transform(testData);

            IEnumerable<float[]> probabilities = scoredData.GetColumn<float[]>("grid");

            return probabilities;
        }

        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }

        private static Image DrawBoundingBox(Image image, IList<YoloBoundingBox> filteredBoundingBoxes)
        {

            var originalImageHeight = image.Height;
            var originalImageWidth = image.Width;

            foreach (var box in filteredBoundingBoxes)
            {
                // Get Bounding Box Dimensions
                var x = (uint)Math.Max(box.Dimensions.X, 0);
                var y = (uint)Math.Max(box.Dimensions.Y, 0);
                var width = (uint)Math.Min(originalImageWidth - x, box.Dimensions.Width);
                var height = (uint)Math.Min(originalImageHeight - y, box.Dimensions.Height);

                // Resize To Image
                x = (uint)originalImageWidth * x / 416;
                y = (uint)originalImageHeight * y / 416;
                width = (uint)originalImageWidth * width / 416;
                height = (uint)originalImageHeight * height / 416;

                // Bounding Box Text
                string text = $"{box.Label} ({(box.Confidence * 100).ToString("0")}%)";

                using (Graphics thumbnailGraphic = Graphics.FromImage(image))
                {
                    thumbnailGraphic.CompositingQuality = CompositingQuality.HighQuality;
                    thumbnailGraphic.SmoothingMode = SmoothingMode.HighQuality;
                    thumbnailGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    // Define Text Options
                    Font drawFont = new Font("Arial", 12, FontStyle.Bold);
                    SizeF size = thumbnailGraphic.MeasureString(text, drawFont);
                    SolidBrush fontBrush = new SolidBrush(Color.Black);
                    Point atPoint = new Point((int)x, (int)y - (int)size.Height - 1);

                    // Define BoundingBox options
                    Pen pen = new Pen(box.BoxColor, 3.2f);
                    SolidBrush colorBrush = new SolidBrush(box.BoxColor);

                    // Draw text on image 
                    thumbnailGraphic.FillRectangle(colorBrush, (int)x, (int)(y - size.Height - 1), (int)size.Width, (int)size.Height);
                    thumbnailGraphic.DrawString(text, drawFont, fontBrush, atPoint);

                    // Draw bounding box on image
                    thumbnailGraphic.DrawRectangle(pen, x, y, width, height);
                }
            }

            return image;
        }
    }
}

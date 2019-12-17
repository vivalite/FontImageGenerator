﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Media;
using System.Xml.Schema;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Brush = System.Drawing.Brush;
using Color = System.Drawing.Color;
using Size = System.Drawing.Size;


namespace TextImageGenerator
{
    public partial class Main : Form
    {
        const int IMG_WIDTH = 80;
        const int IMG_HIGHT = 80;

        const double VAL_PERCENT = 0.2;
        const double TEST_PERCENT = 0.1;

        private FontCollections _fontCollections;
        private Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;
        private string _windowTitle = "Font Text Generator";

        public Main()
        {
            _fontCollections = new FontCollections();

            InitializeComponent();

            InitControls();

            this.Text = _windowTitle;
        }

        private void InitControls()
        {
            checkedListBoxFonts.Items.Clear();

            foreach (string dir in Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "Fonts"))
            {
                checkedListBoxFonts.Items.Add(new CheckListFontItem { Text = Path.GetFileName(dir), Path = dir });
            }

        }

        private struct CheckListFontItem
        {
            public string Text;
            public string Path;
            public override string ToString()
            {
                return Text;
            }
        }

        private NativeMethods.TEXTMETRICW GetTextMetrics(IDeviceContext dc, Font font)
        {
            NativeMethods.TEXTMETRICW result;
            IntPtr hDC;
            IntPtr hFont;
            IntPtr hFontDefault;

            hDC = IntPtr.Zero;
            hFont = IntPtr.Zero;
            hFontDefault = IntPtr.Zero;

            try
            {
                hDC = dc.GetHdc();

                hFont = font.ToHfont();
                hFontDefault = NativeMethods.SelectObject(hDC, hFont);

                NativeMethods.GetTextMetrics(hDC, out result);
            }
            finally
            {
                if (hFontDefault != IntPtr.Zero)
                {
                    NativeMethods.SelectObject(hDC, hFontDefault);
                }

                if (hFont != IntPtr.Zero)
                {
                    NativeMethods.DeleteObject(hFont);
                }

                dc.ReleaseHdc();
            }

            return result;
        }

        private async void buttonStart_Click(object sender, EventArgs e)
        {
            _dispatcher.Invoke(() =>
            {
                buttonStart.Enabled = false;
            });

            await Task.Run(() => { StartProcessing(); });

            _dispatcher.Invoke(() =>
            {
                buttonStart.Enabled = true;
            });
        }

        private void StartProcessing()
        {
            _fontCollections.Clear();

            DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + @"Output\");
            Thread.Sleep(0);
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Output\"))
            {
                Thread.Sleep(200);
            }

            //Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"Output\");
            //Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"Output\Test\");
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"Output\Test\Test_Folder\");
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Output\Test\Test_Folder\"))
            {
                Thread.Sleep(200);
            }
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"Output\Train\");
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Output\Train\"))
            {
                Thread.Sleep(200);
            }
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"Output\Validate\");
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Output\Validate\"))
            {
                Thread.Sleep(200);
            }


            foreach (CheckListFontItem fontItem in checkedListBoxFonts.CheckedItems)
            {
                foreach (string fontFile in Directory.GetFiles(fontItem.Path))
                {
                    _fontCollections.AddFont(fontFile, fontItem.Text);

                    string outFolderTrain = AppDomain.CurrentDomain.BaseDirectory + @"Output\Train\" + fontItem.Text + @"\";
                    string outFolderValidate = AppDomain.CurrentDomain.BaseDirectory + @"Output\Validate\" + fontItem.Text + @"\";

                    Directory.CreateDirectory(outFolderTrain);
                    if (!Directory.Exists(outFolderTrain))
                    {
                        Thread.Sleep(200);
                    }
                    Directory.CreateDirectory(outFolderValidate);
                    if (!Directory.Exists(outFolderValidate))
                    {
                        Thread.Sleep(200);
                    }
                }
            }

            //int fontGroupCounter = 0;
            string[] imageTextSource = { };

            if (radioButtonGBK.Checked)
            {
                imageTextSource = Properties.Resources.GBK.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
            }
            else if (radioButtonSC3K.Checked)
            {
                imageTextSource = Properties.Resources.SCTOP3K.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
            }
            else if (radioButtonSCTC3K.Checked)
            {
                imageTextSource = (Properties.Resources.SCTOP3K + Properties.Resources.TCTOP3K).Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
            }

            foreach (IGrouping<string, PrivateFontCollection> groupItem in _fontCollections.FontCollection)
            {
                var items = groupItem.ToList();
                string outFolderTrain = AppDomain.CurrentDomain.BaseDirectory + @"Output\Train\" + groupItem.Key;
                string outFolderValidate = AppDomain.CurrentDomain.BaseDirectory + @"Output\Validate\" + groupItem.Key;
                string outFolderTest = AppDomain.CurrentDomain.BaseDirectory + @"Output\Test\Test_Folder";

                var selectWords = 1000;
                int.TryParse(textBoxSelectWords.Text, out selectWords);

                var repeat = 1;
                int.TryParse(textBoxRepeat.Text, out repeat);

                string[] imageTextSource1 = imageTextSource.OrderBy(x => Guid.NewGuid()).Take(selectWords).ToArray();

                _dispatcher.Invoke(() => { this.Text = _windowTitle + " - Currently Working On: " + groupItem.Key; });

                //Debug.Print(checkedListBoxFonts.CheckedItems[fontGroupCounter++].ToString());


                for (int k = 0; k < repeat; k++)
                {
                    int k2 = k;
                    Parallel.For(0, items.Count, i =>
                    {
                        PrivateFontCollection fontCollection = items[i];
                        Font font = new Font(fontCollection.Families[0], 200);

                        Graphics g = CreateGraphics();
                        NativeMethods.TEXTMETRICW textMetrics = GetTextMetrics(g, font);
                        //Debug.Print("   " + tess.tmCharSet.ToString("X"));

                        if (textMetrics.tmCharSet == (byte)NativeMethods.FontCharSet.GB2312_CHARSET)
                        {
                            int k1 = k2;
                            Parallel.For(0, imageTextSource1.Length, j =>
                            {
                                using (Image textImage = DrawText(imageTextSource1[j], font))
                                {
                                    Image finalImage = ResizeImageKeepAspectRatio(textImage, IMG_WIDTH, IMG_HIGHT);

                                    finalImage.Save(outFolderTrain + $@"\{i}_{font.Name}_{j}_{k1}.png", ImageFormat.Png);

                                }
                            });
                        }
                    });
                }

                if (checkBoxSplitData.Checked)
                {
                    int numValTake = (int)(Directory.GetFiles(outFolderTrain).Length * VAL_PERCENT);
                    int numTestTake = (int)(Directory.GetFiles(outFolderTrain).Length * TEST_PERCENT);

                    string[] generatedFiles = Directory.GetFiles(outFolderTrain);

                    if (generatedFiles.Length > 0)
                    {
                        string[] testFiles = generatedFiles.OrderBy(x => Guid.NewGuid()).Take((int)(numValTake)).ToArray();

                        foreach (var tfile in testFiles)
                        {
                            File.Move(tfile, outFolderValidate + @"\" + Path.GetFileName(tfile));
                        }
                    }

                    generatedFiles = Directory.GetFiles(outFolderTrain);

                    if (generatedFiles.Length > 0)
                    {
                        string[] testFiles = generatedFiles.OrderBy(x => Guid.NewGuid()).Take((int)(numTestTake)).ToArray();

                        foreach (var tfile in testFiles)
                        {
                            if (!File.Exists(outFolderTest + @"\" + Path.GetFileName(tfile)))
                            {
                                File.Move(tfile, outFolderTest + @"\" + Path.GetFileName(tfile));
                            }
                        }
                    }

                }

            }

            string[] allTrainFontFolders = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + @"Output\Train\");

            foreach (string trainFontFolder in allTrainFontFolders)
            {
                if (!Directory.GetFiles(trainFontFolder).Any())
                {
                    DeleteDirectory(trainFontFolder);
                }
            }

            string[] allValidateFontFolders = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + @"Output\Validate\");

            foreach (string validateFontFolder in allValidateFontFolders)
            {
                if (!Directory.GetFiles(validateFontFolder).Any())
                {
                    DeleteDirectory(validateFontFolder);
                }
            }

            _dispatcher.Invoke(() => { this.Text = _windowTitle + @" Done!"; });
        }

        public static List<bool> GetHash(Bitmap bmpSource)
        {
            List<bool> lResult = new List<bool>();
            //create new image with 16x16 pixel
            Bitmap bmpMin = new Bitmap(bmpSource, new Size(32, 32));
            for (int j = 0; j < bmpMin.Height; j++)
            {
                for (int i = 0; i < bmpMin.Width; i++)
                {
                    //reduce colors to true / false                
                    lResult.Add(bmpMin.GetPixel(i, j).GetBrightness() < 0.5f);
                }
            }
            return lResult;
        }

        public static Image DrawText(string text, Font fontOptional = null, Color? textColorOptional = null, Color? backColorOptional = null, Size? minSizeOptional = null)
        {
            Font font = Control.DefaultFont;
            if (fontOptional != null)
                font = fontOptional;

            Color textColor = Color.Black;
            if (textColorOptional != null)
                textColor = (Color)textColorOptional;

            Color backColor = Color.White;
            if (backColorOptional != null)
                backColor = (Color)backColorOptional;

            Size minSize = Size.Empty;
            if (minSizeOptional != null)
                minSize = (Size)minSizeOptional;

            //first, create a dummy bitmap just to get a graphics object
            SizeF textSize;
            using (Image img = new Bitmap(1, 1))
            {
                using (Graphics drawing = Graphics.FromImage(img))
                {
                    //measure the string to see how big the image needs to be
                    textSize = drawing.MeasureString(text, font);
                    if (!minSize.IsEmpty)
                    {
                        textSize.Width = textSize.Width > minSize.Width ? textSize.Width : minSize.Width;
                        textSize.Height = textSize.Height > minSize.Height ? textSize.Height : minSize.Height;
                    }
                }
            }

            //create a new image of the right size
            Image retImg = GetRandomBackground((int) textSize.Width, (int) textSize.Height, out int foregroundColorCode);   //new Bitmap((int)textSize.Width, (int)textSize.Height);
            
            textColor = Color.FromArgb(foregroundColorCode,foregroundColorCode,foregroundColorCode);

            using (var drawing = Graphics.FromImage(retImg))
            {
                //paint the background
                //drawing.Clear(backColor);

                //create a brush for the text
                using (Brush textBrush = new SolidBrush(textColor))
                {
                    drawing.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
                    drawing.DrawString(text, font, textBrush, 0, 0);
                    drawing.Save();
                }
            }
            return retImg;
        }

        /// <summary>
        /// Resize an image keeping its aspect ratio (cropping may occur).
        /// </summary>
        /// <param name="source"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Image ResizeImageKeepAspectRatio(Image source, int width, int height)
        {
            Image result = null;

            try
            {
                if (source.Width != width || source.Height != height)
                {
                    // Resize image
                    float sourceRatio = (float)source.Width / source.Height;

                    using (var target = new Bitmap(width, height))
                    {
                        using (var g = System.Drawing.Graphics.FromImage(target))
                        {
                            g.CompositingQuality = CompositingQuality.HighQuality;
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = SmoothingMode.HighQuality;

                            // Scaling
                            float scaling;
                            float scalingY = (float)source.Height / height;
                            float scalingX = (float)source.Width / width;
                            if (scalingX < scalingY) scaling = scalingX; else scaling = scalingY;

                            int newWidth = (int)(source.Width / scaling);
                            int newHeight = (int)(source.Height / scaling);

                            // Correct float to int rounding
                            if (newWidth < width) newWidth = width;
                            if (newHeight < height) newHeight = height;

                            // See if image needs to be cropped
                            int shiftX = 0;
                            int shiftY = 0;

                            if (newWidth > width)
                            {
                                shiftX = (newWidth - width) / 2;
                            }

                            if (newHeight > height)
                            {
                                shiftY = (newHeight - height) / 2;
                            }

                            // Draw image
                            g.DrawImage(source, -shiftX, -shiftY, newWidth, newHeight);
                        }

                        result = (Image)target.Clone();
                    }
                }
                else
                {
                    // Image size matched the given size
                    result = (Image)source.Clone();
                }
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }

        private class FontCollections
        {
            private List<KeyValuePair<string, PrivateFontCollection>> _privateFontCollection = new List<KeyValuePair<string, PrivateFontCollection>>();

            public ILookup<string, PrivateFontCollection> FontCollection => _privateFontCollection.ToLookup((i) => i.Key, (i) => i.Value);

            public void AddFont(string fullFileName, string fontGroupName)
            {
                AddFont(File.ReadAllBytes(fullFileName), fontGroupName);
            }

            public void AddFont(byte[] fontBytes, string fontGroupName)
            {
                var handle = GCHandle.Alloc(fontBytes, GCHandleType.Pinned);
                IntPtr pointer = handle.AddrOfPinnedObject();
                try
                {
                    PrivateFontCollection pfc = new PrivateFontCollection();
                    pfc.AddMemoryFont(pointer, fontBytes.Length);

                    _privateFontCollection.Add(new KeyValuePair<string, PrivateFontCollection>(fontGroupName, pfc));
                }
                finally
                {
                    handle.Free();
                }
            }

            public void Clear()
            {
                _privateFontCollection.Clear();
            }
        }

        private void buttonSelectAll_Click(object sender, EventArgs e)
        {
            var fontCount = checkedListBoxFonts.Items.Count;

            for (int i = 0; i < fontCount; i++)
            {
                checkedListBoxFonts.SetItemChecked(i, true);
            }
        }

        private void buttonLoadFont_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string fontFolderName = Path.GetFileName(fbd.SelectedPath);
                    string[] filePaths = Directory.GetFiles(fbd.SelectedPath).Where(s => s.ToLower().EndsWith(".ttf") || s.ToLower().EndsWith(".ttc")).ToArray();

                    foreach (var filePath in filePaths)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(filePath);
                        var fontFolder = AppDomain.CurrentDomain.BaseDirectory + @"Fonts\" + fontFolderName + "_" + fileName + @"\";
                        Directory.CreateDirectory(fontFolder);
                        File.Copy(filePath, fontFolder + fontFolderName + "_" + Path.GetFileName(filePath));
                    }

                }

                InitControls();
            }
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            



        }


        private static Bitmap GetRandomBackground(int width,int height, out int foregroundColorCode)
        {
            string backgroundImagesFolder = @"D:\Labelme_train_set\Images\";

            Random rnd = new Random(DateTime.Now.Millisecond);
            List<string> imageFilePaths = Directory.EnumerateFiles(backgroundImagesFolder, "*.*", SearchOption.AllDirectories).ToList();

            string randomImagePath = imageFilePaths.OrderBy(x => Guid.NewGuid()).Take(1).FirstOrDefault();

            Mat randomImageOriginal = new Mat(randomImagePath); // image in BGR

            Rect cropRect = new Rect(rnd.Next(0, randomImageOriginal.Width - width), rnd.Next(0, randomImageOriginal.Height - height), width, height);

            Mat randomImage = new Mat(randomImageOriginal, cropRect);
            Mat imgXyz = randomImage.CvtColor(ColorConversionCodes.BGR2XYZ);
            Mat imgL = imgXyz.Split()[1];

            double resultL2Rgb = imgL.Mean().Val0; // L2 = background mean value

            double resultL2 = resultL2Rgb / 255;
            resultL2 = resultL2 <= 0.03928 ? resultL2 / 12.92 : Math.Pow((resultL2 + 0.055) / 1.055, 2.4);

            double ratio;
            double resultL1;
            do
            {
                double c = rnd.Next(0, 255) / 255D;
                c = c <= 0.03928 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4);
                resultL1 = 0.2126 * c + 0.7152 * c + 0.0722 * c;
                ratio = (Math.Max(resultL1, resultL2) + 0.05) / (Math.Min(resultL1, resultL2) + 0.05);

            } while (ratio <= 4.5); // cannot be large than 4.5 or will freeze

            double rgbColorCodeL1 = resultL1 <= 0.03928 ? resultL1 * 12.92 : Math.Pow(resultL1, 5 / 12D) * 1.055 - 0.055;
            rgbColorCodeL1 *= 255; // foreground (text) color code

            foregroundColorCode = (int) rgbColorCodeL1;

            //Debug.Print(resultL1 + " " + resultL2 + " R:"+ ratio + " L1 CODE:" + rgbColorCodeL1 + " L2 CODE:" + resultL2Rgb);

            return randomImage.ToBitmap();
        }


        private BitmapImage Convert(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        private void DeleteDirectory(string path)
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                DeleteDirectory(directory);
            }

            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, true);
            }
        }
    }
}

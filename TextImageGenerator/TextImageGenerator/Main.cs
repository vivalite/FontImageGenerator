using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

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

            foreach (CheckListFontItem fontItem in checkedListBoxFonts.CheckedItems)
            {
                foreach (string fontFile in Directory.GetFiles(fontItem.Path))
                {
                    _fontCollections.AddFont(fontFile, fontItem.Text);

                    string outFolderTrain = AppDomain.CurrentDomain.BaseDirectory + @"Output\Train\" + fontItem.Text;
                    string outFolderValidate = AppDomain.CurrentDomain.BaseDirectory + @"Output\Validate\" + fontItem.Text;
                    string outFolderTest = AppDomain.CurrentDomain.BaseDirectory + @"Output\Test\Test_Folder";

                    if (Directory.Exists(outFolderTrain))
                    {
                        Directory.Delete(outFolderTrain, true);
                    }

                    if (Directory.Exists(outFolderValidate))
                    {
                        Directory.Delete(outFolderValidate, true);
                    }

                    if (Directory.Exists(outFolderTest))
                    {
                        Directory.Delete(outFolderTest, true);
                    }

                    while (Directory.Exists(outFolderTrain) || Directory.Exists(outFolderValidate) || Directory.Exists(outFolderTest))
                    {
                        Thread.Sleep(200);
                    }

                    Directory.CreateDirectory(outFolderTrain);
                    Directory.CreateDirectory(outFolderValidate);
                    Directory.CreateDirectory(outFolderTest);
                }
            }

            foreach (IGrouping<string, PrivateFontCollection> groupItem in _fontCollections.FontCollection)
            {
                var items = groupItem.ToList();
                string outFolderTrain = AppDomain.CurrentDomain.BaseDirectory + @"Output\Train\" + groupItem.Key;
                string outFolderValidate = AppDomain.CurrentDomain.BaseDirectory + @"Output\Validate\" + groupItem.Key;
                string outFolderTest = AppDomain.CurrentDomain.BaseDirectory + @"Output\Test\Test_Folder";

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

                var selectWords = 1000;
                int.TryParse(textBoxSelectWords.Text, out selectWords);

                imageTextSource = imageTextSource.OrderBy(x => Guid.NewGuid()).Take(selectWords).ToArray();

                _dispatcher.Invoke(() => { this.Text = _windowTitle + " - Currently Working On: " + groupItem.Key; });

                Parallel.For(0, items.Count, (i) =>
                {
                    PrivateFontCollection fontCollection = items[i];
                    Font font = new Font(fontCollection.Families[0], 200);

                    Parallel.For(0, imageTextSource.Length, (j) =>
                    {
                        using (Image textImage = DrawText(imageTextSource[j], font))
                        {
                            Image finalImage = ResizeImageKeepAspectRatio(textImage, IMG_WIDTH, IMG_HIGHT);

                            finalImage.Save(outFolderTrain + $@"\{i}_{font.Name}_{j}_.png", ImageFormat.Png);
                        }
                    });
                });

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
                            File.Move(tfile, outFolderTest + @"\" + Path.GetFileName(tfile));
                        }
                    }

                }

            }
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
            Image retImg = new Bitmap((int)textSize.Width, (int)textSize.Height);
            using (var drawing = Graphics.FromImage(retImg))
            {
                //paint the background
                drawing.Clear(backColor);

                //create a brush for the text
                using (Brush textBrush = new SolidBrush(textColor))
                {
                    drawing.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
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
                    string[] filePaths = Directory.GetFiles(fbd.SelectedPath).Where(s => s.ToLower().EndsWith(".ttf") || s.ToLower().EndsWith(".ttc")).ToArray();

                    foreach (var filePath in filePaths)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(filePath);
                        var fontFolder = AppDomain.CurrentDomain.BaseDirectory + @"Fonts\" + fileName + @"\";
                        Directory.CreateDirectory(fontFolder);
                        File.Copy(filePath, fontFolder + Path.GetFileName(filePath));

                    }

                }

                InitControls();
            }
        }
    }
}

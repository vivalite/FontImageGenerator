using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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

namespace TextImageGenerator
{
	public partial class Main : Form
	{
		private FontCollections _fontCollections;

		public Main()
		{
			_fontCollections = new FontCollections();

			InitializeComponent();

			InitControls();
		}

		private void InitControls()
		{
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

		private void buttonStart_Click(object sender, EventArgs e)
		{
			StartProcessing();
		}

		private void StartProcessing()
		{
			_fontCollections.Clear();

			foreach (CheckListFontItem fontItem in checkedListBoxFonts.CheckedItems)
			{
				foreach (string fontFile in Directory.GetFiles(fontItem.Path))
				{
					_fontCollections.AddFont(fontFile, fontItem.Text);

					string outFolder = AppDomain.CurrentDomain.BaseDirectory + @"Output\" + fontItem.Text;

					if (Directory.Exists(outFolder))
					{
						Directory.Delete(outFolder, true);
					}

					while (Directory.Exists(outFolder))
					{
						Thread.Sleep(200);
					}

					Directory.CreateDirectory(outFolder);
				}
			}

			foreach (IGrouping<string, PrivateFontCollection> groupItem in _fontCollections.FontCollection)
			{
				var items = groupItem.ToList();
				string outFolder = AppDomain.CurrentDomain.BaseDirectory + @"Output\" + groupItem.Key;

				string[] imageTextSource = Properties.Resources.SCTOP3K.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

				for (var i = 0; i < items.Count; i++)
				{
					PrivateFontCollection fontCollection = items[i];
					Font font = new Font(fontCollection.Families[0], 200);

					for (int j = 0; j < imageTextSource.Length; j++)
					{
						using (Image textImage = DrawText(imageTextSource[j], font))
						{
							using (MemoryStream ms = new MemoryStream())
							{
								textImage.Save(ms, ImageFormat.Bmp);

                                PngBitmapEncoder encoder = new PngBitmapEncoder();

								encoder.Frames.Add(BitmapFrame.Create(ms));

								using (FileStream stream = new FileStream(outFolder + $@"\{i}_{font.Name}_{j}_.png", FileMode.Create))
								{
									encoder.Save(stream);
								}
							}
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
	}
}

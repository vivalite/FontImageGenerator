using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

// Retrieving font and text metrics using C#
// http://www.cyotek.com/blog/retrieving-font-and-text-metrics-using-csharp
// Copyright © 2016 Cyotek Ltd. All Rights Reserved.


// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable InconsistentNaming

namespace TextImageGenerator
{
  internal static class NativeMethods
  {
    #region Externals

    [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool DeleteObject(IntPtr hObject);

    [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
    public static extern bool GetTextMetrics(IntPtr hdc, out TEXTMETRICW lptm);

    [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiObj);

    [DllImport("gdi32.dll")]
    public static extern uint GetFontUnicodeRanges(IntPtr hdc, IntPtr lpgs);

    #endregion

    public static List<FontRange> GetUnicodeRangesForFont(Font font)
    {
        Graphics g = Graphics.FromHwnd(IntPtr.Zero);
        IntPtr hdc = g.GetHdc();
        IntPtr hFont = font.ToHfont();
        IntPtr old = SelectObject(hdc, hFont);
        uint size = GetFontUnicodeRanges(hdc, IntPtr.Zero);
        IntPtr glyphSet = Marshal.AllocHGlobal((int)size);
        GetFontUnicodeRanges(hdc, glyphSet);
        List<FontRange> fontRanges = new List<FontRange>();
        int count = Marshal.ReadInt32(glyphSet, 12);
        for (int i = 0; i < count; i++)
        {
            FontRange range = new FontRange();
            range.Low = (UInt16)Marshal.ReadInt16(glyphSet, 16 + i * 4);
            range.High = (UInt16)(range.Low + Marshal.ReadInt16(glyphSet, 18 + i * 4) - 1);
            fontRanges.Add(range);
        }
        SelectObject(hdc, old);
        Marshal.FreeHGlobal(glyphSet);
        g.ReleaseHdc(hdc);
        g.Dispose();
        return fontRanges;
    }

    public static bool CheckIfCharInFont(char character, Font font)
    {
        UInt16 intval = Convert.ToUInt16(character);
        List<FontRange> ranges = GetUnicodeRangesForFont(font);
        bool isCharacterPresent = false;
        foreach (FontRange range in ranges)
        {
            if (intval >= range.Low && intval <= range.High)
            {
                isCharacterPresent = true;
                break;
            }
        }
        return isCharacterPresent;
    }

    #region Nested type: TEXTMETRICW

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct TEXTMETRICW
    {
      public int tmHeight;

      public int tmAscent;

      public int tmDescent;

      public int tmInternalLeading;

      public int tmExternalLeading;

      public int tmAveCharWidth;

      public int tmMaxCharWidth;

      public int tmWeight;

      public int tmOverhang;

      public int tmDigitizedAspectX;

      public int tmDigitizedAspectY;

      public ushort tmFirstChar;

      public ushort tmLastChar;

      public ushort tmDefaultChar;

      public ushort tmBreakChar;

      public byte tmItalic;

      public byte tmUnderlined;

      public byte tmStruckOut;

      public byte tmPitchAndFamily;

      public byte tmCharSet;
    }

    public struct FontRange
    {
        public UInt16 Low;
        public UInt16 High;
    }

    public enum FontCharSet
    {
        ANSI_CHARSET = 0x00000000,
        DEFAULT_CHARSET = 0x00000001,
        SYMBOL_CHARSET = 0x00000002,
        MAC_CHARSET = 0x0000004D,
        SHIFTJIS_CHARSET = 0x00000080,
        HANGUL_CHARSET = 0x00000081,
        JOHAB_CHARSET = 0x00000082,
        GB2312_CHARSET = 0x00000086,
        CHINESEBIG5_CHARSET = 0x00000088,
        GREEK_CHARSET = 0x000000A1,
        TURKISH_CHARSET = 0x000000A2,
        VIETNAMESE_CHARSET = 0x000000A3,
        HEBREW_CHARSET = 0x000000B1,
        ARABIC_CHARSET = 0x000000B2,
        BALTIC_CHARSET = 0x000000BA,
        RUSSIAN_CHARSET = 0x000000CC,
        THAI_CHARSET = 0x000000DE,
        EASTEUROPE_CHARSET = 0x000000EE,
        OEM_CHARSET = 0x000000FF
    }

    #endregion
  }
}

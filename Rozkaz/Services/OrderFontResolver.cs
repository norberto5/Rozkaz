using PdfSharp.Drawing;
using PdfSharp.Fonts;
using System.Collections.Generic;
using System.IO;

namespace Rozkaz.Services
{
    public class OrderFontResolver : IFontResolver
    {
        private const string fontsPath = "wwwroot/fonts/";

        private readonly Dictionary<string, FontInfo> fonts = new Dictionary<string, FontInfo>();

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            string faceName = familyName.ToLower() + (isBold ? "|b" : "") + (isItalic ? "|i" : "");
            if (fonts.TryGetValue(faceName, out FontInfo item))
            {
                var result = new FontResolverInfo(item.Name, item.SimulateBold, item.SimulateItalic);
                return result;
            }
            return null;
        }

        public byte[] GetFont(string faceName) => fonts.TryGetValue(faceName, out FontInfo item) ? item.Data : null;

        public void AddFont(string familyName, XFontStyle style, string filename, bool simulateBold = false, bool simulateItalic = false)
        {
            using (var fileStream = new FileStream($"{fontsPath}{filename}", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                int length = (int)fileStream.Length;
                byte[] data = new byte[length];
                int read = fileStream.Read(data, 0, length);

                AddFont(familyName, style, data, simulateBold, simulateItalic);
            }
        }

        public void AddFont(string familyName, XFontStyle style, byte[] data, bool simulateBold = false, bool simulateItalic = false)
        {
            AddFontToDictionary(familyName, style, data, false, false);

            if (simulateBold && (style & XFontStyle.Bold) == 0)
            {
                AddFontToDictionary(familyName, style | XFontStyle.Bold, data, true, false);
            }

            if (simulateItalic && (style & XFontStyle.Italic) == 0)
            {
                AddFontToDictionary(familyName, style | XFontStyle.Italic, data, false, true);
            }

            if (simulateBold && (style & XFontStyle.Bold) == 0 &&
                simulateItalic && (style & XFontStyle.Italic) == 0)
            {
                AddFontToDictionary(familyName, style | XFontStyle.BoldItalic, data, true, true);
            }
        }

        private void AddFontToDictionary(string familyName, XFontStyle style, byte[] data, bool simulateBold, bool simulateItalic)
        {
            var fontInfo = new FontInfo
            {
                Name = familyName.ToLower(),
                Data = data,
                SimulateBold = simulateBold,
                SimulateItalic = simulateItalic
            };

            if ((style & XFontStyle.Bold) != 0)
            {
                fontInfo.Name += "|b";
            }
            if ((style & XFontStyle.Italic) != 0)
            {
                fontInfo.Name += "|i";
            }

            if (GetFont(fontInfo.Name) != null)
                return;

            fonts.Add(fontInfo.Name, fontInfo);
        }

        private struct FontInfo
        {
            public string Name;
            public byte[] Data;
            public bool SimulateBold;
            public bool SimulateItalic;
        }
    }
}
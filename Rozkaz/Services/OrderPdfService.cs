﻿using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace home_site.Services
{
    public class OrderPdfService
    {
        private const string orderFilename = "tmp.pdf";
        private static readonly double pageLeftRightMargin = 80;
        private static readonly double pageTopBottomMargin = 40;

        private double realPageWidth => page.Width - pageLeftRightMargin * 2;

        private double actualHeight;

        private PdfPage page;
        private XGraphics gfx;
        private XTextFormatter textFormatter;

        private readonly XFont normalFont;
        private readonly XFont bold11Font;
        private readonly XFont boldFont;
        private readonly XFont italicFont;
        private readonly XFont titleFont;

        private readonly XFont unitNameFont;
        private readonly XFont unitSecondaryFont;
        private readonly XFont unitSecondaryBoldFont;


        public OrderPdfService()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            normalFont = new XFont("Museo 300", 10.5f, XFontStyle.Regular);
            bold11Font = new XFont("Museo 300", 11f, XFontStyle.Bold);
            boldFont = new XFont("Museo 300", 10.5f, XFontStyle.Bold);
            italicFont = new XFont("Museo 300", 10.5f, XFontStyle.Italic);
            titleFont = new XFont("Museo 300", 16, XFontStyle.Bold);
            unitNameFont = new XFont("Museo 300", 12, XFontStyle.Regular);
            unitSecondaryFont = new XFont("Museo 300", 8, XFontStyle.Regular);
            unitSecondaryBoldFont = new XFont("Museo 300", 8, XFontStyle.Bold);
        }

        private void Reset()
        {
            actualHeight = pageTopBottomMargin;
            page = null;
            gfx = null;
            textFormatter = null;
        }

        public string CreateSampleOrder()
        {
            Reset();

            var document = new PdfDocument();
            document.Info.Title = "Rozkaz L. 1/2019";
            document.Info.Subject = "Rozkaz L. 1/2019";
            document.Info.Author = "Norbert Piątkowski";
            document.Info.Creator = "Rozkaz! © 2019 norberto5.pl Norbert Piątkowski";

            page = document.AddPage();
            gfx = XGraphics.FromPdfPage(page);
            textFormatter = new XTextFormatter(gfx);

            DrawHeader();

            DrawSingleLineString("Szczecin, 14 kwietnia 2019 r.", normalFont, XStringFormats.TopRight);
            DrawText("Naczelny programista\nNorbert Piątkowski");
            DrawTitle("Rozkaz L. 1/2019");

            DrawSpace(2);
            DrawItalic("Wstęp okolicznościowy (święta państwowe, rocznice, szczególne wydarzenia w Związku)");
            DrawSpace(2);
            DrawItalic("Wyjątki z rozkazu komendanta Hufca Szczecin ZHP L. 0 / 2018 z dnia..... 2018 r.");
            DrawSpace(2);

            DrawBold11("1. Zarządzenia i informacje");
            DrawBold("1.1. Zarządzenia");
            DrawText("Przykład:");
            DrawText("1.1.1. Zwołuję Zlot Drużyny ..........");
            DrawBold("1.2. Informacje");
            DrawText("Przykład:");
            DrawText("1.2.1. Podaję do wiadomości, ze przy drużynie zawiązało się Koło Przyjaciół Harcerstwa. Na przewodniczącego Koła został wybrany pan ..........");
            DrawText("1.2.2. Informuję o decyzji Rady Drużyny z dnia …. w sprawie ….. Treść decyzji stanowi załącznik nr 1 do niniejszego rozkazu. ");

            DrawSpace(1);
            DrawBold11("2. Drużyna");
            DrawBold("2.1. Mianowania funkcyjnych");
            DrawText("Przykład:");
            DrawText("2.1.1. Na wniosek Rady Drużyny mianuję sam. Janinę Barys przyboczną z dniem ………");
            DrawText("2.1.2. Na wniosek Rady Drużyny mianuję wyw. Tomasza Łęckiego kronikarzem drużyny z dniem ………");

            document.Save(orderFilename);
            return orderFilename;
        }

        private void DrawHeader()
        {
            var logo = XImage.FromFile("wwwroot/images/identyfikatorZHP-zielony.png");
            gfx.DrawImage(logo, new XRect(pageLeftRightMargin + 20, actualHeight, logo.PixelWidth / 6, logo.PixelHeight / 6));

            gfx.BeginContainer();

            gfx.DrawRoundedRectangle(new XSolidBrush(XColor.FromArgb(0xFF85A314)), new XRect(370, pageTopBottomMargin, 200, 50), new XSize(10, 10));
            DrawSingleLineString("22 Drużyna Harcerska", unitNameFont, XStringFormats.Center, new XRect(370, pageTopBottomMargin - unitNameFont.Height / 2, 200, 50), XBrushes.White);
            DrawSingleLineString("\"Błękitna\"", unitNameFont, XStringFormats.Center, new XRect(370, pageTopBottomMargin + unitNameFont.Height / 2, 200, 50), XBrushes.White);


            actualHeight += logo.PixelHeight /6 + 20;
            logo.Dispose();
        }

        private void DrawSpace(int count) => DrawString(new string('\n', count-1), normalFont);

        private void DrawText(string text) => DrawString(text, normalFont);

        private void DrawBold(string text) => DrawString(text, boldFont);

        private void DrawBold11(string text) => DrawString(text, bold11Font);

        private void DrawItalic(string text) => DrawString(text, italicFont);

        private void DrawTitle(string text) => DrawSingleLineString(text, titleFont, XStringFormats.TopCenter);

        private void DrawSingleLineString(string text, XFont font, XStringFormat stringFormat, XRect rect = new XRect(), XBrush fontColor = null)
        {
            if(fontColor == null)
            {
                fontColor = XBrushes.Black;
            }

            if (rect.Width == 0)
            {
                rect = new XRect(pageLeftRightMargin, actualHeight, realPageWidth, font.Height);
            }

            gfx.DrawString(text, font, fontColor, rect, stringFormat);
            actualHeight += font.Height;
        }

        private void DrawString(string text, XFont font, XRect rect = new XRect())
        {
            if(rect.Width == 0)
            {
                rect = new XRect(pageLeftRightMargin, actualHeight, realPageWidth, font.Height);
            }

            string[] lines = text.Split('\n');

            foreach(string line in lines)
            {
                if(gfx.MeasureString(line, font).Width > realPageWidth)
                {
                    foreach(string splittedLine in SplitLine(line, font))
                    {
                        rect.Y = actualHeight;
                        textFormatter.DrawString(splittedLine, font, XBrushes.Black, rect);
                        actualHeight += font.Height;
                    }
                }
                else
                {
                    rect.Y = actualHeight;
                    textFormatter.DrawString(line, font, XBrushes.Black, rect);
                    actualHeight += font.Height;
                }
            }
        }

        private string[] SplitLine(string line, XFont font)
        {
            string[] words = line.Split(' ');
            var parts = new List<string>();
            string part = string.Empty;
            foreach (string word in words)
            {
                if (gfx.MeasureString(part + " " + word, font).Width < realPageWidth)
                {
                    part += string.IsNullOrEmpty(part) ? word : " " + word;
                }
                else
                {
                    parts.Add(part);
                    part = word;
                }
            }
            parts.Add(part);

            return parts.ToArray();
        }
    }
}

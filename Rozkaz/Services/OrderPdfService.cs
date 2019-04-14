using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using Rozkaz.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Text;

namespace Rozkaz.Services
{
    public class OrderPdfService
    {
        private const string orderFilename = "tmp.pdf";
        private static readonly double pageLeftRightMargin = 55;
        private static readonly double pageTopBottomMargin = 37;

        private double RealPageWidth => page.Width - pageLeftRightMargin * 2;

        private OrderModel model;
        private OrderInfoModel Info => model?.Info;
        private double actualHeight;

        private PdfPage page;
        private XGraphics gfx;
        private XTextFormatter textFormatter;

        private readonly XFont normalFont;
        private readonly XFont boldBiggerFont;
        private readonly XFont boldFont;
        private readonly XFont quoteFont;
        private readonly XFont titleFont;

        private readonly XFont unitNameFont;
        private readonly XFont unitSecondaryFont;

        private static XImage identifier = XImage.FromFile("wwwroot/images/identyfikatorZHP-zielony.png");
        private static XImage logo = XImage.FromFile("wwwroot/images/logo_zhp_zielone.png");
        private static XImage wosm_wagggs = XImage.FromFile("wwwroot/images/wosm_wagggs.png");


        public OrderPdfService()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var pfc = new PrivateFontCollection();
            pfc.AddFontFile("wwwroot/fonts/Museo 100.otf");
            pfc.AddFontFile("wwwroot/fonts/Museo 300.otf");

            normalFont = new XFont(new Font(pfc.Families[1], 11, FontStyle.Regular, GraphicsUnit.World));
            quoteFont = new XFont(new Font(pfc.Families[0], 11, FontStyle.Regular, GraphicsUnit.World));
            boldFont = new XFont(new Font(pfc.Families[1], 11, FontStyle.Bold, GraphicsUnit.World));
            boldBiggerFont = new XFont(new Font(pfc.Families[1], 12, FontStyle.Bold, GraphicsUnit.World));
            titleFont = new XFont(new Font(pfc.Families[1], 16, FontStyle.Bold, GraphicsUnit.World));
            unitNameFont = new XFont(new Font(pfc.Families[1], 12, FontStyle.Regular, GraphicsUnit.World));
            unitSecondaryFont = new XFont(new Font(pfc.Families[1], 7, FontStyle.Regular, GraphicsUnit.World));
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
            var model = new OrderModel()
            {
                Info = new OrderInfoModel()
                {
                    Author = "Norbert Piątkowski",
                    City = "Szczecin",
                    Date = DateTime.Now,
                    OrderNumber = 1,
                    Unit = new UnitModel()
                    {
                        NameFirstLine = "22. Drużyna Harcerska",
                        NameSecondLine = "\"Błękitna\"",
                        SubtextLines = new List<string>()
                        {
                            "Chorągiew Zachodniopomorska ZHP",
                            "Hufiec Szczecin",
                            "Naczelny programista",
                        }
                    }
                },
                OccassionalIntro = "Wstęp okolicznościowy (święta państwowe, rocznice, szczególne wydarzenia w Związku)",
                ExceptionsFromAnotherOrder = "Wyjątki z rozkazu komendanta Hufca Szczecin ZHP L. 22 / 2019 z dnia..... 2019 r.",
                Categories = new List<OrderCategory>()
                {
                    new OrderCategory("Zarządzenia i informacje", new List<OrderSubcategory>()
                    {
                        new OrderSubcategory("Zarządzenia", new List<SubcategoryElement>()
                        {
                            new SubcategoryElement("Zwołuję Zlot Drużyny .......")
                        }),
                        new OrderSubcategory("Informacje", new List<SubcategoryElement>()
                        {
                            new SubcategoryElement("Podaję do wiadomości, ze przy drużynie zawiązało się Koło Przyjaciół Harcerstwa. Na przewodniczącego Koła został wybrany pan .........."),
                            new SubcategoryElement("Informuję o decyzji Rady Drużyny z dnia …. w sprawie ….. Treść decyzji stanowi załącznik nr 1 do niniejszego rozkazu.")
                        })
                    }),
                    new OrderCategory("Drużyna", new List<OrderSubcategory>()
                    {
                        new OrderSubcategory("Mianowania funkcyjnych", new List<SubcategoryElement>()
                        {
                            new SubcategoryElement("Na wniosek Rady Drużyny mianuję sam. Janinę Barys przyboczną z dniem ......."),
                            new SubcategoryElement("Na wniosek Rady Drużyny mianuję wyw. Tomasza Łęckiego kronikarzem drużyny z dniem .......")
                        })
                    })
                }
            };

            PdfDocument document = Init(model);

            Header();

            //DrawText("L.dz. 15/2019");

            DrawTitle($"Rozkaz L. {Info.OrderNumber}/{Info.Date.Year}");

            DrawSpace(2);
            if(!string.IsNullOrEmpty(model.OccassionalIntro))
            {
                DrawQuote(model.OccassionalIntro);
                DrawSpace(2);
            }
            if(!string.IsNullOrEmpty(model.ExceptionsFromAnotherOrder))
            {
                DrawQuote(model.ExceptionsFromAnotherOrder);
                DrawSpace(2);
            }

            uint categoryNumber = 0;

            foreach(OrderCategory category in model.Categories)
            {
                categoryNumber++;
                DrawBiggerBold($"{categoryNumber}. {category.Name}");

                uint subcategoryNumber = 0;
                foreach(OrderSubcategory subcategory in category.Subcategories)
                {
                    subcategoryNumber++;
                    DrawBold($"{categoryNumber}.{subcategoryNumber}. {subcategory.Name}");

                    uint elementNumber = 0;
                    foreach(SubcategoryElement element in subcategory.Elements)
                    {
                        elementNumber++;
                        DrawText($"{categoryNumber}.{subcategoryNumber}.{elementNumber}. {element.Description}");
                    }
                }
                DrawSpace(1);
            }

            Sign();
            Footer();

            document.Save(orderFilename);
            return orderFilename;
        }

        private PdfDocument Init(OrderModel model)
        {
            Reset();

            this.model = model;

            var document = new PdfDocument();
            document.Info.Title = $"Rozkaz L. {Info.OrderNumber}/{Info.Date.Year}";
            document.Info.Subject = $"Rozkaz L. {Info.OrderNumber}/{Info.Date.Year}";
            document.Info.Author = Info.Author;
            document.Info.Creator = "Rozkaz! © 2019 norberto5.pl Norbert Piątkowski";

            page = document.AddPage();
            gfx = XGraphics.FromPdfPage(page);
            textFormatter = new XTextFormatter(gfx);

            return document;
        }

        private void Header()
        {
            gfx.DrawImage(identifier, new XRect(pageLeftRightMargin, actualHeight, identifier.PixelWidth / 6f, identifier.PixelHeight / 6f));

            double unitMargin = 2;
            double x = 360;
            double y = pageTopBottomMargin;
            double width = 180;
            double height = unitNameFont.Height * 2 + unitMargin * 2;

            var unitRectangleSize = new XRect(x, y, width, height);
            gfx.DrawRoundedRectangle(new XSolidBrush(XColor.FromArgb(0xFF85A314)), unitRectangleSize, new XSize(10, 10));

            x += unitMargin; y += unitMargin;
            DrawSingleLineString(Info.Unit?.NameFirstLine ?? string.Empty, unitNameFont, XStringFormats.TopLeft, new XRect(x, y, width, unitNameFont.Height), XBrushes.White);
            DrawSingleLineString(Info.Unit?.NameSecondLine ?? string.Empty, unitNameFont, XStringFormats.TopLeft, new XRect(x, y + unitNameFont.Height, width, unitNameFont.Height), XBrushes.White);


            y = pageTopBottomMargin + unitRectangleSize.Height + unitMargin;

            foreach(string subline in Info.Unit?.SubtextLines)
            {
                DrawSingleLineString(subline, unitSecondaryFont, XStringFormats.TopLeft, new XRect(x, y, width, unitSecondaryFont.Height));
                y += unitSecondaryFont.Height;
            }

            actualHeight = pageTopBottomMargin + identifier.PixelHeight /6 + 40;
            DrawSingleLineString($"{Info.City ?? string.Empty}, {Info.Date.ToString("dd.MM.yyyy")} r.", normalFont, XStringFormats.TopRight);
        }

        private void Sign()
        {
            double x = 420;
            double y = actualHeight + 30;
            double width = 100;
            double height = normalFont.Height;

            DrawSingleLineString("CZUWAJ!", normalFont, XStringFormats.Center, new XRect(x, y, width, height));
            DrawSingleLineString(Info.Author ?? string.Empty, normalFont, XStringFormats.Center, new XRect(x, y + normalFont.Height + 5, width, height));
        }

        private void Footer()
        {
            double width = 70;
            double height = (float)  logo.PixelHeight / logo.PixelWidth * width;
            double x = pageLeftRightMargin;
            double y = page.Height - height - 5;

            gfx.DrawImage(logo, new XRect(x, y, width, height));


            width = wosm_wagggs.PixelWidth / 4f;
            height = wosm_wagggs.PixelHeight / 4f;
            x = page.Width - pageLeftRightMargin - width;
            y = page.Height - height - 5;

            gfx.DrawImage(wosm_wagggs, new XRect(x, y, width, height));
        }

        private void DrawSpace(int count) => DrawString(new string('\n', count-1), normalFont);

        private void DrawText(string text) => DrawString(text, normalFont);

        private void DrawBold(string text) => DrawString(text, boldFont);

        private void DrawBiggerBold(string text) => DrawString(text, boldBiggerFont);

        private void DrawQuote(string text) => DrawString(text, quoteFont);

        private void DrawTitle(string text) => DrawSingleLineString(text, titleFont, XStringFormats.TopCenter);

        private void DrawSingleLineString(string text, XFont font, XStringFormat stringFormat, XRect rect = new XRect(), XBrush fontColor = null)
        {
            if(fontColor == null)
            {
                fontColor = XBrushes.Black;
            }

            if (rect.Width == 0)
            {
                rect = new XRect(pageLeftRightMargin, actualHeight, RealPageWidth, font.Height);
            }

            gfx.DrawString(text, font, fontColor, rect, stringFormat);
            actualHeight += font.Height;
        }

        private void DrawString(string text, XFont font, XRect rect = new XRect())
        {
            if(rect.Width == 0)
            {
                rect = new XRect(pageLeftRightMargin, actualHeight, RealPageWidth, font.Height);
            }

            string[] lines = text.Split('\n');

            foreach(string line in lines)
            {
                if(gfx.MeasureString(line, font).Width > RealPageWidth)
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
                if (gfx.MeasureString(part + " " + word, font).Width < RealPageWidth)
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

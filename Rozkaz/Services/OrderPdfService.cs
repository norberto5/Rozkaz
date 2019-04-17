using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using Rozkaz.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rozkaz.Services
{
    public class OrderPdfService
    {
        private static readonly string orderFilename = $"{Path.GetTempPath()}/tmp.pdf";
        private const string museo100 = "Museo 100";
        private const string museo300 = "Museo 300";

        private const double pageLeftRightMargin = 55;
        private const double pageTopMargin = 37;
        private const double pageBottomMargin = 110;

        private double RealPageWidth => page.Width - pageLeftRightMargin * 2;

        private string OrderString => $"Rozkaz L. {Info?.OrderNumber}/{Info?.Date.Year}";

        private OrderModel model;
        private OrderInfoModel Info => model?.Info;
        private double actualHeight;

        private PdfDocument document;
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

        private static readonly XImage identifier = XImage.FromFile("wwwroot/images/identyfikatorZHP-zielony.png");
        private static readonly XImage logo = XImage.FromFile("wwwroot/images/logo_zhp_zielone.png");
        private static readonly XImage wosm_wagggs = XImage.FromFile("wwwroot/images/wosm_wagggs.png");

        public OrderPdfService()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var fontResolver = new OrderFontResolver();
            GlobalFontSettings.FontResolver = fontResolver;

            fontResolver.AddFont(museo100, XFontStyle.Regular, $"{museo100}.otf", true, true);
            fontResolver.AddFont(museo300, XFontStyle.Regular, $"{museo300}.otf", true, true);

            normalFont = new XFont(museo300, 11, XFontStyle.Regular);
            quoteFont = new XFont(museo100, 11, XFontStyle.Regular);
            boldFont = new XFont(museo300, 11, XFontStyle.Bold);
            boldBiggerFont = new XFont(museo300, 12, XFontStyle.Bold);
            titleFont = new XFont(museo300, 16, XFontStyle.Bold);
            unitNameFont = new XFont(museo300, 12, XFontStyle.Regular);
            unitSecondaryFont = new XFont(museo300, 7, XFontStyle.Regular);
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
                        }),
                        new OrderSubcategory("Zwolnienia funkcyjnych", new List<SubcategoryElement>()
                        {
                            new SubcategoryElement("Na wniosek Rady Drużyny z dnia …… zwalniam ćw. Jacka Orła z funkcji zastępowego zastępu „Twardych” z dniem ………"),
                            new SubcategoryElement("Na wniosek Rady Drużyny zwalniam HO Piotra Kanię z funkcji przybocznego z dniem ………")
                        }),
                        new OrderSubcategory("Powołania do rady drużyny", new List<SubcategoryElement>()
                        {
                            new SubcategoryElement("Powołuję w skład Rady Drużyny sam. Janinę Barys, przyboczną, z dniem ………")
                        }),
                        new OrderSubcategory("Zwolnienia z rady drużyny", new List<SubcategoryElement>()
                        {
                            new SubcategoryElement("Zwalniam ze składu Rady Drużyny ćw. Jacka Orła z dniem ………")
                        }),
                    }),
                    new OrderCategory("Zastępy", new List<OrderSubcategory>()
                    {
                        new OrderSubcategory("Utworzenia zastępu", new List<SubcategoryElement>()
                        {
                            new SubcategoryElement("Na wniosek Rady Drużyny powołuję zastęp „Sępów” w składzie:\n… – zastępowa")
                        }),
                        new OrderSubcategory("Rozwiązanie zastępu", new List<SubcategoryElement>()
                        {
                            new SubcategoryElement("Na wniosek Rady Drużyny rozwiązuję zastęp „Sępów”. Dotychczasowi członkowie zastępu zostają przydzieleni do następujących zastępów:\n...")
                        }),
                        new OrderSubcategory("Zmiana składu zastępów", new List<SubcategoryElement>()
                        {
                            new SubcategoryElement("Przenoszę z zastępu „Orłów” do zastępu „Sokołów” wyw. Jana Kota.")
                        })
                    }),
                    new OrderCategory("Instrumenty metodyczne", new List<OrderSubcategory>()
                    {
                        new OrderSubcategory("Zamknięcie próby na stopień", new List<SubcategoryElement>()
                        {
                            new SubcategoryElement("Na wniosek Rady Drużyny z dnia ……… zamykam próbę i przyznaję stopień ćwika odkr. Antoniemu Szpakowi.")
                        }),
                        new OrderSubcategory("Otwarcie próby na stopień", new List<SubcategoryElement>()
                        {
                            new SubcategoryElement("Na wniosek Rady Drużyny z dnia ………. otwieram próbę na stopień ćwika:\nodkr. Janowi Nowakowi\nwyw. Tomaszowi Kowalskiemu")
                        })
                    })
                }
            };

            return CreateOrder(model);
        }

        public string CreateOrder(OrderModel model)
        {
            PdfDocument document = Init(model);

            Header();
            Footer();

            //DrawText("L.dz. 15/2019");

            DrawSpecialSingleLineString(OrderString, titleFont, XStringFormats.TopCenter);

            Space(2);
            if (!string.IsNullOrEmpty(model.OccassionalIntro))
            {
                DrawString(model.OccassionalIntro, quoteFont);
                Space(2);
            }
            if (!string.IsNullOrEmpty(model.ExceptionsFromAnotherOrder))
            {
                DrawString(model.ExceptionsFromAnotherOrder, quoteFont);
                Space(2);
            }

            uint categoryNumber = 0;

            foreach (OrderCategory category in model.Categories)
            {
                categoryNumber++;
                DrawString($"{categoryNumber}. {category.Name}", boldBiggerFont);

                uint subcategoryNumber = 0;
                foreach (OrderSubcategory subcategory in category.Subcategories)
                {
                    subcategoryNumber++;
                    DrawString($"{categoryNumber}.{subcategoryNumber}. {subcategory.Name}", boldFont);

                    uint elementNumber = 0;
                    foreach (SubcategoryElement element in subcategory.Elements)
                    {
                        elementNumber++;
                        DrawString($"{categoryNumber}.{subcategoryNumber}.{elementNumber}. {element.Description}", normalFont);
                    }
                }
                Space(1);
            }

            Sign();

            document.Save(orderFilename);
            return orderFilename;
        }

        private void Reset()
        {
            actualHeight = pageTopMargin;
            document = null;
            page = null;
            gfx = null;
            textFormatter = null;
        }

        private PdfDocument Init(OrderModel model)
        {
            Reset();

            this.model = model;

            document = new PdfDocument();
            document.Info.Title = OrderString;
            document.Info.Subject = OrderString;
            document.Info.Author = Info.Author;
            document.Info.Creator = "Rozkaz! © 2019 norberto5.pl Norbert Piątkowski";

            page = document.AddPage();
            gfx = XGraphics.FromPdfPage(page);
            textFormatter = new XTextFormatter(gfx);

            return document;
        }

        private void NextPage()
        {
            page = document.AddPage();
            gfx = XGraphics.FromPdfPage(page);
            textFormatter = new XTextFormatter(gfx);
            Footer();
            actualHeight = pageTopMargin;
        }

        private void Header()
        {
            gfx.DrawImage(identifier, new XRect(pageLeftRightMargin, actualHeight, identifier.PixelWidth / 6f, identifier.PixelHeight / 6f));

            double unitMargin = 2;
            double x = 360;
            double y = pageTopMargin;
            double width = 180;
            double height = unitNameFont.Height * 2 + unitMargin * 2;

            var unitRectangleSize = new XRect(x, y, width, height);
            gfx.DrawRoundedRectangle(new XSolidBrush(XColor.FromArgb(0xFF85A314)), unitRectangleSize, new XSize(10, 10));

            x += unitMargin; y += unitMargin;
            DrawSpecialSingleLineString(Info.Unit?.NameFirstLine ?? string.Empty, unitNameFont, XStringFormats.TopLeft, new XRect(x, y, width, unitNameFont.Height), XBrushes.White);
            DrawSpecialSingleLineString(Info.Unit?.NameSecondLine ?? string.Empty, unitNameFont, XStringFormats.TopLeft, new XRect(x, y + unitNameFont.Height, width, unitNameFont.Height), XBrushes.White);


            y = pageTopMargin + unitRectangleSize.Height + unitMargin;

            foreach(string subline in Info.Unit?.SubtextLines ?? new List<string>())
            {
                DrawSpecialSingleLineString(subline, unitSecondaryFont, XStringFormats.TopLeft, new XRect(x, y, width, unitSecondaryFont.Height));
                y += unitSecondaryFont.Height;
            }

            actualHeight = pageTopMargin + identifier.PixelHeight /6 + 40;
            DrawSpecialSingleLineString($"{Info.City ?? string.Empty}, {Info.Date.ToString("dd.MM.yyyy")} r.", normalFont, XStringFormats.TopRight);
        }

        private void Sign()
        {
            double x = 420;
            double y = actualHeight + 30;
            double width = 100;
            double height = normalFont.Height;

            DrawSpecialSingleLineString("CZUWAJ!", normalFont, XStringFormats.Center, new XRect(x, y, width, height));
            DrawSpecialSingleLineString(Info.Author ?? string.Empty, normalFont, XStringFormats.Center, new XRect(x, y + normalFont.Height + 5, width, height));
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

        private void Space(int count) => DrawString(new string('\n', count-1), normalFont);

        private void DrawSpecialSingleLineString(string text, XFont font, XStringFormat stringFormat, XRect rect = new XRect(), XBrush fontColor = null)
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
                        WriteLine(splittedLine, font, rect);
                    }
                }
                else
                {
                    WriteLine(line, font, rect);
                }
            }
        }

        private void WriteLine(string text, XFont font, XRect rect)
        {
            rect.Y = actualHeight;
            textFormatter.DrawString(text, font, XBrushes.Black, rect);
            actualHeight += font.Height;

            if (actualHeight > page.Height - pageBottomMargin)
            {
                NextPage();
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


using ProjektBartoszRuta.Models;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using VectSharp;
using VectSharp.PDF;

namespace ProjektBartoszRuta.Services
{
    public delegate void PDFDelegate(string path, int width, int height);
    public delegate void PNGDelegate(string path, int width, int height, FileInfo fi);
    public class DiagramService : IDiagramService
    {
        public event PDFDelegate onPDFCreating;
        public event PNGDelegate onPNGCreated;
        public void DiagramToPdf(string path, UseCaseDiagram useCaseDiagram)
        {
            int width = 600, height = 400, lHeight = 130, lWidth = 40, lGap = 30, lStartX = 40, rStartX = width - lStartX - lWidth;
            
            var actors = useCaseDiagram.Actors;

            var cases = useCaseDiagram.UseCases;
            int ucHeight = 50, ucWidth = 200, ucStartX = (int)(width / 2.0 - ucWidth / 2.0), ucGap = 10, count = cases.Count;

            var h = 80 + (ucHeight + ucGap) * count;
            height = h > height ? h : height;
            h = (int) ((lHeight+lGap+20) * Math.Ceiling(actors.Count()/2.0));
            height = h > height ? h : height;

            Document doc = new Document();
            Document d = new Document();
            doc.Pages.Add(new Page(width, height));
            Graphics gpr = doc.Pages.Last().Graphics;
            //gpr.Scale(2, 2);
            //var arc = new VectSharp.GraphicsPath().Arc(100, 100, 30, 0, Math.PI);
            //gpr.StrokePath(arc, Colour.FromRgb(255, 0, 0));
            //gpr.FillText(50, 400, "Przykładowy tekst", new Font(new FontFamily(FontFamily.StandardFontFamilies.Courier),15), Colour.FromRgb(0, 0, 0));
            //doc.SaveAsPDF(path);
            FontFamily courier = new FontFamily(FontFamily.StandardFontFamilies.Courier);

            gpr.FillText(width / 2 - gpr.MeasureText(useCaseDiagram.Name, new Font(courier, 20)).Width / 2.0, 20, useCaseDiagram.Name, new Font(courier, 20), Colour.FromRgb(0, 0, 0));

            
            int left = (int) Math.Ceiling(actors.Count / 2.0);
            int right = actors.Count / 2;
            var leftSide = new double[left];
            var rightSide = new double[right];

            switch(left % 2)
            {
                case 0: // parzyste - 2 elementy środkowe
                    if (left == 0)
                        break;
                    leftSide[0] = 40 + (height - 40) / 2.0 - lGap / 2.0; // 2 środkowe elementy
                    leftSide[1] = 40 + (height - 40) / 2.0 + lGap / 2.0;
                    leftSide[0] += -lHeight;
                    leftSide[1] += 0; // jest git
                    for (int i = 2; i < left; i += 2)
                    {
                        leftSide[i] = leftSide[i - 2] - lGap - lHeight;
                        leftSide[i + 1] = leftSide[i - 1] + lHeight + lGap;
                    }
                    leftSide = leftSide.OrderBy(_ => _).ToArray(); // głupie że nie ide od środka, ale nie chce mi się zmieniać
                    break;
                default: // nieparzyste - 1 srodkowy
                    leftSide[left / 2] = 40 + (height - 40) / 2.0; // środkowy element
                    leftSide[left / 2] += -lHeight / 2.0;
                    for (int i = left / 2 - 1, j = left / 2 + 1; j < left; i--, j++)
                    {
                        leftSide[i] = leftSide[i + 1] - lGap - lHeight;
                        leftSide[j] = leftSide[j - 1] + lHeight + lGap;
                    }
                    break;
            }

            switch (right % 2)
            {
                case 0: // parzyste - 2 elementy środkowe
                    if (right == 0)
                        break;
                    rightSide[0] = 40 + (height - 40) / 2.0 - lGap / 2.0; // 2 środkowe elementy
                    rightSide[1] = 40 + (height - 40) / 2.0 + lGap / 2.0;
                    rightSide[0] += -lHeight;
                    rightSide[1] += 0; // jest git
                    for (int i = 2; i < right; i += 2)
                    {
                        rightSide[i] = rightSide[i - 2] - lGap - lHeight;
                        rightSide[i + 1] = rightSide[i - 1] + lHeight + lGap;
                    }
                    rightSide = rightSide.OrderBy(_ => _).ToArray(); // głupie że nie ide od środka, ale nie chce mi się zmieniać
                    break;
                default: // nieparzyste - 1 srodkowy
                    rightSide[right / 2] = 40 + (height - 40) / 2.0; // środkowy element
                    rightSide[right / 2] += -lHeight / 2.0;
                    for (int i = right / 2 - 1, j = right / 2 + 1; j < right; i--, j++)
                    {
                        rightSide[i] = rightSide[i + 1] - lGap - lHeight;
                        rightSide[j] = rightSide[j - 1] + lHeight + lGap;
                    }
                    break;
            }

            var arc = new GraphicsPath().Arc(20, 20, 20, 0, 2 * Math.PI); // jakby szablony dla głowy i ludzika
            var ludzik = new GraphicsPath().MoveTo(20, 40).LineTo(20, 100)
                .MoveTo(0, 70).LineTo(20, 50).LineTo(40, 70)
                .MoveTo(0, 130).LineTo(20, 100).LineTo(40, 130);

            var head1 = arc/*.Transform(x => new Point(x.X/2.0, x.Y/2.0))*/.Transform(x => new Point(x.X + 40, x.Y + 40 + (height - 40) /2 - 75));
            var ludzik1 = ludzik/*.Transform(x => new Point(x.X / 2.0, x.Y / 2.0))*/.Transform(x => new Point(x.X + 40, x.Y + 40 + (height - 40) / 2 - 75));
            //gpr.FillPath(head1, Colour.FromRgb(255, 230, 161));
            //gpr.StrokePath(head1, Colour.FromRgb(0, 0, 0));
            //gpr.StrokePath(ludzik1, Colour.FromRgb(0, 0, 0));
            //gpr.FillText(40 + 20 - gpr.MeasureText(useCaseDiagram.Actors.ElementAt(0).Name, new Font(courier, 10)).Width / 2.0, 130 + 40 + (height - 40) / 2 - 75, useCaseDiagram.Actors.ElementAt(0).Name, new Font(courier, 10), Colour.FromRgb(0, 0, 0));

            //Use Cases
            //int i = 0;
            
            
            Font ucFont = new Font(courier, 12);
            var max = cases.Aggregate((a, b) => a.Name.Length > b.Name.Length ? a : b);

            // Zwiększanie elips gdy nie ma miejsca
            //var maxLength = gpr.MeasureText(max.Name, ucFont).Width;
            //if (maxLength > ucWidth) {
            //    ucStartX -= (int) (maxLength - ucWidth) / 2;
            //    ucWidth = (int) maxLength + 10;
            //}

            // Zmniejszenie wszystkich czcionek gdy brak miejsca
            //double maxLength; 
            //while ((maxLength = gpr.MeasureText(max.Name, ucFont).Width) > ucWidth)
            //    ucFont = new Font(courier, ucFont.FontSize - 1);


            if (useCaseDiagram.UseCases.Count < 0)
                return;

            double[] topLefts = new double[cases.Count];

            switch (useCaseDiagram.UseCases.Count % 2)
            {
                case 0: // parzyste
                    topLefts[0] = 40 + (height - 40) / 2.0 - ucGap / 2; // 2 środkowe elementy
                    topLefts[1] = 40 + (height - 40) / 2.0 + ucGap / 2;
                    topLefts[0] += -ucHeight;
                    topLefts[1] += 0; // jest git
                    for (int i = 2; i < count; i += 2)
                    {
                        topLefts[i] = topLefts[i - 2] - ucGap - ucHeight;
                        topLefts[i + 1] = topLefts[i - 1] + ucHeight + ucGap;
                    }
                    topLefts = topLefts.OrderBy(_ => _).ToArray(); // głupie że nie ide od środka, ale nie chce mi się zmieniać
                    break;

                default: // nieparzyste
                    topLefts[count / 2] = 40 + (height - 40) / 2.0; // środkowy element
                    topLefts[count / 2] += -ucHeight / 2.0;
                    for (int i = count / 2 - 1, j = count / 2 + 1; j < count; i--, j++)
                    {
                        topLefts[i] = topLefts[i + 1] - ucGap - ucHeight;
                        topLefts[j] = topLefts[j - 1] + ucHeight + ucGap;
                    }
                    break;
            }



            

            for (int i = 0; i < count; i++) // usecasy
            {
                var textLength = gpr.MeasureText(cases.ElementAt(i).Name, ucFont);
                //gpr.StrokeRectangle(ucStartX, topLefts[i], ucWidth, ucHeight, Colour.FromRgb(0, 0, 0));
                //gpr.FillText(ucStartX + ucWidth/2.0 - textLength.Width/2.0, topLefts[i] + ucHeight/2.0, cases.ElementAt(i).Name, ucFont, Colour.FromRgb(0, 0, 0),TextBaselines.Middle);
                var elipsa = new GraphicsPath().MoveTo(ucStartX, topLefts[i] + ucHeight / 2.0).EllipticalArc(ucWidth / 2.0, ucHeight / 2.0, 0.0, true, true, new Point(ucStartX + ucWidth, topLefts[i] + ucHeight / 2.0));
                //elipsa.MoveTo(ucStartX, topLefts[i] + ucHeight / 2.0).EllipticalArc(ucWidth/2.0, ucHeight/2.0, 0.0, true, false, new Point(ucStartX + ucWidth, topLefts[i] + ucHeight / 2.0));
                elipsa.EllipticalArc(ucWidth / 2.0, ucHeight / 2.0, 0.0, true, true, new Point(ucStartX, topLefts[i] + ucHeight / 2.0));
                gpr.FillPath(elipsa, Colour.FromRgb(181, 213, 247));
                gpr.StrokePath(elipsa, Colour.FromRgb(0, 0, 0));

                // Zmniejszanie czcionki pojedynczego usecasa
                Font font = ucFont;
                double len;
                while ((len = gpr.MeasureText(cases.ElementAt(i).Name, font).Width) > ucWidth)
                    font = new Font(courier, font.FontSize - 1);
                gpr.FillText(ucStartX + ucWidth / 2.0 - len / 2.0, topLefts[i] + ucHeight / 2.0, cases.ElementAt(i).Name, font, Colour.FromRgb(0, 0, 0), TextBaselines.Middle);

                //gpr.FillText(ucStartX + ucWidth / 2.0 - textLength.Width / 2.0, topLefts[i] + ucHeight / 2.0, cases.ElementAt(i).Name, ucFont, Colour.FromRgb(0, 0, 0), TextBaselines.Middle);
            }

            for (int i = 0; i < left; i++) // ludziki
            {
                var head = arc.Transform(x => new Point(x.X + lStartX, x.Y + leftSide[i]));
                var lud = ludzik.Transform(x => new Point(x.X + lStartX, x.Y + leftSide[i]));
                gpr.FillPath(head, Colour.FromRgb(255, 230, 161));
                gpr.StrokePath(head, Colour.FromRgb(0, 0, 0));
                gpr.StrokePath(lud, Colour.FromRgb(0, 0, 0));
                gpr.FillText(lStartX + lWidth / 2.0 - gpr.MeasureText(actors.ElementAt(i).Name, new Font(courier, 10)).Width / 2.0, leftSide[i] + lHeight + 5, actors.ElementAt(i).Name, new Font(courier, 10), Colour.FromRgb(0, 0, 0));
                var joins = actors.ElementAt(i).UseCaseActorJoins;
                foreach (var join in joins)
                {
                    int indeks = cases.ToList().IndexOf(join.UseCase);
                    //var linia = new GraphicsPath().MoveTo(lStartX + lWidth + 10, leftSide[i] + lHeight / 2.0).LineTo(ucStartX, topLefts[indeks] + ucHeight / 2.0);
                    var linia = new GraphicsPath().MoveTo(Przeciecie(new Point(lStartX+lWidth, leftSide[i] + lHeight / 2.0), 
                        new Point(ucStartX, topLefts[indeks] + ucHeight / 2.0), lStartX+lWidth+10))
                        .LineTo(ucStartX, topLefts[indeks] + ucHeight / 2.0);

                    gpr.StrokePath(linia, Colour.FromRgb(0, 0, 0));
                }
            }

            for (int i = 0; i < right; i++) // ludziki
            {
                var head = arc.Transform(x => new Point(x.X + rStartX, x.Y + rightSide[i]));
                var lud = ludzik.Transform(x => new Point(x.X + rStartX, x.Y + rightSide[i]));
                gpr.FillPath(head, Colour.FromRgb(255, 230, 161));
                gpr.StrokePath(head, Colour.FromRgb(0, 0, 0));
                gpr.StrokePath(lud, Colour.FromRgb(0, 0, 0));
                gpr.FillText(rStartX + lWidth / 2.0 - gpr.MeasureText(actors.ElementAt(i + left).Name, new Font(courier, 10)).Width / 2.0, rightSide[i] + lHeight + 5, actors.ElementAt(i + left).Name, new Font(courier, 10), Colour.FromRgb(0, 0, 0));
                var joins = actors.ElementAt(i + left).UseCaseActorJoins;
                foreach(var join in joins)
                {
                    int indeks = cases.ToList().IndexOf(join.UseCase);
                    //var linia = new GraphicsPath().MoveTo(rStartX - 10, rightSide[i] + lHeight / 2.0).LineTo(ucStartX + ucWidth, topLefts[indeks] + ucHeight / 2.0);
                    var linia = new GraphicsPath().MoveTo(Przeciecie(new Point(rStartX, rightSide[i] + lHeight / 2.0),
                        new Point(ucStartX + ucWidth, topLefts[indeks] + ucHeight / 2.0), rStartX - 10))
                        .LineTo(ucStartX + ucWidth, topLefts[indeks] + ucHeight / 2.0);
                    gpr.StrokePath(linia, Colour.FromRgb(0, 0, 0));
                    
                }
            }
            onPDFCreating?.Invoke(path, width, height);

            doc.SaveAsPDF(path);
        }

        public void PdfToPNG(string pathPDF, string path, int width, int height)
        {
            PdfDocument doc = new PdfDocument();
            doc.LoadFromFile(pathPDF);

            PdfUnitConvertor unitCvtr = new PdfUnitConvertor();
            //float pixelWidth = unitCvtr.ConvertUnits(doc.Pages[0].Size.Width, PdfGraphicsUnit.Point, PdfGraphicsUnit.Pixel);
            //float pixelHeight = unitCvtr.ConvertUnits(doc.Pages[0].Size.Height, PdfGraphicsUnit.Point, PdfGraphicsUnit.Pixel);
            //float pixelWidth = unitCvtr.ConvertUnits(doc.Pages[0].Size.Width, PdfGraphicsUnit.Pixel, PdfGraphicsUnit.Point);
            //float pixelHeight = unitCvtr.ConvertUnits(doc.Pages[0].Size.Height, PdfGraphicsUnit.Pixel, PdfGraphicsUnit.Point);

            //System.Drawing.Image i = doc.SaveAsImage(0,width, (int) (width/(doc.Pages[0].Size.Width / doc.Pages[0].Size.Height)));
            //System.Drawing.Image i = doc.SaveAsImage(0, Spire.Pdf.Graphics.PdfImageType.Metafile);
            System.Drawing.Image i = width >= 400 ? doc.SaveAsImage(0, Spire.Pdf.Graphics.PdfImageType.Metafile) : doc.SaveAsImage(0,500,500);
            var a = resizeImage(i, new Size(width, height));
            //a.Save(path, ImageFormat.Png);
            
            a.Save(path, ImageFormat.Png);
            onPNGCreated?.Invoke(path, a.Width, a.Height, new FileInfo(path));
        }

        Point Przeciecie(Point a, Point b, double x) // z jest pionową linią
        {
            double A1 = (a.Y - b.Y) / (a.X - b.X);
            double B1 = a.Y - A1 * a.X;

            double y = A1 * x + B1;

            return new Point(x, y);
        }

        static System.Drawing.Image resizeImage(System.Drawing.Image imgToResize, Size size)
        {
            //Get the image current width  
            int sourceWidth = imgToResize.Width;
            //Get the image current height  
            int sourceHeight = imgToResize.Height;
            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            //Calulate  width with new desired size  
            nPercentW = ((float)size.Width / (float)sourceWidth);
            //Calculate height with new desired size  
            nPercentH = ((float)size.Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;
            //New Width  
            int destWidth = (int)(sourceWidth * nPercent);
            //New Height  
            int destHeight = (int)(sourceHeight * nPercent);
            System.Drawing.Bitmap b = new System.Drawing.Bitmap(destWidth, destHeight);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            // Draw image with new width and height  
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return (System.Drawing.Image)b;
        }
    }
}
using ProjektBartoszRuta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektBartoszRuta.Services
{
    interface IDiagramService
    {
        event PDFDelegate onPDFCreating;
        event PNGDelegate onPNGCreated;
        void DiagramToPdf(String path, UseCaseDiagram useCaseDiagram);
        void PdfToPNG(string pathPDF, string path, int width, int height);
    }
}

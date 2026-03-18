using SolidPrinciples.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolidPrinciples.Savers
{
    public class ReportSaver
    {
        public void SaveToFile(Report report)
        {
            Console.WriteLine($"Saving report: {report.Title}");
        }
    }
}

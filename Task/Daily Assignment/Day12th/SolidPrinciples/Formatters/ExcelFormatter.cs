using SolidPrinciples.Interfaces;
using SolidPrinciples.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolidPrinciples.Formatters
{
    public class ExcelFormatter : IReportFormatter
    {
        public void Format(Report report)
        {
            Console.WriteLine("Formatting report as Excel");
        }
    

    }
}

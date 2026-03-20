using SolidPrinciples.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolidPrinciples.Generators
{
    public class ReportGenerator
    {
        public Report GenerateReport(string title)
        {
            return new Report
            {
                Title = title,
                Content = "Report Content"
            };
        }
    }
}
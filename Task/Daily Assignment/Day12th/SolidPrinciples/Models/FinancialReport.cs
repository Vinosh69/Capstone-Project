using System;
using System.Collections.Generic;
using System.Text;

namespace SolidPrinciples.Models
{
    public class FinancialReport : Report
    {
        public override void Generate()
        {
            Console.WriteLine("Generating Financial Report");
        }
    
    }
}

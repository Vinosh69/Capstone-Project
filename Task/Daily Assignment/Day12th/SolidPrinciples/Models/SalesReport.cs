using System;
using System.Collections.Generic;
using System.Text;

namespace SolidPrinciples.Models
{
    public class SalesReport : Report
    {
        public override void Generate()
        {
            Console.WriteLine("Generating Sales Report");
        }
    

    }
}

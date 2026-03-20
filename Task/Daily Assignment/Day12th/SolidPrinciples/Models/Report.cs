using System;
using System.Collections.Generic;
using System.Text;

namespace SolidPrinciples.Models
{
    public class Report
    {
        public string Title { get; set; }
        public string Content { get; set; }

        public virtual void Generate()
        {
            Console.WriteLine("Generating base report...");
        }
    }
}

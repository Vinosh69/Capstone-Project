using SolidPrinciples.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolidPrinciples.Interfaces
{
    public interface IReportFormatter
    {
        void Format(Report report);
    }
}

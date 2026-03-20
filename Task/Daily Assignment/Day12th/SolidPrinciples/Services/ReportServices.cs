using SolidPrinciples.Interfaces;
using SolidPrinciples.Models;

namespace SolidPrinciples.Services
{
    public class ReportService
    {
        private readonly IReportFormatter _formatter;

        public ReportService(IReportFormatter formatter)
        {
            _formatter = formatter;
        }

        public void ProcessReport(Report report)
        {
            report.Generate();
            _formatter.Format(report);
        }
    }
}
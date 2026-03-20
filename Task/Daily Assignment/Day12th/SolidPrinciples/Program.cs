using SolidPrinciples.Formatters;
using SolidPrinciples.Generators;
using SolidPrinciples.Models;
using SolidPrinciples.Savers;
using SolidPrinciples.Services;


class Program
{
    static void Main(string[] args)
    {
        // SRP - Generate Report
        var generator = new ReportGenerator();
        var report = generator.GenerateReport("Annual Report");

        // LSP - Using derived class
        Report financialReport = new FinancialReport();
        financialReport.Generate();

        // OCP + DIP - Using Formatter through abstraction
        var formatter = new PdfFormatter();
        var service = new ReportService(formatter);
        service.ProcessReport(report);

        // SRP - Save report
        var saver = new ReportSaver();
        saver.SaveToFile(report);

        Console.ReadLine();
    }
}
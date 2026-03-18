using DesignPatterns.Pattern;
class Program
{
    static void Main(string[] args)
    {
        // Get first instance
        Logger logger1 = Logger.Instance;
        logger1.Log("Application Started");

        // Get second instance
        Logger logger2 = Logger.Instance;
        logger2.Log("Processing Data");

        // Verify both instances are same
        if (logger1 == logger2)
        {
            Console.WriteLine("Both logger instances are same. Singleton works!");
        }

        Console.ReadLine();
    }
}
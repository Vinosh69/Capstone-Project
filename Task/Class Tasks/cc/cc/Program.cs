using System;
using System.Linq;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        string[] words = { "hello", "world", "linq", "csharp" };

        var people = new[]
        {
            new { Name = "Alice", Age = 25 },
            new { Name = "Bob", Age = 30 },
            new { Name = "Charlie", Age = 25 },
            new { Name = "David", Age = 35 }
        };

        // We will write LINQ practice here 👇
        var evenNumbers = numbers.Where(n => n % 2 == 0);

        foreach (var n in evenNumbers)
        {
            Console.WriteLine(n);
        }


        var squares = numbers.Select(n => n * n);

        foreach (var n in squares)
        { Console.WriteLine(n); }


        var chars = words.SelectMany(w => w.ToCharArray());

        foreach (var c in chars)
        {
            Console.WriteLine(c);
        }


        var sortedPeople = people.OrderBy(p => p.Age).ThenBy(p => p.Name);

        foreach (var p in sortedPeople)
        { Console.WriteLine($"{p.Name} - {p.Age}"); }


        Console.WriteLine("Count: " + numbers.Count());
        Console.WriteLine("Sum: " + numbers.Sum());
        Console.WriteLine("Average: " + numbers.Average());
        Console.WriteLine("Min: " + numbers.Min());
        Console.WriteLine("Max: " + numbers.Max());


        Console.WriteLine(numbers.Any(n => n > 8));     // true?
        Console.WriteLine(numbers.All(n => n > 0));     // true?
        Console.WriteLine(numbers.Contains(5));         // true?

        var first3 = numbers.Take(3);
        var skip3 = numbers.Skip(3);

        Console.WriteLine("Take 3:");
        foreach (var n in first3) Console.WriteLine(n);

        Console.WriteLine("Skip 3:");
        foreach (var n in skip3) { Console.WriteLine(n); }

        Console.WriteLine("First: " + numbers.First());
        Console.WriteLine("Last: " + numbers.Last());
        Console.WriteLine("ElementAt(2): " + numbers.ElementAt(2));


        var grouped = people.GroupBy(p => p.Age);

        foreach (var group in grouped)
        {
            Console.WriteLine("Age: " + group.Key);
            foreach (var person in group)
            {
                Console.WriteLine("  " + person.Name);
            }
        }

        int[] numsWithDup = { 1, 2, 2, 3, 3, 4, 5 };

        var distinctNums = numsWithDup.Distinct();

        foreach (var n in distinctNums)
            Console.WriteLine(n);





    }
}
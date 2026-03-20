
using System;

class Student
{
    private int marks;

    public int Marks
    {
        get { return marks; }
        set
        {
            if (value >= 0)
                marks = value;
        }
    }
}

class Program
{
    static void Main()
    {
        Student s = new Student();
        s.Marks = 85;
        Console.WriteLine(s.Marks);
    }
}

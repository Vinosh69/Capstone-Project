using Xunit;
using calculatorap;

public class calculatortest
{
    [Fact]
    public void Add_TwoNumbers_ReturnsSum()
    {
        var calculator = new calculator();

        var result = calculator.Add(2, 3);

        Assert.Equal(5, result);
    }
}

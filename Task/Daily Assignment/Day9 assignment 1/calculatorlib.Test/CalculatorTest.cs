using Microsoft.VisualStudio.TestTools.UnitTesting;
using calculatorlib;
using System;

namespace CalculatorLib.Tests
{
    [TestClass]
    public class CalculatorTests
    {
        public Calculator calculatorInstance;

        [TestInitialize]
        public void InitializeCalculator()
        {
            calculatorInstance = new Calculator();
        }

        [TestMethod]
        public void Add_TwoValidNumbers_ReturnsCorrectResult()
        {
            double firstValue = 2;
            double secondValue = 3;

            double sumResult = calculatorInstance.Add(firstValue, secondValue);

            Assert.AreEqual(5, sumResult);
        }

        [TestMethod]
        public void Subtract_TwoValidNumbers_ReturnsCorrectResult()
        {
            double firstValue = 5;
            double secondValue = 3;

            double differenceResult = calculatorInstance.Subtract(firstValue, secondValue);

            Assert.AreEqual(2, differenceResult);
        }

        [TestMethod]
        public void Multiply_TwoValidNumbers_ReturnsCorrectResult()
        {
            double firstValue = 4;
            double secondValue = 3;

            double productResult = calculatorInstance.Multiply(firstValue, secondValue);

            Assert.AreEqual(12, productResult);
        }

        [TestMethod]
        public void Divide_TwoValidNumbers_ReturnsCorrectResult()
        {
            double dividendValue = 10;
            double divisorValue = 2;

            double quotientResult = calculatorInstance.Divide(dividendValue, divisorValue);

            Assert.AreEqual(5, quotientResult);
        }

        [TestMethod]
        public void Divide_ByZero_ShouldThrow()
        {
            var calc = new Calculator();
            Assert.Throws<DivideByZeroException>(() => calc.Divide(10, 0));
        }
        public void Divide_ByZero_ThrowsDivideByZeroException()
        {
            double dividendValue = 10;
            double divisorValue = 0;

            calculatorInstance.Divide(dividendValue, divisorValue);
        }

        [TestMethod]
        public void Add_ZeroToNumber_ReturnsSameNumber()
        {
            double originalValue = 5;
            double zeroValue = 0;

            double resultValue = calculatorInstance.Add(originalValue, zeroValue);

            Assert.AreEqual(5, resultValue);
        }
    }
}
using NUnit.Framework;

namespace Umb.Testing.Tests.SampleTests
{
    public class Calculator
    {
        public static int Add(int x, int y)
        {
            return x + y;
        }
    }

    [TestFixture]
    public class Adding_Numbers
    {
        [Test]
        public void Returns_Sum()
        {
            const int expected = 5;
            var actual = Calculator.Add(3, 2);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}

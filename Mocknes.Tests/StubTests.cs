using FluentAssertions;

using static Mocknes.Mocknes;
namespace Mocknes.Tests;




internal interface ITest
{
    int Calculate();
    int Calculate(int x, int y);
    Task<int> CalculateAsync();
    string StringMethod(string x);
}


public class Stubs
{
    [Fact]
    public void Stub_Without_Paramaters_Should_Be_Invoked()
    {
        var mock = Mock<ITest>(m =>
        {
            m.When(x => x.Calculate()).ThenReturn(42);
        });
        mock.Calculate().Should().Be(42);
    }

    [Fact]
    public void Mock_Should_Invoke_Correct_Stub()
    {
        var mock = Mock<ITest>(m =>
        {
            m.When(x => x.Calculate(2, 3)).ThenReturn(42);
            m.When(x => x.Calculate(2, 2)).ThenReturn(41);
        });
        mock.Calculate(2, 2).Should().Be(41);
        mock.Calculate(2, 3).Should().Be(42);
    }

    [Fact]
    public async Task Stub_Should_Be_Awaitable_When_ReturnAsync()
    {
        var mock = Mock<ITest>(m =>
        {
            m.When(x => x.CalculateAsync()).ThenReturnAsync(50);
        });

        var result = await mock.CalculateAsync();
        result.Should().Be(50);
    }

    [Fact]
    public void Mock_Should_Invoke_Correct_Stub_When_Argument_Is_String()
    {
        var mock = Mock<ITest>(m =>
        {
            m.When(x => x.StringMethod("Hello")).ThenReturn("World");
        });

        mock.StringMethod("Hello").Should().Be("World");
    }
}
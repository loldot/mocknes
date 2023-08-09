# mocknes
## A shady AF mocking library 🕶️
Use irresponsibly

## Example
```c#
using static Mocknes.Mocknes;

int value = 3;

var testI = Mock<ITest>(m =>
{
    m.When(x => x.Calculate(2,2)).ThenReturn(4);
    m.When(x => x.Calculate(2, value)).ThenReturn(48);
    m.When(x => x.CalculateAsync()).ThenReturnAsync(50);
});

Console.WriteLine(testI.Calculate(2, 2));
Console.WriteLine(testI.Calculate(2, 3));
Console.WriteLine(await testI.CalculateAsync());


internal interface ITest
{
    int Calculate();
    int Calculate(int x, int y);
    Task<int> CalculateAsync() ;
}
```

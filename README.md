# mocknes
## A naive library for stub implementations in dotnet 🕶️

## Example
```c#

internal interface ISunglassShop
{
    int GetPrice(string customerName);
}

using FluentAssertions;
using static Mocknes.Mocknes;

[Fact]
public void Bjornar_Should_Get_Sunglasses_For_Free()
{
    var shop = Mock<ISunglassShop>(m =>
    {
        m.When(x => x.GetPrice("Bjørnar")).ThenReturn(0);
    });

    shop.GetPrice("Bjørnar").Should().Be(0);
}

```

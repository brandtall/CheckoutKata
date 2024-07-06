namespace UnitTests;

public class Tests
{
    [Test]
    public void ShouldReturnTotal_NoOFfers()
    {
        var sut = new Checkout();

        var total = sut.Total();
        
        Assert.NotNull(total);
    }
    
}

public class Checkout
{
    public int Total()
    {
        return 0;
    }
}
namespace UnitTests;

public class Tests
{
    [Test]
    public void ShouldReturnTotal_NoOffers()
    {
        Dictionary<string, double> pricing = new Dictionary<string, double>();
        pricing.Add("A", 4);
        pricing.Add("B", 6);
        pricing.Add("C", 8.2);
        
        
        var sut = new Checkout(pricing);
        var total = sut.Total();
        
        
        Assert.That(total, Is.EqualTo(4 + 6 + 8.2));
    }
    
}

public class Checkout
{
    private readonly double _sum;

    public Checkout(Dictionary<string,double>? pricing)
    {
        if (pricing == null) return;
        foreach (var price in pricing.Values)
        {
            _sum += price;
        }
    }

    public double Total()
    {
        return _sum;
    }
}
namespace UnitTests;

public class Tests
{
    [Test]
    public void ShouldReturnZero_WhenNoPricesAreSet()
    {
        Dictionary<string, double> pricing = new Dictionary<string, double>();
        Dictionary<string, int>? items = new Dictionary<string, int>();
        
        var sut = new Checkout(items, pricing, null);
        var total = sut.Total();
        
        
        Assert.That(total, Is.EqualTo(0));
    }

    [Test]
    public void ShouldReturnTotal_WithNoOffers()
    {
        Dictionary<string, double> pricing = new Dictionary<string, double>();
        Dictionary<string, int>? items = new Dictionary<string, int>();

        pricing.Add("A", 4);
        pricing.Add("B", 6);
        pricing.Add("C", 8.2);
        
        items.Add("A", 1);
        items.Add("B", 1);
        items.Add("C", 1);
        
        
        var sut = new Checkout(items, pricing, null);
        var total = sut.Total();
        
        
        Assert.That(total, Is.EqualTo(4 + 6 + 8.2));
    }
    [Test]
    public void ShouldReturnTotal_WithOneOffers()
    {
        Dictionary<string, double>? pricing = new Dictionary<string, double>();
        Dictionary<string, Dictionary<int, double>>? offers = new Dictionary<string, Dictionary<int, double>>();
        Dictionary<string, int>? items = new Dictionary<string, int>();
        
        pricing.Add("A", 50);
        pricing.Add("B", 30);
        pricing.Add("C", 20);
        
        offers.Add("A", new Dictionary<int, double>(){{3, 130}});
        offers.Add("B", new Dictionary<int, double>(){{2, 45}});
        
        items.Add("A", 3);
        items.Add("B", 2);
        items.Add("C", 1);
        
        var sut = new Checkout(items, pricing, offers);
        var total = sut.Total();
        
        
        Assert.That(total, Is.EqualTo(130+45+20));
    }
    
}

public class Checkout
{
    private readonly double _sum;

    public Checkout(Dictionary<string, int>? items, Dictionary<string, double>? pricing, Dictionary<string, Dictionary<int, double>>? offers)
    {
        if (pricing is null || items is null) return;
        if (offers is null) offers = new Dictionary<string, Dictionary<int, double>>();
        
        foreach (var item in pricing)
        {
            var sku = item.Key;
            var quantity = items[sku];
            var itemPrice = item.Value;
            if (offers.ContainsKey(sku))
            {
                _sum += offers[sku].ContainsKey(quantity) ? offers[sku][quantity] : itemPrice;
            }
            else
            {
                _sum += quantity * itemPrice;
            }

        }
    }

    public double Total()
    {
        return _sum;
    }
}
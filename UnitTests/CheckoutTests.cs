namespace UnitTests;

public class Tests
{
    [Test]
    public void ShouldReturnZero_WhenNoPricesAreSet()
    {
        Dictionary<string, double> pricing = new Dictionary<string, double>();
        Dictionary<string, int>? items = new Dictionary<string, int>();
        
        var cart = new List<Item>();
        
        var sut = new Checkout(null, cart);
        var total = sut.Total;
        
        
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

        var cart = new List<Item>
        {
            new("A", 4, 1),
            new("B", 6, 1),
            new("C", 8.2, 1)
        };

        var sut = new Checkout(null, cart);
        var total = sut.Total;
        
        
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
        
        var cart = new List<Item>
        {
            new("A", 50, 3),
            new("B", 30, 2),
            new("C", 20, 1)
        };
        
        var sut = new Checkout(offers, cart);
        var total = sut.Total;
        
        
        Assert.That(total, Is.EqualTo(130+45+20));
    }
    
    [Test]
    public void ShouldReturnTotal_WithOneOffers_And_QuantityIsHigherThanOffer()
    {
        Dictionary<string, double>? pricing = new Dictionary<string, double>();
        Dictionary<string, Dictionary<int, double>>? offers = new Dictionary<string, Dictionary<int, double>>();
        Dictionary<string, int>? items = new Dictionary<string, int>();
        
        pricing.Add("A", 50);
        pricing.Add("B", 30);
        pricing.Add("C", 20);
        
        offers.Add("A", new Dictionary<int, double>(){{3, 130}});
        offers.Add("B", new Dictionary<int, double>(){{2, 45}});
        
        items.Add("A", 4);
        items.Add("B", 3);
        items.Add("C", 2);
        
        var cart = new List<Item>
        {
            new("A", 50, 4),
            new("B", 30, 3),
            new("C", 20, 2)
        };
        
        var sut = new Checkout(offers, cart);
        var total = sut.Total;
        
        
        Assert.That(total, Is.EqualTo(130+50+30+45+40));
    }
    
    [Test]
    public void ShouldReturnTotal_WithMultipleOffers()
    {
        Dictionary<string, double>? pricing = new Dictionary<string, double>();
        Dictionary<string, Dictionary<int, double>>? offers = new Dictionary<string, Dictionary<int, double>>();
        Dictionary<string, int>? items = new Dictionary<string, int>();
        
        pricing.Add("A", 50);
        pricing.Add("B", 30);
        pricing.Add("C", 20);
        
        offers.Add("A", new Dictionary<int, double>(){{3, 130}, {5, 200}});
        offers.Add("B", new Dictionary<int, double>(){{2, 45}, {4, 65}});
        
        items.Add("A", 5);
        items.Add("B", 3);
        items.Add("C", 2);
        
        var cart = new List<Item>
        {
            new("A", 50, 5),
            new("B", 30, 3),
            new("C", 20, 2)
        };
        
        var sut = new Checkout(offers, cart);
        var total = sut.Total;
        
        
        Assert.That(total, Is.EqualTo(200+45+30+40));
    }
    
}

public class Item
{
    public Item(string sku, double price, int quantity)
    {
        Sku = sku;
        Price = price;
        Quantity = quantity;
    }

    public string Sku { get; set; }
    
    public double Price { get; set; }
    
    public int Quantity { get; set; }
}

public class Checkout
{
    public double Total { get; private set; }

    public Checkout(Dictionary<string, Dictionary<int, double>>? offers, List<Item>? cart)
    {
        offers ??= new Dictionary<string, Dictionary<int, double>>();
        if (cart == null) return;

        Total = 0;
        
        foreach (var item in cart)
        {
            CalculateTotal(offers, item.Quantity, item.Price, item.Sku);
        }
    }

    private void CalculateTotal(Dictionary<string, Dictionary<int, double>> offers, int quantity, double price, string itemSku)
    {
        Total += quantity * price;
        ApplyOffer(offers, itemSku, quantity, price);
    }

    private void ApplyOffer(Dictionary<string, Dictionary<int, double>> offers, string sku, int quantity, double price)
    {
        if (!offers.TryGetValue(sku, out var offer)) return;
        var offerQuantity = offer.Where(o => o.Key <= quantity).MaxBy(o => o.Key).Key; 
        Total -= offerQuantity*price - offers[sku][offerQuantity];
    }
}
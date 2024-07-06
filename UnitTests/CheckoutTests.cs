namespace UnitTests;

public class Tests
{
    [Test]
    public void ShouldReturnZero_WhenNoPricesAreSet()
    {
        var cart = new List<Item>();
        
        var sut = new Checkout(null, cart);
        var total = sut.Total;
        
        
        Assert.That(total, Is.EqualTo(0));
    }

    [Test]
    public void ShouldReturnTotal_WithNoOffers()
    {
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
        Dictionary<string, Dictionary<int, double>>? offers = new Dictionary<string, Dictionary<int, double>>();
        
        offers.Add("A", new Dictionary<int, double>(){{3, 130}});
        offers.Add("B", new Dictionary<int, double>(){{2, 45}});

        var item1 = new Item("A", 50, 3);
        var item2 = new Item("B", 30, 2);
        var item3 = new Item("C", 20, 1);

        new Offer(item1, 3, 130);
        
        
        var cart = new List<Item>
        {
            item1,
            item2,
            item3
        };
        
        var sut = new Checkout(offers, cart);
        var total = sut.Total;
        
        
        Assert.That(total, Is.EqualTo(130+45+20));
    }
    
    [Test]
    public void ShouldReturnTotal_WithOneOffers_And_QuantityIsHigherThanOffer()
    {
        Dictionary<string, Dictionary<int, double>>? offers = new Dictionary<string, Dictionary<int, double>>();

        offers.Add("A", new Dictionary<int, double>(){{3, 130}});
        offers.Add("B", new Dictionary<int, double>(){{2, 45}});
        
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
        Dictionary<string, Dictionary<int, double>>? offers = new Dictionary<string, Dictionary<int, double>>();

        offers.Add("A", new Dictionary<int, double>(){{3, 130}, {5, 200}});
        offers.Add("B", new Dictionary<int, double>(){{2, 45}, {4, 65}});
        
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
    
    [Test]
    public void ShouldReturnTotal_WithMultipleOffers_WhereOffersAreAppliedMultipleTimes()
    {
        Dictionary<string, Dictionary<int, double>>? offers = new Dictionary<string, Dictionary<int, double>>();

        offers.Add("A", new Dictionary<int, double>(){{3, 130}, {5, 200}});
        offers.Add("B", new Dictionary<int, double>(){{2, 45}, {4, 65}});
        
        var cart = new List<Item>
        {
            new("A", 50, 15),
            new("B", 30, 6),
            new("C", 20, 2)
        };
        
        var sut = new Checkout(offers, cart);
        var total = sut.Total;
        
        
        Assert.That(total, Is.EqualTo(200+200+200+45+65+40));
    }
    
}

public class Offer(Item item, int quantity, double price)
{
    public Item Product { get; set; } = item;

    public int Quantity { get; set; } = quantity;

    public double Price { get; set; } = price;
}

public class Item(string sku, double price, int quantity)
{
    public string Sku { get; set; } = sku;

    public double Price { get; set; } = price;

    public int Quantity { get; set; } = quantity;
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
            CalculateTotal(offers, item);
        }
    }

    private void CalculateTotal(Dictionary<string, Dictionary<int, double>> offers, Item item)
    {
        Total += item.Quantity * item.Price;
        ApplyOffer(offers, item);
    }

    private void ApplyOffer(Dictionary<string, Dictionary<int, double>> offers,
        Item item)
    {
        if (!offers.TryGetValue(item.Sku, out var itemLevelOffers)) return;
        var itemQuantity = item.Quantity;
        var minOfferQuantity = itemLevelOffers.Where(o => o.Key <= itemQuantity).MinBy(o => o.Key).Key;
        while (itemQuantity >= minOfferQuantity)
        {
            var offerQuantity = itemLevelOffers.Where(o => o.Key <= itemQuantity).MaxBy(o => o.Key).Key;
            itemQuantity -= offerQuantity;
            Total -= offerQuantity*item.Price - offers[item.Sku][offerQuantity];
        }
    }
}
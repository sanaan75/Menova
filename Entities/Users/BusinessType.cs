namespace Entities.Users;

[Flags]
public enum BusinessType
{
    Cafe = 1,
    Restaurant = 2,
    CafeRestaurant = 4,
    FastFood = 8,
    Bakery = 16,
    CoffeeShop = 32,
    Hotel = 64,
    Bar = 128,
    NightClub = 256,
    FoodTruck = 512,
    Catering = 1024,
    PastryShop = 2048,
    IceCreamShop = 4096,
    JuiceBar = 8192,
    TeaHouse = 16384,
    All = Cafe | Restaurant | CafeRestaurant | FastFood | Bakery | CoffeeShop | Hotel | Bar | NightClub | FoodTruck | Catering | PastryShop | IceCreamShop | JuiceBar | TeaHouse
}
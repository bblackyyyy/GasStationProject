namespace WebApplication2
{
    public class Customer
    {
        public FuelType fuel_type { get; set; }
        public int amount { get; set; }

        public float get_price(float tax, float price_per_liter)
        {
            return (tax + price_per_liter) *  amount;
        }
        
        
    }
}


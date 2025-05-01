using Supabase;
using DotNetEnv;
using static WebApplication2.FuelType;
using System.Linq;
using System.Threading.Tasks;using Postgrest;
using Postgrest.Attributes;
using Postgrest.Models;
using Client = Supabase.Client;


namespace WebApplication2
{
    
    public class InventoryItem : BaseModel
    {
        [Column("fuel_name")]
        public string FuelName { get; set; }

        [Column("available")]
        public float Available { get; set; }
        
        [Column("price")]
        public float Price { get; set; }

        [Column("tax")]
        public float Tax { get; set; }
    }
    
    public class DB_Connection

    {
        private Client _client;

        public DB_Connection()

        {
            DotNetEnv.Env.Load();
            var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL");
            var supabaseKey = Environment.GetEnvironmentVariable("SUPABASE_KEY");
            
            _client = new Client(supabaseUrl, supabaseKey);
        }


        public async Task Start()
        {
            await _client.InitializeAsync();
        }

        public async Task<bool> CheckAvaible(FuelType fuel, float amount)
        {
            var item = await _client.From<InventoryItem>().Where(i => i.FuelName == fuel.ToString()).Single();

            if (item is null)
            {
                throw new InvalidOperationException($"No inventory record for {fuel}");
            }

            return amount <= item.Available;
        }

        
        public async Task<float> GetAvailable(FuelType fuel)
        {
            var item = await _client
                .From<InventoryItem>().Where(i => i.FuelName == fuel.ToString()).Single();

            return item?.Available ?? 0f;
        }

        
        public async Task ReduceInventory(FuelType fuel, float amount)
        {
            
            var current = await GetAvailable(fuel);
            if (amount > current)
                throw new InvalidOperationException("Not enough fuel to subtract.");

            
            var updated = new InventoryItem
            {
                FuelName = fuel.ToString(),
                Available = current - amount
            };

            
            await _client
                .From<InventoryItem>()
                .Update(updated);
        }

        
        public async Task<float> GetPrice(FuelType fuel)
        {
            var item = await _client.From<InventoryItem>().Where(i => i.FuelName == fuel.ToString()).Single();

            return item?.Price ?? 0f;
        }

        
        public async Task<float> GetTax(FuelType fuel)
        {
            var item = await _client.From<InventoryItem>().Where(i => i.FuelName == fuel.ToString()).Single();

            return item?.Tax ?? 0f;
        }


        private int hel = 1;

    }
}
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Supabase;
using Supabase.Postgrest;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Exceptions;
using Supabase.Postgrest.Models;
using DotNetEnv;

namespace WebApplication2
{
    
    [Table("petrol_type")]
    public class PetrolType : BaseModel
    {
        [PrimaryKey("id", false)] public int Id { get; set; }

        [Column("petrol_name")] public string FuelName { get; set; }

        [Column("price_per_liter")] public float Price { get; set; }
        
        [Column("tax")] public float fuel_tax { get; set; }
    }
    
    
    
    
    [Table("inventory")]
    public class Inventory : BaseModel
    {
        
        [PrimaryKey("inventory_id", false)]
        public long InventoryId { get; set; }

        [Column("petrol_name")]
        public string PetrolName { get; set; }

        [Column("last_restock_date")]
        public DateTime LastRestockDate { get; set; }

        [Column("gasstation_id")]
        public long GasStationId { get; set; }

        [Column("available")]
        public float Available { get; set; }
    }
    
    
    [Table("transaction")]
    public class Transact : BaseModel
    {
        
        [PrimaryKey("transaction_id", false)]
        public long TransactionId { get; set; }

        
        [Column("data_time")]
        public DateTime DataTime { get; set; }

        
        [Column("quantity_liters")]
        public float QuantityLiters { get; set; }

        
        [Column("money")]
        public float Money { get; set; }

        
        [Column("fuel_type")]
        public string FuelType { get; set; }

        
        [Column("station_id")]
        public long StationId { get; set; }
    }

    
    

    
    public class DB_Connection
    {
        
        
        private readonly string _supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL");

        private readonly string _supabaseKey =Environment.GetEnvironmentVariable("SUPABASE_KEY");
            

        private Supabase.Client _supabaseClient;
        private Task _initializationTask;

        public DB_Connection()
        {
            if (string.IsNullOrWhiteSpace(_supabaseUrl) ||
                string.IsNullOrWhiteSpace(_supabaseKey))
                throw new InvalidOperationException("Supabase URL or Key are empty.");

            
            _initializationTask = InitializeSupabaseClient();
        }

        private async Task InitializeSupabaseClient()
        {
            var options = new Supabase.SupabaseOptions { AutoConnectRealtime = true };
            _supabaseClient = new Supabase.Client(_supabaseUrl, _supabaseKey, options);
            await _supabaseClient.InitializeAsync();
            Console.WriteLine("Supabase client initialized.");
        }

        
        public async Task<float> GetPrice(string fuel)
        {
            
            await _initializationTask;

            if (_supabaseClient == null)
            {
                Console.Error.WriteLine("Supabase client not initialized.");
                return 0f;
            }

            
            
                var trimmed = fuel?.Trim() ?? "";
                var response = await _supabaseClient
                    .From<PetrolType>()
                    
                    .Select(x => new object[] { x.Price })
                    
                    .Filter("petrol_name", Constants.Operator.ILike, trimmed)
                    .Get();

               
                return response.Models.FirstOrDefault()?.Price ?? 0f;
            
            
        }
        
        
        public async Task<float> GetTax(string fuel)
        {
            
            await _initializationTask;

            if (_supabaseClient == null)
            {
                Console.Error.WriteLine("Supabase client not initialized.");
                return 0f;
            }

            
            
            var trimmed = fuel?.Trim() ?? "";
            var response = await _supabaseClient
                .From<PetrolType>()
                    
                .Select(x => new object[] { x.fuel_tax })
                    
                .Filter("petrol_name", Constants.Operator.ILike, trimmed)
                .Get();

               
            return response.Models.FirstOrDefault()?.fuel_tax ?? 0f;
            
            
        }
        
        
        
        
        public async Task<float> GetAvailable(int stationId, string fuel)
        {
            
            await _initializationTask;

            if (_supabaseClient == null)
            {
                Console.Error.WriteLine("Supabase client not initialized.");
                return 0f;
            }

            var trimmedFuel = fuel?.Trim() ?? "";

            
                
                var response = await _supabaseClient
                    .From<Inventory>()
                    
                    .Select(x => new object[] { x.Available })
                    
                    .Filter("gasstation_id", Constants.Operator.Equals, stationId)
                    
                    .Filter("petrol_name", Constants.Operator.ILike, trimmedFuel)
                    .Get();


                return (float)response.Models.FirstOrDefault()?.Available;
            
            
        }
        
        
        
        public async Task<float> SetAvailable(int stationId, string fuel, float newAmount)
        {
            await _initializationTask;
            if (_supabaseClient == null) return 0f;

            var trimmedFuel = fuel?.Trim() ?? "";

            
            var fullRecord = await _supabaseClient
                .From<Inventory>()
                .Select("*")   
                .Filter("gasstation_id", Constants.Operator.Equals, stationId)
                .Filter("petrol_name",  Constants.Operator.ILike,   trimmedFuel)
                .Single();

            if (fullRecord == null)
                return 0f;

            float old  = fullRecord.Available;
            float new_am = old - newAmount;
            fullRecord.Available = new_am;

           
            var updateResponse = await _supabaseClient
                .From<Inventory>()
                .Update(fullRecord);

            return updateResponse.Models?.FirstOrDefault()?.Available ?? 0f;
        }


        public async Task<long> SetTransaction(long stationId, string fuel, float litres, float money, DateTime date)
        {
            
            await _initializationTask;
            if (_supabaseClient == null)
            {
                Console.Error.WriteLine("Supabase client not initialized.");
                return 0;
            }

            
            var txn = new Transact
            {
                StationId     = stationId,
                DataTime      = date,
                QuantityLiters= litres,
                Money         = money,
                FuelType      = fuel?.Trim() ?? ""
            };

            
            var response = await _supabaseClient
                .From<Transact>()
                .Insert(txn);

            
            return response.Models?.FirstOrDefault()?.TransactionId ?? 0L;
        }


        





        
        
        
        
        
    }
}
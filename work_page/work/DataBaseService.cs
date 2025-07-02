using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Supabase;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Linq;
using DotNetEnv;



namespace work
{
    internal class DataBaseService
    {
        private readonly Supabase.Client _supabaseClient;

        private static readonly string SupabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL")
            ?? throw new Exception("Missing SUPABASE_URL environment variable");

        private static readonly string SupabaseKey = Environment.GetEnvironmentVariable("SUPABASE_KEY")
            ?? throw new Exception("Missing SUPABASE_KEY environment variable");

        public DataBaseService()
        {
            _supabaseClient = new Supabase.Client(SupabaseUrl, SupabaseKey);
            _supabaseClient.InitializeAsync().Wait();
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                var session = await _supabaseClient.Auth.SignIn(email, password);
                return session != null && !string.IsNullOrEmpty(session.AccessToken);
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Inventory>> GetInventoryAsync()
        {
            var response = await _supabaseClient
                .From<Inventory>()
                .Get();
            return response.Models;
        }

        public async Task<List<PetrolType>> GetPetrolTypesAsync()
        {
            var response = await _supabaseClient
                .From<PetrolType>()
                .Get();
            return response.Models;
        }

        public async Task UpdateInventoryAsync(Inventory inventory)
        {
            await _supabaseClient
                .From<Inventory>()
                .Where(i => i.inventory_id == inventory.inventory_id)
                .Set(i => i.available, inventory.available)
                .Update();
        }
        public async Task UpdatePetrolTypeAsync(PetrolType petrolType)
        {
            await _supabaseClient
                .From<PetrolType>()
                .Update(petrolType);
        }
        public async Task<List<MonitoringRow>> GetMonitoringRowsAsync()
        {
            var transaction = (await _supabaseClient
                .From<Transaction>()
                .Get()).Models;

            var grouped = transaction
                .GroupBy(t => new { t.StationId, t.PumpId })
                .Select(g => new MonitoringRow
                {
                    StationId = g.Key.StationId,
                    PumpId = g.Key.PumpId,
                    Amount = g.Sum(x => x.QuantityLiters),
                    Income = g.Sum(x => (double)x.Money),
                    Date = g.Max(x => x.DataTime)
                })
                .OrderBy(r => r.StationId)
                .ThenBy(r => r.PumpId)
                .ToList();

            return grouped;
        }

        public async Task<List<Transaction>> GetTransactionsByGroupAsync(long stationId, long pumpId)
        {
            var transactions = (await _supabaseClient
                .From<Transaction>()
                .Where(t => t.StationId == stationId && t.PumpId == pumpId)
                .Get()).Models;

            return transactions;
        }

        public async Task<List<Transaction>> GetTransactionsAsync(DateTime fromDate, DateTime toDate)
        {
            return (await _supabaseClient
                .From<Transaction>()
                .Where(t => t.DataTime >= fromDate && t.DataTime <= toDate)
                .Get()).Models;
        }
    }

    [Table("inventory")]
    public class Inventory : BaseModel
    {
        public int inventory_id { get; set; }
        public string petrol_name { get; set; } = string.Empty;
        [Column("available")]
        public double available { get; set; }
        public double max { get; set; }
    }

    [Table("petrol_type")]
    public class PetrolType : BaseModel
    {
        [PrimaryKey("id", false)]
        public int id { get; set; }
        public string petrol_name { get; set; } = string.Empty;
        [Column("price_per_liter")]
        public double price_per_liter { get; set; }
        [Column("tax")]
        public double tax { get; set; }
    }

    public class MonitoringRow
    {
        public long? StationId { get; set; }
        public long? PumpId { get; set; }
        public double Amount { get; set; }
        public double Income { get; set; }
        public DateTime Date { get; set; }
    }

    [Table("transaction")]
    public class Transaction : BaseModel
    {
        [Column("transaction_id")]
        public long TransactionId { get; set; }
        [Column("data_time")]
        public DateTime DataTime { get; set; }
        [Column("quantity_liters")]
        public double QuantityLiters { get; set; }
        [Column("money")]
        public decimal Money { get; set; }
        [Column("fuel_type")]
        public string FuelType { get; set; }
        [Column("station_id")]
        public long? StationId { get; set; }
        [Column("pump_id")]
        public long? PumpId { get; set; }
    }
}
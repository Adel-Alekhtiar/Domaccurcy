using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Domaccurcy.Core.Models;
using Newtonsoft.Json;

namespace Domaccurcy.Core.Services
{
    public class SnapshotStorage
    {
        private readonly string _snapshotsDir;

        public SnapshotStorage(string snapshotsDir = "snapshots")
        {
            _snapshotsDir = snapshotsDir;
            Directory.CreateDirectory(_snapshotsDir);
        }

        public async Task SaveAsync(DomSnapshot snapshot)
        {
            var domain = GetDomain(snapshot.Url);
            var timestamp = snapshot.Timestamp.ToString("yyyyMMdd_HHmmss");
            var filename = Path.Combine(_snapshotsDir, $"{domain}_{timestamp}.json");

            var json = JsonConvert.SerializeObject(snapshot, Formatting.Indented);
            await File.WriteAllTextAsync(filename, json);

            Console.WriteLine($"Snapshot saved: {filename}");
        }

        public async Task<DomSnapshot?> LoadLatestAsync(string url)
        {
            var domain = GetDomain(url);
            var files = Directory.GetFiles(_snapshotsDir, $"{domain}_*.json")
                .OrderByDescending(f => f)
                .ToList();

            if (!files.Any())
                return null;

            var json = await File.ReadAllTextAsync(files[0]);
            return JsonConvert.DeserializeObject<DomSnapshot>(json);
        }

        private static string GetDomain(string url)
        {
            try
            {
                var uri = new Uri(url);
                var host = uri.Host.ToLowerInvariant();
                var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(host)))[..8];
                return $"{host.Replace(".", "_").Replace("-", "_")}_{hash}";
            }
            catch
            {
                return "unknown";
            }
        }
    }
}

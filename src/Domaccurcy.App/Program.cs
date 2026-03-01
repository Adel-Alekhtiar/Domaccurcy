using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domaccurcy.Core.Models;
using Domaccurcy.Core.Services;

namespace Domaccurcy.App
{
    class Program
    {
        private static readonly string parentDirectory = Directory.GetCurrentDirectory() + "\\snapshots";
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Domaccurcy - AI-Based Website DOM Change Detection ===");
            Console.WriteLine();

            string url;
            if (args.Length > 0)
            {
                url = args[0];
            }
            else
            {
                Console.Write("Enter URL to analyze: ");
                url = Console.ReadLine()?.Trim() ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                Console.WriteLine("Error: URL cannot be empty.");
                return;
            }

            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                url = "https://" + url;

            Console.WriteLine($"\nAnalyzing: {url}");

            var loader = new WebsiteLoader();
            var extractor = new DomExtractor();
            var storage = new SnapshotStorage(parentDirectory);
            var detector = new ChangeDetector();

            // Load previous snapshot
            Console.WriteLine("Checking for previous snapshot...");
            var previousSnapshot = await storage.LoadLatestAsync(url);

            // Load current page
            Console.WriteLine("Loading page (this may take a moment)...");
            DomSnapshot currentSnapshot;
            try
            {
                currentSnapshot = await loader.LoadAsync(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading page: {ex.Message}");
                return;
            }

            // Extract DOM nodes
            Console.WriteLine("Extracting DOM nodes...");
            currentSnapshot.Nodes = extractor.Extract(currentSnapshot.HtmlContent);

            Console.WriteLine($"Extracted {currentSnapshot.Nodes.Count} root nodes.");

            if (previousSnapshot == null)
            {
                Console.WriteLine("\nThis is the first snapshot for this domain.");
                Console.WriteLine("No changes to detect yet.");
            }
            else
            {
                Console.WriteLine($"\nPrevious snapshot from: {previousSnapshot.Timestamp:yyyy-MM-dd HH:mm:ss} UTC");
                Console.WriteLine("Detecting changes...");

                var changes = detector.Detect(previousSnapshot, currentSnapshot);

                if (!changes.Any())
                {
                    Console.WriteLine("No changes detected.");
                }
                else
                {
                    Console.WriteLine($"\nDetected {changes.Count} change(s):");

                    var added = changes.Count(c => c.ChangeType == Domaccurcy.Core.Models.ChangeType.Added);
                    var removed = changes.Count(c => c.ChangeType == Domaccurcy.Core.Models.ChangeType.Removed);
                    var modified = changes.Count(c => c.ChangeType == Domaccurcy.Core.Models.ChangeType.Modified);

                    Console.WriteLine($"  Added:    {added}");
                    Console.WriteLine($"  Removed:  {removed}");
                    Console.WriteLine($"  Modified: {modified}");

                    Console.WriteLine("\nDetails (first 20 changes):");
                    foreach (var change in changes.Take(20))
                    {
                        var nodeInfo = change.NewNode ?? change.OldNode;
                        Console.WriteLine($"  [{change.ChangeType}] {change.XPath}" +
                            (nodeInfo?.TagName != null ? $" <{nodeInfo.TagName}>" : ""));
                    }
                }
            }

            // Save new snapshot
            Console.WriteLine("\nSaving snapshot...");
            await storage.SaveAsync(currentSnapshot);

            Console.WriteLine("\nDone.");
        }
    }
}

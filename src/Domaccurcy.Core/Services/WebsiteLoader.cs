using System;
using System.Threading.Tasks;
using Domaccurcy.Core.Models;
using PuppeteerSharp;

namespace Domaccurcy.Core.Services
{
    public class WebsiteLoader
    {
        private static readonly Task _browserReady = new BrowserFetcher().DownloadAsync();

        public async Task<DomSnapshot> LoadAsync(string url)
        {
            await _browserReady;

            var launchOptions = new LaunchOptions
            {
                Headless = true,
                Args = new[]
                {
                    "--no-sandbox",
                    "--disable-setuid-sandbox",
                    "--disable-blink-features=AutomationControlled",
                    "--disable-infobars",
                    "--window-size=1920,1080"
                }
            };

            await using var browser = await Puppeteer.LaunchAsync(launchOptions);
            await using var page = await browser.NewPageAsync();

            await page.SetUserAgentAsync(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");

            await page.EvaluateExpressionOnNewDocumentAsync(StealthPlugin.GetStealthScript());

            await page.GoToAsync(url, new NavigationOptions
            {
                WaitUntil = new[] { WaitUntilNavigation.Networkidle2 },
                Timeout = 30000
            });

            var html = await page.GetContentAsync();

            var css = await page.EvaluateFunctionAsync<string>(@"() => {
                let cssContent = '';

                // Inline styles from elements
                const elements = document.querySelectorAll('[style]');
                elements.forEach(el => {
                    cssContent += el.getAttribute('style') + '\n';
                });

                // Style tag content
                const styleTags = document.querySelectorAll('style');
                styleTags.forEach(tag => {
                    cssContent += tag.textContent + '\n';
                });

                // External stylesheet content
                const sheets = Array.from(document.styleSheets);
                sheets.forEach(sheet => {
                    try {
                        const rules = Array.from(sheet.cssRules || []);
                        rules.forEach(rule => {
                            cssContent += rule.cssText + '\n';
                        });
                    } catch (e) {
                        // Cross-origin stylesheets may throw
                    }
                });

                return cssContent;
            }");

            return new DomSnapshot
            {
                Url = url,
                Timestamp = DateTime.UtcNow,
                HtmlContent = html,
                CssContent = css ?? string.Empty
            };
        }
    }
}

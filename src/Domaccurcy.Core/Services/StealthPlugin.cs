namespace Domaccurcy.Core.Services
{
    public static class StealthPlugin
    {
        public static string GetStealthScript()
        {
            return @"
// Override webdriver
Object.defineProperty(navigator, 'webdriver', {
    get: () => undefined,
    configurable: true
});

// Override plugins to appear non-empty
Object.defineProperty(navigator, 'plugins', {
    get: () => {
        const plugins = [
            { name: 'Chrome PDF Plugin', filename: 'internal-pdf-viewer', description: 'Portable Document Format' },
            { name: 'Chrome PDF Viewer', filename: 'mhjfbmdgcfjbbpaeojofohoefgiehjai', description: '' },
            { name: 'Native Client', filename: 'internal-nacl-plugin', description: '' }
        ];
        plugins.refresh = () => {};
        plugins.item = (i) => plugins[i];
        plugins.namedItem = (name) => plugins.find(p => p.name === name) || null;
        Object.setPrototypeOf(plugins, PluginArray.prototype);
        return plugins;
    },
    configurable: true
});

// Override languages
Object.defineProperty(navigator, 'languages', {
    get: () => ['en-US', 'en'],
    configurable: true
});

// Randomize screen dimensions slightly
const screenWidth = 1920 + Math.floor(Math.random() * 80 - 40);
const screenHeight = 1080 + Math.floor(Math.random() * 80 - 40);
Object.defineProperty(screen, 'width', { get: () => screenWidth, configurable: true });
Object.defineProperty(screen, 'height', { get: () => screenHeight, configurable: true });
Object.defineProperty(screen, 'availWidth', { get: () => screenWidth, configurable: true });
Object.defineProperty(screen, 'availHeight', { get: () => screenHeight - 40, configurable: true });

// Override permissions
const originalQuery = window.navigator.permissions.query;
window.navigator.permissions.query = (parameters) =>
    parameters.name === 'notifications'
        ? Promise.resolve({ state: Notification.permission })
        : originalQuery(parameters);
";
        }
    }
}

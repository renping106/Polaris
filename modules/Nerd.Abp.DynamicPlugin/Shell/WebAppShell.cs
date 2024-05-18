using Microsoft.AspNetCore.Builder;

namespace Nerd.Abp.DynamicPlugin.Shell
{
    internal static class WebAppShell
    {
        private static WebAppCache? _webAppCache;
        private static readonly object instanceLock = new object();

        public static WebAppCache GetShell(Func<bool, ValueTask<WebApplicationBuilder>> builderInit)
        {
            if (_webAppCache == null)
            {
                lock (instanceLock)
                {
                    if (_webAppCache == null)
                    {
                        _webAppCache = InitShellHostAsync(builderInit).GetAwaiter().GetResult();
                    }
                }
            }

            return _webAppCache!;
        }

        public static async ValueTask<(bool Success, string Message)> UpdateShellHostAsync()
        {
            try
            {
                var builderInit = _webAppCache!.BuilderInit;
                var newShell = await InitShellHostAsync(builderInit, true);
                if (newShell != null)
                {
                    _webAppCache = newShell;
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
            return (true, string.Empty);
        }

        private static async ValueTask<WebAppCache> InitShellHostAsync(
            Func<bool, ValueTask<WebApplicationBuilder>> builderInit,
            bool loadPlugins = false)
        {
            var shellAppBuilder = await builderInit(loadPlugins);

            var shellApp = shellAppBuilder.Build();

            Action<IApplicationBuilder> configure = async (builder) =>
            {
                await shellApp.InitializeApplicationAsync();
            };

            configure(shellApp);

            // Build the request pipeline.
            var requestDelegate = ((IApplicationBuilder)shellApp).Build();

            return new WebAppCache(shellApp.Services, requestDelegate, builderInit);
        }
    }
}

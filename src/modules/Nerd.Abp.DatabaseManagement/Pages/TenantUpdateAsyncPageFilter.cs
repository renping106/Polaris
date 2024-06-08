using Microsoft.AspNetCore.Mvc.Filters;
using Nerd.Abp.DatabaseManagement.Services.Interfaces;
using Volo.Abp.DependencyInjection;

namespace Ping.Nerd.Web.Filters
{
    public class TenantUpdateAsyncPageFilter : IAsyncPageFilter, ITransientDependency
    {
        private readonly ITenantUpdateAppService _updateAppService;

        public TenantUpdateAsyncPageFilter(ITenantUpdateAppService updateAppService)
        {
            _updateAppService = updateAppService;
        }

        public async Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            if (await _updateAppService.HasUpdatesAsync())
            {
                await _updateAppService.UpdateDatabaseAsync();
            }            
        }

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context,
                                                      PageHandlerExecutionDelegate next)
        {
            // Do post work.
            await next.Invoke();
        }
    }
}

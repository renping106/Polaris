﻿using Microsoft.AspNetCore.Mvc.Filters;
using Polaris.Abp.DatabaseManagement.Services.Interfaces;
using Volo.Abp.DependencyInjection;

namespace Ping.Polaris.Web.Filters;

public class TenantUpdateAsyncPageFilter(ITenantUpdateAppService updateAppService) : IAsyncPageFilter, ITransientDependency
{
    private readonly ITenantUpdateAppService _updateAppService = updateAppService;

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

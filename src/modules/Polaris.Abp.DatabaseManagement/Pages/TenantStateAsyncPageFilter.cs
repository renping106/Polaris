﻿using Microsoft.AspNetCore.Mvc.Filters;
using Polaris.Abp.DatabaseManagement.Services.Interfaces;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Ping.Polaris.Web.Filters
{
    public class TenantStateAsyncPageFilter : IAsyncPageFilter, ITransientDependency
    {
        private readonly ISetupAppService _setupAppService;
        private readonly ICurrentTenant _currentTenant;
        private static readonly string _setupPath = "/Setup/Install";

        public TenantStateAsyncPageFilter(ISetupAppService setupAppService,
            ICurrentTenant currentTenant)
        {
            _setupAppService = setupAppService;
            _currentTenant = currentTenant;
        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            if (!_setupAppService.IsInitialized(_currentTenant.Id)
                && context.HttpContext.Request.Path.Value?.IndexOf(_setupPath) < 0)
            {
                var queryString = "";
                if (_currentTenant.Id.HasValue)
                {
                    queryString = $"?tenant={_currentTenant.Id}";
                }
                context.HttpContext.Response.Redirect(_setupPath + queryString);
            }

            return Task.CompletedTask;
        }

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context,
                                                      PageHandlerExecutionDelegate next)
        {
            // Do post work.
            await next.Invoke();
        }
    }
}
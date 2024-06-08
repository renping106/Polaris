﻿using Microsoft.EntityFrameworkCore;
using Volo.Abp.DependencyInjection;

namespace Nerd.Abp.Host.Data;

public class HostEFCoreDbSchemaMigrator : ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public HostEFCoreDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the HostDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<HostDbContext>()
            .Database
            .MigrateAsync();
    }
}

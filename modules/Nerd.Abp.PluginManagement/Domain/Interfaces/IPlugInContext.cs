﻿using System.Runtime.Loader;

namespace Nerd.Abp.PluginManagement.Domain.Interfaces
{
    public interface IPlugInContext
    {
        AssemblyLoadContext? Context { get; }
        void UnloadContext();
        IReadOnlyList<Type> DbContextTypes { get; }
    }
}

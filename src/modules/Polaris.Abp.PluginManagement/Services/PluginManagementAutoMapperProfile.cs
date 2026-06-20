using Polaris.Abp.PluginManagement.Domain.Interfaces;
using Polaris.Abp.PluginManagement.Services.Dtos;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Polaris.Abp.PluginManagement.Services;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class  PlugInDescriptorToPlugInDescriptorDtoMapper : MapperBase<IPlugInDescriptor, PlugInDescriptorDto>
{
    public override partial PlugInDescriptorDto Map(IPlugInDescriptor source);

    public override partial void Map(IPlugInDescriptor source, PlugInDescriptorDto destination);
}

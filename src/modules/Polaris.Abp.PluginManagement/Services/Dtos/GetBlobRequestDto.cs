﻿using System.ComponentModel.DataAnnotations;

namespace Polaris.Abp.PluginManagement.Services.Dtos;

public class GetBlobRequestDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
}

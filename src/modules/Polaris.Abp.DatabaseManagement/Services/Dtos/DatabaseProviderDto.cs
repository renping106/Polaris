namespace Polaris.Abp.DatabaseManagement.Services.Dtos
{
    public class DatabaseProviderDto
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public bool HasConnectionString { get; set; }
        public string SampleConnectionString { get; set; } = string.Empty;
    }
}

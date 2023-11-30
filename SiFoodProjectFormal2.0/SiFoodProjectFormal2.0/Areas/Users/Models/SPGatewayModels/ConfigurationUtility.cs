namespace SiFoodProjectFormal2._0.Areas.Users.Models.SPGatewayModels;
using Microsoft.Extensions.Configuration;


public class ConfigurationUtility
{
    private readonly IConfiguration _configuration;

    public ConfigurationUtility(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetAppSetting(string appSettingKey)
    {
        string result = string.Empty;

        if (!string.IsNullOrWhiteSpace(_configuration[appSettingKey]))
        {
            result = _configuration[appSettingKey].Trim();
        }

        return result;
    }
}




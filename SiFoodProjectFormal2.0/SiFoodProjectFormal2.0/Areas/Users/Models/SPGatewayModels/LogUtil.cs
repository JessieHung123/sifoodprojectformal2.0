namespace SiFoodProjectFormal2._0.Areas.Users.Models.SPGatewayModels
{
    public class LogUtil
    {
        private readonly ConfigurationUtility _configurationUtility;
        private readonly PathUtil _pathUtil;

        public LogUtil(ConfigurationUtility configurationUtility, PathUtil pathUtil)
        {
            _configurationUtility = configurationUtility;
            _pathUtil = pathUtil;
        }
        public void WriteLog(string msg)
        {
            try
            {
                string logDirectory = _pathUtil.MapPath(_configurationUtility.GetAppSetting("LogDirectory"));

                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                string nowString = DateTime.Now.ToString("yyyyMMdd");
                string logFile = Path.Combine(logDirectory, $"log_{nowString}.txt");

                File.AppendAllText(logFile, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {msg}{Environment.NewLine}");
            }
            catch (Exception)
            {
                // Handle the exception
            }
        }

        public void WriteLog(Exception exception)
        {
            try
            {
                string logDirectory = _pathUtil.MapPath(_configurationUtility.GetAppSetting("LogDirectory"));

                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                string nowString = DateTime.Now.ToString("yyyyMMdd");
                string logFile = Path.Combine(logDirectory, $"log_{nowString}.txt");

                File.AppendAllText(logFile, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {exception.Message}{Environment.NewLine}{exception.StackTrace}{Environment.NewLine}");
            }
            catch
            {
                // Handle the exception
            }
        }
    }
}



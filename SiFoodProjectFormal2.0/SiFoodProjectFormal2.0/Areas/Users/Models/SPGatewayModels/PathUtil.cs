using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting.Internal;

namespace SiFoodProjectFormal2._0.Areas.Users.Models.SPGatewayModels
{
    public class PathUtil
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private HostingEnvironment hostingEnvironment;

        public PathUtil(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public PathUtil(HostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        public string MapPath(string path)
        {
            if (_hostingEnvironment.ContentRootPath != null)
            {
                // Combine the content root path with the specified path
                string combinedPath = Path.Combine(_hostingEnvironment.ContentRootPath, path);
                return Path.GetFullPath(combinedPath);
            }
            return path;
        }
    }

}


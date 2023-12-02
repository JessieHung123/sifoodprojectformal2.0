using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiFoodProjectFormal2._0.Areas.Drivers.NewFolder3;
using SiFoodProjectFormal2._0.Models;

namespace SiFoodProjectFormal2._0.Areas.Drivers.Controllers
{
    [Route("api/DeliveryOrderapi/[action]")]

    [Area("Drivers")]
    public class DeliveryOrderapiController:Controller
    {
        private readonly Sifood3Context _context;

        public DeliveryOrderapiController(Sifood3Context context)
        {
            _context = context;

        }
        public object WaitForDeliveryOrderSimple()
        {

            //Distance 前端算後端算?
            return _context.Orders.Where(o => o.StatusId == 2 && o.DeliveryMethod=="外送").Include(o => o.User).Include(o => o.Store).Select(o => new DeliveryOrderVM
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate.ToString(),
                Address = o.Address,
                StoreName = o.Store.StoreName,
                StoreAddress=o.Store.Address,
                UserName = o.User.UserName,
                Latitude=(decimal)o.Store.Latitude,
                Longitude= (decimal)o.Store.Longitude
            });

            
        }
        //public object WaitForDeliveryOrderDetails()
        //{
        //    //return _context.Orders.Where(o => o.StatusId == 2).Include(o => o.User).Include(o => o.Store).Include(x => x.OrderDetails).ThenInclude(x => x.Product).Select(o => new

        //    //)
        //}

    }
}

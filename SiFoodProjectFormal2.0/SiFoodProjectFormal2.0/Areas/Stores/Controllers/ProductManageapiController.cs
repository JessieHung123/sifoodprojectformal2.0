using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiFoodProjectFormal2._0.Areas.Stores.ViewModels;
using SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels;
using SiFoodProjectFormal2._0.Models;

namespace SiFoodProjectFormal2._0.Areas.Stores.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductManageapiController : ControllerBase
    {
        private readonly Sifood3Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IStoreIdentityService _storeIdentityService;

       public ProductManageapiController(Sifood3Context context, IWebHostEnvironment webHostEnvironment, IStoreIdentityService storeIdentityService)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _storeIdentityService = storeIdentityService;
        }


        public async Task<List<ProductManageVM>> GetAll()
        {
            string targetStoreId = _storeIdentityService.GetStoreId();
            return await _context.Products
                .Include(p => p.Category)
                .Where(e => e.StoreId == targetStoreId && e.IsDelete==1).Select(x => new ProductManageVM
                {
                    StoreId = x.StoreId,
                    UnitPrice = x.UnitPrice,
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category.CategoryName,
                    ReleasedQty = x.ReleasedQty,
                    OrderedQty = x.OrderedQty,
                    PhotoPath = x.PhotoPath,
                    Description = x.Description,
                    RealeasedTime = x.RealeasedTime,
                    SuggestPickUpTime= x.SuggestPickUpTime,
                    SuggestPickEndTime = x.SuggestPickEndTime,
                }).ToListAsync();
        }

        public async Task<List<ProductManageVM>> Filter(string? text)
        {
            string targetStoreId = _storeIdentityService.GetStoreId();
            var query = _context.Products.Include(p => p.Category).Where(e => e.StoreId == targetStoreId && e.IsDelete ==1);

            if (!string.IsNullOrEmpty(text))
            {
                query = query.Where(e => e.ProductName.Contains(text) || e.Category.CategoryName.Contains(text));
            }
            return await query.Select(x => new ProductManageVM
            {
                StoreId = x.StoreId,
                UnitPrice = x.UnitPrice,
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                CategoryId = x.CategoryId,
                CategoryName = x.Category.CategoryName,
                ReleasedQty = x.ReleasedQty,
                OrderedQty = x.OrderedQty,
                PhotoPath = x.PhotoPath,
                Description = x.Description,
                RealeasedTime = x.RealeasedTime,
                SuggestPickUpTime = x.SuggestPickUpTime,
                SuggestPickEndTime = x.SuggestPickEndTime,
            }).ToListAsync();
        }


        [HttpPut("{id}")]
        public async Task<string> SoftDelete(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return "找不到產品！";
            }

            product.IsDelete = 0;
            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return "更新成功！";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return "找不到產品！";
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpDelete("{productId}")]
    public async Task<string> deleteProduct(int productId)
    {
        if (_context.Products == null)
        {
            return "刪除商品失敗!";
        }
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
        {
            return "刪除商品失敗!";
        }
        try
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

        }
        catch (DbUpdateException)
        {
            return "刪除商品關聯紀錄失敗!";
        }
        return "刪除商品成功!";
    }

    private bool ProductExists(int productId)
        {
            return (_context.Products?.Any(e => e.ProductId == productId)).GetValueOrDefault();
        }


        [HttpPost]
        public async Task<string> postProduct([FromForm]AddProductVM addProductDTO)
        {
            string targetStoreId = _storeIdentityService.GetStoreId();

            Product prodct = new Product
            {
                StoreId = targetStoreId,
                ProductName = addProductDTO.ProductName,
                CategoryId = addProductDTO.CategoryId,
                Description = addProductDTO.Description,
                ReleasedQty = addProductDTO.ReleasedQty,
                UnitPrice = addProductDTO.UnitPrice,
                SuggestPickUpTime=addProductDTO.SuggestPickUpTime,
                SuggestPickEndTime=addProductDTO.SuggestPickEndTime,
                RealeasedTime = DateTime.Now,
                PhotoPath = await SavePhoto(addProductDTO.ImageFile)
            };
            _context.Products.Add(prodct);
            await _context.SaveChangesAsync();
            return "新增成功";
        }

        //儲存照片專用方法
        private async Task<string> SavePhoto(IFormFile photo)
        {
            if (photo != null)
            {
                var fileName = Path.GetFileName(photo.FileName);
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Products", fileName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                return $"/images/Products/{fileName}";
            }
            return null;
        }

        [HttpPut("{id}")]
        public async Task<string> putProduct(int id,[FromForm] PutProductVM putProductVM)
        {
            string targetStoreId = _storeIdentityService.GetStoreId();

            if (id != putProductVM.ProductId)
            {
                return "修改商品失敗!";
            }
            Product product = await _context.Products.FindAsync(id);
            product.StoreId = targetStoreId;
            product.ProductName = putProductVM.ProductName;
            product.CategoryId = putProductVM.CategoryId;
            product.Description = putProductVM.Description;
            product.ReleasedQty = putProductVM.ReleasedQty;
            product.UnitPrice = putProductVM.UnitPrice;
            product.SuggestPickUpTime = putProductVM.SuggestPickUpTime;
            product.SuggestPickEndTime = putProductVM.SuggestPickEndTime;
           if (putProductVM.ImageFile != null)
            {
                product.PhotoPath = await SavePhoto(putProductVM.ImageFile);
            }

            _context.Entry(product).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return "修改商品失敗!";
                }
                else
                {
                    throw;
                }
            }
            return "修改商品成功!";
        }
    }
}


//// GET: api/StoreProducts
//[HttpGet]
//public async Task<IEnumerable<ProductManageDTO>> GetProducts()
//{
//    return await _context.Products
//        .Include(p => p.Category)
//        .Where(e => e.StoreId == targetStoreId).Select(x => new ProductManageDTO
//        {
//            StoreId = x.StoreId,
//            UnitPrice = x.UnitPrice,
//            ProductId = x.ProductId,
//            ProductName = x.ProductName,
//            CategoryId = x.CategoryId,
//            CategoryName = x.Category.CategoryName,
//            ReleasedQty = x.ReleasedQty,
//            OrderedQty = x.OrderedQty,
//            PhotoPath = x.PhotoPath,
//            Description = x.Description,
//            RealeasedTime = x.RealeasedTime,
//        }).ToListAsync();
//}

//[HttpPost("Filter")] //api/ProductManage/Filter
//public async Task<IEnumerable<ProductManageDTO>> Filter([FromBody] ProductManageDTO productManageDTO)
//{
//    return await _context.Products
//        .Include(p => p.Category)
//        .Where(e => e.StoreId == targetStoreId &&(e.ProductName.Contains(productManageDTO.ProductName) 
//        || e.Category.CategoryName.Contains(productManageDTO.CategoryName))).Select(x => new ProductManageDTO
//        {
//            StoreId = x.StoreId,
//            UnitPrice = x.UnitPrice,
//            ProductId = x.ProductId,
//            ProductName = x.ProductName,
//            CategoryId = x.CategoryId,
//            CategoryName = x.Category.CategoryName,
//            ReleasedQty = x.ReleasedQty,
//            OrderedQty = x.OrderedQty,
//            PhotoPath = x.PhotoPath,
//            Description = x.Description,
//            RealeasedTime = x.RealeasedTime,
//        }).ToListAsync();
//}



//// GET: api/ProductManage/5
//[HttpGet("{id}")]
//public async Task<ProductManageDTO> GetProduct(int id)
//{
//    return  await _context.Products
//        .Include(p => p.Category)
//        .Where(e => e.StoreId == targetStoreId).Select(x => new ProductManageDTO
//        {
//            StoreId = x.StoreId,
//            UnitPrice = x.UnitPrice,
//            ProductId = x.ProductId,
//            ProductName = x.ProductName,
//            CategoryId = x.CategoryId,
//            CategoryName = x.Category.CategoryName,
//            ReleasedQty = x.ReleasedQty,
//            OrderedQty = x.OrderedQty,
//            PhotoPath = x.PhotoPath,
//            Description = x.Description,
//            RealeasedTime = x.RealeasedTime,
//        }).FirstOrDefaultAsync();
//}

//// PUT: api/ProductManage/5
//// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//[HttpPut("{id}")]
//public async Task<string> PutProduct(int id, Product product)
//{
//    if (id != product.ProductId)
//    {
//        return "修改商品失敗!";
//    }
//    _context.Entry(product).State = EntityState.Modified;

//    try
//    {
//        await _context.SaveChangesAsync();
//    }
//    catch (DbUpdateConcurrencyException)
//    {
//        if (!ProductExists(id))
//        {
//            return "修改商品失敗!";
//        }
//        else
//        {
//            throw;
//        }
//    }

//    return "修改商品成功!";
//}

//// POST: api/StoreProducts
//// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//[HttpPost]
//public async Task<string> PostProduct(Product product)
//{
//    _context.Products.Add(product);
//    await _context.SaveChangesAsync();
//    return "新增商品成功!";
//}

//// DELETE: api/StoreProducts/5
//[HttpDelete("{id}")]
//public async Task<string> DeleteProduct(int id)
//{
//    if (_context.Products == null)
//    {
//        return "刪除商品失敗!";
//    }
//    var product = await _context.Products.FindAsync(id);
//    if (product == null)
//    {
//        return "刪除商品失敗!";
//    }
//    try
//    {
//        _context.Products.Remove(product);
//        await _context.SaveChangesAsync();

//    }
//    catch(DbUpdateException ex)
//    {
//        return "刪除商品關聯紀錄失敗!";
//    }
//    return "刪除商品失敗!";
//}

//private bool ProductExists(int id)
//{
//    return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
//}

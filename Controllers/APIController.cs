using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Northwind.Models;

namespace Northwind.Controllers
{
    public class APIController : Controller
    {
        // this controller depends on the NorthwindRepository  --  dependancy injection using entity framework lines 11 & 12
        private NorthwindContext _northwindContext;
        public APIController(NorthwindContext db) => _northwindContext = db;

        [HttpGet, Route("api/product")]
        // returns all products
        public IEnumerable<Product> Get() => _northwindContext.Products.OrderBy(p => p.ProductName);

        [HttpGet, Route("api/product/{id}")]
        // returns specific product
        public Product Get(int id) => _northwindContext.Products.FirstOrDefault(p => p.ProductId == id);
          [HttpGet, Route("api/product/discontinued/{discontinued}")]
        // returns all products where discontinued = true/false
        public IEnumerable<Product> GetDiscontinued(bool discontinued) => _northwindContext.Products.Where(p => p.Discontinued == discontinued).OrderBy(p => p.ProductName);
         [HttpGet, Route("api/category/{CategoryId}/product")]
        // returns all products in a specific category
        public IEnumerable<Product> GetByCategory(int CategoryId) => _northwindContext.Products.Where(p => p.CategoryId == CategoryId).OrderBy(p => p.ProductName);
         [HttpGet, Route("api/category/{CategoryId}/product/discontinued/{discontinued}")]
        // returns all products in a specific category where discontinued = true/false
        public IEnumerable<Product> GetByCategoryDiscontinued(int CategoryId, bool discontinued) => _northwindContext.Products.Where(p => p.CategoryId == CategoryId && p.Discontinued == discontinued).OrderBy(p => p.ProductName);

        //returns all categories
        [HttpGet, Route ("api/category")]
        public IEnumerable<Category> GetCategory() => _northwindContext.Categories.OrderBy(p => p.CategoryName);
         
        [HttpPost, Route("api/addtocart")]
        // adds a row to the cartitem table
        public CartItem Post([FromBody] CartItemJSON cartItem) => _northwindContext.AddToCart(cartItem);

        [HttpGet, Route("api/category/{CategoryId}/orderdetail")]
        public IEnumerable<OrderDetailJSON> GetOrderDetails(int CategoryId) => _northwindContext.OrderDetails.Where(od => od.Product.CategoryId == CategoryId).GroupBy(od => od.Product.ProductName).Select(grp => new OrderDetailJSON { 
          ProductName = grp.Key, 
          Revenue = grp.Sum(x => x.Quantity * x.UnitPrice * (1 - x.Discount))
        }).OrderByDescending(p => p.Revenue);

    }
}
using ConnectionPoolingInDapperDotNetCore.Model;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ConnectionPoolingInDapperDotNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IDbConnection _dbConnection;

        // Constructor injection of IDbConnection
        public ProductsController(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var query = "SELECT * FROM Products";
            var products = await _dbConnection.QueryAsync<Product>(query);
            return Ok(products);
        }

        // GET: api/products/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var query = "SELECT * FROM Products WHERE Id = @Id";
            var product = await _dbConnection.QueryFirstOrDefaultAsync<Product>(query, new { Id = id });

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // POST: api/products
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Product product)
        {
            var query = "INSERT INTO Products (Name, Price, StockQuantity, Description) VALUES (@Name, @Price, @StockQuantity, @Description)";
            var result = await _dbConnection.ExecuteAsync(query, product);

            if (result == 0)
                return BadRequest("Failed to create product");

            return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
        }

        // PUT: api/products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Product product)
        {
            var query = "UPDATE Products SET Name = @Name, Price = @Price, StockQuantity = @StockQuantity, Description = @Description WHERE Id = @Id";
            var result = await _dbConnection.ExecuteAsync(query, new { product.Name, product.Price, product.StockQuantity, product.Description, Id = id });

            if (result == 0)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var query = "DELETE FROM Products WHERE Id = @Id";
            var result = await _dbConnection.ExecuteAsync(query, new { Id = id });

            if (result == 0)
                return NotFound();

            return NoContent();
        }
    }
}


using Microsoft.AspNetCore.Mvc;
using WebApiDia2.Contract.Dtos;
using WebApiDia2.Entities;

namespace WebApiDia2.Contract
{
    public interface IProductRepository : IRepository<Product>
    {

        // Métodos adicionales si es necesario
        Task<IEnumerable<Product>> GetProductsPagedAsyncSp(string searchTerm, int pageNumber, int pageSize);

        Task<IEnumerable<Product>> GetProductsPagedAsyncEf(string searchTerm, int pageNumber, int pageSize);
        Task<ProductDTO> GetProductDetailsByIdAsync(int id);
        Task<Boolean> UpdateInventAsync(int productId, int typeId, decimal amount, int userId);
        Task<List<UserKardexSummaryDto>> GetKardexSummaryByUserAsync(DateTime startDate, DateTime endDate);
        Task<IActionResult> GetKardexSummary([FromQuery] DateTime startDate, [FromQuery] DateTime endDate);

    }
}

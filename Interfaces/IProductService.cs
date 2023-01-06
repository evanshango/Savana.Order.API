using Savana.Order.API.Entities;

namespace Savana.Order.API.Interfaces; 

public interface IProductService {
    Task<ProductEntity?> GetProductById(string productId);
    Task UpdateProductPromoExpiry(ProductEntity existingProduct, string type);
    Task UpdateProductQuantity(ProductEntity existingProduct, string type);
}
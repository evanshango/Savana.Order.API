using MassTransit;
using Savana.Order.API.Entities;
using Savana.Order.API.Interfaces;
using Treasures.Common.Events;
using Treasures.Common.Interfaces;

namespace Savana.Order.API.Services;

public class ProductService : IProductService {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;

    public ProductService(IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint) {
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<ProductEntity?> GetProductById(string productId) => await _unitOfWork
        .Repository<ProductEntity>().GetByIdAsync(productId);

    public async Task UpdateProductPromoExpiry(ProductEntity ext, string type) => await SaveProductChanges(ext, type);
    
    public async Task UpdateProductQuantity(ProductEntity ext, string type) => await SaveProductChanges(ext, type);

    private async Task SaveProductChanges(ProductEntity existing, string type) {
        var res = _unitOfWork.Repository<ProductEntity>().UpdateAsync(existing);
        await _unitOfWork.Complete();

        switch (type) {
            case "stock":
                await _publishEndpoint.Publish(new StockEvent(res.ProductId!, existing.Stock));
                break;
            case "promo":
                await _publishEndpoint.Publish(new PromoEvent(res.ProductId!, res.FinalPrice));
                break;
        }
    }
}
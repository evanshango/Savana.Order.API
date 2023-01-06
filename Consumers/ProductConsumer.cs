using System.Text.Json;
using MassTransit;
using Savana.Order.API.Entities;
using Treasures.Common.Events;
using Treasures.Common.Interfaces;
using Treasures.Common.Messages;

namespace Savana.Order.API.Consumers;

public class ProductConsumer : IConsumer<ProductEvent> {
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProductConsumer> _logger;

    public ProductConsumer(IUnitOfWork unitOfWork, ILogger<ProductConsumer> logger) {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProductEvent> context) {
        var prodEvent = context.Message;
        if (string.IsNullOrEmpty(prodEvent.Json)) return;

        var prodMsg = JsonSerializer.Deserialize<ProductMessage>(prodEvent.Json);

        switch (prodEvent.EventType) {
            case "PRODUCT_CREATED":
                await SaveNewProduct(prodMsg);
                break;
            case "PRODUCT_UPDATED":
                await UpdateExistingProduct(prodEvent.ProductId, prodMsg);
                break;
            case "PRODUCT_DELETED":
                await DeleteExistingProduct(prodEvent.ProductId, prodMsg);
                break;
            default:
                _logger.LogError("Invalid EventType received...");
                break;
        }
    }

    private async Task SaveNewProduct(ProductMessage? prodMsg) {
        if (prodMsg == null) return;
        var newProduct = new ProductEntity {
            ProductId = prodMsg.Id, Name = prodMsg.Name, ImageUrl = prodMsg.ImageUrl,
            InitialPrice = prodMsg.InitialPrice,
            Brand = prodMsg.Brand, FinalPrice = prodMsg.FinalPrice, Stock = prodMsg.Stock, Active = prodMsg.Active,
            Owner = prodMsg.Owner, UpdatedBy = prodMsg.UpdatedBy, UpdatedAt = prodMsg.UpdatedAt,
            PromoExpiry = prodMsg.PromoExpiry
        };
        var res = _unitOfWork.Repository<ProductEntity>().AddAsync(newProduct);
        var result = await _unitOfWork.Complete();
        if (result >= 1) {
            _logger.LogInformation("Product with id {Id} was successfully created", res.ProductId);
        } else {
            _logger.LogError("Product with id {Id} failed creation", res.ProductId);
        }
    }

    private async Task UpdateExistingProduct(string productId, ProductMessage? prodMsg) {
        var existingProd = await FetchProduct(productId);
        if (existingProd == null || prodMsg == null) return;

        existingProd.Name = prodMsg.Name ?? existingProd.Name;
        existingProd.InitialPrice = prodMsg.InitialPrice;
        existingProd.FinalPrice = existingProd.GetFinalPrice();
        existingProd.ImageUrl = prodMsg.ImageUrl ?? existingProd.ImageUrl;
        existingProd.Brand = prodMsg.Brand ?? existingProd.Brand;
        existingProd.Stock = prodMsg.Stock;
        existingProd.PromoExpiry = prodMsg.PromoExpiry;
        existingProd.UpdatedAt = prodMsg.UpdatedAt;
        existingProd.UpdatedBy = prodMsg.UpdatedBy ?? existingProd.UpdatedBy;
        existingProd.Owner = prodMsg.Owner ?? existingProd.Owner;

        await SaveProductChanges(existingProd, "update");
    }

    private async Task DeleteExistingProduct(string productId, ProductMessage? prodMsg) {
        var existingProd = await FetchProduct(productId);
        if (existingProd == null || prodMsg == null) return;

        existingProd.Active = prodMsg.Active;
        existingProd.UpdatedBy = prodMsg.UpdatedBy;
        existingProd.UpdatedAt = prodMsg.UpdatedAt;

        await SaveProductChanges(existingProd, "delete");
    }

    private async Task SaveProductChanges(ProductEntity existingProd, string action) {
        var res = _unitOfWork.Repository<ProductEntity>().UpdateAsync(existingProd);
        var result = await _unitOfWork.Complete();

        if (result >= 1) {
            _logger.LogInformation("Product {Action} for product with id {Id} was successful by user {User}",
                action, res.ProductId, res.UpdatedBy
            );
        } else {
            _logger.LogError("Product {Act} for product with id {Id} failed", action, res.ProductId);
        }
    }

    private async Task<ProductEntity?> FetchProduct(string productId) => await _unitOfWork
        .Repository<ProductEntity>().GetByIdAsync(productId);
}
﻿using System.Collections.Frozen;
using TrainingTDDWithCleanArch.Application.Inputs;
using TrainingTDDWithCleanArch.Domain.AggregateRoots.Products;
using TrainingTDDWithCleanArch.Domain.Interfaces;

namespace TrainingTDDWithCleanArch.Application.UseCases;

public interface IProductUseCases
{
    Task<Validation<Error, FrozenSet<Product>>> GetProducts(CancellationToken cancellation);
    Task<Validation<Error, Product>> GetProductById(Guid productId, CancellationToken cancellation);
    Task<Validation<Error, Product>> GetProductByName(string productName, CancellationToken cancellation);
    Task<Validation<Error, Product>> CreateProduct(CreateProductInput productInput, CancellationToken cancellation);
}

public sealed class ProductUseCases(ILogger<ProductUseCases> logger, IProductRepository productRepository, ICategoryUseCases categoryUseCases) : IProductUseCases
{
    private readonly ILogger<ProductUseCases> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    private readonly ICategoryUseCases _categoryUseCases = categoryUseCases ?? throw new ArgumentNullException(nameof(categoryUseCases));

    public async Task<Validation<Error, FrozenSet<Product>>> GetProducts(CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName}", nameof(GetProducts));
        return await _productRepository.Get(cancellation);
    }

    public async Task<Validation<Error, Product>> GetProductById(Guid productId, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {ProductId}", nameof(GetProductById), productId);
        return await _productRepository.GetById(productId, cancellation);
    }

    public async Task<Validation<Error, Product>> GetProductByName(string productName, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {ProductName}", nameof(GetProductByName), productName);
        return await _productRepository.GetByName(productName, cancellation);
    }

    public async Task<Validation<Error, Product>> CreateProduct(CreateProductInput productInput, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {ProductInput}", nameof(CreateProduct), productInput);

        var categoryResult = await _categoryUseCases.GetOrCreateCategory(productInput, cancellation);
        if (categoryResult.IsFail) return (Seq<Error>)categoryResult;

        var category = categoryResult.SuccessToSeq().First();
        var productResult = productInput.ToProduct();
        if (productResult.IsFail)
            return productResult;

        var product = productResult.SuccessToArray().First();
        product.SetCategory(category);
        var repoResult = await _productRepository.Insert(product, cancellation);

        if (repoResult != ValidationResult.Success)
            return Error.New(repoResult.ErrorMessage!);

        product.SetCategory(category);
        return product;
    }
}

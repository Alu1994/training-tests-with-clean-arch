﻿using System.Collections.Frozen;
using CleanArchitectureSampleProject.Application.Inputs;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Domain.Interfaces.Repositories;

namespace CleanArchitectureSampleProject.Application.UseCases;

public interface IProductUseCases
{
    Task<Validation<Error, FrozenSet<Product>>> GetProducts(CancellationToken cancellation);
    Task<Validation<Error, Product>> GetProductById(Guid productId, CancellationToken cancellation);
    Task<Validation<Error, Product>> GetProductByName(string productName, CancellationToken cancellation);
    Task<Validation<Error, Product>> CreateProduct(CreateProductInput productInput, CancellationToken cancellation);
    Task<Validation<Error, Product>> UpdateProduct(UpdateProductInput productInput, CancellationToken cancellation);
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
        return await categoryResult.MatchAsync(async category => 
            await CreateProduct(productInput, category, cancellation), 
            error => error);

        Task<Validation<Error, Product>> CreateProduct(CreateProductInput productInput, Category category, CancellationToken cancellation)
        {
            return productInput.ToProduct().MatchAsync<Validation<Error, Product>>(async product =>
            {
                product.SetCategory(category);
                var repoResult = await _productRepository.Insert(product, cancellation);

                if (repoResult != ValidationResult.Success)
                    return Error.New(repoResult.ErrorMessage!);
                return product.WithCategory(category);
            }, error => error);
        }
    }

    public async Task<Validation<Error, Product>> UpdateProduct(UpdateProductInput productInput, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {ProductInput}", nameof(UpdateProduct), productInput);

        var categoryResult = await _categoryUseCases.GetOrCreateCategory(new CreateProductInput(productInput), cancellation);
        return await categoryResult.MatchAsync(async category =>
            await UpdateProduct(productInput, category, cancellation),
            error => error);

        Task<Validation<Error, Product>> UpdateProduct(UpdateProductInput productInput, Category category, CancellationToken cancellation)
        {
            productInput.SetCategory(category);
            var result = productInput.ToProduct();
            return result.MatchAsync<Validation<Error, Product>>(async product =>
            {
                var repoResult = await _productRepository.Update(product, cancellation);
                if (repoResult != ValidationResult.Success)
                    return Error.New(repoResult.ErrorMessage!);
                return product.WithCategory(category);
            }, error => error);
        }
    }
}

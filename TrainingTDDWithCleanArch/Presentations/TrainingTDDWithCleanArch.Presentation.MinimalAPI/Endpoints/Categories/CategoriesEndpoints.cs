﻿using CleanArchitectureSampleProject.CrossCuttingConcerns;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Frozen;
using System.Net;
using TrainingTDDWithCleanArch.Application.Inputs;
using TrainingTDDWithCleanArch.Application.UseCases;
using TrainingTDDWithCleanArch.Domain.AggregateRoots.Products.Entities;

namespace TrainingTDDWithCleanArch.Presentation.MinimalAPI.Endpoints.Categories;

public static class CategoriesEndpoints
{
    public readonly struct Logging;
    private const string TagName = "Categories";
    private const string Controller = "category";
    private const string ContentType = "application/json";
    private const int Success = StatusCodes.Status200OK;
    private const int BadRequest = StatusCodes.Status400BadRequest;

    public static WebApplication MapCategories(this WebApplication app)
    {
        app.MapGet($"/{Controller}", async (ILogger<Logging> logger, ICategoryUseCases categoryUseCases, CancellationToken cancellation) =>
        {
            return await GetAllCategories(logger, categoryUseCases, cancellation);
        })
        .Produces<FrozenSet<Category>>(Success, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .WithConfigSummaryInfo("Get All Categories", TagName);

        app.MapGet($"/{Controller}/{{categoryId:Guid}}", async (ILogger<Logging> logger, ICategoryUseCases categoryUseCases, Guid categoryId, CancellationToken cancellation) =>
        {
            return await GetById(logger, categoryUseCases, categoryId, cancellation);
        })
        .Produces<Category>(Success, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .WithConfigSummaryInfo($"Get {Controller} By Id", TagName);

        app.MapGet($"/{Controller}/by-name/{{categoryName}}", async (ILogger<Logging> logger, ICategoryUseCases categoryUseCases, string categoryName, CancellationToken cancellation) =>
        {
            return await GetByName(logger, categoryUseCases, categoryName, cancellation);
        })
        .Produces<Category>(Success, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .WithConfigSummaryInfo($"Get {Controller} By Name", TagName);

        app.MapPost($"/{Controller}", async (ILogger<Logging> logger, ICategoryUseCases categoryUseCases, CreateCategoryInput category, CancellationToken cancellation) =>
        {
            return await CreateCategory(logger, categoryUseCases, category, cancellation);
        })
        .Accepts<CreateCategoryInput>(ContentType)
        .Produces<Category>(Success, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .WithConfigSummaryInfo($"Create {Controller}", TagName);

        return app;
    }

    public static async Task<IResult> GetAllCategories(ILogger<Logging> logger, ICategoryUseCases categoryUseCases, CancellationToken cancellation)
    {
        const string errorTitle = "Error while getting all categories.";

        var result = await categoryUseCases.GetCategories(cancellation);
        return result.Match(success => Results.Ok(success),
            error =>
            {
                var errorMessage = logger.LogSeqError(error);
                return Results.Problem(
                    type: HttpStatusCode.BadRequest.ToString(),
                    title: errorTitle,
                    detail: errorMessage,
                    statusCode: StatusCodes.Status400BadRequest
                );
            }
        );
    }

    public static async Task<IResult> GetById(ILogger<Logging> logger, ICategoryUseCases categoryUseCases, Guid categoryId, CancellationToken cancellation)
    {
        const string errorTitle = "Error while getting category by id.";

        var result = await categoryUseCases.GetCategoryById(categoryId, cancellation);
        return result.Match(success => Results.Ok(success),
            error =>
            {
                var errorMessage = logger.LogSeqError(error);
                return Results.Problem(
                    type: HttpStatusCode.BadRequest.ToString(),
                    title: errorTitle,
                    detail: errorMessage,
                    statusCode: StatusCodes.Status400BadRequest
                );
            }
        );
    }

    public static async Task<IResult> GetByName(ILogger<Logging> logger, ICategoryUseCases categoryUseCases, string categoryName, CancellationToken cancellation)
    {
        const string errorTitle = "Error while getting category by name.";

        var result = await categoryUseCases.GetCategoryByName(categoryName, cancellation);
        return result.Match(success => Results.Ok(success),
            error =>
            {
                var errorMessage = logger.LogSeqError(error);
                return Results.Problem(
                    type: HttpStatusCode.BadRequest.ToString(),
                    title: errorTitle,
                    detail: errorMessage,
                    statusCode: StatusCodes.Status400BadRequest
                );
            }
        );
    }

    public static async Task<IResult> CreateCategory(ILogger<Logging> logger, ICategoryUseCases categoryUseCases, CreateCategoryInput category, CancellationToken cancellation)
    {
        const string errorTitle = "Error while creating new category.";

        var result = await categoryUseCases.CreateCategory(category, cancellation);
        return result.Match(success => Results.Ok(success),
            error =>
            {
                var errorMessage = logger.LogSeqError(error);
                return Results.Problem(
                    type: HttpStatusCode.BadRequest.ToString(),
                    title: errorTitle,
                    detail: errorMessage,
                    statusCode: StatusCodes.Status400BadRequest
                );
            }
        );
    }
}

﻿using Microsoft.Extensions.DependencyInjection;

namespace TrainingTDDWithCleanArch.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        return services.AddSingleton<IProductUseCases, ProductUseCases>()
            .AddSingleton<ICategoryUseCases, CategoryUseCases>();
    }
}
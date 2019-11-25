﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infrastructure.Swagger
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration Configuration)
        {
            var options = new SwaggerOptions();
            Configuration.GetSection(nameof(SwaggerOptions)).Bind(options);
            services.Configure<SwaggerOptions>(Configuration.GetSection(nameof(SwaggerOptions)));

            if (string.IsNullOrWhiteSpace(options.Title))
            {
                options.Title = AppDomain.CurrentDomain.FriendlyName.Trim().Trim('_');
            }

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(options.Version, options);
            });

            return services;
        }

        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, IConfiguration Configuration)
        {
            var options = new SwaggerOptions();
            Configuration.GetSection(nameof(SwaggerOptions)).Bind(options);

            if (string.IsNullOrWhiteSpace(options.Title))
            {
                options.Title = AppDomain.CurrentDomain.FriendlyName.Trim().Trim('_');
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{options.Version}/swagger.json", options.Title);
                c.RoutePrefix = options.RoutePrefix;
            });

            return app;
        }
    }
}

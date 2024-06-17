using Api.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace  Api
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.Configure(app =>
                    {
                        app.UseRouting();

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapPost("/api/SuperStore/SaveWithXSD", async context =>
                            {
                                var file = context.Request.Form.Files.GetFile("file");

                                var superStore = new SuperStoreController();
                                if (file == null)
                                {
                                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                    await context.Response.WriteAsync("Can't open the file, try again.");
                                    return;
                                }

                                try
                                {
                                    
                                    superStore.ProcessXmlFileWithXSD(file);
                                    context.Response.StatusCode = StatusCodes.Status200OK;
                                    await context.Response.WriteAsync("XML file is valid according to the provided XSD schema.");
                                }
                                catch (Exception ex) 
                                {
                                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                    
                                    await context.Response.WriteAsync($"Error validating XML file: {ex.Message}");
                                }
                            });

                            endpoints.MapPost("/api/SuperStore/SaveWithRNG", async context =>
                            {
                                var file = context.Request.Form.Files.GetFile("file");

                                if (file == null)
                                {
                                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                    await context.Response.WriteAsync("Can't open the file, try again.");
                                    return;
                                }

                                var superStore = new SuperStoreController();
                                try
                                {
                                    var (isValid, errorMessage) = superStore.ProcessXmlFileWithRNG(file);

                                    if (isValid)
                                    {
                                        context.Response.StatusCode = StatusCodes.Status200OK;
                                        await context.Response.WriteAsync("XML file is valid according to the provided RNG schema.");
                                    }
                                    else
                                    {
                                        throw new Exception(errorMessage);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                    await context.Response.WriteAsync($"Error validating XML file: {ex.Message}");
                                }
                            });

                            endpoints.MapGet("/api/SuperStore/Temperature/{cityName}", async context =>
                            {
                                var cityName = context.Request.RouteValues["cityName"]?.ToString();

                                if (string.IsNullOrWhiteSpace(cityName))
                                {
                                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                    await context.Response.WriteAsync("City name must be provided.");
                                    return;
                                }

                                var countryController = new SuperStoreController();
                                try
                                {
                                    var temperatures = await countryController.GetCurrentTemperatures(cityName);

                                    if (temperatures.Count == 0)
                                    {
                                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                                        await context.Response.WriteAsync("City not found.");
                                        return;
                                    }

                                    context.Response.StatusCode = StatusCodes.Status200OK;
                                    context.Response.ContentType = "application/json";
                                    await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(temperatures));
                                }
                                catch (Exception ex)
                                {
                                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                                    await context.Response.WriteAsync($"An error occurred: {ex.Message}");
                                }
                            });



                        });
                    });

                });



    }

}

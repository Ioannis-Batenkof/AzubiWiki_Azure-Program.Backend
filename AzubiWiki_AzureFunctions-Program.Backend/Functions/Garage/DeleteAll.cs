using AutoMapper;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;

namespace AzubiWiki_AzureFunctions_Program.Backend.Functions.Garage
{
    public class DeleteAll
    {
        private readonly IStorageService<Core.Model.Garage> _storageService;

        public DeleteAll(IStorageService<Core.Model.Garage> storageService)
        {
            _storageService = storageService;
        }

        [Function("DeleteAllGarages")]
        [OpenApiOperation(operationId: "DeleteAllGarages", tags: new[] { "Garage" }, Visibility = OpenApiVisibilityType.Undefined, Description = "Deletes all Garage objects and updates the database.")]
        [OpenApiSecurity(schemeName: "bearer_auth", schemeType: SecuritySchemeType.Http, BearerFormat = "JWT", Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NoContent, Description = "The objects were deleted")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.FailedDependency, contentType: "application/json", bodyType: typeof(FileNotFoundException), Description = "Database was not loaded properly or is in maintenance")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Ambiguous, contentType: "application/json", bodyType: typeof(Exception), Description = "Unexpected/Unhandled Exception")]

        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "garages")] HttpRequestData req)
        {
            try
            {
                await _storageService.DeleteAll();

                var response = req.CreateResponse(HttpStatusCode.NoContent);
                return response;
            }
            catch (FileNotFoundException ex)
            {
                var response = req.CreateResponse(HttpStatusCode.FailedDependency);
                response.Headers.Add("Content-Type", "application/json");
                await response.WriteStringAsync(ex.Message);
                return response;
            }
            catch (Exception ex)
            {
                var response = req.CreateResponse(HttpStatusCode.UnprocessableContent);
                response.Headers.Add("Content-Type", "application/json");
                await response.WriteStringAsync(ex.Message);
                return response;
            }
        }
    }
}

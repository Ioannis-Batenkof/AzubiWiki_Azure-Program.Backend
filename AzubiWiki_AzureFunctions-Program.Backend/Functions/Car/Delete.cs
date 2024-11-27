using AzubiWiki_AzureFunctions_Program.Backend.Core.Interfaces;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using FluentAssertions.Execution;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Exceptions;
using FluentAssertions;

namespace AzubiWiki_AzureFunctions_Program.Backend.Functions.Car
{
    public class Delete
    {
        private readonly IStorageService<Core.Model.Car> _storageService;

        public Delete(IStorageService<Core.Model.Car> storageService)
        {
            _storageService = storageService;
        }

        [Function("DeleteCar")]
        [OpenApiOperation(operationId: "DeleteCar", tags: new[] { "Car" }, Visibility = OpenApiVisibilityType.Undefined, Description = "Deletes a Car object and updates the database.")]
        [OpenApiSecurity(schemeName: "bearer_auth", schemeType: SecuritySchemeType.Http, BearerFormat = "JWT", Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NoContent, Description = "The object was deleted")]
        [OpenApiResponseWithBody(statusCode:HttpStatusCode.UnprocessableContent,contentType:"application/json",bodyType:typeof(UnprocessableGuidValueException), Description = "Could not process the given ID")]
        [OpenApiResponseWithBody(statusCode:HttpStatusCode.FailedDependency, contentType:"application/json",bodyType:typeof(FileNotFoundException), Description = "Database was not loaded properly or is in maintenance")]
        [OpenApiResponseWithBody(statusCode:HttpStatusCode.ExpectationFailed, contentType:"application/json", bodyType:typeof(AssertionFailedException), Description ="Expectations were not met")]
        [OpenApiResponseWithBody(statusCode:HttpStatusCode.Ambiguous, contentType:"application/json", bodyType: typeof(Exception), Description = "Unexpected/Unhandled Exception")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "cars/{id}")] HttpRequestData req, string id)
        {
            Guid ID;
            try
            {
                ID = Guid.Parse(id);
                ID.Should().NotBeEmpty();
            }
            catch (Exception ex)
            {
                var response = req.CreateResponse(HttpStatusCode.UnprocessableContent);
                response.Headers.Add("Content-Type", "application/json");
                await response.WriteStringAsync(ex.Message);
                return response;
            }

            try
            {
                await _storageService.Delete(ID);

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
            catch (AssertionFailedException ex)
            {
                var response = req.CreateResponse(HttpStatusCode.ExpectationFailed);
                response.Headers.Add("Content-Type", "application/json");
                await response.WriteStringAsync(ex.Message);
                return response;
            }
            catch (Exception ex)
            {
                var response = req.CreateResponse(HttpStatusCode.Ambiguous);
                response.Headers.Add("Content-Type", "application/json");
                await response.WriteStringAsync(ex.Message);
                return response;
            }
        }



    }
}

using AzubiWiki_AzureFunctions_Program.Backend.Core.Exceptions;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Interfaces;
using AzubiWiki_AzureFunctions_Program.Backend.Repositories;
using FluentAssertions;
using FluentAssertions.Execution;
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
    public class RemoveCarFromGarage
    {
        private readonly IStorageService<Core.Model.Garage> _garageStorageService;
        private readonly IStorageService<Core.Model.Car> _carStorageService;

        public RemoveCarFromGarage(IStorageService<Core.Model.Garage> storageService, IStorageService<Core.Model.Car> carStorageService)
        {
            _garageStorageService = storageService;
            _carStorageService = carStorageService;
        }

        [Function("RemoveCarFromGarage")]
        [OpenApiOperation(operationId: "RemoveCarFromGarage", tags: new[] { "Garage" }, Visibility = OpenApiVisibilityType.Undefined, Description = "Removes the specified Car Object from the specified Garage Object.")]
        [OpenApiSecurity(schemeName: "bearer_auth", schemeType: SecuritySchemeType.Http, BearerFormat = "JWT", Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NoContent, Description = "The specified Car was successfully removed from the specified Garage!")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.UnprocessableContent, contentType: "application/json", bodyType: typeof(UnprocessableGuidValueException), Description = "The expected value is a Guid (ID) but the user entered a different value type")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.ExpectationFailed, contentType: "application/json", bodyType: typeof(AssertionFailedException), Description = "Object did not pass the initial validation.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.FailedDependency, contentType: "application/json", bodyType: typeof(FileNotFoundException), Description = "Database was not loaded properly or is in maintenance")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Ambiguous, contentType: "application/json", bodyType: typeof(Exception), Description = "Unexcpected/Unhandled Exception")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "garages/{gid}/cars/{cid}")] HttpRequestData req, string gid, string cid)
        {
            Guid GID;
            Guid CID;
            try
            {
                GID = Guid.Parse(gid);
                CID = Guid.Parse(cid);

                GID.Should().NotBeEmpty();
                CID.Should().NotBeEmpty();
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
                Core.Model.Garage garage = await _garageStorageService.Read(GID);
                Core.Model.Car car = await _carStorageService.Read(CID);
                bool found = false;

                for (int i = 0; i < garage.Cars.Count; i++)
                {
                    if (garage.Cars[i].ID == CID)
                    {
                        garage.Cars.RemoveAt(i);
                        found = true;
                        break;
                    }
                }

                found.Should().BeTrue("It found the car to delete");

                car.Available = true;
                await _carStorageService.Update(car);
                await _garageStorageService.Update(garage);

                var response = req.CreateResponse(HttpStatusCode.NoContent);
                return response;
            }
            catch (AssertionFailedException ex)
            {
                var response = req.CreateResponse(HttpStatusCode.ExpectationFailed);
                response.Headers.Add("Content-Type", "application/json");
                await response.WriteStringAsync(ex.Message);
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
                var response = req.CreateResponse(HttpStatusCode.Ambiguous);
                response.Headers.Add("Content-Type", "application/json");
                await response.WriteStringAsync(ex.Message);
                return response;
            }
        }
    }
}

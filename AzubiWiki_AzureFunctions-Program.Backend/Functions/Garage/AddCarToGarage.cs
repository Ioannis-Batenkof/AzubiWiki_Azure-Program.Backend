using AzubiWiki_AzureFunctions_Program.Backend.Core.Interfaces;
using FluentAssertions;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Extensions;
using AzubiWiki_AzureFunctions_Program.Backend.Repositories;
using Microsoft.AspNetCore.Server.HttpSys;
using AzubiWiki_AzureFunctions_Program.Backend.Contracts.Queries;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Exceptions;
using FluentAssertions.Execution;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace AzubiWiki_AzureFunctions_Program.Backend.Functions.Garage
{
    public class AddCarToGarage
    {
        private readonly IStorageService<Core.Model.Garage> _garageStorageService;
        private readonly IStorageService<Core.Model.Car> _carStorageService;

        public AddCarToGarage(IStorageService<Core.Model.Garage> storageService, IStorageService<Core.Model.Car> carStorageService)
        {
            _garageStorageService = storageService;
            _carStorageService = carStorageService;
        }

        [Function("AddCarToGarage")]
        [OpenApiOperation(operationId: "AddCarToGarage", tags: new[] { "Garage" }, Visibility = OpenApiVisibilityType.Undefined, Description = "Asigns a Car Object to a Garage Object.")]
        [OpenApiSecurity(schemeName: "bearer_auth", schemeType: SecuritySchemeType.Http, BearerFormat = "JWT", Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NoContent, Description = "The specified Car was successfully assigned to the specified Garage!")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.UnprocessableContent, contentType: "application/json", bodyType: typeof(UnprocessableGuidValueException), Description = "The expected value is a Guid (ID) but the user entered a different value type")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.ExpectationFailed, contentType: "application/json", bodyType: typeof(AssertionFailedException), Description = "Object did not pass the initial validation.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.FailedDependency, contentType: "application/json", bodyType: typeof(FileNotFoundException), Description = "Database was not loaded properly or is in maintenance")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.ServiceUnavailable, contentType: "application/json", bodyType: typeof(CarIsUnavailableException), Description = "Requested Car Object is already assighen to another Garage Object")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Ambiguous, contentType: "application/json", bodyType: typeof(Exception), Description = "Unexcpected/Unhandled Exception")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "Garages/{GID}/Cars/{CID}")] HttpRequestData req, string GID, string CID)
        {
            Guid gid;
            Guid cid;
            try
            {
                gid = Guid.Parse(GID);
                cid = Guid.Parse(CID);

                gid.Should().NotBeEmpty();
                cid.Should().NotBeEmpty();
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
                Core.Model.Car car = await _carStorageService.Read(cid);
                Core.Model.Garage garage = await _garageStorageService.Read(gid);

                if (garage.Cars == null)
                {
                    garage.Cars = new List<Core.Model.Car>();
                }

                if (car.Available == false)
                {
                    throw new CarIsUnavailableException();
                }

                garage.Cars.Add(car);
                car.Available = false;

                await _garageStorageService.Update(garage);
                await _carStorageService.Update(car);

                var response = req.CreateResponse(HttpStatusCode.NoContent);
                return response;
            }
            catch (CarIsUnavailableException ex)
            {
                var response = req.CreateResponse(HttpStatusCode.ServiceUnavailable);
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

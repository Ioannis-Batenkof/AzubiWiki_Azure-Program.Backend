using System.Net;
using AutoMapper;
using AzubiWiki_AzureFunctions_Program.Backend.Contracts.Queries;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Exceptions;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Interfaces;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Services.Validation;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace AzubiWiki_AzureFunctions_Program.Backend.Functions.Car
{
    public class Update
    {
        private readonly IStorageService<Core.Model.Car> _storageService;
        private readonly IMapper _mapper;

        public Update(Core.Interfaces.IStorageService<Core.Model.Car> storageService, IMapper mapper)
        {
            _storageService = storageService;
            _mapper = mapper;
        }

        [Function("UpdateCar")]
        [OpenApiOperation(operationId: "UpdateCar", tags: new[] { "Car" }, Visibility = OpenApiVisibilityType.Undefined, Description = "Updates a Car object and updates the database.")]
        [OpenApiSecurity(schemeName: "bearer_auth", schemeType: SecuritySchemeType.Http, BearerFormat = "JWT", Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NoContent, Description = "The object was updated")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.UnprocessableContent, contentType: "application/json", bodyType: typeof(UnprocessableGuidValueException), Description = "Could not process the given ID")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.FailedDependency, contentType: "application/json", bodyType: typeof(FileNotFoundException), Description = "Database was not loaded properly or is in maintenance")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.ExpectationFailed, contentType: "application/json", bodyType: typeof(AssertionFailedException), Description = "Expectations were not met")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Ambiguous, contentType: "application/json", bodyType: typeof(Exception), Description = "Unexpected/Unhandled Exception")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "Cars/{ID}")] HttpRequestData req, string ID)
        {
            Guid id;
            CarQ car;
            try
            {
                id = Guid.Parse(ID);
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                car = JsonConvert.DeserializeObject<CarQ>(requestBody);

                car.Should().NotBeNull();

                car.ID = id;
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
                Validate validate = new();
                if (validate.NewCar(car))
                {
                    CarQ CarData = _mapper.Map<CarQ>(await _storageService.Read(car.ID));
                    car.Available = CarData.Available;
                    await _storageService.Update(_mapper.Map<Core.Model.Car>(car));
                }

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

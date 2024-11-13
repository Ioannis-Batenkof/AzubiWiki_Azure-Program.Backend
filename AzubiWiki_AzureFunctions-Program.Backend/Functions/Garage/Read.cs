using AutoMapper;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Interfaces;
using FluentAssertions;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using AzubiWiki_AzureFunctions_Program.Backend.Contracts.DTOs;
using Newtonsoft.Json;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Exceptions;
using FluentAssertions.Execution;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace AzubiWiki_AzureFunctions_Program.Backend.Functions.Garage
{
    public class Read
    {
        private readonly IStorageService<Core.Model.Garage> _storageService;
        private readonly IMapper _mapper;

        public Read(IStorageService<Core.Model.Garage> storageService, IMapper mapper)
        {
            _storageService = storageService;
            _mapper = mapper;
        }

        [Function("ReadGarage")]
        [OpenApiOperation(operationId: "ReadGarage", tags: new[] { "Garage" }, Visibility = OpenApiVisibilityType.Undefined, Description = "Reads the specified Garage Object from the database.")]
        [OpenApiSecurity(schemeName: "bearer_auth", schemeType: SecuritySchemeType.Http, BearerFormat = "JWT", Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GarageDTO), Description = "The object was found")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.UnprocessableContent, contentType: "application/json", bodyType: typeof(UnprocessableGuidValueException), Description = "Could not process the given ID")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.FailedDependency, contentType: "application/json", bodyType: typeof(FileNotFoundException), Description = "Database was not loaded properly or is in maintenance")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.ExpectationFailed, contentType: "application/json", bodyType: typeof(AssertionFailedException), Description = "Expectations were not met")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Ambiguous, contentType: "application/json", bodyType: typeof(Exception), Description = "Unexpected/Unhandled Exception")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Garages/{ID}")] HttpRequestData req, string ID)
        {
            Guid id;
            try
            {
                id = Guid.Parse(ID);

                id.Should().NotBeEmpty();
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
                GarageDTO garage = _mapper.Map<GarageDTO>(await _storageService.Read(id));

                garage.Should().NotBeNull();

                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "application/json");
                await response.WriteStringAsync(JsonConvert.SerializeObject(garage));
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

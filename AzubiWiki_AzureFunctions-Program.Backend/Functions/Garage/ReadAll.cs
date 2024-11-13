using AutoMapper;
using AzubiWiki_AzureFunctions_Program.Backend.Contracts.DTOs;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Interfaces;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Net;
namespace AzubiWiki_AzureFunctions_Program.Backend.Functions.Garage
{
    public class ReadAll
    {
        private readonly IStorageService<Core.Model.Garage> _storageService;
        private readonly IMapper _mapper;

        public ReadAll(IStorageService<Core.Model.Garage> storageService, IMapper mapper)
        {
            _storageService = storageService;
            _mapper = mapper;
        }

        [Function("ReadAllGarages")]
        [OpenApiOperation(operationId: "ReadAllGarages", tags: new[] { "Garage" }, Visibility = OpenApiVisibilityType.Undefined, Description = "Retrieves all Garage objects from the database.")]
        [OpenApiSecurity(schemeName: "bearer_auth", schemeType: SecuritySchemeType.Http, BearerFormat = "JWT", Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<GarageDTO>), Description = "List retrieved successfully")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.FailedDependency, contentType: "application/json", bodyType: typeof(FileNotFoundException), Description = "Database was not loaded properly or is in maintenance")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.ExpectationFailed, contentType: "application/json", bodyType: typeof(AssertionFailedException), Description = "Expectations were not met")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Ambiguous, contentType: "application/json", bodyType: typeof(Exception), Description = "Unexpected/Unhandled Exception")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Garages")] HttpRequestData req)
        {
            try
            {
                List<GarageDTO> garages = new();
                List<Core.Model.Garage> models = await _storageService.ReadAll();
                foreach(Core.Model.Garage model in models)
                {
                    garages.Add(_mapper.Map<GarageDTO>(model));
                }

                garages.Should().NotBeEmpty();

                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "application/json");
                await response.WriteStringAsync(JsonConvert.SerializeObject(garages, Formatting.Indented));
                return response;
            }
            catch (Exception ex) 
            {
                var response = req.CreateResponse(HttpStatusCode.InternalServerError);
                response.Headers.Add("Content-Type", "application/json");
                await response.WriteStringAsync(ex.Message);
                return response;
            }
        }
    }
}

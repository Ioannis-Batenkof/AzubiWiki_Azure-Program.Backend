using AutoMapper;
using AzubiWiki_AzureFunctions_Program.Backend.Contracts.DTOs;
using AzubiWiki_AzureFunctions_Program.Backend.Contracts.Queries;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Exceptions;
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


namespace AzubiWiki_AzureFunctions_Program.Backend.Functions.Car
{
    public class ReadAll
    {
        private readonly IStorageService<Core.Model.Car> _storageService;
        private readonly IMapper _mapper;

        public ReadAll(IStorageService<Core.Model.Car> storageService, IMapper mapper)
        {
            _mapper = mapper;
            _storageService = storageService;
        }

        [Function("ReadAllCars")]
        [OpenApiOperation(operationId: "ReadAllCars", tags: new[] { "Car" }, Visibility = OpenApiVisibilityType.Undefined, Description = "Retrieves all Car objects from the database.")]
        [OpenApiSecurity(schemeName: "bearer_auth", schemeType: SecuritySchemeType.Http, BearerFormat = "JWT", Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK,contentType:"application/json", bodyType:typeof(List<CarDTO>) , Description = "List retrieved successfully")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.FailedDependency, contentType: "application/json", bodyType: typeof(FileNotFoundException), Description = "Database was not loaded properly or is in maintenance")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.ExpectationFailed, contentType: "application/json", bodyType: typeof(AssertionFailedException), Description = "Expectations were not met")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Ambiguous, contentType: "application/json", bodyType: typeof(Exception), Description = "Unexpected/Unhandled Exception")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "cars")] HttpRequestData req)
        {
            try
            {
                List<Core.Model.Car> cars = await _storageService.ReadAll();
                List<CarDTO> carDTOs = new List<CarDTO>();

                foreach (Core.Model.Car car in cars)
                {
                    carDTOs.Add(_mapper.Map<CarDTO>(car));
                }

                carDTOs.Should().NotBeEmpty().And.HaveCountGreaterThanOrEqualTo(1);

                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "application/json");
                await response.WriteStringAsync(JsonConvert.SerializeObject(carDTOs));
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

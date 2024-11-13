using AutoMapper;
using AzubiWiki_AzureFunctions_Program.Backend.Contracts.DTOs;
using AzubiWiki_AzureFunctions_Program.Backend.Contracts.Queries;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Exceptions;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Interfaces;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Model;
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
    public class Read
    {
        private readonly IStorageService<Core.Model.Car> _storageService;
        private readonly IMapper _mapper;

        public Read(IStorageService<Core.Model.Car> storageService, IMapper mapper)
        {
            _storageService = storageService;
            _mapper = mapper;
        }

        [Function("ReadCar")]
        [OpenApiOperation(operationId: "ReadCar", tags: new[] { "Car" }, Visibility = OpenApiVisibilityType.Undefined, Description = "Reads the specified Car Object from the database.")]
        [OpenApiSecurity(schemeName: "bearer_auth", schemeType: SecuritySchemeType.Http, BearerFormat = "JWT", Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType:"application/json", bodyType:typeof(CarDTO), Description = "The object was found")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.UnprocessableContent, contentType: "application/json", bodyType: typeof(UnprocessableGuidValueException), Description = "Could not process the given ID")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.FailedDependency, contentType: "application/json", bodyType: typeof(FileNotFoundException), Description = "Database was not loaded properly or is in maintenance")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.ExpectationFailed, contentType: "application/json", bodyType: typeof(AssertionFailedException), Description = "Expectations were not met")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Ambiguous, contentType: "application/json", bodyType: typeof(Exception), Description = "Unexpected/Unhandled Exception")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Cars/{ID}")] HttpRequestData req, string ID)
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
                Core.Model.Car car = await _storageService.Read(id);

                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "application/json");
                await response.WriteStringAsync(JsonConvert.SerializeObject(_mapper.Map<CarDTO>(car), Formatting.Indented));
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

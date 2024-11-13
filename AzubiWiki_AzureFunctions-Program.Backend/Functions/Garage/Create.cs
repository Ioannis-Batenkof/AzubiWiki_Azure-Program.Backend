using AutoMapper;
using AzubiWiki_AzureFunctions_Program.Backend.Contracts.Queries;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Interfaces;
using FluentAssertions;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Services.Validation;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Exceptions;
using FluentAssertions.Execution;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace AzubiWiki_AzureFunctions_Program.Backend.Functions.Garage
{
    public class Create
    {
        private readonly IStorageService<Core.Model.Garage> _storageService;
        private readonly IMapper _mapper;

        public Create(IStorageService<Core.Model.Garage> storageService, IMapper mapper)
        {
            _storageService = storageService;
            _mapper = mapper;
        }

        [Function("CreateGarage")]
        [OpenApiOperation(operationId: "CreateGarage", tags: new[] { "Garage" }, Visibility = OpenApiVisibilityType.Undefined, Description = "Creates a Garage object and saves it to the database.")]
        [OpenApiSecurity(schemeName: "bearer_auth", schemeType: SecuritySchemeType.Http, BearerFormat = "JWT", Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NoContent, Description = "Object was successfully created!")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.UnprocessableContent, contentType: "application/json", bodyType: typeof(UnprocessableNumberValueException), Description = "The expected value is a number but the user entered a different value type")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.ExpectationFailed, contentType: "application/json", bodyType: typeof(AssertionFailedException), Description = "Object did not pass the initial validation.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.FailedDependency, contentType: "application/json", bodyType: typeof(FileNotFoundException), Description = "Database was not loaded properly or is in maintenance")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Ambiguous, contentType: "application/json", bodyType: typeof(Exception), Description = "Unexcpected/Unhandled Exception")]
        [OpenApiRequestBody(contentType: "application/josn", bodyType: typeof(GarageQ), Required = true, Description = "This is the object required to execute a creation")]

        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Garages")] HttpRequestData req)
        {
            GarageQ garage = new();
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                garage = JsonConvert.DeserializeObject<GarageQ>(requestBody);

                garage.Should().NotBeNull();
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
                validate.Garage(garage).Should().BeTrue("Validation Status");

                garage.ID = Guid.NewGuid();
                await _storageService.Create(_mapper.Map<Core.Model.Garage>(garage));

                var response = req.CreateResponse(HttpStatusCode.NoContent);
                return response;
            }
            catch (UnprocessableNumberValueException ex)
            {
                var response = req.CreateResponse(HttpStatusCode.UnprocessableContent);
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

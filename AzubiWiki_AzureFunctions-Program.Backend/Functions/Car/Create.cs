using AzubiWiki_AzureFunctions_Program.Backend.Contracts.Queries;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Interfaces;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Services.Validation;
using AutoMapper;
using FluentAssertions.Execution;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Exceptions;

namespace AzubiWiki_AzureFunctions_Program.Backend.Functions.Car
{
    public class Create
    {
        private readonly ILogger<Create> _logger;
        private readonly IStorageService<Core.Model.Car> _storageService;
        private readonly IMapper _mapper;


        public Create(ILogger<Create> logger, IStorageService<Core.Model.Car> storageService, IMapper mapper)
        {
            _logger = logger;
            _storageService = storageService;
            _mapper = mapper;
        }

        [Function("CreateCar")]
        [OpenApiOperation(operationId: "CreateCar", tags: new[] { "Car" }, Visibility = OpenApiVisibilityType.Undefined, Description = "Creates a Car object and saves it to the database.")]
        [OpenApiSecurity(schemeName: "bearer_auth", schemeType: SecuritySchemeType.Http, BearerFormat = "JWT", Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NoContent, Description = "Object was successfully created!")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.UnprocessableContent, contentType: "application/json", bodyType: typeof(UnprocessableNumberValueException), Description = "The expected value is a number but the user entered a different value type")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.ExpectationFailed, contentType:"application/json", bodyType: typeof(AssertionFailedException), Description = "Object did not pass the initial validation.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.FailedDependency,contentType:"application/json", bodyType: typeof(FileNotFoundException), Description = "Database was not loaded properly or is in maintenance")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Ambiguous,contentType: "application/json", bodyType: typeof(Exception) ,Description = "Unexcpected/Unhandled Exception")]
        [OpenApiRequestBody(contentType: "application/josn", bodyType: typeof(CarQ), Required = true, Description = "This is the object required to execute a creation")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "cars")] HttpRequestData req)
        {
            CarQ car = new();
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                car = JsonConvert.DeserializeObject<CarQ>(requestBody);
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
                Validate validate = new Validate();
                if (validate.Car(car))
                {
                    car.ID = Guid.NewGuid();
                    await _storageService.Create(_mapper.Map<Core.Model.Car>(car));
                }

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

using AzubiWiki_AzureFunctions_Program.Backend.Core.Interfaces;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Model;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace AzubiWiki_AzureFunctions_Program.Backend.UnitTest.CarRepository.Test
{
    public class ReadAll
    {
        private readonly Mock<IFileHandler> _fileHandlerMock = new Mock<IFileHandler>();


        [Fact]
        public async Task CarRepository_NotEmptyList_Success()
        {
            _fileHandlerMock.Setup(r => r.Exists(It.IsAny<string>())).Returns(true);

            Car car = new Car { Manufacturer = "Audi", Model = "A8", Horsepower = 123, ID = Guid.NewGuid(), Year = 2022 };
            List<Car> cars = new List<Car>();
            cars.Add(car);
            string jsonContent = JsonConvert.SerializeObject(cars);

            _fileHandlerMock.Setup(r => r.ReadAllTextAsync(It.IsAny<string>())).Returns(Task.FromResult(jsonContent));

            Repositories.CarRepository repo = new(_fileHandlerMock.Object);

            cars = new();
            cars = await repo.ReadAll();

            Assert.NotEmpty(cars);
        }


        [Fact]
        public async Task CarRepository_EmptyList_AssertionFailedException()
        {
            List<Car> cars = new();
            string jsonContent = JsonConvert.SerializeObject(new List<Car>());

            _fileHandlerMock.Setup(r => r.Exists(It.IsAny<string>())).Returns(true);
            _fileHandlerMock.Setup(r => r.ReadAllTextAsync(It.IsAny<string>())).Returns(Task.FromResult(jsonContent));

            Repositories.CarRepository repo = new(_fileHandlerMock.Object);

            try
            {
                cars = new();
                cars = await repo.ReadAll();
            }
            catch (Exception ex)
            {
                Assert.IsType<XunitException>(ex);
            }
        }



        [Fact]
        public async Task CarRepository_FileDoesNotExist_FileNotFoundException()
        {
            _fileHandlerMock.Setup(r => r.Exists(It.IsAny<string>())).Returns(false);

            Repositories.CarRepository repo = new(_fileHandlerMock.Object);

            try
            {
                await repo.ReadAll();
            }
            catch (Exception ex)
            {
                Assert.IsType<FileNotFoundException>(ex);
            }
        }
    }
}

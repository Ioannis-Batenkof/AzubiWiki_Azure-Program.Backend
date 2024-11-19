using AzubiWiki_AzureFunctions_Program.Backend.Core.Interfaces;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Model;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace AzubiWiki_AzureFunctions_Program.Backend.UnitTest.GarageRepository.UnitTest
{
    public class Update
    {
        private readonly Mock<IFileHandler> _fileHandlerMock = new Mock<IFileHandler>();


        [Fact]
        public async Task GarageRepository_ValidCredentials_Success()
        {
            _fileHandlerMock.Setup(u => u.Exists(It.IsAny<string>())).Returns(true);
            _fileHandlerMock.Setup(u => u.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            List<Garage> garages = new List<Garage> { new Garage { BelongsTo = "me", ID = Guid.NewGuid() } };

            _fileHandlerMock.Setup(u => u.ReadAllTextAsync(It.IsAny<string>())).Returns(Task.FromResult(JsonConvert.SerializeObject(garages, Formatting.Indented)));

            Garage garage = new Garage { BelongsTo = "not me" , ID = garages[0].ID };
            
            Repositories.GarageRepository repo = new(_fileHandlerMock.Object);

            await repo.Update(garage);

            _fileHandlerMock.Verify(u => u.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once());

        }


        [Fact]
        public async Task GarageRepository_ObjectNotFound_AssertionFailedException()
        {
            _fileHandlerMock.Setup(u => u.Exists(It.IsAny<string>())).Returns(true);
            _fileHandlerMock.Setup(u => u.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            List<Garage> garages = new List<Garage> { new Garage { BelongsTo = "me", ID = Guid.NewGuid() } };

            _fileHandlerMock.Setup(u => u.ReadAllTextAsync(It.IsAny<string>())).Returns(Task.FromResult(JsonConvert.SerializeObject(garages, Formatting.Indented)));

            Garage garage = new Garage { BelongsTo = "not me", Cars = new List<Car> { new Car { ID = Guid.NewGuid(), Horsepower = 123, Manufacturer = "aaa", Model = "1", Year = 2002 } } };

            Repositories.GarageRepository repo = new(_fileHandlerMock.Object);

            try
            {
                await repo.Update(garage);
            }
            catch (Exception ex)
            {
                Assert.IsType<XunitException>(ex);
            }
        }



        [Fact]
        public async Task GarageRepository_FileDoesNotExist_FileNotFoundException()
        {
            _fileHandlerMock.Setup(u => u.Exists(It.IsAny<string>())).Returns(false);

            Repositories.GarageRepository repo = new(_fileHandlerMock.Object);

            try
            {
                await repo.Update(new Garage());
            }
            catch (Exception ex)
            {
                Assert.IsType<FileNotFoundException>(ex);
            }
        }
    }
}

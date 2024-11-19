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
    public class ReadAll
    {
        private readonly Mock<IFileHandler> _fileHandlerMock = new Mock<IFileHandler>();


        [Fact]
        public async Task GarageRepository_NotEmptyList_Success()
        {
            _fileHandlerMock.Setup(r => r.Exists(It.IsAny<string>())).Returns(true);

            Garage garage = new Garage { BelongsTo = "me", ID = Guid.NewGuid() };
            List<Garage> garages = new List<Garage> { garage };

            _fileHandlerMock.Setup(r => r.ReadAllTextAsync(It.IsAny<string>())).Returns(Task.FromResult(JsonConvert.SerializeObject(garages, Formatting.Indented)));

            Repositories.GarageRepository repo = new(_fileHandlerMock.Object);

            garages = new();
            garages = await repo.ReadAll();

            Assert.NotEmpty(garages);
        }


        [Fact]
        public async Task GarageRepository_EmptyList_AssertionFailedException()
        {
            List<Garage> garages = new();

            _fileHandlerMock.Setup(r => r.Exists(It.IsAny<string>())).Returns(true);
            _fileHandlerMock.Setup(r => r.ReadAllTextAsync(It.IsAny<string>())).Returns(Task.FromResult(JsonConvert.SerializeObject(garages, Formatting.Indented)));

            Repositories.GarageRepository repo = new(_fileHandlerMock.Object);

            try
            {
                garages = await repo.ReadAll();
            }
            catch (Exception ex)
            {
                Assert.IsType<XunitException>(ex);
            }
        }


        [Fact]
        public async Task GarageRepository_FileDoesNotExist_AssertionFailedException()
        {
            _fileHandlerMock.Setup(r => r.Exists(It.IsAny<string>())).Returns(false);

            Repositories.GarageRepository repo = new(_fileHandlerMock.Object);

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

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

namespace AzubiWiki_AzureFunctions_Program.Backend.UnitTest.GarageRepository.Test
{
    public class Delete
    {
        private readonly Mock<IFileHandler> _fileHandlerMock = new Mock<IFileHandler>();


        [Fact]
        public async Task GarageRepository_ValidCredentials_Successful()
        {
            _fileHandlerMock.Setup(d => d.Exists(It.IsAny<string>())).Returns(true);
            _fileHandlerMock.Setup(d => d.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            List<Garage> garages = new List<Garage> { new Garage { BelongsTo = "me", ID = Guid.NewGuid() }, new Garage { ID = Guid.NewGuid(), BelongsTo = "not me" } };

            _fileHandlerMock.Setup(d => d.ReadAllTextAsync(It.IsAny<string>())).Returns(Task.FromResult(JsonConvert.SerializeObject(garages, Formatting.Indented)));

            Repositories.GarageRepository repo = new(_fileHandlerMock.Object);

            await repo.Delete(garages[0].ID);

            _fileHandlerMock.Verify(d => d.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        }


        [Fact]
        public async Task GarageRepository_FileDoesNotExist_FileNotFoundException()
        {
            _fileHandlerMock.Setup(d => d.Exists(It.IsAny<string>())).Returns(false);

            Repositories.GarageRepository repo = new(_fileHandlerMock.Object);

            try
            {
                await repo.Delete(Guid.NewGuid());
            }
            catch (Exception ex)
            {
                Assert.IsType<FileNotFoundException>(ex);
            }
        }


        [Fact]
        public async Task GarageRepository_ObjectNotFound_AssertionFailedException()
        {
            _fileHandlerMock.Setup(d => d.Exists(It.IsAny<string>())).Returns(true);
            _fileHandlerMock.Setup(d => d.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            List<Garage> garages = new List<Garage> { new Garage { BelongsTo = "me", ID = Guid.NewGuid() }, new Garage { ID = Guid.NewGuid(), BelongsTo = "not me" } };

            _fileHandlerMock.Setup(d => d.ReadAllTextAsync(It.IsAny<string>())).Returns(Task.FromResult(JsonConvert.SerializeObject(garages, Formatting.Indented)));

            Repositories.GarageRepository repo = new(_fileHandlerMock.Object);

            try
            {
                await repo.Delete(Guid.NewGuid());
            }
            catch (Exception ex)
            {
                Assert.IsType<XunitException>(ex);
            }
        }
    }
}

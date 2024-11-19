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
    public class Read
    {
        private readonly Mock<IFileHandler> _mockFileHandler = new Mock<IFileHandler>();


        [Fact]
        public async Task GarageRepository_ObjectFound_Successful()
        {
            List<Garage> garages = new List<Garage> { new Garage { BelongsTo = "me", ID = Guid.NewGuid() }, new Garage { BelongsTo = "you", ID = Guid.NewGuid() } };

            _mockFileHandler.Setup(r => r.Exists(It.IsAny<string>())).Returns(true);
            _mockFileHandler.Setup(r => r.ReadAllTextAsync(It.IsAny<string>())).Returns(Task.FromResult(JsonConvert.SerializeObject(garages, Formatting.Indented)));

            Repositories.GarageRepository repo = new(_mockFileHandler.Object);

            Garage garage = await repo.Read(garages[0].ID);

            Assert.Equal("me", garage.BelongsTo);
        }


        [Fact]
        public async Task GarageRepository_ObjectNotFound_AssertionFailedException()
        {
            List<Garage> garages = new List<Garage> { new Garage { BelongsTo = "me", ID = Guid.NewGuid() }, new Garage { BelongsTo = "you", ID = Guid.NewGuid() } };

            _mockFileHandler.Setup(r => r.Exists(It.IsAny<string>())).Returns(true);
            _mockFileHandler.Setup(r => r.ReadAllTextAsync(It.IsAny<string>())).Returns(Task.FromResult(JsonConvert.SerializeObject(garages, Formatting.Indented)));

            Repositories.GarageRepository repo = new(_mockFileHandler.Object);

            try
            {
                await repo.Read(Guid.NewGuid());
            }
            catch (Exception ex)
            {
                Assert.IsType<XunitException>(ex);
            }
        }


        [Fact]
        public async Task GarageRepository_FileDoesNotExist_FileNotFoundException()
        {
            List<Garage> garages = new List<Garage> { new Garage { BelongsTo = "me", ID = Guid.NewGuid() }, new Garage { BelongsTo = "you", ID = Guid.NewGuid() } };

            _mockFileHandler.Setup(r => r.Exists(It.IsAny<string>())).Returns(false);

            Repositories.GarageRepository repo = new(_mockFileHandler.Object);

            try
            {
                await repo.Read(Guid.NewGuid());
            }
            catch (Exception ex)
            {
                Assert.IsType<FileNotFoundException>(ex);
            }
        }
    }
}

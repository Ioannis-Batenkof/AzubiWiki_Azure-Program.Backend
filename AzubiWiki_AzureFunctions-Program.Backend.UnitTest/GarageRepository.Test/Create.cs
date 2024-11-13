using AzubiWiki_AzureFunctions_Program.Backend.Core.Interfaces;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Model;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzubiWiki_AzureFunctions_Program.Backend.UnitTest.GarageRepository.Test
{
    public class Create
    {
        private readonly Mock<IFileHandler> _mockFileHandler = new Mock<IFileHandler>();


        [Fact]
        public async Task GarageRepository_FileExistsValidCredentials_Successful()
        {
            Garage garage = new Garage { BelongsTo = "aga", ID = Guid.NewGuid() };
            List<Garage> garages = new List<Garage>() { garage };

            _mockFileHandler.Setup(c => c.Create(It.IsAny<string>())).Returns(It.IsAny<FileStream>);
            _mockFileHandler.Setup(c => c.Exists(It.IsAny<string>())).Returns(true);
            _mockFileHandler.Setup(c => c.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            _mockFileHandler.Setup(c => c.ReadAllTextAsync(It.IsAny<string>())).Returns(Task.FromResult(JsonConvert.SerializeObject(garages, Formatting.Indented)));

            Repositories.GarageRepository repo = new(_mockFileHandler.Object);

            await repo.Create(garage);

            _mockFileHandler.Verify(c => c.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }


        [Fact]
        public async Task GarageRepository_FileDoesNotExistValidCredetials_Successful()
        {
            Garage garage = new Garage { BelongsTo = "aga", ID = Guid.NewGuid() };

            _mockFileHandler.Setup(c => c.Create(It.IsAny<string>())).Returns(It.IsAny<FileStream>);
            _mockFileHandler.Setup(c => c.Exists(It.IsAny<string>())).Returns(false);
            _mockFileHandler.Setup(c => c.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            Repositories.GarageRepository repo = new(_mockFileHandler.Object);

            await repo.Create(garage);

            _mockFileHandler.Verify(c => c.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        }
    }
}

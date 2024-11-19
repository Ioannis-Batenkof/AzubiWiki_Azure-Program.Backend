using AzubiWiki_AzureFunctions_Program.Backend.Core.Interfaces;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Model;
using FluentAssertions;
using Newtonsoft.Json;

namespace AzubiWiki_AzureFunctions_Program.Backend.Repositories
{
    public class GarageRepository : IStorageService<Garage>
    {
        private readonly string FilePath = Environment.GetEnvironmentVariable("GarageFile");
        private readonly IFileHandler _fileHandler;

        public GarageRepository(IFileHandler fileHandler)
        {
            _fileHandler = fileHandler;
        }


        public async Task Create(Garage garage)
        {
            List<Garage> garages = new List<Garage>();
            string jsonContent;

            if (_fileHandler.Exists(FilePath))
            {
                jsonContent = await _fileHandler.ReadAllTextAsync(FilePath);
                garages = JsonConvert.DeserializeObject<List<Garage>>(jsonContent);

                if (garages == null)
                {
                    garages = new List<Garage>();
                }
            }
            else
            {
                _fileHandler.Create(FilePath);
            }

            garages.Add(garage);

            jsonContent = JsonConvert.SerializeObject(garages, Formatting.Indented);
            await _fileHandler.WriteAllTextAsync(FilePath, jsonContent);
        }



        public async Task<List<Garage>> ReadAll()
        {
            if (_fileHandler.Exists(FilePath))
            {
                List<Garage> garages = new();

                string jsonContent = await _fileHandler.ReadAllTextAsync(FilePath);
                garages = JsonConvert.DeserializeObject<List<Garage>>(jsonContent);

                garages.Should().NotBeNullOrEmpty().And.HaveCountGreaterThan(0);

                for (int i = 0; i < garages.Count; i++)
                {
                    if (garages[i].Cars != null && garages[i].Cars.Count > 0)
                    {
                        int j = 0;
                        try
                        {
                            CarRepository carRepository = new CarRepository(_fileHandler);
                            for (j = 0; j < garages[i].Cars.Count; j++)
                            {
                                garages[i].Cars[j] = await carRepository.Read(garages[i].Cars[j].ID);
                            }
                        }
                        catch
                        {
                            garages[i].Cars.RemoveAt(j);
                        }
                    }
                }

                return garages;
            }
            else
            {
                throw new FileNotFoundException();
            }
        }



        public async Task<Garage> Read(Guid ID)
        {
            if (_fileHandler.Exists(FilePath))
            {
                List<Garage> garages = await ReadAll();

                Garage garage = garages.FirstOrDefault(g => g.ID == ID);

                garage.Should().NotBeNull();

                return garage;
            }
            else
            {
                throw new FileNotFoundException();
            }
        }



        public async Task Update(Garage newGarage)
        {
            if (_fileHandler.Exists(FilePath))
            {
                List<Garage> garages = await ReadAll();
                bool found = false;

                for (int i = 0; i < garages.Count; i++)
                {
                    if (garages[i].ID == newGarage.ID)
                    {
                        if (newGarage.Cars != null)
                        {
                            garages[i].Cars = newGarage.Cars;
                        }
                        else
                        {
                            if (garages[i].BelongsTo != newGarage.BelongsTo)
                            {
                                if (!String.IsNullOrWhiteSpace(newGarage.BelongsTo))
                                {
                                    garages[i].BelongsTo = newGarage.BelongsTo;
                                }
                            }
                        }

                        found = true;
                        break;
                    }
                }

                found.Should().BeTrue("It should have found the Garage");

                string jsonContent = JsonConvert.SerializeObject(garages, Formatting.Indented);
                await _fileHandler.WriteAllTextAsync(FilePath, jsonContent);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }



        public async Task DeleteAll()
        {
            if (_fileHandler.Exists(FilePath))
            {
                List<Garage> garages = await ReadAll();
                foreach (Garage garage in garages)
                {
                    if (garage.Cars != null && garage.Cars.Count > 0)
                    {
                        try
                        {
                            CarRepository repo = new CarRepository(_fileHandler);
                            foreach (Car car in garage.Cars)
                            {
                                car.Available = false;
                                await repo.Update(car);
                            }
                        }
                        catch
                        {
                            //skip
                        }


                    }
                }

                _fileHandler.Delete(FilePath);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }



        public async Task Delete(Guid ID)
        {
            if (_fileHandler.Exists(FilePath))
            {
                List<Garage> garages = await ReadAll();
                bool found = false;

                for (int i = 0; i < garages.Count; i++)
                {
                    if (garages[i].ID == ID)
                    {
                        if (garages[i].Cars != null && garages[i].Cars.Count > 0)
                        {
                            try
                            {
                                CarRepository repo = new(_fileHandler);
                                foreach (Car car in garages[i].Cars)
                                {
                                    car.Available = true;
                                    await repo.Update(car);
                                }
                            }
                            catch
                            {
                                //skip
                            }

                        }

                        garages.RemoveAt(i);
                        found = true;
                        break;
                    }
                }

                found.Should().BeTrue("It found the object to delete");

                string jsonContent = JsonConvert.SerializeObject(garages, Formatting.Indented);
                await _fileHandler.WriteAllTextAsync(FilePath, jsonContent);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }



        public async Task AddCarToGarage(Car car, Guid id)
        {
            if (_fileHandler.Exists(FilePath))
            {
                List<Garage> garages = await ReadAll();
                bool found = false;

                for (int i = 0; i < garages.Count; i++)
                {
                    if (garages[i].ID == id)
                    {
                        garages[i].Cars.Add(car);
                        found = true;
                        break;
                    }
                }

                found.Should().BeTrue("It found the specified garage");

                string jsonContent = JsonConvert.SerializeObject(garages, Formatting.Indented);
                await _fileHandler.WriteAllTextAsync(FilePath, jsonContent);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }



        public async Task RemoveCarFromGarage(Guid cid, Guid gid)
        {
            if (_fileHandler.Exists(FilePath))
            {
                List<Garage> garages = await ReadAll();
                bool garageFound = false;
                bool carFound = false;

                for (int i = 0; i < garages.Count; i++)
                {
                    if (garages[i].ID == gid)
                    {
                        for (int j = 0; j < garages[i].Cars.Count; j++)
                        {
                            if (garages[i].Cars[j].ID == cid)
                            {
                                garages[i].Cars.RemoveAt(j);
                                carFound = true;
                                break;
                            }
                        }
                        garageFound = true;
                        break;
                    }
                }

                garageFound.Should().BeTrue("It should have found the specified garage");
                carFound.Should().BeTrue("It should have found the specified car");

                string jsonContent = JsonConvert.SerializeObject(garages, Formatting.Indented);
                await _fileHandler.WriteAllTextAsync(FilePath, jsonContent);

            }
            else
            {
                throw new FileNotFoundException();
            }
        }
    }
}

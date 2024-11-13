using AzubiWiki_AzureFunctions_Program.Backend.Core.Interfaces;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Model;
using Newtonsoft.Json;
using FluentAssertions;

namespace AzubiWiki_AzureFunctions_Program.Backend.Repositories
{
    public class CarRepository : IStorageService<Car>
    {
        private readonly string FilePath = Environment.GetEnvironmentVariable("CarFile");
        private readonly IFileHandler _fileHandler;

        public CarRepository(IFileHandler fileHandler)
        {
            _fileHandler = fileHandler;
        }

        public async Task Create(Car car)
        {
            car.Available = true;
            List<Car> cars = new();
            string jsonContent;

            if (_fileHandler.Exists(FilePath))
            {
                jsonContent = await _fileHandler.ReadAllTextAsync(FilePath);
                cars = JsonConvert.DeserializeObject<List<Car>>(jsonContent);

                if (cars == null)
                {
                    cars = new List<Car>();
                }
            }
            else
            {
                _fileHandler.Create(FilePath);
            }

            cars.Add(car);

            jsonContent = JsonConvert.SerializeObject(cars, Formatting.Indented);
            await _fileHandler.WriteAllTextAsync(FilePath, jsonContent);
        }


        public async Task<List<Car>> ReadAll()
        {
            if (_fileHandler.Exists(FilePath))
            {
                List<Car> cars = new();

                string jsonContent = await _fileHandler.ReadAllTextAsync(FilePath);
                cars = JsonConvert.DeserializeObject<List<Car>>(jsonContent);

                cars.Should().NotBeNull().And.HaveCountGreaterThan(0);

                return cars;
            }
            else
            {
                throw new FileNotFoundException();
            }
        }


        public async Task<Car> Read(Guid ID)
        {
            if (_fileHandler.Exists(FilePath))
            {
                List<Car> cars = await ReadAll();

                Car car = cars.FirstOrDefault(i => i.ID == ID);

                car.Should().NotBeNull();

                return car;
            }
            else
            {
                throw new FileNotFoundException();
            }
        }


        public async Task Update(Car newCar)
        {
            if (_fileHandler.Exists(FilePath))
            {
                List<Car> cars = await ReadAll();
                bool found = false;

                for (int i = 0; i < cars.Count; i++)
                {
                    if (cars[i].ID == newCar.ID)
                    {
                        if (!String.IsNullOrWhiteSpace(newCar.Manufacturer))
                        {
                            if (newCar.Manufacturer != cars[i].Manufacturer)
                            {
                                cars[i].Manufacturer = newCar.Manufacturer;
                            }
                        }

                        if (!String.IsNullOrWhiteSpace(newCar.Model))
                        {
                            if (newCar.Model != cars[i].Model)
                            {
                                cars[i].Model = newCar.Model;
                            }
                        }

                        if (newCar.Year != 0)
                        {
                            if (newCar.Year != cars[i].Year)
                            {
                                cars[i].Year = newCar.Year;
                            }
                        }

                        if (newCar.Horsepower != 0)
                        {
                            if (newCar.Horsepower != cars[i].Horsepower)
                            {
                                cars[i].Horsepower = newCar.Horsepower;
                            }
                        }

                        if (newCar.Available != cars[i].Available)
                        {
                            cars[i].Available = newCar.Available;
                        }

                        found = true;
                        break;
                    }
                }

                found.Should().BeTrue("It found the object");

                string jsonContent = JsonConvert.SerializeObject(cars, Formatting.Indented);
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
                try
                {
                    GarageRepository repo = new(_fileHandler);
                    List<Garage> garages = await repo.ReadAll();
                    foreach (Garage g in garages)
                    {
                        if (g.Cars != null && g.Cars.Count > 0)
                        {
                            foreach (Car c in g.Cars)
                            {
                                await repo.RemoveCarFromGarage(c.ID, g.ID);
                            }
                        }
                    }
                }
                catch
                {
                    _fileHandler.Delete(FilePath);
                }
                _fileHandler.Delete(FilePath);
            }
        }


        public async Task Delete(Guid ID)
        {
            if (_fileHandler.Exists(FilePath))
            {
                List<Car> cars = await ReadAll();
                bool found = false;

                for (int i = 0; i < cars.Count; i++)
                {
                    if (cars[i].ID == ID)
                    {
                        if (cars[i].Available == false)
                        {
                            try
                            {
                                GarageRepository repo = new(_fileHandler);
                                List<Garage> garages = await repo.ReadAll();
                                foreach (Garage garage in garages)
                                {
                                    if (garage.Cars != null && garage.Cars.Count > 0)
                                    {
                                        if (garage.Cars.Contains(cars[i]))
                                        {
                                            await repo.RemoveCarFromGarage(ID, garage.ID);
                                            break;
                                        }
                                    }

                                }
                            }
                            catch
                            {
                                //skip
                            }

                        }

                        cars.RemoveAt(i);
                        found = true;
                        break;
                    }
                }

                found.Should().BeTrue("It should've found the object");

                string jsonContent = JsonConvert.SerializeObject(cars, Formatting.Indented);
                await _fileHandler.WriteAllTextAsync(FilePath, jsonContent);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }
    }
}

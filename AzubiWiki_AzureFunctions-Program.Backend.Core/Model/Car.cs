namespace AzubiWiki_AzureFunctions_Program.Backend.Core.Model
{
    public class Car
    {
        public Guid ID { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int Horsepower { get; set; }
        public bool Available { get; set; } = true;

    }
}

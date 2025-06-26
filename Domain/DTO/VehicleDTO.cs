namespace minimalapi.domain.dto
{
    public record VehicleDTO
    {
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Color { get; set; }
        public int Year { get; set; }    
    }
}
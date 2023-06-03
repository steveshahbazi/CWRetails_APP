namespace CWRetails_API.Model.DTO
{
    public class ToppingDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public Decimal Price { get; set; } = 1;
    }
}

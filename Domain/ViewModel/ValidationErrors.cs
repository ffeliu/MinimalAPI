public record ValidationErrors
{
    public List<string> Messages { get; set; } = new();
}
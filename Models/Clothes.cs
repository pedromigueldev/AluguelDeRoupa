namespace AluguelRoupa.Models;

public record Clothes(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    decimal WashingPrice,
    byte[] Image
) : IEntity;

public static class ClothesFun
{
    extension(Clothes)
    {
        public static Clothes New(
            string Name,
            string Description,
            decimal Price,
            decimal WashingPrice,
            byte[] Image
        ) => new(Guid.CreateVersion7(), Name, Description, Price, WashingPrice, Image);
    }
}
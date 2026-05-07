using System.ComponentModel.DataAnnotations;
using AluguelRoupa.Models;

namespace AluguelRoupa.DTO.Clothes;

public record ClothesCreate(
    string Name,
    string Description,
    decimal Price,
    decimal WashingPrice,
    IFormFile Image
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Description))
            yield return new ValidationResult(
                "Nome e descrição não podem ser vazios",
                [nameof(Name), nameof(Description)]);
            
        if (Price <= 0)
            yield return new ValidationResult(
                "Preço deve ser maior que 0.",
                [nameof(Price)]);

        if (WashingPrice <= 0)
            yield return new ValidationResult(
                "Preço de lavagem deve ser maior que 0.",
                [nameof(WashingPrice)]);
    }
}

public static class ClothesCreateFun
{
    private static byte[] ToBytes(IFormFile file)
    {
        using var ms = new MemoryStream();
        file.CopyTo(ms);
        return ms.ToArray();
    }
    
    private static Models.Clothes NewFrom(ClothesCreate clothesCreate) => 
        Models.Clothes.New(
            clothesCreate.Name, 
            clothesCreate.Description, 
            clothesCreate.Price, 
            clothesCreate.WashingPrice,
            clothesCreate.Image is { Length: > 0 }
                ? ToBytes(clothesCreate.Image)
                : []
        );

    extension (ClothesCreate clothesCreate)
    {
        public Models.Clothes NewFromCreateWithId(Guid Id) =>
            NewFrom(clothesCreate) with { Id = Id };
    }

    extension(Models.Clothes)
    {
        public static Models.Clothes NewFromCreate(ClothesCreate clothesCreate) =>
            NewFrom(clothesCreate);
    }

}
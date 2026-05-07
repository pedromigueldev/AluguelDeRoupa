
namespace AluguelRoupa.DTO;

public record NonEmptyString(string Value)
{
    public string Value {get; init; } = !string.IsNullOrEmpty(Value) ? Value.Trim() 
        : throw new ArgumentException ("A string must be non-empty", nameof (Value));

    public static implicit operator string (NonEmptyString value) => value.Value;  
};
namespace Savana.Order.API.Requests.Params; 

public class AddressParams {
    public string? Email { get; set; }
    public string? Name { get; set; }
    public bool Enabled { get; set; } = true;
}
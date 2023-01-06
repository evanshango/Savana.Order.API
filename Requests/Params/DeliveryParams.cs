using Treasures.Common.Helpers;

namespace Savana.Order.API.Requests.Params; 

public class DeliveryParams: Pagination{
    public string? Title { get; set; }
    public bool Enabled { get; set; } = true;
}
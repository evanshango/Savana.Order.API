using Treasures.Common.Helpers;

namespace Savana.Order.API.Requests.Params; 

public class OrderParams : Pagination {
    public string? Email { get; set; }
}
namespace OrderingApp.WebApi.Domain.Models
{
    public enum OrderState
    {
        Cancelled = -1,
        Draft = 0,
        Delivered = 1,
    }
}
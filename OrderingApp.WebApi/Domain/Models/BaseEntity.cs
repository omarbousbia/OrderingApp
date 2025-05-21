namespace OrderingApp.WebApi.Domain.Models
{
    public class BaseEntity<T>
    {
        public T Id { get; set; }
    }
}
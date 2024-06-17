using System.Runtime.Serialization;

namespace Api.Model
{

    [DataContract]

    public class SuperStore
    {
        [DataMember(Order = 0)]
        public int Id { get; set; }
        [DataMember(Order = 1)]
        public string OrderId { get; set; }
        [DataMember(Order = 2)]
        public string OrderDate { get; set; }
        [DataMember(Order = 3)]
        public string ShipDate { get; set; }
        [DataMember(Order = 4)]
        public string? ShipMode { get; set; }
        [DataMember(Order = 5)]
        public string CustomerId { get; set; }
        [DataMember(Order = 6)]
        public string? Segment { get; set; }
        [DataMember(Order = 7)]
        public string? CustomerName { get; set; }
        [DataMember(Order = 8)]
        public string? Country { get; set; }
        [DataMember(Order = 9)]
        public string? City { get; set; }
        [DataMember(Order = 10)]
        public string? State { get; set; }
        [DataMember(Order = 11)]
        public string? PostCode { get; set; }
        [DataMember(Order = 12)]
        public string? Region { get; set; }
        [DataMember(Order = 13)]
        public string? ProductId { get; set; }
        [DataMember(Order = 14)]
        public string? Category { get; set; }
        [DataMember(Order = 15)]
        public string? SubCategory { get; set; }
        [DataMember(Order = 16)]

        public string? ProductName { get; set; }
        [DataMember(Order = 17)]
        public decimal Sales { get; set; }
        [DataMember(Order = 18)]
        public int Quantity { get; set; }
        [DataMember(Order = 19)]
        public decimal Discount { get; set; }
        [DataMember(Order = 20)]
        public decimal Profit { get; set; }






    }


}

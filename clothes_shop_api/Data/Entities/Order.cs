using System;
using System.Collections.Generic;

namespace clothes_shop_api.Data.Entities
{
    public partial class Order
    {
        public int Id { get; set; }
        public string? Note { get; set; }
        public decimal Total { get; set; }
        public byte Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int UserId { get; set; }
    }
}

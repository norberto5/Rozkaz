using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rozkaz.Models
{
    public class OrderModel
    {
        [Key]
        public Guid Uid { get; set; }

        public User Owner { get; set; }

        public OrderInfoModel Info { get; set; } = new OrderInfoModel();

        public string OccassionalIntro { get; set; }

        public string ExceptionsFromAnotherOrder { get; set; }

        public List<OrderCategory> Categories { get; set; } = new List<OrderCategory>();
    }

    public class OrderInfoModel
    {
        public uint OrderNumber { get; set; }

        public OrderType OrderType { get; set; }

        public string Author { get; set; }

        public UnitModel Unit { get; set; } = new UnitModel();

        public string City { get; set; }

        public DateTime Date { get; set; }
    }

    public enum OrderType
    {
        Unknown,
        Normal,
        Special
    }

    public class UnitModel
    {
        [Key]
        public Guid Uid { get; set; }

        public string NameFirstLine { get; set; }

        public string NameSecondLine { get; set; }

        public string[] SubtextLines { get; set; }
    }
}

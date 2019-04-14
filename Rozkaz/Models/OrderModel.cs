using System;
using System.Collections.Generic;

namespace Rozkaz.Models
{
    public class OrderModel
    {
        public OrderInfoModel Info { get; set; }

        public string OccassionalIntro { get; set; }

        public string ExceptionsFromAnotherOrder { get; set; }
    }

    public class OrderInfoModel
    {
        public uint OrderNumber { get; set; }

        public string Author { get; set; }

        public UnitModel Unit { get; set; }

        public string City { get; set; }

        public DateTime Date { get; set; }
    }

    public class UnitModel
    {
        public string NameFirstLine { get; set; }

        public string NameSecondLine { get; set; }

        public List<string> SubtextLines { get; set; }
    }
}

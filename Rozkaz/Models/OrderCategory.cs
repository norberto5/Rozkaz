using System.Collections.Generic;

namespace Rozkaz.Models
{
  public class OrderCategory
  {
    public OrderCategory(string name, List<OrderSubcategory> subcategories)
    {
      Name = name;
      Subcategories = subcategories;
    }

    public string Name { get; set; }

    public List<OrderSubcategory> Subcategories { get; set; }
  }

  public class OrderSubcategory
  {
    public OrderSubcategory(string name, List<SubcategoryElement> elements)
    {
      Name = name;
      Elements = elements;
    }

    public string Name { get; set; }

    public List<SubcategoryElement> Elements { get; set; }
  }

  public class SubcategoryElement
  {
    public SubcategoryElement(string description)
    {
      Description = description;
    }

    public string Description { get; set; }
  }
}

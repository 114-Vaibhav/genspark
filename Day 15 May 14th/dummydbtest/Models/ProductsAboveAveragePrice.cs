using System;
using System.Collections.Generic;

namespace dummydbtest.Models;

public partial class ProductsAboveAveragePrice
{
    public string? Productname { get; set; }

    public decimal? Unitprice { get; set; }
}

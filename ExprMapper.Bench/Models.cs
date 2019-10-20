using System;

namespace ExprMapper.Bench
{
    public class L
    {
        public static L Instance => new L
        {
            Id = 1,
            Name = "Smith",
            Year = DateTime.Now,
            Child = new L
            {
                Id = 2,
                Name = "John",
                Year = DateTime.Now.AddDays(1)
            }
        };

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Year { get; set; }
        public L Child { get; set; }
    }

    public class R
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Year { get; set; }
        public R Child { get; set; }
    }
}

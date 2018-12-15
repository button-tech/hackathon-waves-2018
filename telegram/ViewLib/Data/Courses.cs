namespace ViewLib.Data
{
    public class CoursesStamp
    {
        public double USD { get; set; }
        public double EUR { get; set; }
        public double RUB { get; set; }
    }

    public class Courses
    {
        public CoursesStamp WAVES { get; set; }
        public CoursesStamp BTC { get; set; }
        public CoursesStamp ETH { get; set; }
        public CoursesStamp LTC { get; set; }
        public CoursesStamp BCH { get; set; }
        public CoursesStamp ETC { get; set; }
    }
}
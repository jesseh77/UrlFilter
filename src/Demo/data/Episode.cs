namespace DemoApi.data
{
    public class Episode
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Original_Air_Date { get; set; }
        public string Production_Code { get; set; }
        public int Season { get; set; }
        public int Number_In_Season { get; set; }
        public int Number_In_Series { get; set; }
        public float? Us_Viewers_In_Millions { get; set; }
        public int? Views { get; set; }
        public float? Imdb_rating { get; set; }
        public int? Imdb_votes { get; set; }
        public string image_url { get; set; }
        public string video_url { get; set; }
    }
}

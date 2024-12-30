namespace www.Models.Import
{
    //public class Pg
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public string Doctype { get; set; }

    //    public List<Pg> LstPgs { get; set; } = new List<Pg>();
    //}


    // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
    public class LstPg
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Doctype { get; set; }
        public List<LstPg> LstPgs { get; set; }
    }

    //public class Root
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public string Doctype { get; set; }
    //    public List<LstPg> LstPgs { get; set; }
    //}
}

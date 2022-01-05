using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bl.Models
{
    public class Inventory
    {
        //Variables
        public int MemberId { get; set; }
        public int AccountId { get; set; }
        public int ToolId { get; set; }
        public int UpdateId { get; set; }
        public int DeleteId { get; set; }
        public string DocType { get; set; }
        public Boolean RapidInput { get; set; }


        //Visibilities
        public ItemSections ItemSection { get; set; }
        public Boolean ShowUpdateRecord { get; set; }


        //Single Objects
        public bl.EF.Item NewItem { get; set; }
        public bl.EF.Item UpdateItem { get; set; }


        //Lists
        public IEnumerable<bl.EF.Item> LstItems { get; set; }
        public List<DuplicateItems> LstDuplicateItems { get; set; }
        public IEnumerable<bl.EF.Location> LstLocations { get; set; }
        public IEnumerable<bl.EF.Location> LstBugoutLocations { get; set; }
        public IEnumerable<bl.EF.Category> LstCategories { get; set; }
        public IEnumerable<String> LstCategoryTitles { get; set; }
        public IEnumerable<bl.EF.FuelType> LstFuelTypes { get; set; }
        public IEnumerable<bl.EF.BatteryType> LstBatteryTypes { get; set; }
        public IEnumerable<bl.EF.Gender> LstGenders { get; set; }
        public IEnumerable<bl.EF.Member> LstMembers { get; set; }
        public IEnumerable<bl.EF.Season> LstSeasons { get; set; }
        public IEnumerable<bl.EF.MeasurementType> LstMeasurementTypes { get; set; }
        public IEnumerable<bl.EF.MeasurementSystem> LstMeasurementSystems { get; set; }
        public IEnumerable<bl.EF.MeasurementState> LstMeasurementStates { get; set; }
        public List<Link> LstLinks { get; set; }



        public Inventory()
        {
            NewItem = new bl.EF.Item();
            NewItem.Name = string.Empty;
            LstDuplicateItems = new List<DuplicateItems>();
            ItemSection = new ItemSections();
            LstLinks = new List<Link>();
        }
    }
}

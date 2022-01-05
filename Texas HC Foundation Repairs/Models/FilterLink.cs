using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class FilterLink
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Value { get; set; }
        public string Filter { get; set; }


        public FilterLink() { }
        public FilterLink(string _name, string _id, string _value, string _filter)
        {
            //Set values
            Name = _name;
            Id = _id;
            Value = _value;
            Filter = _filter;
        }
    }
}



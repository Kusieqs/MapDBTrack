using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDBTrack
{
    public class Customer
    {
        public int employee_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string contact_number { get; set; }
        public string description { get; set; }
        public Customer(int employee_id, string first_name, string last_name, string contact_number, string description)
        {
            this.employee_id = employee_id;
            this.first_name = first_name;
            this.last_name = last_name;
            this.contact_number = contact_number;
            this.description = description;
        }

    }

    public class Place : Customer
    {
        public int customer_id { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string postal_code { get; set; }
        public string street { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public Place(int employee_id, string first_name, string last_name, string contact_number, string description,
                 int customer_id, string province, string city, string postal_code, string street, double longitude, double latitude)
                 : base(employee_id, first_name, last_name, contact_number, description)
        {
            this.customer_id = customer_id;
            this.province = province;
            this.city = city;
            this.postal_code = postal_code;
            this.street = street;
            this.longitude = longitude;
            this.latitude = latitude;
        }

    }
}

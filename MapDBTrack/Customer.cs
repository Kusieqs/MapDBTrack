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
        public string contact_number { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string description { get; set; }

        public string email { get; set; }
        public Customer(int employee_id, string contact_number, string first_name, string last_name, string description, string email)
        {
            this.employee_id = employee_id;
            this.contact_number = contact_number;
            this.first_name = first_name;
            this.last_name = last_name;
            this.description = description;
            this.email = email;
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
        public Place(int employee_id, string contact_number, string first_name, string last_name, string description, string email,
                 int customer_id, string province, string city, string postal_code, string street, double latitude, double longitude)
                 : base(employee_id, contact_number, first_name, last_name, description, email)
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

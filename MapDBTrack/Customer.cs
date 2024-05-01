using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDBTrack
{
    class Customer
    {
        private int id;
        private int employee_id;
        private string first_name;
        private string last_name;
        private string contact_number;
        private string description;
        public Customer()
        {
            
        }

    }

    class Place : Customer
    {
        private int customer_id;
        private string province;
        private string city;
        private string postal_code;
        private string street;
        private double longitude;
        private double latitude;
        public Place() 
        {
            
        }

    }
}

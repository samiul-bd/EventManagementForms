using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EventManagementForms.Entities
{
    public class Event
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public bool IsMultipleProgramEvent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ImageUrl { get; set; }        
        public decimal Budget { get; set; }
        public int CustomerId { get; set; }


        public List<Programs> Programs { get; set; } = new List<Programs>();
    }
}

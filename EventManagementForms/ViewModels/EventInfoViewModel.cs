using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementForms.ViewModels
{
    public class EventInfoViewModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public bool IsMultipleProgramEvent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ImageUrl { get; set; }
        public byte[] ImageBinary { get; set; }
        public decimal Budget { get; set; }
        public int CustomerId { get; set; }
        public string ProgramsName { get; set; }
        public string CustomerName { get; set; }
        public string MobileNo { get; set; }

    }
}

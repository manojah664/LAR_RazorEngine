using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace LogAndReg.Models
{
    public class MemberModels
    {
        [DisplayName("Member Name")]
        public string Name { get; set; }

        //To change label title value  
        [DisplayName("Telephone / Mobile Number")]
        public string PhoneNumber { get; set; }

        //To change label title value  
        [DisplayName("Upload File")]
        public string ImagePath { get; set; }

        public string ImageName { get; set; }

        public Nullable<bool> IsModified { get; set; }

        public string ModifiedName { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }

    }
}
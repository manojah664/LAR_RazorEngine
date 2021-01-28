using System;  
using System.Collections.Generic;  
using System.ComponentModel;  
using System.ComponentModel.DataAnnotations;  
using System.Linq;  
using System.Web;
    

namespace LogAndReg.Models  
{  
    public class MemberModel  
    {  
        
  
        //To change label title value  
        [DisplayName("File Name")]  
        public string FileName { get; set; }

        [DisplayName("File Path")]
        public string FilePath { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }

    }
}  
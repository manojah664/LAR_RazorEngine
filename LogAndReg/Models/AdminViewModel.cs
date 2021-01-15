using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace LogAndReg.Models
{
    public class AdminViewModel
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email Id is Required ")]
        //  [RegularExpression(@"^([0-9a-zA-Z](?>[-.\w]*[0-9a-zA-Z])*@(?>[0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        [RegularExpression("^[a-zA-Z0-9]{8,}$")]
        // [MinLength(6, ErrorMessage = "Minimum 6 Characters Required")]
        public string Password { get; set; }
    }
}
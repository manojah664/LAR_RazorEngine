using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LogAndReg.Models
{
    public class Forgot
    {
        [Required(ErrorMessage ="Email is Required")]
         [RegularExpression(@"^([0-9a-zA-Z](?>[-.\w]*[0-9a-zA-Z])*@(?>[0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$")]

        public string Email { get; set; }

        [Display(Name ="New Password")]
        [Required(ErrorMessage ="New Password is Required")]
        [RegularExpression("^[a-zA-Z0-9]{8,}$")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name ="Confirm Password")]
        [Required(ErrorMessage ="Confirm Password is Required")]
        [RegularExpression("^[a-zA-Z0-9]{8,}$")]
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
       
    }
}
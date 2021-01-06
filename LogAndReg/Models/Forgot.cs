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
        public string Email { get; set; }

        [Display(Name ="New Password")]
        [Required(ErrorMessage ="New Password is Required")]
        [DataType(DataType.Password)]

        public string NewPassword { get; set; }

        [Display(Name ="Confirm Password")]
        [Required(ErrorMessage ="Confirm Password is Required")]
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
       
    }
}
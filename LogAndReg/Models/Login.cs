using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LogAndReg.Models
{
    public class Login
    {
        [Required(AllowEmptyStrings =false,ErrorMessage ="Email Id is Required")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings =false,ErrorMessage ="Password is Required")]
        [MinLength(6,ErrorMessage ="Minimun 6 characters Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name ="Remember Me")]
        public bool RememberMe { get; set; }

    }
}
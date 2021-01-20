using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LogAndReg.Models
{
    public class NewView
    {

        [Display(Name = "User Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "User Name is Required")]
        [RegularExpression("^[A-Z]{1}[a-zA-Z]{4,}$")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is Required")]
        // [MinLength(6, ErrorMessage = "Minimun 6 characters Required")]
        [RegularExpression("^[a-zA-Z0-9]{8,}$")]

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

    }
}
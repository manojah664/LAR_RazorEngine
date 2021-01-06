using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LogAndReg.Models
{
    [MetadataType(typeof(UserMetaData))]
    public partial class tblUse
    {

    }
    public class UserMetaData
    {

       
        [Display(Name ="User Name")]
        [Required(AllowEmptyStrings =false,ErrorMessage ="User Name is Required")]
        public string Username { get; set; }

        [Required(AllowEmptyStrings =false,ErrorMessage ="Email Id is Required ")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "User Name is Required")]
        [DataType(DataType.Password)]
        [MinLength(6,ErrorMessage ="Minimum 6 Characters Required")]
        public string Password { get; set; }

        public bool IsEmailVerified { get; set; }
        public System.Guid ActivationCode { get; set; }
    }
}
﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LogAndReg.Models
{
    public class User
    {
        public string Address { get; set; }

        [Display(Name = "User Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "User Name is Required")]
        [RegularExpression("^[A-Z]{1}[a-zA-Z]{4,}$")]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email Id is Required ")]
        [RegularExpression(@"^([0-9a-zA-Z](?>[-.\w]*[0-9a-zA-Z])*@(?>[0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        [RegularExpression("^[a-zA-Z0-9]{8,}$")]
        // [MinLength(6, ErrorMessage = "Minimum 6 Characters Required")]
        public string Password { get; set; }

        [Display(Name = "Date Of Birth")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter the Date Of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Mobile Number")]
        [Required(AllowEmptyStrings = false)]
        [RegularExpression("^[6-9]{1}[0-9]{9}$")]
        public string MobileNumber { get; set; }
        public string Gender { get; set; }
        public int Uid { get; set; }
        public Nullable<int> Countryid { get; set; }
        public Nullable<int> StateId { get; set; }
        public Nullable<int> CityId { get; set; }
        public Nullable<bool> IsEmailVerified { get; set; }
        public Nullable<System.Guid> ActivationCode { get; set; }
        public bool IsActive { get; set; }
        public string Cname { get; set; }
        public string Sname { get; set; }
        public string Cityname { get; set; }




    }
}

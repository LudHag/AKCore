using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AKCore.DataModel;

namespace AKCore.Models
{
    public class UpcommingModel
    {
        public IList Events { get; set; }
    }
}
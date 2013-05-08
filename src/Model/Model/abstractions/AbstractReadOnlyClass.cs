using System;
using System.Web;
using System.ComponentModel;
using System.Collections.Generic;
using Model.Interfaces;

namespace Model
{
    public abstract class AbstractReadOnlyClass: ICreateFields
    {
        private bool _isDeleted;

        public int ID
        {
            get; set;
        }

        public DateTime Created
        {
            get; set;
        }

        public string CreatedBy
        {
            get; set;
        }

        public bool IsDeleted
        {
            get { return _isDeleted; } 
        }
    }
}

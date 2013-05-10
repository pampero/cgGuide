using System;
using System.Web;
using System.ComponentModel;
using System.Collections.Generic;
using Model.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public abstract class AbstractUpdatableClass: ICreateUpdateDeleteFields
    {
        protected static readonly string _versionDefault = "NotSet";
        private List<BusinessRule> _businessRules = new List<BusinessRule>();
        private List<string> _validationErrors = new List<string>();

        [Key]
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

        public DateTime? Deleted
        {
            get; set;
        }

        public string DeletedBy
        {
            get; set;
        }

        public DateTime? LastUpdated
        {
            get; set;
        }

        public string LastUpdatedBy
        {
            get; set;
        }

        public bool IsDeleted
        {
            get; set;
        }

        public List<string> ValidationErrors
        {
            get { return _validationErrors; }
        }

        protected void AddRule(BusinessRule rule)
        {
            _businessRules.Add(rule);
        }


        public bool Validate()
        {
            bool isValid = true;

            _validationErrors.Clear();

            foreach (BusinessRule rule in _businessRules)
            {
                if (!rule.Validate(this))
                {
                    isValid = false;
                    _validationErrors.Add(rule.ErrorMessage);
                }
            }
            return isValid;
        }


       
    }
}

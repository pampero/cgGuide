﻿using System;
using System.Web;
using System.ComponentModel;

namespace Model
{
    public class Customer : AbstractUpdatableClass
    {
        public string Name
        {
            get;
            set;
        }

        public ContractTypeEnum DefaultContractType
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    }
}

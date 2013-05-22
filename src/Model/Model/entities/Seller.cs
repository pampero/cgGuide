using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Model.Enums;

namespace Model
{
    public class Seller: AbstractUpdatableClass
    {
        //public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public string Owner { get; set; }        

        public string WebSite { get; set; }
        public string Email { get; set; }

        public string OpenHours { get; set; }
        public string CompleteAddress { get; set; } // Dirección desnormalizada
        public string PhoneNumber1 { get; set; }
        public string PhoneNumber2 { get; set; }

        public PriceRange PriceRange { get; set; }
        public RatingRange Rating { get; set; }

        public List<Attribute> Attributes { get; set; }

        //[ForeignKey("HeadLocation")]
        public int HeadLocationId { get; set; }
        public Location HeadLocation { get; set; }
        
        public List<Location> BranchLocations { get; set; }
        public List<PaymentMethod> PaymentMethods { get; set; }

        //[ForeignKey("DefaultPicture")]
        public int? DefaultPictureId { get; set; }
        public Picture DefaultPicture { get; set; }

        public List<Picture> Pictures { get; set; }

        //[ForeignKey("Category")]
        //public int CategoryId { get; set; }
        //public Category Category { get; set; }

        //[ForeignKey("SubCategory")]
      //  public int? SubCategoryId { get; set; }
        public List<SubCategory> SubCategories { get; set; }
    }

}

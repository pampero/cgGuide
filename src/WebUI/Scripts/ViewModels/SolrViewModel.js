// Class to represent a row in the seat reservations grid
function ProductReservation(id, name, city) {
    var self = this;
    self.id = id;
    self.name = name;
    self.city = city;
}


// Overall viewmodel for this screen, along with initial state
function SellersViewModel() {
    var self = this;

    $.getJSON("http://localhost:1586/Search/GetAllSellers", { }, function (result) {
        
         $.map(result.sellers, function (seller) {
             var newSeller = new ProductReservation(seller.id, seller.name, seller.city);
            
             self.sellers.push(newSeller);
        });
        
    });
    
    self.sellers = ko.observableArray([]);
    
    self.changeLanguage = function () {
        $.ajax({
            type: 'POST',
            url: "http://localhost:1586/Search/ChangeLanguage",
            contentType: "application/json; charset=utf-8",
            traditional: true,
            data: JSON.stringify({}),
            success: function (result) {
                alert(result.Message);
            },
            error: function (xhr, ajaxOptions, error) {
                alert(xhr.status);
                alert('Error: ' + xhr.responseText);
            }
        });
    };
    
    // Operations
    self.addProduct = function () {
    //    $.ajax({
    //        type: 'POST',
    //        url: "http://localhost:1586/POC/Solr/AddProduct",
    //        contentType: "application/json; charset=utf-8",
    //        traditional: true,
    //        data: JSON.stringify({}),
    //        success: function (result) {
    //            self.products.push(new ProductReservation(result.product.id, result.product.name, result.product.manufacturer, result.product.price));
    //            alert("Producto agregado con éxito");
    //        },
    //        error: function (xhr, ajaxOptions, error) {
    //            alert(xhr.status);
    //            alert('Error: ' + xhr.responseText);
    //        }
    //    });
    };

    self.removeProduct = function (product) {

    //    $.ajax({
    //        type: 'POST',
    //        url: "http://localhost:1586/POC/Solr/Remove",
    //        contentType: "application/json; charset=utf-8",
    //        traditional: true,
    //        data: JSON.stringify({ Product: product }),
    //        success: function (result) {
    //            if (result.ok) {
    //                self.products.remove(product);
    //                alert("Producto eliminado con éxito");
    //            }
    //            else {
    //                alert("Error al eliminar");
    //            }
    //        },
    //        error: function (xhr, ajaxOptions, error) {
    //            alert(xhr.status);
    //            alert('Error: ' + xhr.responseText);
    //        }
    //    });
       
    };
    
   
}


ko.applyBindings(new SellersViewModel());
// Class to represent a row in the seat reservations grid
function ProductReservation(id, name, manufacturer, price) {
    var self = this;
    self.id = id;
    self.name = name;
    self.manufacturer = manufacturer;
    self.price = price;
    
    self.formattedPrice = ko.computed(function () {
        return self.price ? "$" + self.price.toFixed(2) : "None";
    });
}


// Overall viewmodel for this screen, along with initial state
function ProductsViewModel() {
    var self = this;

    $.getJSON("http://localhost:1586/Search/GetAllProducts", { }, function (result) {
        
         $.map(result.products, function (product) {
             var newProduct = new ProductReservation(product.id, product.name, product.manufacturer, product.price);
            
            self.products.push(newProduct);
        });
        
    });
    
    self.products = ko.observableArray([]);
    
    // Computed data
    self.totalPrice = ko.computed(function () {
        var total = 0;
        for (var i = 0; i < self.products().length; i++)
            total += self.products()[i].price;
        return total;
    });

    // Operations
    self.addProduct = function () {
        $.ajax({
            type: 'POST',
            url: "http://localhost:1586/POC/Solr/AddProduct",
            contentType: "application/json; charset=utf-8",
            traditional: true,
            data: JSON.stringify({}),
            success: function (result) {
                self.products.push(new ProductReservation(result.product.id, result.product.name, result.product.manufacturer, result.product.price));
                alert("Producto agregado con éxito");
            },
            error: function (xhr, ajaxOptions, error) {
                alert(xhr.status);
                alert('Error: ' + xhr.responseText);
            }
        });
    };


    self.changeLanguage = function () {
        $.ajax({
            type: 'POST',
            url: "http://localhost:1586/POC/Solr/ChangeLanguage",
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

    self.removeProduct = function (product) {

        $.ajax({
            type: 'POST',
            url: "http://localhost:1586/POC/Solr/Remove",
            contentType: "application/json; charset=utf-8",
            traditional: true,
            data: JSON.stringify({ Product: product }),
            success: function (result) {
                if (result.ok) {
                    self.products.remove(product);
                    alert("Producto eliminado con éxito");
                }
                else {
                    alert("Error al eliminar");
                }
            },
            error: function (xhr, ajaxOptions, error) {
                alert(xhr.status);
                alert('Error: ' + xhr.responseText);
            }
        });
       
    };
    
   
}


ko.applyBindings(new ProductsViewModel());
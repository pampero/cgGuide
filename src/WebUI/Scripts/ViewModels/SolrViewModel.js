// Class to represent a row in the seat reservations grid
function ProductReservation(id, name, city, attributes, categories, rating) {
    var self = this;
    self.id = id;
    self.name = name;
    self.city = city;
    self.attributes = attributes;
    self.categories = categories;
    self.rating = rating;
}


function ParallelItem(key, value) {
    var self = this;
    self.Key = key;
    self.Value = value;
    self.Checked = false;
}


// Overall viewmodel for this screen, along with initial state
function SellersViewModel() {
    var self = this;
    self.sellers = ko.observableArray([]);

    $.ajax({
        type: 'POST',
        url: "/Search/GetAllSellers",
        contentType: "application/json; charset=utf-8",
        traditional: true,
        data: JSON.stringify({ parallelsDto: self.parallels }),
        success: function (result) {
            self.sellers.removeAll();
            $.map(result.sellers, function (seller) {
                var newSeller = new ProductReservation(seller.id, seller.name, seller.city, seller.attributes, seller.categories, seller.rating);
                self.sellers.push(newSeller);
            });
        },
        error: function (xhr, ajaxOptions, error) {
            alert(xhr.status);
            alert('Error: ' + xhr.responseText);
        }
    });
   
    // PENDIENTE: HACER EL OBJETO SEARCHPARAMETERS PARA MANEJARLO ENTERAMENTE CON KNOCKOUT.
    self.clickCheckBox = function (input) {
        
        var parallelItem = new ParallelItem(input.id.split("|")[0], input.id.split("|")[1]);

        if (input.checked) {
            parallelItem.Checked = true;
        }
        else {
            parallelItem.Checked = false;
        }

        $.ajax({
            type: 'POST',
            url: "/Search/GetAllSellers",
            contentType: "application/json; charset=utf-8",
            traditional: true,
            data: JSON.stringify({ parallelItemDto: parallelItem }),
            success: function (result) {
                self.sellers.removeAll();
                $.map(result.sellers, function (seller) {
                    var newSeller = new ProductReservation(seller.id, seller.name, seller.city, seller.attributes, seller.categories, seller.rating);
                    self.sellers.push(newSeller);
                });
            },
            error: function (xhr, ajaxOptions, error) {
                alert(xhr.status);
                alert('Error: ' + xhr.responseText);
            }
        });
    };
}

var sellerViewModel = new SellersViewModel();
ko.applyBindings(sellerViewModel);
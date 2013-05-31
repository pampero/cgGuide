// Class to represent a row in the seat reservations grid
function BusinessEntity(id, name, city, attributes, categories, rating) {
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
function BusinessesViewModel() {
    var self = this;
    self.businesses = ko.observableArray([]);

    $.ajax({
        type: 'POST',
        url: "/Search/GetAllSellers",
        contentType: "application/json; charset=utf-8",
        traditional: true,
        data: JSON.stringify({ parallelsDto: self.parallels }),
        success: function (result) {
            self.businesses.removeAll();
            $.map(result.sellers, function (biz) {
                var newBusiness = new BusinessEntity(biz.id, biz.name, biz.city, biz.attributes, biz.categories, biz.rating);
                self.businesses.push(newBusiness);
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
                self.businesses.removeAll();
                $.map(result.sellers, function (biz) {
                    var newBusiness = new BusinessEntity(biz.id, biz.name, biz.city, biz.attributes, biz.categories, biz.rating);
                    self.businesses.push(newBusiness);
                });
            },
            error: function (xhr, ajaxOptions, error) {
                alert(xhr.status);
                alert('Error: ' + xhr.responseText);
            }
        });
    };
}

var businessesViewModel = new BusinessesViewModel();
ko.applyBindings(businessesViewModel);
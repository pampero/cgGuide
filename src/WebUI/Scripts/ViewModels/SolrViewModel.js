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

function SearchParameters(freeSearch, facets, parallels) {
    var self = this;
    self.FreeSearch = freeSearch;
    self.FacetCollection = facets;
    self.Parallels = parallels;
}

function KeyValuePair(key, value) {
    var self = this;
    self.Key = key;
    self.Value = value;
}

function FacetCollection(key, facets) {
    var self = this;
    self.Key = key;
    self.Facets = facets;
}

function ParallelCollection(key, parallels) {
    var self = this;
    self.Key = key;
    self.Parallels = parallels;
}

// Overall viewmodel for this screen, along with initial state
function SellersViewModel() {
    var self = this;
    var searchParameters;
    
    $.getJSON("http://localhost:1586/Search/GetAllSellers",  JSON.stringify( {  } ), function (result) {
        self.sellers.removeAll();
        
         $.map(result.sellers, function (seller) {
             var newSeller = new ProductReservation(seller.id, seller.name, seller.city, seller.attributes, seller.categories, seller.rating);
            
             self.sellers.push(newSeller);
        });
        
    });

    // PENDIENTE: HACER EL OBJETO SEARCHPARAMETERS PARA MANEJARLO ENTERAMENTE CON KNOCKOUT.
    self.clickCheckBox = function () {
        
        var keyValuePair1 = new KeyValuePair("1", "Faceta 1");
        var keyValuePair2 = new KeyValuePair("2", "Faceta 2");

        var facets = [keyValuePair1, keyValuePair2];
        var parallels = [keyValuePair1, keyValuePair2];
        
        var facetCollection = new FacetCollection("Categoria1", facets);
        
        var parallelCollection = new ParallelCollection("Categoria 2", parallels);
        
        //  searchParameters = new SearchParameters("FreeSearch", facetCollection, parallelCollection);
        searchParameters = new SearchParameters("FreeSearch", facets, parallels);

        $.ajax({
            type: 'POST',
            url: "http://localhost:1586/Search/GetAllSellers",
            contentType: "application/json; charset=utf-8",
            traditional: true,
            data: JSON.stringify({ searchParametersDto: searchParameters }),
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

    self.sellers = ko.observableArray([]);
   
}


ko.applyBindings(new SellersViewModel());
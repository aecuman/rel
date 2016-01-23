$(document).ready(function () {
    var dataSourceUrl = 'https://spatial.virtualearth.net/REST/v1/data/515d38d4d4e348d9a61c615f59704174/CoffeeShops/CoffeeShop';
    var autoCompleteDataSourceUrl = 'https://spatial.virtualearth.net/REST/v1/data/a4834fe8a10f47a1af839675eabe0951/CoffeeShopCities/CoffeeShopCities';

    var maxSuggestions = 5;
    var maxResults = 10;
    var dataLayer, infobox;

    // Load the map
    var map = new Microsoft.Maps.Map($("#myMap")[0], {
        credentials: 'ArP9YXvDyZRDN2LPAKH7XtJAK-9bOgPVlu90FXatfLdRaH9246hGOQWnnH3qbF4m'
    });

    // Create a layer to load results to.
    var dataLayer = new Microsoft.Maps.EntityCollection();
    map.entities.push(dataLayer);

    // Create to load infobox in so that it appears above all results.
    var infoboxLayer = new Microsoft.Maps.EntityCollection();
    map.entities.push(infoboxLayer);

    //Create an infobox that we can reuse.
    var infobox = new Microsoft.Maps.Infobox(new Microsoft.Maps.Location(0, 0), {
        visible: false,
        offset: new Microsoft.Maps.Point(0, 20),
        height: 180
    });
    infoboxLayer.push(infobox);

    var sessionKey;
    map.getCredentials(function (c) {
        sessionKey = c;
    });

    // Load the Bing Maps Search Manager
    var searchManager = null;

    Microsoft.Maps.loadModule('Microsoft.Maps.Search', {
        callback: function () {
            searchManager = new Microsoft.Maps.Search.SearchManager(map);
        }
    });

    function GetSuggestions(query, callback) {
        var queryUrl = autoCompleteDataSourceUrl + "?spatialFilter=bbox(-90,-180,90,180)&$format=json" +
            "&$filter=StartsWith(FormattedAddress_Lower,'" + encodeURI(query.toLowerCase()) + "')%20eq%20true" +
            "&$top=" + maxSuggestions + "&key=" + sessionKey;

        $.ajax({
            url: queryUrl,
            dataType: "jsonp",
            jsonp: "jsonp",
            success: function (data) {
                callback(data.d.results);
            },
            error: function (e) {
                alert(e.statusText);
            }
        });
    }

    function FindNearbyLocations(lat, lon) {
        dataLayer.clear();

        var queryUrl = dataSourceUrl + "?spatialFilter=nearby(" + lat + "," + lon + ",20)&$format=json" +
            "&$top=" + maxResults + "&key=" + sessionKey;

        $.ajax({
            url: queryUrl,
            dataType: "jsonp",
            jsonp: "jsonp",
            success: function (data) {
                var results = data.d.results;

                if (results.length > 0) {
                    var locs = [];

                    //Loop through results and add to map
                    for (var i = 0; i < results.length; i++) {
                        var loc = new Microsoft.Maps.Location(results[i].Latitude, results[i].Longitude);
                        var pin = new Microsoft.Maps.Pushpin(loc);
                        pin.Metadata = results[i];

                        Microsoft.Maps.Events.addHandler(pin, 'click', ShowInfobox);
                        dataLayer.push(pin);
                        locs.push(loc);
                    }

                    //Use the array of locations from the results to set the map view to show all locations.
                    if (locs.length > 1) {
                        map.setView({ bounds: Microsoft.Maps.LocationRect.fromLocations(locs), padding: 80 });
                    } else {
                        map.setView({ center: locs[0], zoom: 15 });
                    }
                }
            },
            error: function (e) {
                alert(e.statusText);
            }
        });
    }

    function ShowInfobox(e) {
        if (e.target.Metadata) {
            var data = e.target.Metadata;
            var desc = [data.StoreType, '<br/><br/>', data.AddressLine, '<br/>', data.Locality, ' ', data.AdminDistrict, ' ',
                data.PostalCode, '<br/>', data.CountryRegion, '<br/><br/>Hours: ', data.Open, ' - ', data.Close];

            if (data.IsWiFiHotSpot) {
                desc.push('<br/>Wifi Available');
            }

            infobox.setLocation(e.target.getLocation());
            infobox.setOptions({ visible: true, title: data.Name, description: desc.join('') });
        }
    }

    //Wire up auto complete functionality
    $("#searchBox").autocomplete({
        source: function (request, response) {
            GetSuggestions(request.term, function (results) {
                if (results) {
                    response($.map(results, function (item) {
                        return {
                            data: item,
                            label: item.FormattedAdress,
                            value: item.FormattedAdress
                        }
                    }));
                }
            });
        },
        select: function (event, ui) {  //Suggestion selected
            var item = ui.item.data;

            //Center map over selected location
            map.setView({ center: new Microsoft.Maps.Location(item.Latitude, item.Longitude), zoom: 11 });

            //Find nearby locations
            FindNearbyLocations(item.Latitude, item.Longitude);
        },
        minLength: 1    //Minimium number of characters before auto suggest is triggered
    });

    function GeocodeQuery() {
        var query = $("#searchBox").val();

        if (query != '') {
            searchManager.geocode({
                where: query,
                callback: function (r) {
                    if (r && r.results && r.results.length > 0) {
                        if (r.results.length == 1) {
                            //Set the map view over the geocoded location
                            map.setView({ bounds: r.results[0].bestView });

                            //Find nearby shops
                            FindNearbyLocations(r.results[0].location.latitude, r.results[0].location.longitude);
                        } else {
                            var geocodeResults = [];
                            var result;

                            //Loop through geocodie results and create a disambiguous box.
                            for (var i = 0; i < r.results.length; i++) {
                                result = r.results[i];

                                geocodeResults.push({
                                    name: result.name,
                                    latitude: result.location.latitude,
                                    longitude: result.location.longitude,
                                    bestView: result.bestView
                                });

                                $('#resultsList').append('<li rel="' + i + '">' + result.name + '</li>');
                            }

                            $('#resultsList li').click(function () {
                                var idx = $(this).attr('rel');
                                var r = geocodeResults[idx];

                                $("#searchBox").val(r.name);

                                //Set the map view over the selected location
                                map.setView({ bounds: r.bestView });

                                //Find nearby shops
                                FindNearbyLocations(r.latitude, r.longitude);

                                $('#resultsList').html('');
                                $('#resultsDialog').dialog('close');
                            });

                            $('#resultsDialog').dialog('open');
                        }
                    } else {
                        $('#resultsList').html('No results found.');
                        $('#resultsDialog').dialog('open');
                    }
                },
                error: function (e) {
                    alert(e.statusText);
                }
            });
        }
    }

    //Handle users enter key press
    $("#searchBox").keypress(function (event) {
        if (event.which == 13) {
            event.preventDefault();
            GeocodeQuery();
        } else if (event.which == 39) {
            return false;
        }
    });

    //Create the search button
    $('#searchBtn').button({
        icons: {
            primary: "ui-icon-search"
        },
        text: false
    }).click(GeocodeQuery);

    //Functionality for geocoding users query against Bing Maps
    var geocodeResults = [];

    $('#resultsDialog').dialog({
        modal: true,
        autoOpen: false,
        title: 'Geocode Results'
    });
});
a8DashboardModule.service('LocationService', ['$http', '$rootScope', function ($http, $rootScope) {

    function GetSiteNameFromQueryString() {
        var key = null;
        var value = null;
        var retStr = "Discovery1";
        var queries = {};
        queries = document.location.search.substr(1).split('&');
        if (queries.toString().search("=") === 8) {
            var i = queries.toString().split('=');
            key = i[0].toString();
            value = i[1].toString();
        }
        if (key == "SiteName" && (value == "Discovery1" || value == "Discovery2")) {
            retStr = value;
        }
        return retStr;
    }


    this.getLocations = function () {
        return $http({
            method: "GET",
            url: "http://localhost:62527/DashBoard/api/GetLocation?ConnectionString=" + GetSiteNameFromQueryString(),
            dataType: JSON
        });
    };

    
    this.DateDownload = function () {
        return $http({
            method: "GET",
            url: "http://localhost:62527/DashBoard/api/GetDate?ConnectionString=" + GetSiteNameFromQueryString(),
            dataType: JSON
        });
    };

    this.GenerateReport = function (searchObject) {
        var URL = "http://localhost:62527/DashBoard/api/GenerateReport";
        return $http.post(URL, searchObject)
    };

   
    //this.ImportTUIDisCovery = function (objFormData) {
    //    alert(objFormData['formData']);
    //    var URL = "api/SaveTUIDiscovery";
    //    return $http.post(URL, objFormData)
    //}    
}]);
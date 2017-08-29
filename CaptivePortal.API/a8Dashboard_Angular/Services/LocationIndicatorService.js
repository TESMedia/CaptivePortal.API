a8DashboardModule.service('LocationIndicatorService', ['$http', '$rootScope', function ($http, $rootScope) {

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


    function getParameterByName(name, url) {
        var key = null;
        var value = null;
        var retStr = "";
        var queries = {};
        queries = document.location.search.substr(1).split('?');
        if (queries != null) {
            var i = queries.toString().split('=');
            key = i[0].toString();
            value = i[1].toString();
        }
        if (key == "id") {
            retStr = value;
        }
        return retStr;
    }

    this.Index = function () {
        return $http({
            method: "GET",
            url: a8DashboardBaseUrl + "/locationIndicators/Index?SiteName=" + GetSiteNameFromQueryString(),
            dataType: JSON
        });
    };
    this.editDetail = function () {
        return $http({
            method: "GET",
            url: a8DashboardBaseUrl + "/locationIndicators/Edit?id=" + getParameterByName() + "&SiteName=" + GetSiteNameFromQueryString(),
            dataType: JSON
        });
    };

    this.editPop = function (id) {
        return $http({
            method: "GET",
            url: a8DashboardBaseUrl + "/locationIndicators/Edit?id=" + id + "&SiteName=" + GetSiteNameFromQueryString(),
            dataType: JSON

        });
    };

    this.deleteLocIndicator = function (AreaOfInterestId, LoctionIndicatorId) {
        var objLocation = { SiteName: GetSiteNameFromQueryString(), LoctionIndicatorId: LoctionIndicatorId }
        return $http({
            method: "POST",
            data: objLocation,
            url: a8DashboardBaseUrl + "/locationIndicators/DeleteLocationIndicator"

        });

    };
    this.deleteNeighArea = function (AreaOfInterestId, NeighBourId) {
        var objLocation = { SiteName: GetSiteNameFromQueryString(), AreaOfInterestId: AreaOfInterestId, NeighBourId: NeighBourId }
        return $http({
            method: "POST",
            data: objLocation,
            url: a8DashboardBaseUrl + "/locationIndicators/DeleteNeighBourArea"
        });
    };

    this.deleteLocation = function (AreaOfInterestId) {
        var objLocation = { SiteName: GetSiteNameFromQueryString(), AreaOfInterestId: AreaOfInterestId }
        return $http({
            method: "POST",
            data: objLocation,
            url: a8DashboardBaseUrl + "/locationIndicators/Delete"

        });
    }

    this.editLocAndNegh = function (editInfo) {
        return $http({
            method: "POST",
            data: editInfo,
            url: a8DashboardBaseUrl + "/locationIndicators/Edit"

        });
    }

}]);
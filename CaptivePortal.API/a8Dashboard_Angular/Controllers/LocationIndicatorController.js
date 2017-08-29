a8DashboardModule.controller('LocationIndicatorController', ['$scope', 'LocationIndicatorService', '$rootScope', function ($scope, LocationIndicatorService, $rootScope) {
    $scope.editInfo = [];
    $scope.getIndex = function () {
        LocationIndicatorService.Index().success(function (d) {
            $scope.locationIndiCatorDetails = d;
            //$scope.LabelDiscovery = GetSiteNameFromQueryString();
            //$scope.selectdiscovery = GetSiteNameFromQueryString();
        }, function (error) { });
    };

    $scope.editDetails = function () {
        LocationIndicatorService.editDetail().success(function (d) {
            debugger;
            $scope.editInfo = d;
            $scope.lstLength = d.lstMapLocations.length;
        }, function (error) { });
    };
    $scope.addLocationIndicator = function () {
        var locationIndicatorItem = { LoctionIndicator: "", LoctionIndicatorId: 0, AreaOfInterestId: $scope.editInfo.AreaOfInterestId, Name: null }
        $scope.editInfo.lstMapLocations.push(locationIndicatorItem);
    }

    $scope.removeLocationIndicator = function () {
        var locationIndicatorItem = { LoctionIndicator: "", LoctionIndicatorId: 0, AreaOfInterestId: 0, Name: null }
        $scope.editInfo.lstMapLocations.splice(-1, 1);
    }

    $scope.addNeighBourName = function () {
        var neighBourItem = { NeighBourName: "", NeighBourId: 0, AreaOfInterestId: $scope.editInfo.AreaOfInterestId, Name: null }
        $scope.editInfo.lstNeighBourMaps.push(neighBourItem);
    }

    $scope.removeNeighBourName = function () {
        var neighBourItem = { NeighBourName: "", NeighBourId: 0, AreaOfInterestId: 0, Name: null }
        $scope.editInfo.lstNeighBourMaps.splice(-1, 1);
    }
    $scope.edit = function (id) {
        LocationIndicatorService.editPop(id).success(function (d) {
            location.href = "EditLocation?id=" + id;
        }, function (error) { });
    };

     $scope.delete = function (id) {
        LocationIndicatorService.deleteLocation(id).success(function (d) {
            location.href = "Locations?SiteId=2";
        }, function (error) { });
    };

    $scope.deleteLocationIndicator = function (AreaOfInterestId, LoctionIndicatorId) {
        LocationIndicatorService.deleteLocIndicator(AreaOfInterestId, LoctionIndicatorId).success(function (d) {
            location.href = "EditLocation?id=" + AreaOfInterestId;
        }, function (error) { });
    };

     $scope.deleteNeighBourArea = function (AreaOfInterestId, NeighBourId) {
        LocationIndicatorService.deleteNeighArea(AreaOfInterestId, NeighBourId).success(function (d) {
            location.href = "EditLocation?id=" + AreaOfInterestId;
        }, function (error) { });
    };

    $scope.editPost = function () {
        LocationIndicatorService.editLocAndNegh($scope.editInfo).success(function (d) {
            if($scope.editInfo.lstNeighBourMaps.length==0)
            {
                var itemNeghBourList = { NeighBourName: $scope.editInfo.NeighBourName, NeighBourId: 0, AreaOfInterestId: $scope.editInfo.AreaOfInterestId, Name: null }
                $scope.editInfo.lstNeighBourMaps.push(itemNeghBourList);
                LocationIndicatorService.editLocAndNegh($scope.editInfo).success(function (d) {
                }, function (error) { });

            }
        }, function (error) { });
    };

}]);
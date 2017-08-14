//Angular Module
var a8DashboardModule = angular.module('a8DashboardModule', ['ui-notification', 'ngLoadingSpinner', 'angularSpinner', 'ui.bootstrap'])
.directive('ngFiles', ['$parse', function ($parse) {

    function fn_link(scope, element, attrs) {

        var onChange = $parse(attrs.ngFiles);

        element.on('change', function (event) {
            onChange(scope, { $files: event.target.files });
        });
        element.on('dragover', function(e) {
            e.preventDefault();
            e.stopPropagation();
        });
        element.on('dragenter', function(e) {
            e.preventDefault();
            e.stopPropagation();
        });
        element.on('drop', function (e) {
            e.preventDefault();
            e.stopPropagation();
            if (e.originalEvent.dataTransfer) {
                if (e.originalEvent.dataTransfer.files.length > 0) {
                    //upload(e.originalEvent.dataTransfer.files);
                    onChange(scope, { $files: e.originalEvent.dataTransfer.files });
                }
            }
            return false;
        });

    };

    return {
        link: fn_link
    }
}])
.filter('pagination', function () {
    return function (input, start) {
        start = +start;
        return input.slice(start);
    };
});


a8DashboardModule.config(function (NotificationProvider) {
    NotificationProvider.setOptions({
        delay: 4000,
        startTop: 20,
        startRight: 10,
        verticalSpacing: 20,
        horizontalSpacing: 20,
        positionX: 'center',
        positionY: 'top'
    });
});




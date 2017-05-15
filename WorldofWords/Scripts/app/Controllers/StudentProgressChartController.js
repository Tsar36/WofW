app.controller('StudentProgressChartController', ["$scope", "$window", "$routeParams", "GroupService", "CourseService", "EditUserProfileSevice", 
    function ($scope, $window, $routeParams, GroupService, CourseService, EditUserProfileSevice) {
    $scope.init = function () {
        $scope.isPassedValueValid = isInteger($routeParams.groupId) && isInteger($routeParams.userId);
        if ($scope.isPassedValueValid) {
            EditUserProfileSevice.getUserNameById($routeParams.userId)
                .then(function (result) {
                    $scope.userName = result;
                    GroupService.getGroupById($routeParams.groupId)
                        .then(function (response) {
                            $scope.group = response;
                            if ($scope.group == null) {
                                $scope.isPassedValueValid = false;
                            }
                            else {
                                CourseService.getCourseDetailWithUserId($scope.group.CourseId, $routeParams.userId)
                                .then(function (response) {
                                    $scope.course = response;
                                    initializeChart();
                                });
                            }
                        });
                }, function (error) {
                    $scope.isPassedValueValid = false;
                });
        }
    }

    var initializeChart = function () {
        var passedData = [];
        var remainingData = [];
        for (var key in $scope.course.WordSuites) {
            passedData.push({ y: $scope.course.WordSuites[key].Progress, label: $scope.course.WordSuites[key].Name });
            remainingData.push({ y: (100 - $scope.course.WordSuites[key].Progress), label: $scope.course.WordSuites[key].Name });
        }
        var chart = new CanvasJS.Chart("chartContainer",
	    {
	        title: {
	            text: $scope.course.Name
	        },
	        axisY: {
	            title: "percent"
	        },
	        animationEnabled: true,
	        toolTip: {
	            shared: true,
	            content: "{name}: <strong>#percent%</strong>",
	        },
	        data: [
		    {
		        type: "stackedBar100",
		        showInLegend: true,
		        name: "Done Work",
		        dataPoints: passedData
		    },
		    {
		        type: "stackedBar100",
		        showInLegend: true,
		        name: "Remaining Work",
		        dataPoints: remainingData
		    }
	        ]
	    });
        chart.render();
    };

    var isInteger = function (nVal) {
        var reg = /^\d+$/;
        return reg.test(nVal);
    };

    $scope.init();
}]);
app.controller('IndexController', ["$http", "$scope", "$window", "$modal", "IndexService", "UserService", "EditUserProfileSevice", "ConstService", "$location", "TicketService",
    function ($http, $scope, $window, $modal, IndexService, UserService, EditUserProfileSevice, ConstService, $location, TicketService) {
        var userName = EditUserProfileSevice.getUserName()
            .then(function (result) {
                userName = result;
                $scope.userName = userName;
            });
        var userInfo = UserService.getUserData();

        $scope.defaultTheme = "styles-concat-blue-theme.css";
        $scope.listOfThemes = [
            { name: "Dark theme", url: "styles-concat-dark-theme.css" },
            { name: "Blue theme", url: "styles-concat-blue-theme.css" },
        ];

        $scope.expandCourse = false;
        $scope.expandManager = false;

        //$scope.$watch('defaultTheme', function (theme) {
        //    $scope.defaultTheme = theme;
        //    console.log($scope.defaultTheme);
        //    //$scope.showThemeValue();
        //});

        $scope.showThemeValue = function () {
            console.log($scope.listOfThemes + "  - listOfThemes");
            console.log($scope.defaultTheme + "  - defaultTheme");
        }

        $scope.showIcon = true;
        $scope.showWordSuitesIcon = true;
        $scope.sideBarIsMinimized = false;
        $scope.$parent.$on('hideSideBar',function (e) {
            userInfo = null;
        })
        $scope.isLoggedIn = function () {
            
            return !!userInfo;
        };
        $scope.getName = userName;
        $scope.isStudent = userInfo && userInfo.roles.indexOf(ConstService.StudentRole) >= 0;
        $scope.isTeacher = userInfo && userInfo.roles.indexOf(ConstService.TeacherRole) >= 0;
        $scope.isAdmin = userInfo && userInfo.roles.indexOf(ConstService.AdminRole) >= 0;
        $scope.onLogoClick = function () {
            if (userInfo) {
                if (userInfo.roles.indexOf(ConstService.AdminRole) >= 0) {
                    location.replace('/Index#/Users');
                } else {
                    if (userInfo.roles.indexOf(ConstService.TeacherRole) >= 0) {
                        location.replace('/Index#/TeacherManager');
                    } else {
                        if (userInfo.roles.indexOf(ConstService.StudentRole) >= 0)
                            location.replace('/Index#/Courses/');
                    };
                };
            };
        };
        if (userInfo) {
            if (userInfo.roles.indexOf(ConstService.TeacherRole) >= 0) {
                $scope.showWordSuitesIcon = false;
            };
            $scope.userIconURL = userInfo.id + '.png';
        };
        $scope.logOut = function () {
            UserService.setUserData(null);
            $http.defaults.headers.common.Authorization = null;
            userInfo = UserService.getUserData();
            $location.replace('/Index#/');
        };
        $scope.openLoginModal = function () {
            var modalInstance = $modal.open({
                templateUrl: '../Views/LoginModal.html',
                controller: 'LoginController',
                size: ConstService.small
            });
            modalInstance.result.then();
        };
        $scope.openRegisterModal = function () {
            var modalInstance = $modal.open({
                templateUrl: '../Views/RegisterModal.html',
                controller: 'RegisterController',
                size: ConstService.small
            });
            modalInstance.result.then();
        };
        $scope.toggleSidebar = function () {
            $scope.sideBarIsMinimized = !$scope.sideBarIsMinimized;
            $scope.showIcon = !$scope.showIcon;
        };
        $scope.getBodySidebarClass = function () {
            return !$scope.isLoggedIn() ? ConstService.sidebarClosed
                : $scope.sideBarIsMinimized ? ConstService.sidebarMinimized : null;
        };
        $scope.showManagersList = function () {
            $scope.showManagerList = false;
        };
        $scope.showCoursesList = function () {
            $scope.showCourseList = false;
        };

        $scope.$on('updateTicketCounterForAdmin', function (e) {
            $scope.$apply(function () {
                TicketService.getAmountOfUnreadTicketForAdmin().then(function (response) {
                    $scope.unreadByAdmin = response;
                });
            });
        });

        $scope.$on('updateTicketCounterForUser', function (e) {
            $scope.$apply(function () {
                TicketService.getAmountOfUnreadTicketForUser().then(function (response) {
                    $scope.unreadByUser = response;
                });
            });
        });
    }]);
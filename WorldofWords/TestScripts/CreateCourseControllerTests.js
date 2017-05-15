describe('CreateCourseController', function () {
    beforeEach(module('MyApp'));
    var controller, scope, windowObj, wordSuiteObject = [{ name: "WordSuite1", Id: 6 }];

    var windowObj = { location: { href: '' } };

    beforeEach(module(function ($provide) {
        $provide.value('$window', windowObj);
    }));

    beforeEach(inject(function ($controller, $rootScope, $window) {

        WordSuiteServiceMock = {
            getWordSuitesByLanguageID: function () {
                return {
                    then: function (response) { return response(wordSuiteObject); }
                }
            }
        }

        scope = $rootScope.$new();
        controller = $controller('CreateCourseController', {
            $scope: scope,
            'WordSuiteService': WordSuiteServiceMock
        });
    }));

    it("location should be equal: 'Index#/UserCourses'", function () {
        scope.goToCourses();
        expect(scope.goToCourses).toBeDefined();
        expect(windowObj.location.href).toEqual('Index#/UserCourses');
    });

    //it("getWordSuitesForCurrentLanguage", function () {
    //    expect(scope.getWordSuitesForCurrentLanguage).toBeDefined();
    //    scope.course = { Language: { Id: 5 }};
    //    scope.getWordSuitesForCurrentLanguage();
    //    scope.$root.$digest();
        
    //    expect(scope.wordSuites).toEqual(wordSuiteObject);
    //})


});
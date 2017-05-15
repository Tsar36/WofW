describe("CreateWordTranslationController", function () {
    var $scope, WordServiceMock, array = [{wordId: 5, origWord:"Dog", translateWord:"Собака"}];

    beforeEach(module("MyApp"));
    beforeEach(inject(function ($controller, $rootScope) {
        WordServiceMock = {
            searchForWords: function(searchWord, language, wordsToSearch){
                return {
                    then: function (response) {
                        return (response(array));
                    }
                }
            },
            searchForTranslations: function (searchWord, language) {
                return {
                    then: function (response) {
                        return (response(array));
                    }
                }
            }
        }
        $scope = $rootScope.$new();
        $controller("CreateWordTranslationController", {
            $scope: $scope,
            languageId: 6,
            WordsService: WordServiceMock,
            $modalInstance: {}
        });
    }));

    it("CreateWordTranslationController that return words array", function () {
        expect($scope.searchWords).toBeDefined();
        var result = $scope.searchWords("Dog", "Eng", "lsls");
        expect(result).toEqual(array);
    });

    it("searchTranslations that return words array", function () {
        expect($scope.searchTranslations).toBeDefined();
        var result = $scope.searchWords("Dog", "Ukr");
        expect(result).toEqual(array);
    });
})
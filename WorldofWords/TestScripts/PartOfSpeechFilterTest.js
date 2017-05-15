// Written by Andriy Kusyi
// Reviewer : 
describe('Filter : partOfSpeechFilter', function () {
    var partOfSpeechFilter;
    beforeEach(module('MyApp'));
    beforeEach(inject(function (_partOfSpeechFilter_) {
        partOfSpeechFilter = _partOfSpeechFilter_;
    }
    ));
    beforeEach(
        words = [{
            Name: 'Love',
            PartOfSpeech: 1
        }, {
            Name: 'this',
            PartOfSpeech: 2
        }, {
            Name: 'filter',
            PartOfSpeech: 3
        }, {
            Name: '!!!',
            PartOfSpeech: null
        }]
    )
    it('should return words only without partsOfSpeech', function () {
        var selectedPartsOfSpeech = [
            {
                Name: "without"
            }
        ];
        var result = [
            {
                Name: '!!!',
                PartOfSpeech: null
            }
        ];
        expect(partOfSpeechFilter(words, selectedPartsOfSpeech)).toEqual(result);
    });

    it('should return selected parts of speech', function () {
        var selectedPartsOfSpeech = [
            {
                Name: "noun",
                id: 1
            }, {
                Name: "verb",
                id: 2
            }
        ],
        expected = [
            {
                Name: 'Love',
                PartOfSpeech: 1
            }, {
                Name: 'this',
                PartOfSpeech: 2
            }
        ],
        actual = partOfSpeechFilter(words, selectedPartsOfSpeech).sort(function (a, b) {
            if (a.Name === b.Name) {
                return a.PartOfSpeech > b.PartOfSpeech;
            } else {
                return a.Name > b.Name;
            }
        });
        expect(actual).toEqual(expected);
    });

    it('should return selected parts of speech and without', function () {
        var selectedPartsOfSpeech = [
            {
                Name: "without",
            }, {
                Name: "verb",
                id: 2
            }
        ],

        actual = partOfSpeechFilter(words, selectedPartsOfSpeech).sort(function (a, b) {
            if (a.Name === b.Name) {
                return a.PartOfSpeech > b.PartOfSpeech;
            } else {
                return a.Name > b.Name;
            }
        }),
        expected = [
            {
                Name: '!!!',
                PartOfSpeech: null
            }, {
                Name: 'this',
                PartOfSpeech: 2
            }
        ];

        expect(actual).toEqual(expected);
    });

});
// input arr- 10 or less words 
// output arr - words with  suiteble partsOfSpeech
app.filter('partOfSpeechFilter', function () {
    return function (inputArr, selectedPartsOfSpeech) {
        if (angular.isArray(selectedPartsOfSpeech) && inputArr !== undefined) {
            var outputArr = [];
            inputArr.forEach(function (input, index, arr) {

                var isWordWithoutPartOfSpeech = input.PartOfSpeechId === undefined || input.PartOfSpeechId == null;

                for (var i = 0; i < selectedPartsOfSpeech.length; i++) {
                    if (selectedPartsOfSpeech[i].Name === 'without' && isWordWithoutPartOfSpeech) {
                        outputArr.push(input);
                    }
                    else if (!isWordWithoutPartOfSpeech && input.PartOfSpeechId === selectedPartsOfSpeech[i].Id) {
                        outputArr.push(input);
                    }
                }
            });
            return outputArr;
        }
    }
});
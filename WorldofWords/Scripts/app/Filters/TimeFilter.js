app.filter('milisecondToDateTime', [function () {
    return function (miliseconds) {
        return new Date(1970, 0, 1).setMilliseconds(miliseconds);
    };
}]);

app.filter('seconds', [function () {
    return function (seconds) {
        return '0:0' + seconds;
    };
}]);

app.filter('TimeWithWords', [function () {
    return function (miliseconds) {
        var label = '';
        miliseconds = miliseconds / (1000 * 60);
        var minute = Math.floor(miliseconds);
        if (minute) {
            label = minute + ' minute';
            if (minute !== 1) { label += 's' };
        }
        var second = Math.round((miliseconds % 1) * 60);
        if (second) {
            label += ' ' + second + ' second';
            if (second !== 1) { label += 's' };
        }
        return label;
    };
}])

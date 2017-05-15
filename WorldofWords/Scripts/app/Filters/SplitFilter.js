app.filter('splitByComa', function () {
    return function (input) {
        return input.join(', ');
    }
})
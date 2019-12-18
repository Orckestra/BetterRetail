(function () {
    'use strict';

    var yargs = require('yargs').argv;

    if (yargs.dir) {
        var express = require('express'),
            open = require('open'),
            port = 3000;

        var app = express();
        app.use(express.static(yargs.dir));
        console.log('Serving dir ' + yargs.dir);
        console.log('Server started on port:' + port);
        app.listen(port);
    }

})();
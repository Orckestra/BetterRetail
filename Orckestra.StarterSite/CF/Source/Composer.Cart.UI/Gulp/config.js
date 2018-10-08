(function() {
    'use strict';

    module.exports = function() {
        var mainConfiguration = require('./common/config'),
            _ = require('lodash'),
            path = require('path'),
            config = {};

        config = _.merge(mainConfiguration, {});    //TODO: Add configuration for Cart Bladeset.

        return config;
    }();
})();

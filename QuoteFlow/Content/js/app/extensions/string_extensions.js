(function () {

    /**
     * String.Format equivalent. Replaces a string marked by sequential {X} 
     * arguments.
     * Usage: 'Added {0} by {1} to your collection.'.f(title, artist);
     */
    String.prototype.format = String.prototype.f = function () {
        var s = this,
            i = arguments.length;

        while (i--) {
            s = s.replace(new RegExp('\\{' + i + '\\}', 'gm'), arguments[i]);
        }
        return s;
    };
})();
/// <reference path="../jquery-1.4.4.js" />
/// <reference path="../jquery-1.4.4-vsdoc.js" />

/*!  
** Copyright (c) 2011 Ray Liang (http://www.dotnetage.com)
** Dual licensed under the MIT and GPL licenses:
** http://www.opensource.org/licenses/mit-license.php
** http://www.gnu.org/licenses/gpl.html
** 
**----------------------------------------------------------------
** title        : DJME inputFilter
** version   : 2.0.0
** modified: 2010-1-6
** depends:
**    jquery.ui.core.js
**    jquery.ui.widget.js
**----------------------------------------------------------------
*/

(function ($) {
    $.widget("dna.inputFilter", {
        options: {
            number: false,
            maxlength: false,
            decimalDigits: 0,
            mode: "validchars",
            validChars: null,
            invalidChars: null
        },
        _create: function () {
            var self = this, opts = this.options, el = this.element;
            var numChars = "1234567890";
            if (opts.number) {
                if (isNaN(parseInt(el.val())))
                    el.val(self._getDefaultValue());

                el.bind("change", function () {
                    if (isNaN(parseInt(el.val())))
                        el.val(self._getDefaultValue());
                });
            }

            if (opts.maxlength) {
                el.attr("maxlength", opts.maxlength);
            }

            el.bind("keypress", function (event) {
                var _c = event.charCode;
                if ((_c == undefined) || (_c == 0)) _c = event.keyCode;

                var ignores = [8, 9, 16, 17, 18, 19, 20, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43];
                for (var ig = 0; ig < ignores.length; ig++) {
                    if (ignores[ig] == _c)
                        return true;
                }

                if (opts.number) {
                    if (_c == 46) {
                        if (opts.decimalDigits == 0)
                            return false;

                        if (el.val().indexOf(".") > -1)
                            return false;
                        return true;
                    }
                    //var _smallPads = [96, 97, 98, 99, 100, 101, 102, 103, 104, 105];
                    var _smallPads = [48, 49, 50, 51, 52, 53, 54, 55, 56, 57];
                    for (var s = 0; s < _smallPads.length; s++) {
                        if (_smallPads[s] == _c)
                            return true;
                    }
                    return self._isValid(numChars, _c);
                }

                if (opts.mode == "validchars") {
                    if (opts.validChars)
                        return self._isValid(opts.validChars, _c);
                } else {
                    if (opts.invalidChars)
                        return !self._isValid(opts.invalidChars, _c);
                }

            });
        },
        _getDefaultValue: function () {
            var v = "0";
            if (this.options.decimalDigits) {
                v += ".";
                for (var d = 0; d < this.options.decimalDigits; d++) {
                    v = v + "0";
                }
            }
            return v;
        },
        _isValid: function (chars, code) {
            for (var i = 0; i < chars.length; i++) {
                if (chars.charCodeAt(i) == code)
                    return true;
            }
            return false;
        },
        destroy: function () {
            this.element.unbind("keypress");
            $.Widget.prototype.destroy.call(this);
        }
    });
})(jQuery);   
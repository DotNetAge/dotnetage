/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

(function ($) {
    $.widget("dna.taoRating", {
        options: {
            itemCount: 5,
            iconClass: "d-icon-star",
            hoverClass: "d-icon-star-2",
            activeClass: "d-icon-star-3",
            altField: null,
            change: null,
            url: null,
            method: "POST",
            field: "value"
        },
        _create: function () {
            var self = this, opts = this.options, el = this.element, eventPrefix = this.widgetEventPrefix;
            this._unobtrusive();

            if (opts.change)
                this.element.bind(eventPrefix + "change", opts.change);

            _starts = $("<ul/>").addClass("d-reset d-rating");
            el.after(_starts);
            el.hide();

            for (var i = 1; i < opts.itemCount + 1; i++) {
                _start = $("<li/>").appendTo(_starts)
                                           .attr("title", i)
                                           .data("tooltip-position", "top")
                                           .data("val", i);
                if (opts.iconClass) _start.addClass(opts.iconClass);

                if (i <= parseInt(el.val())) {
                    _start.isActive(true);
                    if (opts.activeClass) _start.addClass(opts.activeClass);
                }
            }

            _starts.bind("mouseleave", function () {
                if (!self.widget().isDisable() && el.attr("readonly") != "readonly")
                    self.setValue(parseInt(el.val()));
            });

            $(">li", _starts).mouseenter(function () {
                if (!self.widget().isDisable() && el.attr("readonly") != "readonly") {
                    var curVal = $(this).data("val");
                    self._setRatings(curVal, "d-state-hover");
                }
            }).click(function () {
                if (!self.widget().isDisable() && el.attr("readonly") != "readonly") {
                    var _v = $(this).data("val");
                    self.setValue(_v)
                     ._triggerEvent("change", _v);
                    self._fillValue(parseInt(el.val()));
                    self._doPost();
                }
            });

            self._fillValue(parseInt(el.val()));
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            if (el.data("count")!=undefined) {
                var amt = el.dataInt("count");
                opts.itemCount = isNaN(amt) ? 0 : amt;
            }
            if (el.data("onchange")) opts.change = new Function("event", "ui", el.data("onchange"));
            if (el.data("icon")) opts.iconClass = el.data("icon");
            if (el.data("to")) opts.altField = el.datajQuery("to");
            if (el.data("icon-hover")) opts.hoverClass = el.data("icon-hover");
            if (el.data("icon-active")) opts.activeClass = el.data("icon-active");
            if (el.data("url")) opts.url = el.data("url");
            if (el.data("method")) opts.method = el.data("method");
            if (el.data("field")) opts.field = el.data("field");
            return this;
        },
        _fillValue: function (val) {
            var opts = this.options;
            if (opts.altField) {
                if ($(opts.altField).length)
                    $(opts.altField).text(isNaN(val) ? 0 : val);
            }
        },
        _doPost: function () {
            var self = this;
            if (this.options.url) {
                var postData = {}, val = parseInt(this.element.val());
                postData[this.options.field] = isNaN(val) ? 0 : val;

                $.ajax({
                    type: this.options.method,
                    url: this.options.url,
                    data: postData
                }).done(function (value) {
                    self.setValue(parseInt(value));
                    self._fillValue(parseInt(value));
                    self.disable();
                });
            }
        },
        _setRatings: function (_value, _class) {
            var opts = this.options, _starts = $("li", this.widget());
            if (opts.hoverClass)
                _starts.removeClass(opts.hoverClass);

            _starts.removeClass("d-state-hover")
                      .each(function (i, n) {
                          var _start = $(this),
                          _rating = _start.data("val");
                          if (_rating <= _value) {
                              _start.addClass(_class);
                              if (_class == "d-state-hover") {
                                  if (opts.hoverClass) _start.addClass(opts.hoverClass);
                              }
                              if (_class == "d-state-active") {
                                  if (opts.activeClass) _start.addClass(opts.activeClass);
                              }
                          }
                      });
            return this;
        },
        setValue: function (_val) {
            var _value = !isNaN(_val) ? _val : 0;
            $(">li.d-state-active", this.widget()).removeClass("d-state-active");
            if (this.options.activeClass) {
                $(">li", this.widget()).removeClass(this.options.activeClass);
            }

            this._setRatings(_value, "d-state-active");
            this.element.val(_value);
            return this;
        },
        disable: function () {
            this.widget().isDisable(true);
            return this;
        },
        enable: function () {
            this.widget().isDisable(false);
            return this;
        },
        widget: function () {
            return this.element.next();
        },
        _triggerEvent: function (eventName, eventArgs) {
            this.element.trigger(this.widgetEventPrefix + eventName, eventArgs);
        },
        destroy: function () {
            this.widget().remove();
            this.element.show();
            $.Widget.prototype.destroy.call(this);
        }
    });

})(jQuery);   
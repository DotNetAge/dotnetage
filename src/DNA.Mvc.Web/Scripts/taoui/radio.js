/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

(function ($) {
    $.widget("dna.taoRadios", $.dna.taoDataBindable, {
        options: {
            change: null,
            value: null,
            inline: false,
            checkedIcon: "d-icon-radio-checked",
            uncheckedIcon: "d-icon-radio-unchecked"
        },
        _create: function () {
            var self = this, opts = this.options, el = this.element, eventPrefix = this.widgetEventPrefix;
            this._unobtrusive();
            if (opts.change)
                el.bind(eventPrefix + "change", opts.change);

            el.addClass("d-reset d-radios");

            //Build for select 

            //Build for buttons

            //Build for inputs
            var eles = $("[type='radio']", el);

            eles.each(function (i, n) {
                var _radio = $(n);
                _radio.attr("data-role", "none");
                _radio.wrap(opts.inline ? "<span/>" : "<div/>");
                var wrapper = _radio.parent(),
                labelElement = $("<label/>").appendTo(wrapper);
                wrapper.addClass("d-radio");
                wrapper.disableSelection();
                if (_radio.data("label"))
                    labelElement.text(_radio.data("label"));

                wrapper.click(function () {
                    if (!wrapper.isDisable() && !wrapper.isActive()) {
                        var _siblings = wrapper.siblings();
                        _siblings.removeClass("d-state-active");
                        $(".d-radio-holder", _siblings).removeClass(opts.checkedIcon).addClass(opts.uncheckedIcon);

                        $("input", _siblings).attr("checked", false);
                        _radio.attr("checked", true);
                        wrapper.isActive(true);
                        opts.value = _radio.val();

                        wrapper.find(".d-radio-holder")
                                     .removeClass(opts.uncheckedIcon)
                                     .addClass(opts.checkedIcon);

                        self._triggerEvent("change", { value: _radio.val(), label: _radio.data("label") });
                    }
                });

                if (_radio.attr("title")) {
                    wrapper.attr(_radio.attr("title"));
                    _radio.removeAttr("title");
                }

                $("<span/>").addClass("d-radio-holder")
                                     .addClass(wrapper.isActive() ? opts.checkedIcon : opts.uncheckedIcon)
                                     .prependTo(wrapper);

                _radio.hide();
            });

            if (opts.value != null && opts.value != undefined)
                this.value(opts.value);
            return el;
        },
        value: function (val) {
            var opts = this.options;
            if (val != null && val != undefined) {
                var radios = $("input", this.element);
                radios.each(function (i, n) {
                    var r = $(n);
                    if (r.val() == val.toString()) {
                        var _siblings = r.parent().siblings();
                        _siblings.removeClass("d-state-active");
                        $("input", _siblings).attr("checked", false);
                        r.attr("checked", true).parent().isActive(true);
                        r.siblings(".d-radio-holder")
                         .removeClass("d-icon-radio-unchecked")
                         .addClass("d-icon-radio-checked");

                        if (opts.value != r.val())
                            opts.value = r.val();
                        return;
                    }
                });
            }
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            if (el.data("value") != undefined) opts.value = el.data("value");
            if (el.data("change")) opts.change = new Function("event", "ui", el.data("change"));
            if (el.data("inline") != undefined) opts.inline = el.dataBool("inline");
            if (el.data("icon-checked")) opts.checkedIcon = el.data("icon-checked");
            if (el.data("icon-unchecked")) opts.uncheckedIcon = el.data("icon-unchecked");
            return $.dna.taoDataBindable.prototype._unobtrusive.call(this);
        },
        _onBindingPosition: function (data) {
            var opts = this.options;

            if (opts.mode != "new") {
                if (data.value != undefined)
                    this.element.val(data.value);
            }
            return this;
        },
        _onStateChanged: function (event, state) {
            if (state == "inserted" || state == "removed" || state == "add")
                this.element.val("");
            return this;
        },
        destroy: function () {
            $.Widget.prototype.destroy.call(this);
        }
    });


    $.widget("dna.taoRadio", {
        options: {
            //      change: null,
            //  value: null,
            label: null,
            checkedIcon: "d-icon-radio-checked",
            uncheckedIcon: "d-icon-radio-unchecked"
        },
        _create: function () {
            var self = this, opts = this.options, el = this.element, eventPrefix = this.widgetEventPrefix;
            this._unobtrusive();
            //if (opts.change)
            //    el.bind(eventPrefix + "change", opts.change);

            var _radio = el;
            _radio.wrap("<span/>");
            var wrapper = _radio.parent(),
            labelElement = $("<label/>").appendTo(wrapper);
            wrapper.addClass("d-radio");
            if (el.attr("class")) {
                wrapper.addClass(el.attr("class"));
                el.removeAttr("class");
            }
            wrapper.disableSelection();
            if (opts.label)
                labelElement.text(opts.label);

            el.bind("change", function () {
                if ($(this).attr("checked")) {
                    wrapper.find(".d-radio-holder").removeClass(opts.uncheckedIcon).addClass(opts.checkedIcon);
                }
                else {
                    wrapper.find(".d-radio-holder").removeClass(opts.checkedIcon).addClass(opts.uncheckedIcon);
                }
                wrapper.isActive(opts.value);
            });

            wrapper.click(function () {
                if (!wrapper.isDisable() && !wrapper.isActive()) {
                    var _name = el.attr("name"),
                        oe = $("input[name=" + _name + "]:checked");
                    if (oe.length)
                        oe.attr("checked", null).trigger("change");
                    el.attr("checked", "checked").trigger("change");
                }
            });

            if (_radio.attr("title")) {
                wrapper.attr(_radio.attr("title"));
                _radio.removeAttr("title");
            }

            $("<span/>").addClass("d-radio-holder")
                                 .addClass(el.prop("checked") ? opts.checkedIcon : opts.uncheckedIcon)
                                 .prependTo(wrapper);

            _radio.hide();

            //if (opts.value)
            //    opts.value = el.prop("checked");
            //if (opts.value != null && opts.value != undefined)
            //this.value(opts.value);

            return el;
        },
        //value: function (val) {
        //    var opts = this.options;
        //    if (opts.value != val) {
        //    }
        //},
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            // if (el.data("value") != undefined) opts.value = el.data("value");
            if (el.data("label") != undefined) opts.label = el.data("label");
            //  if (el.data("change")) opts.change = new Function("event", "ui", el.data("change"));
            if (el.data("icon-checked")) opts.checkedIcon = el.data("icon-checked");
            if (el.data("icon-unchecked")) opts.uncheckedIcon = el.data("icon-unchecked");
            return $.dna.taoDataBindable.prototype._unobtrusive.call(this);
        },
        destroy: function () {
            $.Widget.prototype.destroy.call(this);
        }
    });

})(jQuery);
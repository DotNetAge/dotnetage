/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

(function ($) {
    $.widget("dna.taoCheckbox", $.dna.taoDataBindable, {
        options: {
            label: null,
            boolValue: true,
            change: null,
            checkedIcon: "d-icon-checkbox-checked",
            uncheckedIcon: "d-icon-checkbox-unchecked"
        },
        _create: function () {
            var self = this, opts = this.options, el = this.element, eventPrefix = this.widgetEventPrefix;
            this._unobtrusive();
            if (opts.change)
                el.bind(eventPrefix + "change", opts.change);

            el.wrap("<span/>");
            var wrapper = el.parent(),
            labelElement = $("<label/>").appendTo(wrapper);
            wrapper.addClass("d-reset d-checkbox");
            wrapper.disableSelection();
            labelElement.disableSelection();
            if (opts.label)
                labelElement.text(opts.label);
            var _val = el.attr("checked");
            if (_val == true || _val == "checked")
                wrapper.isActive(true);

            if (el.attr("disabled") == true || el.attr("disabled") == "disabled")
                wrapper.isDisable(true);
            
            wrapper.click(function (e) {
                e.stopPropagation();
                e.preventDefault();
                if (!wrapper.isDisable()) {
                    el.attr("checked", !wrapper.isActive());
                    wrapper.isActive(!wrapper.isActive());

                    wrapper.find(".d-check-holder")
                                  .toggleClass(opts.uncheckedIcon)
                                  .toggleClass(opts.checkedIcon);

                    var _checked = el.attr('checked') ? true : false;
                    if (opts.boolValue)
                        el.val(_checked ? "True" : "False");
                    el.trigger("change");
                    self._triggerEvent("change", { value: el.val(), checked: _checked });
                }
            });

            if (el.attr("title")) {
                wrapper.attr(e.attr("title"));
                el.removeAttr("title");
            }
//.append("<span/>")
            $("<span/>").addClass("d-check-holder")
                                 .addClass(wrapper.isActive() ? opts.checkedIcon:opts.uncheckedIcon)
                                .prependTo(wrapper);


            el.hide();
        },
        check: function (val) {
            this.element.attr("checked", val);
            this.element.parent().isActive(val);
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            if (el.data("label")) opts.label = el.data("label");
            if (el.data("icon-checked")) opts.checkedIcon = el.data("icon-checked");
            if (el.data("icon-unchecked")) opts.uncheckedIcon = el.data("icon-unchecked");
            if (el.data("bool")!=undefined) opts.boolValue = el.dataBool("bool");
            if (el.data("change")) opts.change = new Function("event", "ui", el.data("change"));
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
})(jQuery);
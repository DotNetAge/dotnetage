/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

(function ($) {
    $.widget("dna.taoDatepicker", $.dna.taoDropdown, {
        options: {
            iconClass: "d-icon-calendar",
            showAnim: "slideDown",
            showOptions: { direction: 'up' },
            changeMonth: true,
            changeYear: true
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element;
            $.dna.taoDropdown.prototype._create.call(this);
            var tEl = this._textElement();
            opts.altField = el;

            opts.onSelect = function () {
                //self._triggerEvent("change", { value: el.val(), text: tEl.val() })
                self.value(el.val(), tEl.val(), true);
            };

            tEl.datepicker(opts);

            if (el.val())
                tEl.datepicker("setDate", el.val());

        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            if (el.data("append-text")) opts.appendText = el.data("append-text");
            if (el.data("change-month") != undefined) opts.changeMonth = el.data("change-month");
            if (el.data("change-year")) opts.changeYear = el.data("change-year");
            if (el.data("current-text")) opts.currentText = el.data("current-text");
            if (el.data("format")) opts.dateFormat = el.data("format");
            if (el.data("days")) opts.dayNames = el.data("days").split(",");
            if (el.data("min-days")) opts.dayNamesMin = el.data("min-days").split(",");
            if (el.data("default")) opts.defaultDate = el.data("default");
            if (el.data("dur")) opts.duration = el.data("dur");
            if (el.data("firstday") != undefined) opts.firstDay = el.data("firstday");
            if (el.data("goto-current") != undefined) opts.gotoCurrent = el.data("goto-current");
            if (el.data("hide-no-prev-next") != undefined) opts.hideIfNoPrevNext = el.data("hide-no-prev-next");
            if (el.data("rtl") != undefined) opts.isRTL = el.data("rtl");
            if (el.data("max") != undefined) opts.maxDate = el.data("max");
            if (el.data("min") != undefined) opts.minDate = el.data("min");
            if (el.data("select-other-months") != undefined) opts.selectOtherMonths = el.data("select-other-months");
            if (el.data("short- year-cutoff") != undefined) opts.shortYearCutoff = el.data("short- year-cutoff");
            if (el.data("months")) opts.monthNames = el.data("months").split(",");
            if (el.data("short-months")) opts.monthNamesShort = el.data("short-months").split(",");
            if (el.data("nav-as-dateformat") != undefined) opts.navigationAsDateFormat = el.data("nav-as-dateformat");
            if (el.data("buttonpanel") != undefined) opts.showButtonPanel = el.data("buttonpanel");
            if (el.data("show-effect")) opts.showAnim = el.data("show-effect");
            if (el.data("show-at-pos") != undefined) opts.showCurrentAtPos = el.data("show-at-pos");
            if (el.data("month-after-year") != undefined) opts.showMonthAfterYear = el.data("month-after-year");
            if (el.data("next-text")) opts.nextText = el.data("next-text");
            if (el.data("prev-text")) opts.prevText = el.data("prev-text");
            if (el.data("month-num")) opts.numberOfMonths = el.data("month-num");
            if (el.data("weeks") != undefined) opts.showWeek = el.data("weeks");
            if (el.data("week-header")) opts.weekHeader = el.data("week-header");
            //if (el.data("altfield")) opts.altField = el.data("altfield");
            return $.dna.taoDropdown.prototype._unobtrusive.call(this);
        }
    });

    $.widget("dna.taoColorDropdown", $.dna.taoDropdown, {
        options: {
            iconClass: null
        },
        _create: function () {
            var self = this, opts = this.options, el = this.element;
            //$(document).on("click", function () {
            //    self.close();
            //});

            this._unobtrusive();

            el.bind("taoColorpickerchange", function (event, value) {
                $.dna.taoDropdown.prototype.value.call(self, value, value, true);
                self._setButtonColor(value);
            });

            el.taoColorpicker(opts);

            this.options.target = el.next();
            $.dna.taoDropdown.prototype._create.call(this);
            $(".d-drop-button", this.widget()).addClass("d-color-drop-button");

            if (opts.value)
                this._setButtonColor(opts.value);

            return el;
        },
        value: function (color) {
            this.element.taoColorpicker("option", "color", color);
            this._setButtonColor(color);
            return $.dna.taoDropdown.prototype.value.call(this, color);
        },
        _setOption: function (key, value) {
            if (key == "color")
                this.value(value);
            return $.dna.taoDropdown.prototype._setOption.call(this, key, value);
        },
        _setButtonColor: function (color) {
            var dropbutton = $(".d-drop-button", this.widget()).css("background-color", color);

            if (color == "transparent")
                dropbutton.addClass("transparent");
            else {
                dropbutton.removeClass("transparent");
                dropbutton.css("color",$.invertColor(color));
            }
        },
        _onInputChanged: function (originalValue, value) {
            this.element.taoColorpicker("option", "color", value);
            this._setButtonColor(value);
        }
    });

    $.widget("dna.taoComboBox", $.dna.taoDropdown, {
        options: {
            index: -1,
            autoOpen: false,
            filter: "none",
            dropStyle: "dropdown",
            selectionType:"text"
        },
        _unobtrusive: function (element) {
            var el = element ? element : this.element, opts = this.options;
            if (el.data("index") != undefined) opts.index = el.dataInt("index");
            if (el.data("openby")) opts.dropdownEvent = el.data("openby");
            if (el.data("filter")) opts.filter = el.data("filter");
            if (el.data("selection-type")) opts.selectionType = el.data("selection-type");
            if (el.data("autoopen") != undefined) opts.autoOpen = el.dataBool("autoopen");
            if (el.data("drop-style")) {
                var _style = el.data("drop-style");
                if (_style == "menu") {
                    el.data("show-effect", "clip")
                       .data("hide-effect", "clip")
                       .data("hide-speed", 100)
                       .data("from", "middle")
                       .data("input", false);
                }

                if (_style == "dropdownlist")
                    el.data("input", false);
            }

            $.dna.taoDropdown.prototype._unobtrusive.call(this);
            $.dna.taoListbox.prototype._unobtrusive.call(this);

            return this;
        },
        _onBindingPosition: function (data) {
            if (this.options.mode != "new") {
                this.clear();
                this.element.taoListbox("option", "value", data.value);
                this.value(data.value, data.value, true);
            }
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element;
            this.options.target = el;

            $.dna.taoDropdown.prototype._create.call(this);

            el.bind("taoListboxselect", function (event, ui) {
                opts.index = ui.index;
                if (opts.selectionType=="text")
                    $.dna.taoDropdown.prototype.value.call(self, ui.value, ui.text, true);
                else
                    $.dna.taoDropdown.prototype.value.call(self, ui.value, ui.item, true);

                self.close();
            }).bind("taoListboxitemclick", function () {
                self.close();
            });

            el.taoListbox(opts);
            el.taoListbox("widget").removeAttr("style");

            var valHolder = el.taoListbox("getValHolder");
            this.wrapper.append(valHolder);
            var listbox = el.taoListbox("widget");
            listbox.show().removeClass("d-content-val");

            if (opts.value)
                this._setText(el.taoListbox("selectedText"));
            this._setFilter(opts.filter);
        },
        value: function (value, text, forceChange) {
            if (value != undefined) {
                this.element.taoListbox("select", value);
                return this;
            }
            return $.dna.taoDropdown.prototype.value.call(this, value, text, forceChange);
        },
        _onInputChanged: function (originalValue, value) {
            if (!this.element.taoListbox("find", value)) {
                this.element.taoListbox("getValueHolder").val(value);
            }
        },
        _setFilter: function (_filter) {
            var self = this, opts = this.options, wrapper = this.wrapper, input = $(">.d-content-text", wrapper);
            if (_filter == "none") {
                //input.unbind("keyup");
            }
            else {
                var _ds = self.element.taoListbox("datasource");

                if (_ds) {
                    if ($(_ds).data("private")) {
                        if (_filter == "server") {
                            _ds.taoDataSource("option", "serverFiltering", true);
                            _ds.bind("taoDataSourcechanged", function () {
                                if (self.animationWrapper.isVisible())
                                    input.focus();
                            });
                        }
                        else
                            _ds.taoDataSource("option", "serverFiltering", false);
                    }
                }

                input.bind("keyup", function (e) {
                    if (!wrapper.isDisable() && self.animationWrapper.isVisible()) {
                        if (_filter == "server") { //server filtering
                            if (_ds) {
                                self.element.taoListbox("option", "highlightfirst", false);
                                self.element.taoListbox("option", "insertMode", "replace");
                                _ds.taoDataSource("filter", self.element.taoListbox("option", "dataTextField") + "*=\"" + $(this).val() + "\"");
                            }
                        }
                        else { //local filter
                            if ($(this).val() == "") {
                                $(">ul>li", self.animationWrapper).show();
                            }
                            else {
                                var term = $(this).val();
                                if (term) {
                                    var allItems = $(">ul>li", self.animationWrapper);
                                    allItems.each(function (i, n) {
                                        var $n = $(n), itemText = $n.text().toLowerCase();
                                        if (itemText.toLowerCase().indexOf(term) > -1)
                                            $n.show();
                                        else
                                            $n.hide();
                                    });
                                }
                                else {
                                    $(">ul>li", self.animationWrapper).show();
                                }
                            }
                        }
                    }
                });
            }
            this.options.filter = _filter;
        },
        open: function () {
            var el = this.element, opts = this.options, self = this;
            $(".d-picker.d-state-active>[data-role='combobox']").taoComboBox("close");
            $(".d-picker.d-state-active>.d-content-val").taoDropdown("close");

            if (opts.autoBind == false) {
                var _ds = this.element.taoListbox("datasource");
                if (_ds) {
                    var isRemote = _ds.taoDataSource("_remoteReadable");
                    if (isRemote) {
                        _ds.taoDataSource("read").done(function () {
                            $.dna.taoDropdown.prototype.open.call(self);
                        });
                    }
                }
            }

            return $.dna.taoDropdown.prototype.open.call(this);
        },
        clear: function () {
            $.dna.taoDropdown.prototype.clear.call(this);
            this.element.taoListbox("clearSelection");
        },
        find: function (value) {
            return this.element.taoListbox("find", value);
        },
        addItem: function (data) {
            if ($.type(data) == "string")
                return this.element.taoListbox("addItem", { label: data, value: data });
            else
                return this.element.taoListbox("addItem", data);
        }
    });

    $.widget("dna.taoTimepicker", $.dna.taoComboBox, {
        options: {
            iconClass: "d-icon-clock",
            interval: 30,
            "24hours": true
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element, times = [], _val = el.val();

            if (el.data("24hours") != undefined) opts["24hours"] = el.dataBool("24hours");
            if (el.data("interval")) opts.interval = el.dataInt("interval");

            for (var h = 0; h < 24; h++) {
                var m = 0;
                while (m < 60) {
                    if (opts["24hours"]) {
                        var value = (h < 10 ? "0" + h : h) + ":" + (m < 10 ? "0" + m : m);
                        times.push({
                            label: value,
                            value: value
                        });
                    } else {
                        var _h12 = h < 12 ? h : (h - 12);
                        var label = value = (_h12 < 10 ? "0" + _h12 : _h12) + ":" + (m < 10 ? "0" + m : m);

                        if (h < 12)
                            label += " am";
                        else
                            label += " pm";

                        times.push({
                            label: label,
                            value: value
                        });
                    }
                    m += opts.interval;
                }
            }

            opts.height = 120;
            opts.datasource = times;

            $.dna.taoComboBox.prototype._create.call(this);

            //if (_val)
            //    self.value(_val);
        }
    });

})(jQuery);

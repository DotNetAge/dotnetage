(function ($) {
    $.widget("dna.taoColorpicker", {
        options: {
            palette: "websafe",
            dropdown: null,
            altField: null,
            applyTo: null,
            change: null,
            color: null,
            inputholder: true
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element,
                picker = $("<div/>").addClass("d-reset d-ui-widget d-colorpicker");
            this._unobtrusive();

            if (opts.color)
                el.val(opts.color);

            var _color = el.val();
            if (_color && _color.startsWith("rgb"))
                el.val(this._rgbToHex(_color));

            this._picker = picker;
            self.isInitialized = false;

            if (opts.change)
                el.bind(eventPrefix + "change", opts.change);

            var paletteHolder = $("<div/>").addClass("d-palette").appendTo(picker).click(function (event) {
                event.stopPropagation();
                event.preventDefault();
            });

            if (opts.palette == "websafe")
                this._createWebSafePalette(paletteHolder);
            else
                this._far = $.farbtastic($("<div/>").appendTo(paletteHolder), function (color) {
                    if (self.isInitialized)
                        self._setChange(color);
                });

            if (this.options.inputholder)
                this._createHeader(paletteHolder);

            if (opts.dropdown) {
                var dropElement = $(opts.dropdown);
                if (dropElement.length)
                    self._setDropdown(dropElement);
            }

            el.hide();
            
            el.after(picker);

            if (opts.color)
                this.setColor(opts.color);
            else
            {
                if (el.val()) {
                    this.setColor(el.val());
                }
            }

            self.isInitialized = true;
            return el;
        },
        _unobtrusive: function (element) {
            var el = element ? element : this.element, opts = this.options;
            if (el.data("palette")) opts.palette = el.data("palette");
            if (el.data("valueto")) opts.altField = el.datajQuery("valueto");
            if (el.data("applyto")) opts.applyTo = el.datajQuery("applyto");
            if (el.data("dropdown")) opts.dropdown = el.datajQuery("dropdown");
            if (el.data("color")) opts.color = el.data("color");
            if (el.data("change")) opts.change = new Function("event", "color", el.data("change"));
            return this;
        },
        _setDropdown: function (element) {
            var self = this;
            if (element) {
                if (element.length) {
                    element.bind("click", function (event) {
                        if (self.widget().isVisible())
                            return;

                        event.preventDefault();
                        event.stopPropagation();

                        $(".d-colorpicker:visible").stop().slideUp("fast");
                        var selfEle = element;
                        var closeSilder = function () {
                            self.widget().stop().hide();
                            //.slideUp("fast");
                        };
                        if (!self.widget().parent().is("body"))
                            self.widget().appendTo("body");
                        self.widget().show().css({ "z-index": $.topMostIndex() + 1 })
                                          //.stop()
                                          //.slideDown("fast")
                                          .position({
                                              of: selfEle,
                                              at: "left bottom",
                                              my: "left top"
                                          });

                        $(document).one("click", function () {
                            closeSilder();
                        });

                    });

                    this.widget().css({
                        "position": "absolute",
                        "z-index": "3000"
                        //"height": this.widget().height() + "px"
                    })
                                      .appendTo("body").hide();
                    return this;
                }
            }

            if (this.widget().parent().tagName.toLowerCase() == "body")
                this.element.after(this.widget());


            this.widget().css({ "position": "relative" })
                               .show();
            //.removeClass("d-state-expend");
            return this;
        },
        _setChange: function (value) {
            this.setColor(value);
            if (this.isInitialized) {
                this.element.trigger(this.widgetEventPrefix + "change", this.options.color);
                this.element.trigger("change");
            }
        },
        _createHeader: function (palette) {
            var self = this, placeHolder = $("<div/>").addClass("d-color-header d-inline").prependTo(palette),
            colorPreview = $("<div/>").addClass("d-color-preview d-inline").appendTo(placeHolder);

            $("<input/>").attr("data-role", "none")
                                .attr("data-width", "55")
                                .addClass("d-color-input d-inline")
                                .appendTo(placeHolder)
                .click(function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                })
            .bind("keyup", function (e) {
                if (e.keyCode == 13) {
                    self._setChange($(this).val());
                }
            });

            $("<span/>").addClass("d-color-clear d-inline")
                                .appendTo(placeHolder)
                                .click(function () {
                                    self.clear();
                                });
            placeHolder.taoUI();
        },
        _createWebSafePalette: function (palette) {
            var self = this, paletteHolder = $("<div/>").addClass("d-palette").appendTo(palette),
            colors = this._getWebsafeColours();
            for (var i = 0; i < colors.length; i++) {
                $("<div/>").css({ "background-color": colors[i] })
                                 .data("color", colors[i])
                                 .appendTo(paletteHolder)
                                 .click(function () {
                                     self._setChange($(this).data("color"));
                                 });
            }
            paletteHolder.addClass("websafe");
        },
        _rgbToHex: function (str) {
            str = str.replace(/rgb\(|\)/g, "").split(",");
            str[0] = parseInt(str[0], 10).toString(16).toLowerCase();
            str[1] = parseInt(str[1], 10).toString(16).toLowerCase();
            str[2] = parseInt(str[2], 10).toString(16).toLowerCase();
            str[0] = (str[0].length == 1) ? '0' + str[0] : str[0];
            str[1] = (str[1].length == 1) ? '0' + str[1] : str[1];
            str[2] = (str[2].length == 1) ? '0' + str[2] : str[2];
            return ('#' + str.join(""));
        },
        _getWebsafeColours: function () {
            var colours = [];
            var hex;
            var parts = ["00", "33", "66", "99", "cc", "ff"];
            for (var i = 0; i < 216; i++) {
                colours.push("#" +
                        parts[Math.floor(i / 36)] +
                        parts[Math.floor((i / 6) % 6)] +
                        parts[i % 6]);
            }
            return colours;
        },
        _setOption: function (key, value) {
            if (key == "color") {
                this.setColor(value);
                return this;
            }

            if (key == "dropdown") {
                this._setDropdown($(value));
                return this;
            }

            return $.Widget.prototype._setOption.call(this, key, value);
        },
        setColor: function (value) {
            var opts = this.options, color = value, preview = $(".d-color-preview", this.widget());
            if (color.indexOf("rgb") > -1)
                color = this._rgbToHex(value);

            if (this.isClear) {
                this.element.val("transparent");
                opts.color = "transparent";
            }
            else {
                this.element.val(color);
                opts.color = color;
            }

            if (this._far)
                this._far.setColor(color);

            preview.css("background-color", opts.color);
            if (opts.color && opts.color != "transparent")
                preview.css("color", $.invertColor(opts.color));

            if ($(".d-color-input", this._picker).length)
                $(".d-color-input", this._picker).val(opts.color);

            if (opts.altField) {
                var alt = $(opts.altField);
                if (alt.length) {
                    var tag = alt[0].tagName.toLowerCase();
                    if (tag == "input" || tag == "select" || tag == "textarea")
                        alt.val(opts.color);
                    else
                        alt.text(opts.color);
                }
            }

            if (opts.applyTo) {
                var app = $.isFunction(opts.applyTo) ? opts.applyTo() : $(opts.applyTo);
                if (app.length) {
                    app.css("background-color", opts.color);
                    if (opts.color && opts.color != "transparent")
                        app.css("color", $.invertColor(opts.color));
                }
            }
        },
        clear: function () {
            this.isClear = true;
            this._setChange("transparent");
            this.isClear = false;
        },
        widget: function () {
            return this._picker;
        },
        disable: function () {
            this.widget().isDisable(true);
            return this;
        },
        enable: function () {
            this.widget().isDisable(false);
            return this;
        },
        destroy: function () {
            this.element.unbind(this.widgetEventPrefix + "change");
            this._picker.remove();
            this.element.show();
            $.Widget.prototype.destroy.call(this);
        }
    });
})(jQuery);
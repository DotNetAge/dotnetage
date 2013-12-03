/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

(function ($) {
    $.widget("dna.taoDropdown", $.dna.taoDataBindable, {
        options: {
            width: "auto",
            height: "auto",
            iconClass: " d-icon-caret-down",
            iconImg: null,
            value: null,
            text: null,
            placeholder: null,
            show: {
                effect: "slide", //'blind', 'clip', 'drop', 'explode', 'fold', 'puff', 'slide', 'scale', 'size', 'pulsate'
                options: { direction: "up", opacity: 1 },
                duration: 200
            },
            hide: {
                effect: "slide", //'blind', 'clip', 'drop', 'explode', 'fold', 'puff', 'slide', 'scale', 'size', 'pulsate'
                options: { direction: "up", opacity: 0 },
                duration: 200
            },
            target: null,
            open: null,
            close: null,
            change: null,
            inputable: true,
            dropdownEvent: "click",
            from: "bottom",
            fixed: false
        },
        _create: function () {
            var self = this, opts = this.options, el = this.element, eventPrefix = this.widgetEventPrefix;
            this._unobtrusive();
            el.wrap("<div/>");
            var wrapper = this.wrapper = el.parent();
            wrapper.addClass("d-reset d-ui-widget d-picker");
            var input = (opts.inputable ? $("<input/>") : $("<div />")).addClass("d-content-text").appendTo(wrapper);
            //if (el.width())
            //    input.addClass("d-inline");
            //.css({ "width": el.width() - 20 });
            if (el.attr("style") != undefined) {
                wrapper.attr("style", el.attr("style")).show();
                if (opts.inputable) {
                    //input.attr("style", el.attr("style")).show();
                    input.width(wrapper.width() - 25);
                }
            }

            input.addClass("d-ui-widget-content");

            if (el.attr("disabled") == "disabled")
                el.parent().isDisable(true);

            el.addClass("d-content-val").hide();

            if ($.isFunction(opts.open))
                el.bind(eventPrefix + "open", opts.open);

            if ($.isFunction(opts.change))
                el.bind(eventPrefix + "change", opts.change);

            if ($.isFunction(opts.close))
                el.bind(eventPrefix + "close", opts.close);

            var _dbtn = $("<div />").addClass("d-drop-button")
                                                   .appendTo(wrapper);

            wrapper[0].close = function () { self.close(); };

            if (opts.iconImg) {
                _dbtn.width(20)
                         .height(input.outerHeight(true))
                         .css({
                             "background": "url(" + opts.iconImg + ") center center"
                         });
            }
            else {
                if (opts.iconClass)
                    _dbtn.addClass(opts.iconClass);
            }

            //$(document).on("closeallpopup",self.close());

            _dbtn.bind((opts.dropdownEvent ? opts.dropdownEvent : "click"), function (e) {
                e.stopPropagation();
                e.preventDefault();
                if (el.parent().isDisable())
                    return;

                if (self.animationWrapper) {
                    if (self.animationWrapper.isVisible())
                        self.close();
                    else
                        self.open();
                }
                else {
                    if (opts.inputable)
                        $(">.d-content-text", wrapper).focus();
                }
            });

            wrapper.hover(function () {
                if (!wrapper.isActive())
                    wrapper.isHover(true);
            },
             function () {
                 wrapper.isHover(false);
             });

            //console.log(wrapper.parent().css("z-index"));
            var _dropHandler = function () {
                if (!wrapper.isDisable() && !wrapper.hasClass("d-state-readonly")) {
                    if (self.animationWrapper) {
                        if ((opts.inputable && opts.autoOpen) || opts.inputable == false)
                            self.open();
                    }
                    else {
                        wrapper.isHover(false);
                        wrapper.isActive(true);
                    }
                }
            };

            if (opts.inputable) {
                input.bind("focus", function () { _dropHandler(); })
                        .bind("blur", function () {
                            if (!wrapper.hasClass("d-state-readonly"))
                                wrapper.isActive(false);
                        })
                        .bind("keyup", function () {
                            if (el[0].tagName != "SELECT")
                                self._setVal($(this).val());
                        })
                        .bind("change", function () {
                            var orgValue = opts.value, orgText = opts.text;
                            self._setVal($(this).val());
                            self._onInputChanged(orgValue, $(this).val());
                            self._triggerEvent("change", { originalValue: orgValue, originalText: orgText, value: $(this).val(), text: $(this).val() });
                        })
                        .click(function (event) {
                            event.stopPropagation();
                            event.preventDefault();
                        });
            } else {
                input.bind("click", function (event) {
                    event.preventDefault();
                    event.stopPropagation();
                    _dropHandler();
                }).disableSelection();
            }

            if (opts.target) {
                var target = $(opts.target);
                if (target.length) {
                    var anim = self.animationWrapper = $("<div class='d-ui-widget d-drop-container'/>").appendTo(opts.fixed ? el.parent() : "body");

                    if (opts.height || opts.width) {
                        if (opts.height && opts.height != "auto")
                            anim.css({
                                "max-height": opts.height + "px",
                                "overflow-y": "auto"
                            });

                        if (opts.width && opts.width != "auto")
                            anim.css({
                                "width": opts.width + 17 + "px",
                                "overflow-x": "auto"
                            });
                    }

                    anim.append(target).hide();
                }
            }

            //Init values from options
            if (opts.bindingTo)
                this._setBindingSource(opts.bindingTo);

            if (opts.value) {
                el.val(opts.value);
            } else {
                if (el.val())
                    opts.value = el.val();
            }

            if (opts.inputable) {
                if (opts.text)
                    input.val(opts.text);
                else {
                    if (opts.value) {
                        opts.text = opts.value;
                        input.val(opts.text);
                    }
                }

                if (opts.placeholder)
                    input.attr("placeholder", opts.placeholder);
                else {
                    if (el.attr("placeholder"))
                        input.attr("placeholder", el.attr("placeholder"));
                }

            } else {
                if (opts.text)
                    input.text(opts.text);
                else {
                    if (opts.value) {
                        opts.text = opts.value;
                        input.text(opts.text);
                    }

                    if (input.text() == "" && opts.placeholder)
                        input.text(opts.placeholder);
                }
            }
            return el;
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            if (el.data("width")) opts.width = el.dataInt("width");
            if (el.data("height")) opts.height = el.dataInt("height");
            if (el.data("target")) opts.target = el.datajQuery("target");
            if (el.data("icon")) opts.iconClass = el.data("icon");
            if (el.data("placeholder")) opts.placeholder = el.data("placeholder");
            if (el.attr("placeholder"))
                opts.placeholder = el.attr("placeholder");

            if (el.data("img")) opts.iconImg = el.data("img");
            if (el.data("onopen")) opts.open = new Function("event", "ui", el.data("onopen"));
            if (el.data("onclose")) opts.close = new Function("event", "ui", el.data("onclose"));
            if (el.data("change")) opts.change = new Function("event", "ui", el.data("change"));
            if (el.data("dropdown-event")) opts.dropdownEvent = el.data("dropdown-event");
            if (el.data("text")) opts.text = el.data("text");
            if (el.data("value")) opts.value = el.data("value");
            if (el.val())
                opts.value = el.val();

            if (el.data("fixed") != undefined) opts.fixed = el.dataBool("fixed");
            if (el.data("show-effect")) {
                opts.show.effect = el.data("show-effect");
                if (opts.show.effect == "clip")
                    opts.show.options = { direction: "vertical" };
            }

            if (el.data("show-speed"))
                opts.show.duration = el.dataInt("show-speed");
            if (el.data("hide-effect")) {
                opts.hide.effect = el.data("hide-effect");
                if (opts.hide.effect == "clip")
                    opts.hide.options = { direction: "vertical" };
            }

            if (el.data("hide-speed"))
                opts.hide.duration = el.dataInt("hide-speed");
            if (el.data("input") != undefined)
                opts.inputable = el.dataBool("input");
            if (el.data("from"))
                opts.from = el.data("from");

            return $.dna.taoDataBindable.prototype._unobtrusive.call(this);
        },
        _play: function (act, animation) {
            var wrapper = this.animationWrapper;
            var dfd = $.Deferred(function () {
                wrapper.css("zIndex", $.topMostIndex() + 1).stop(false, true)[act](animation.effect, animation.options, animation.duration, function () {
                    dfd.resolve();
                });
            });
            return dfd;
        },
        _setVal: function (value) {
            this.options.value = value ? value : "";
            this.element.val(value ? value : "");
            return this.element;
        },
        _setText: function (val) {
            var _input = this._textElement(), opts = this.options;
            if (val) {
                if (opts.inputable) {
                    this.options.value = val;
                    _input.val(val);
                }
                else {
                    if (val.jquery)
                        _input.html(val.clone().html());
                    else
                        _input.text(val);
                    this.options.text = _input.text();
                }
            } else {
                if (opts.inputable) {
                    _input.val("");
                    if (opts.placeholder)
                        _input.attr("placeholder", opts.placeholder);
                }
                else {
                    _input.text(opts.placeholder);
                }

                this.options.value = "";
            }

            return this.element;
        },
        _textElement: function () {
            return $(">.d-content-text", this.wrapper);
        },
        _onBindingPosition: function (data) {
            ///<summary>Impletement the taoDataBindable</summary>
            if (this.options.mode != "new") {
                this.dataItem = data.dataItem;
                this.text(data.value);
                this.val(data.value);
            }
        },
        _onStateChanged: function (event, state) {
            if (state == "inserted" || state == "removed" || state == "add") {
                this._setText("");
                this._setVal("");
            }
            return this;
        },
        _setOption: function (key, value) {
            if (key == "text")
                this._setText(value);

            if (key == "value")
                this._setVal(value);

            return $.Widget.prototype._setOption.call(this, key, value);
        },
        _onInputChanged: function (originalValue, value) { },
        value: function (val, txt, forceChange) {
            var orgValue = this.options.value, orgText = this.options.text;
            this._setVal(val);

            if (txt)
                this._setText(txt);
            else
                this._setText(val);

            if (forceChange && orgValue != val && orgText != txt)
                this._triggerEvent("change", { originalValue: orgValue, originalText: orgText, value: val, text: txt });
            return this.element;
            // return this.close();
        },
        clear: function () {
            this._setVal("");
            this._setText("");
        },
        disable: function () {
            this.widget().isDisable(true);
            this._textElement().attr("disabled", true);
            return this;
        },
        enable: function () {
            this.widget().isDisable(false);
            this._textElement().attr("disabled", false);
            return this;
        },
        open: function () {
            var self = this, opts = this.options, el = this.widget(), animationWrapper = this.animationWrapper;

            //$(".d-picker.d-state-active>.d-content-val").taoDropdown("close");

            $(".d-picker.d-state-active").each(function (i, pd) {
                if ($.isFunction(pd.close))
                    pd.close();
            });

            if (animationWrapper) {
                if (!animationWrapper.isVisible()) {
                    el.isActive(true);


                    if (opts.width) {
                        animationWrapper.css({
                            "width": opts.width + "px"
                        });
                    }

                    if (opts.height) {
                        animationWrapper.css({
                            "height": opts.height + ($.isNumeric(opts.height) ? "px" : "")
                        });
                    }

                    var _at = "left bottom", _my = "left top";

                    if (opts.from == "top")
                        _at = "left top";


                    if (opts.from == "middle") {
                        _at = "left middle";
                        _my = "left middle";
                    }

                    var offsetX = $(document).scrollLeft(),
                    offsetY = $(document).scrollTop(),
                    _offset = offsetX + "px " + offsetY + "px";

                    animationWrapper.css({
                        "min-width": el.innerWidth(),
                        "z-index": $.topMostIndex() + 1,
                        "opacity": 1
                    }).show().position({ of: this.wrapper, at: _at, my: _my });

                    animationWrapper.show(opts.show.effect, opts.show.options, opts.show.duration);
                    self._triggerEvent("open");
                }
            }

            return this.element;
        },
        close: function () {
            var self = this;
            if (this.animationWrapper) {
                if (this.animationWrapper.isVisible())
                    this.widget().isActive(false);

                this._play("hide", this.options.hide).done(function () {
                    self._triggerEvent("close");
                });
            }
            return this.element;
        },
        widget: function () {
            return this.wrapper;
        },
        destroy: function () {
            this.widget().after(this.element);
            if (this.animationWrapper) {
                if (this.animationWrapper.length)
                    this.animationWrapper.remove();
            }
            this.widget().remove();
            this.element.show().removeClass("d-content-val");
            $.Widget.prototype.destroy.call(this);
        }
    });
})(jQuery);
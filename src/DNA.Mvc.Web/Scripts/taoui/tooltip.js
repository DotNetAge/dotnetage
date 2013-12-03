/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

(function ($) {
    $.widget("dna.taoTooltip", {
        options: {
            open: null,
            close: null,
            position: {
                at: "center top",
                my: "center bottom",
                offset: "0 -10",
                collision: "flip"
            },
            delay: 300,
            show: null,
            hide: null,
            url: null,
            tooltipClass: null,
            width: 250,
            content: null,
            autoClose: true
        },
        _create: function () {
            var self = this, eventPrefix = this.widgetEventPrefix, el = this.element;

            this._unobtrusive();
            if (this.options.open)
                el.bind(eventPrefix + "open", this.options.open);

            if (this.options.close)
                el.bind(eventPrefix + "close", this.options.close);

            var _onopen = function () {
                if (!el.isDisable()) {
                    if (self.options.delay)
                        self._lazyOpen();
                    else
                        self.open(el);
                }
            },
            _onclose = function () {
                if (!el.isDisable()) {
                    if (self.holder) {
                        if (self.holder.isVisible()) {
                            if (self.options.autoClose)
                                self._lazyClose(el);
                            else
                                self.close();
                        } else
                            self._clearTimeout();
                    }
                    else {
                        self._clearTimeout();
                    }
                }
            };

            if (el.isInput()) {
                el.bind("focus", _onopen)
                   .bind("blur", _onclose);
            }
            else {
                //el.hover(_onopen, _onclose);
                el.bind("mouseenter", _onopen)
                   .bind("mouseleave", _onclose);
            }

            if (el.attr("title")) {
                el.data("title", el.attr("title"))
                   .removeAttr("title");
            }
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            if (el.data("tooltip-class") != undefined) opts.tooltipClass = el.data("tooltip-class");
            if (el.data("tooltip-delay") != undefined) opts.delay = el.data("tooltip-delay");
            if (el.data("tooltip-url")) opts.url = el.data("tooltip-url");
            if (el.data("tooltip-width")) opts.width = el.dataInt("tooltip-width");
            if (el.data("tooltip-content")) opts.content = el.datajQuery("tooltip-content");
            if (el.data("tooltip-open")) opts.open = new Function("event", "ui", el.data("tooltip-open"));
            if (el.data("tooltip-close")) opts.close = new Function("event", "ui", el.data("tooltip-close"));

            if (el.data("tooltip-position")) {
                var pos = el.data("tooltip-position");

                //if (pos == "top")
                //    opts.position = {
                //        at: "center top",
                //        my: "center bottom",
                //        offset: "0 -10"
                //    };
                if (pos == "right")
                    opts.position = {
                        my: "left center",
                        at: "right center",
                        offset: "10 0"
                    };

                if (pos == "left")
                    opts.position = {
                        at: "left center",
                        my: "right center",
                        offset: "-10 0"
                    };

                if (pos == "bottom")
                    opts.position = {
                        at: "center bottom",
                        my: "center top",
                        offset: "0 10"
                    };
            }
            return this;
        },
        _triggerEvent: function (eventName, eventArgs) {
            this.element.trigger(this.widgetEventPrefix + eventName, eventArgs);
        },
        _lazyClose: function () {
            var self = this, el = this.element;
            this._clearTimeout(el);
            el.data("timer", setTimeout(function () {
                self.close(el);
            }, 300));
        },
        _lazyOpen: function () {
            var self = this, el = this.element, timer = "timer",
            _clearOpenTimer = function () {
                if (el.data(timer)) {
                    clearTimeout(el.data(timer));
                    el.removeData(timer);
                }
            };

            _clearOpenTimer();

            el.data(timer, setTimeout(function () {
                self.open();
                _clearOpenTimer();
            }, this.options.delay));

        },
        _delegate: function (context, handler) {
            return function () {
                return handler.apply(context, [this]);
            };
        },
        _clearTimeout: function () {
            if (this.element.data("timer") != undefined) {
                clearTimeout(this.element.data("timer"));
                this.element.removeData("timer");
            }
        },
        _setOption: function (key, value) {
            if (key == "content") {
                this.options.content = value;
                this._setContent(value);
                return this;
            }

            if (key == "position") {
                this.options.position = value;
                this._setPosition(value);
                return this;
            }
            return $.Widget.prototype._setOption.call(this, key, value);
        },
        _setContent: function (_content) {
            var content = null, self = this, el = this.element, opts = this.options, pos = opts.position;
            var _holder = self.holder;
            if (self.holder) {
                if ($.isFunction(_content)) {
                    //var _getContent = self._delegate(self.element, _content);
                    var _getContent = $.proxy(_content, el);
                    content = _getContent();
                    //alert("Content generated.");
                    if (content) {
                        _holder.empty();

                        if ($.type(content) == "string")
                            _holder.html(content);
                        else
                            _holder.append(content);

                        //_holder.position(self.options.position);
                    }
                }
                else {
                    content = $(_content);
                    if (content.length)
                        _holder.empty().append(content); //.position(opts.position);
                    else
                        _holder.empty().html(_content); //.position(opts.position);
                }
            }

            if (opts.width > 0)
                holder.css("max-width", opts.width + "px");
            //_holder.width(opts.width);
        },
        _setPointer: function () {
            var opts = this.options;
            this._clearPointer();
            if (opts.position.my == "center bottom")  //top
                this.holder.addClass("d-pos-bottom");

            if (opts.position.my == "center top")  //bottom
                this.holder.addClass("d-pos-top");

            if (opts.position.my == "right center")  //left
                this.holder.addClass("d-pos-right");

            if (opts.position.my == "left center")  //right
                this.holder.addClass("d-pos-left");

        },
        _clearPointer: function () {
            if (this.holder) {
                this.holder.removeClass("d-post-top")
                                   .removeClass("d-post-left")
                                   .removeClass("d-post-right")
                                   .removeClass("d-post-bottom");

            }
        },
        _setPosition: function (_pos) {
            var opts = this.options, pos = _pos ? _pos : opts.position;

            if (this.holder) {
                if (this.holder.isVisible()) {

                    if (!pos.of)
                        pos.of = this.element;

                    this.holder.css({
                        "zIndex": $.topMostIndex()+1
                    });

                    if (pos)
                        this.holder.position(pos);
                }
            }
        },
        open: function () {
            var self = this, el = this.element, opts = this.options, _url = opts.url;

            if (!_url) {
                if (el.data("tooltip-url") != undefined)
                    _url = opts.url = el.data("tooltip-url");
            }

            if (el.data("title") != undefined || _url || opts.content) {
                self._clearTimeout();

                if (self.holder == undefined) {
                    self.holder = $("<div/>").addClass(" d-ui-widget d-ui-widget-content d-tooltip")
                                                           .css({ "opacity": "0" })
                                                           .hide()
                                                           .prependTo("body")
                                                           .hover(function () { self._clearTimeout(); },
                                                                      function () { self._lazyClose(); });
                    self._setPointer();

                    if (opts.tooltipClass)
                        self.holder.addClass(opts.tooltipClass);

                    if (opts.width > 0)
                        self.holder.css("max-width", opts.width + "px");

                }

                var _holder = self.holder;

                if (!_holder.is(":visible")) {
                    var _doShow = function () {
                        if (opts.content) {
                            if (_holder.children().length == 0) {
                                self._setContent(opts.content);
                                _holder.show();
                                self._setPosition();
                            }

                            $(document).one("click", function () {
                                self.close();
                            })
                        }
                        else {
                            if (el.data("title") != undefined) {
                                _holder.html(el.data("title")).show();
                                self._setPosition();

                                //.position(self.options.position);
                            }
                            else {
                                if (_url && !_holder.data("hasContent")) {
                                    _holder.empty()
                                                 .append($("<div/>").height(16).addClass("d-loading"))
                                                 .css({
                                                     "height": "auto",
                                                     "opacity": "1"
                                                 })
                                                 .show();
                                    self._setPosition();
                                    //.position(opts.position);

                                    self._clearPointer();
                                    _holder.load(_url, function () {
                                        self._setPointer();

                                        _holder.css({
                                            "height": "auto",
                                            "opacity": "1"
                                        }).data("hasContent", true).show().taoUI();

                                        self._setPosition();
                                        //.position(opts.position)
                                    });
                                }
                            }
                        }

                        if ($.browser.msie)
                            self.holder.css({ "opacity": "1" }).show();
                        else
                            self.holder.animate({ "opacity": "1" });

                        self._triggerEvent("open", { tooltip: self.holder, element: self.element });
                    };


                    if ($.browser.msie) {
                        _doShow();
                    }
                    else {
                        if (opts.show)
                            _holder.stop()
                                    .show(opts.show.effect, opts.show.options ? opts.show.options : {}, opts.show.duration, function () {
                                        _doShow();
                                    });
                        else
                            _holder.stop().fadeIn("fast", function () {
                                _doShow();
                            });
                    }
                }
            }

            return this;
        },
        close: function () {
            var self = this, opts = this.options;
            if (self.holder) {
                var _holder = $(self.holder);
                if (_holder.isVisible()) {
                    if (opts.hide) {
                        _holder.stop().hide(opts.hide.effect, opts.hide.options ? opts.hide.options : {}, opts.hide.duration, function () {
                            self._triggerEvent("close");
                        });
                    }
                    else {
                        _holder.stop().fadeOut("fast", function () {
                            self._triggerEvent("close", { tooltip: self.holder, element: self.element });
                        });
                    }
                }
            }
            return this;
        },
        widget: function () {
            if (this.holder)
                return this.holder;
            return this.element;
        },
        destroy: function () {
            this._clearTimeout();
            if (this.holder)
                this.holder.remove();

            if (this.element.data("title") != undefined) {
                this.element.attr("title", this.element.data("title"));
                this.element.data("title", null);
            }
            $.Widget.prototype.destroy.call(this);
        }
    });
})(jQuery);
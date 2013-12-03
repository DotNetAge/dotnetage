/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

(function ($) {
    $.widget("dna.taoButton", {
        options: {
            primaryIcon: null,
            primaryCheckIcon: null,
            primaryImg: null,
            primaryCheckImg: null,
            secondaryIcon: null,
            secondaryCheckIcon: null,
            secondaryImg: null,
            secondaryCheckImg: null,
            hoverImg: null,
            activeImg: null,
            isDefault: false,
            label: null
            ,ajaxurl: null,
            ajaxtype: "GET",
            ajaxcomplete: null,
            ajaxsuccess: null,
            ajaxerror: null,
            ajaxdata: null,
            ajaxbeforesend: null
        },
        _create: function () {
            var el = this.element;
            this._unobtrusive();
            this.isToggle = false;
            el.addClass("d-reset d-ui-widget");
            if (el.attr("type") == "checkbox")
                this._createToggle();
            else {
                if (el.attr("type") == "image")
                    this._createImage();
                else
                    this._createButtonCore(el);
            }
            
            if (el.attr("disabled") == "disabled")
                el.isDisable(true);
            return el;
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            if (el.data("label")) opts.label = el.data("label");
            if (el.data("icon-left")) opts.primaryIcon = el.data("icon-left");
            if (el.data("icon-right")) opts.secondaryIcon = el.data("icon-right");
            if (el.data("img-left")) opts.primaryImg = el.data("img-left");
            if (el.data("img-right")) opts.secondaryImg = el.data("img-right");
            if (el.data("icon-left-checked")) opts.primaryCheckIcon = el.data("icon-left-checked");
            if (el.data("icon-right-checked")) opts.secondaryCheckIcon = el.data("icon-right-checked");
            if (el.data("img-left-checked")) opts.primaryCheckImg = el.data("img-left-checked");
            if (el.data("img-right-checked")) opts.secondaryCheckImg = el.data("img-right-checked");
            if (el.data("img-hover")) opts.hoverImg = el.data("img-hover");
            if (el.data("img-active")) opts.activeImg = el.data("img-active");
            if (el.data("default") != undefined) opts.isDefault = el.dataBool("default");

            if (el.data("icon"))
                opts.primaryIcon = el.data("icon");

            //New for ajax action
            //if (el.data("ajax-url")) opts.ajaxurl = el.data("ajax-url");
            //if (el.data("ajax-method")) opts.ajaxtype = el.data("ajax-method");

            //if (el.data("ajax-complete")) {
            //    try {
            //        opts.ajaxcomplete = new Function("jqXHR", "textStatus", el.data("ajax-complete"));
            //    } catch (e) {
            //        opts.ajaxcomplete = null;
            //    }
            //}

            //if (el.data("ajax-beforesend")) {
            //    try {
            //        opts.ajaxbeforesend = new Function("jqXHR", "settings", el.data("ajax-beforesend"));
            //    } catch (e) {
            //        opts.ajaxbeforesend = null;
            //    }
            //}

            //if (el.data("ajax-success")) {
            //    try {
            //        opts.ajaxsuccess = new Function("data", "textStatus", "jqXHR", el.data("ajax-success"));
            //    } catch (e) {
            //        opts.ajaxsuccess = null;
            //    }
            //}

            //if (el.data("ajax-error"))
            //    try {
            //        opts.ajaxerror = new Function("jqXHR", "textStatus", "errorThrown", el.data("ajax-error"));
            //    } catch (e) {
            //        opts.ajaxerror = null;
            //    }

            //if (el.data("ajax-data")) {
            //    try {
            //        opts.ajaxdata = new Function(el.data("ajax-data"));
            //    } catch (e) {
            //        opts.ajaxdata = null;
            //    }
            //}
            return this;
        },
        _createButtonCore: function (el) {
            var self = this, opts = this.options,
            _hover = function () {
                if (!el.isDisable())
                    el.isHover(true);
            },
            _blur = function () {
                if (!el.isDisable()) {
                    el.isHover(false);
                    if (!self.isToggle)
                        el.isActive(false);
                }
            },
            _active = function () {
                if (!el.isDisable())
                    el.isActive(true);
            },
              _deactive = function () {
                  if (!el.isDisable()) {
                      if (!self.isToggle)
                          el.isActive(false);
                  }
              },
              _label = opts.label ? opts.label : el.text();
            if (_label) _label = $.trim(_label);
            el.addClass("d-button")
               .hover(_hover, _blur)
               .mousedown(_active)
               .mouseup(_deactive)
               .focus(_hover)
               .blur(_blur)
               .keydown(_active)
               .keyup(_deactive)
               .attr("tabIndex", 1)
               .disableSelection()

            if (_label) {
                el.wrapInner("<span class='d-button-text' />");
                $(">span.d-button-text", el).disableSelection();
            }

            if (el.attr("role") == "link") el.addClass("link");

            if (opts.primaryImg) {
                var _pi = $("<img/>").addClass("d-primary-icon")
                                                 .attr("src", opts.primaryImg)
                                                 .prependTo(el);
                if (_label == "") _pi.css("margin", "0px");
            } else {
                if (opts.primaryIcon) {
                    var _pi = $("<span/>").addClass("d-primary-icon")
                                    .addClass(opts.primaryIcon)
                                    .prependTo(el);
                    if (_label == "") _pi.css("margin", "0px");
                }
            }

            if (opts.secondaryImg) {
                var _pi = $("<img/>").addClass("d-secondary-icon")
                                                 .attr("src", opts.secondaryImg)
                                                 .appendTo(el);
                //if (_label) _pi.css("margin", "0px");
            } else {
                if (opts.secondaryIcon) {
                    var _si = $("<span/>").addClass("d-secondary-icon")
                                    .addClass(opts.secondaryIcon)
                                    .appendTo(el);
                    //if (_label) _si.css("margin", "0px");
                }
            }

            if (opts.isDefault) 
                el.addClass("d-state-default");
            

            this._widget = el;

            if (opts.ajaxurl) {
                el.unbind("click");
                el.click(function (e) {
                    if (!el.isDisable()) {
                        e.stopPropagation();
                        e.preventDefault();
                        self.disable();
                        var _dat = null;
                        if (opts.ajaxdata) {
                            if ($.isFunction(opts.ajaxdata)) {
                                _dat = opts.ajaxdata();
                            }
                        }

                        $.ajax({
                            type: opts.ajaxtype ? opts.ajaxtype : "GET",
                            url: opts.ajaxurl,
                            data: _dat,
                            complete: opts.ajaxcomplete,
                            success: opts.ajaxsuccess
                        }).always(function () {
                            self.enable();
                        });
                    }
                });
            }
        },
        _createToggle: function () {
            var self = this, el = this.element, id = this.element.attr("id"), labelEL = null;
            this.isToggle = true;
            if (id)
                labelEL = this.element.next("label[for='" + id + "']");
            else
                labelEL = this.element.next("label");

            if (labelEL.length == 0) {
                labelEL = $("<label/>");
                labelEL.text(this.options.label);
                this.element.after(labelEL);
            }

            this._createButtonCore(labelEL);

            if (el.attr("checked")) {
                labelEL.addClass("d-state-active");
                el.val(true);
            }

            this._setCheckIcon(labelEL);

            labelEL.bind("click", function () {
                if (!$(this).isDisable()) {

                    $(this).toggleClass("d-state-active");
                    var checked = false;

                    if ($(this).hasClass("d-state-active"))
                        checked = true;

                    self._setCheckIcon($(this))
                         .element.attr("checked", checked)
                                      .attr("aria-pressed", checked)
                                      .val(checked);
                    el.trigger("change");
                }
            })
                       .unbind("mouseup")
                       .unbind("mousedown")
                       .unbind("keydown")
                       .unbind("keyup");

            if (this.element.attr("style") != undefined)
                labelEL.attr("style", this.element.attr("style"));

            if (this.element.attr("class") != undefined)
                labelEL.addClass(this.element.attr("class"));

            this.element.hide();
        },
        _setCheckIcon: function (el) {
            var opts = this.options, primaryIcon = $(">.d-primary-icon", el),
            secondaryIcon = $(">.d-secondary-icon", el);

            if (el.isActive()) {
                if (primaryIcon.length) {
                    if (primaryIcon[0].tagName.toLowerCase() == "span") { //Icon mode
                        if (opts.primaryCheckIcon) {
                            primaryIcon.removeClass(opts.primaryIcon)
                                             .addClass(opts.primaryCheckIcon);
                        }
                    } else { // Img mode
                        if (opts.primaryCheckImg)
                            primaryIcon.attr("src", opts.primaryCheckImg);
                    }
                }

                if (secondaryIcon.length) {
                    if (secondaryIcon[0].tagName.toLowerCase() == "span") { //Icon mode
                        if (opts.secondaryCheckIcon) {
                            secondaryIcon.removeClass(opts.secondaryIcon)
                                                  .addClass(opts.secondaryCheckIcon);
                        }
                    } else { // Img mode
                        if (opts.secondaryCheckImg)
                            secondaryIcon.attr("src", opts.secondaryCheckImg);
                    }
                }
            }
            else {
                if (primaryIcon.length) {
                    if (primaryIcon[0].tagName.toLowerCase() == "span") { //Icon mode
                        if (opts.primaryIcon) {
                            primaryIcon.removeClass(opts.primaryCheckIcon)
                                             .addClass(opts.primaryIcon);
                        }
                    } else { // Img mode
                        if (opts.primaryImg)
                            primaryIcon.attr("src", opts.primaryImg);
                    }
                }

                if (secondaryIcon.length) {
                    if (secondaryIcon[0].tagName.toLowerCase() == "span") { //Icon mode
                        if (opts.secondaryIcon) {
                            secondaryIcon.removeClass(opts.secondaryCheckIcon)
                                                  .addClass(opts.secondaryIcon);
                        }
                    } else { // Img mode
                        if (opts.secondaryImg)
                            secondaryIcon.attr("src", opts.secondaryImg);
                    }
                }
            }
            return this;
        },
        _createImage: function () {
            var wrapper = this.element.before($("<div/>")).prev(), opts = this.options, self = this;
            wrapper.width(this.element.width()).height(this.element.height());
            var hoverImage = new Image();
            hoverImage.src = opts.hoverImg;
            var activeImage = new Image();
            activeImage.src = opts.activeImg;

            var _srcUrl = self.element.attr("src"), _hUrl = opts.hoverImg, _aUrl = opts.activeImg;

            //if (this.element.data("label"))
            //wrapper.data("label", this.element.data("label"));
            if (opts.label) wrapper.text(opts.label);
            this._createButtonCore(wrapper);
            wrapper.addClass("img")
                         .css({
                             "background-image": "url(" + _srcUrl + ")",
                             "background-position": "center center",
                             "background-repeat": "no-repeat"
                         })
                        .unbind("mouseup")
                        .unbind("mousedown")
                        .unbind("keydown")
                        .unbind("keyup")
                        .bind("click", function () {
                            self.element.click();
                        })
                        .bind("mouseenter", function () {
                            if (_hUrl)
                                wrapper.css("background-image", "url(" + _hUrl) + ")";
                        })
                        .bind("mouseout", function () {
                            wrapper.css("background-image", "url(" + _srcUrl + ")");
                        })
                        .bind("mousedown", function () {
                            if (_aUrl)
                                wrapper.css("background-image", "url(" + _aUrl) + ")";
                        })
                        .bind("mouseup", function () {
                            if (_hUrl)
                                wrapper.css("background-image", "url(" + _hUrl + ")");
                        })
                        .bind("keydown", function (e) {
                            if (e.keyCode == 13 && _aUrl)
                                wrapper.css("background-image", "url(" + _aUrl) + ")";
                        })
                        .bind("keyup", function () {
                            wrapper.css("background-image", "url(" + _srcUrl + ")");
                        })
                        .disableSelection();
            this._widget = wrapper;
            var _img = new Image();
            _img.src = _srcUrl;

            $(_img).bind("load", function () {
                wrapper.width(_img.width);
                wrapper.height(_img.height);
            });

            if (this.element.attr("style") != undefined)
                wrapper.attr("style", this.element.attr("style"));

            if (this.element.attr("class") != undefined)
                wrapper.addClass(this.element.attr("class"));

            this.element.hide();
        },
        _setOption: function (key, value) {
            if (key == "label") {
                var labelEl = $(">.d-button-text", this.element);
                if (labelEl.length == 0)
                    labelEl = $("<span  class='d-button-text' />").appendTo(this.element);
                labelEl.text(value);
                return this;
            }
            return $.Widget.prototype._setOption.call(this, key, value);
        },
        _triggerEvent: function (eventName, eventArgs) {
            this.element.trigger(this.widgetEventPrefix + eventName, eventArgs);
        },
        disable: function () {
            this.widget().isDisable(true);
            if (this.widget()[0].tagName.toLowerCase() == "button")
                this.widget().attr("disabled", true);
            return this.element;
        },
        enable: function () {
            this.widget().isDisable(false);
            if (this.widget()[0].tagName.toLowerCase() == "button")
                this.widget().attr("disabled", false);

            return this.element;
        },
        widget: function () {
            return this._widget;
        },
        destroy: function () {
            $.Widget.prototype.destroy.call(this);
        }
    });

    $.widget("dna.taoButtonGroup", {
        options: {
            index: 0,
            checkable: true,
            click: null,
            type: "horizontal"
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element;
            el.addClass("d-buttons");

            if (el.data("type"))
                opts.type = el.data("type");

            if (el.data("checkable") != undefined)
                opts.checkable = el.dataBool("checkable");

            if (el.data("onclick"))
                opts.click = new Function("event", "ui", el.data("onclick"));

            if (opts.click)
                el.bind(eventPrefix + "click", opts.click);

            var buttons = $(">button", el).taoButton();
            if (opts.checkable)
                buttons.unbind();

            if (opts.type == "vertical")
                el.addClass("d-vertical")

            buttons.each(function (i, btn) {
                if (i == 0)
                    $(btn).addClass("d-first");

                if (i > 0 && i < (buttons.length - 1))
                    $(btn).css({ "border-radius": "0px", "border-left": "none" });

                if (i == (buttons.length - 1))
                    $(btn).addClass("d-last");

                if (opts.index == i) {
                    if (opts.checkable)
                        $(btn).isActive(true);
                }

                $(btn).bind("click", function () {
                    self._triggerEvent("click", { index: i, button: $(btn) });
                });

                if (opts.checkable) {
                    $(btn).hover(function () {
                        if (!el.isDisable() && !$(this).isActive())
                            $(this).isHover(true);
                    }, function () {
                        $(this).isHover(false);
                    }).click(function () {
                        if (!el.isDisable() && !$(this).isActive()) {
                            buttons.removeClass("d-state-active");
                            opts.index = i;
                            $(this).isActive(true);
                        }
                    });
                }
            });
        },
        _triggerEvent: function (eventName, eventArgs) {
            this.element.trigger(this.widgetEventPrefix + eventName, eventArgs);
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
        //        _setOption: function (key, value) {
        //            return $.Widget.prototype._setOption.call(this, key, value);
        //        },
        destroy: function () {
            $.Widget.prototype.destroy.call(this);
        }
    });

})(jQuery);